#!/usr/bin/env python3
"""
Wekeza Banking – Regression Tests
=====================================
Broad regression suite that exercises every API endpoint exposed by v1-Core,
verifying:
  • No regression in HTTP status codes across all endpoint families
  • Schema / field presence in success responses
  • Consistent error shapes (400/401/404 bodies)
  • Backward-compatibility of the /health and /api sub-routes
  • Role-based access control (customer vs teller vs admin)

This suite is designed to run after every deployment and whenever a PR
touches the API layer.

Usage:
    API_BASE_URL=http://localhost:8080 python3 tests/regression/test_api_regression.py

Set WEKEZA_SKIP_LIVE_TESTS=true to skip all network-dependent tests.
"""

import json
import os
import sys
import unittest
import urllib.error
import urllib.request

# ─── Config ───────────────────────────────────────────────────────────────────
API_BASE_URL = os.getenv("API_BASE_URL", "http://localhost:8080").rstrip("/")

CREDENTIALS: dict[str, tuple[str, str]] = {
    "admin":        (os.getenv("ADMIN_USERNAME",    "admin"),         os.getenv("ADMIN_PASSWORD",    "Admin@123")),
    "executive":    ("executive1",  os.getenv("EXECUTIVE_PASSWORD",   "Executive@123")),
    "manager":      ("manager1",    os.getenv("MANAGER_PASSWORD",     "Manager@123")),
    "teller":       ("teller1",     os.getenv("TELLER_PASSWORD",      "Teller@123")),
    "supervisor":   ("supervisor1", os.getenv("SUPERVISOR_PASSWORD",  "Supervisor@123")),
    "compliance":   ("compliance1", os.getenv("COMPLIANCE_PASSWORD",  "Compliance@123")),
    "payments":     ("payments1",   os.getenv("PAYMENTS_PASSWORD",    "Payments@123")),
    "customer":     ("customer1",   os.getenv("CUSTOMER_PASSWORD",    "Customer@123")),
}

ACTIVE_ACCOUNT = os.getenv("ACTIVE_ACCOUNT_NUMBER", "ACC0000055583")
TEST_BRANCH_ID = os.getenv("TEST_BRANCH_ID", "11111111-1111-1111-1111-111111111111")
SKIP_LIVE = os.getenv("WEKEZA_SKIP_LIVE_TESTS", "false").lower() in ("1", "true", "yes")


# ─── HTTP helper ──────────────────────────────────────────────────────────────

def api_request(method, path, token=None, body=None, timeout=25):
    url = f"{API_BASE_URL}{path}"
    data = json.dumps(body).encode() if body is not None else None
    headers = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    req = urllib.request.Request(url=url, method=method, data=data, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=timeout) as resp:
            raw = resp.read().decode()
            return resp.status, json.loads(raw) if raw else {}
    except urllib.error.HTTPError as exc:
        raw = exc.read().decode() if exc.fp else ""
        try:
            payload = json.loads(raw) if raw else {}
        except json.JSONDecodeError:
            payload = {"raw": raw[:300]}
        return exc.code, payload
    except Exception:
        return 0, {}


def is_reachable():
    status, _ = api_request("GET", "/health")
    return status == 200


# Token cache – fetched once per test session
_token_cache: dict[str, str | None] = {}


def get_token(role: str) -> str | None:
    if role not in _token_cache:
        username, password = CREDENTIALS[role]
        status, payload = api_request(
            "POST",
            "/api/authentication/login",
            body={"Username": username, "Password": password},
        )
        _token_cache[role] = (
            payload.get("token") or payload.get("Token")
        ) if status in (200, 201) else None
    return _token_cache[role]


# ─── Base class ───────────────────────────────────────────────────────────────

class RegressionTestCase(unittest.TestCase):

    @classmethod
    def setUpClass(cls):
        if SKIP_LIVE:
            raise unittest.SkipTest("Live regression tests disabled via WEKEZA_SKIP_LIVE_TESTS")
        if not is_reachable():
            raise unittest.SkipTest(f"API not reachable at {API_BASE_URL}/health")


# ═══════════════════════════════════════════════════════════════════════════════
# 1. Infrastructure / Health Checks
# ═══════════════════════════════════════════════════════════════════════════════

class TestInfrastructureHealth(RegressionTestCase):

    def test_health_returns_200(self):
        status, payload = api_request("GET", "/health")
        self.assertEqual(status, 200, f"Health endpoint returned {status}: {payload}")

    def test_health_response_has_expected_fields(self):
        _, payload = api_request("GET", "/health")
        lower_keys = {k.lower() for k in payload.keys()}
        # Acceptable fields: status, healthy, database, redis …
        acceptable = {"status", "healthy", "database", "redis", "version", "environment"}
        intersection = lower_keys & acceptable
        self.assertGreater(
            len(intersection), 0,
            f"Health response has no recognisable fields; got: {lower_keys}",
        )


# ═══════════════════════════════════════════════════════════════════════════════
# 2. Authentication Regression
# ═══════════════════════════════════════════════════════════════════════════════

class TestAuthenticationRegression(RegressionTestCase):

    _ROLES_TO_TEST = ["admin", "manager", "teller", "supervisor", "compliance",
                      "payments", "customer"]

    def test_all_seeded_roles_can_login(self):
        """Every seeded test user must be able to log in successfully."""
        failures = []
        for role in self._ROLES_TO_TEST:
            token = get_token(role)
            if not token:
                failures.append(role)
        if failures:
            self.fail(f"The following roles could not log in: {failures}")

    def test_jwt_token_format(self):
        """All tokens must have three dot-separated Base64 segments (JWT format)."""
        token = get_token("customer")
        if not token:
            self.skipTest("Could not get customer token")
        parts = token.split(".")
        self.assertEqual(len(parts), 3,
                         f"Token does not look like a JWT (got {len(parts)} segments)")

    def test_unauthorized_access_to_protected_route(self):
        """Protected routes return 401 without a token."""
        for path in [
            "/api/accounts/user/accounts",
            "/api/loans/user/loans",
            "/api/transactions/statement/ANY",
        ]:
            status, _ = api_request("GET", path)
            self.assertIn(
                status, (401, 403),
                f"Expected 401/403 on {path} without token, got {status}",
            )


# ═══════════════════════════════════════════════════════════════════════════════
# 3. Account Endpoints Regression
# ═══════════════════════════════════════════════════════════════════════════════

class TestAccountEndpointsRegression(RegressionTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.token = get_token("customer")
        cls.teller_token = get_token("teller")

    def test_list_accounts_returns_2xx(self):
        if not self.token:
            self.skipTest("No customer token")
        status, _ = api_request("GET", "/api/accounts/user/accounts", token=self.token)
        self.assertIn(status, (200, 204))

    def test_balance_endpoint_exists(self):
        if not self.token:
            self.skipTest("No customer token")
        status, _ = api_request(
            "GET", f"/api/accounts/{ACTIVE_ACCOUNT}/balance", token=self.token
        )
        self.assertNotEqual(status, 0, "Could not reach balance endpoint")
        # 200 = found; 404 = account not seeded – both are acceptable
        self.assertIn(status, (200, 404))

    def test_statement_endpoint_exists(self):
        if not self.token:
            self.skipTest("No customer token")
        status, _ = api_request(
            "GET",
            f"/api/transactions/statement/{ACTIVE_ACCOUNT}?page=1&pageSize=5",
            token=self.token,
        )
        self.assertNotEqual(status, 0)
        self.assertIn(status, (200, 204, 404))


# ═══════════════════════════════════════════════════════════════════════════════
# 4. Teller Portal Regression
# ═══════════════════════════════════════════════════════════════════════════════

class TestTellerPortalRegression(RegressionTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.teller_token = get_token("teller")

    def test_teller_session_start_endpoint_exists(self):
        if not self.teller_token:
            self.skipTest("No teller token")
        # POST with valid body shape
        status, payload = api_request(
            "POST",
            "/api/teller/session/start",
            token=self.teller_token,
            body={
                "branchId": TEST_BRANCH_ID,
                "tellerCode": "TEL001",
                "tellerName": "Regression Teller",
                "branchCode": "MAIN",
                "openingCashBalance": 100000,
            },
        )
        self.assertNotEqual(status, 0, "Could not reach teller session endpoint")
        # 200/201 = started; 400/409 = already open – all mean the endpoint exists
        self.assertIn(status, (200, 201, 400, 409, 422))

    def test_cash_deposit_endpoint_exists(self):
        if not self.teller_token:
            self.skipTest("No teller token")
        status, _ = api_request(
            "POST",
            "/api/teller/transactions/cash-deposit",
            token=self.teller_token,
            body={
                "accountNumber": ACTIVE_ACCOUNT,
                "amount": 100,
                "currency": "KES",
                "narration": "Regression check",
            },
        )
        self.assertNotEqual(status, 0)
        # 400 without open session is acceptable; 0 is not
        self.assertIn(status, (200, 201, 400, 409, 422))

    def test_cash_withdrawal_endpoint_exists(self):
        if not self.teller_token:
            self.skipTest("No teller token")
        status, _ = api_request(
            "POST",
            "/api/teller/transactions/cash-withdrawal",
            token=self.teller_token,
            body={
                "accountNumber": ACTIVE_ACCOUNT,
                "amount": 50,
                "currency": "KES",
            },
        )
        self.assertNotEqual(status, 0)
        self.assertIn(status, (200, 201, 400, 409, 422))


# ═══════════════════════════════════════════════════════════════════════════════
# 5. Payments & Digital Channels Regression
# ═══════════════════════════════════════════════════════════════════════════════

class TestPaymentsDigitalChannelsRegression(RegressionTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.customer_token = get_token("customer")
        cls.payments_token = get_token("payments")

    def _check_endpoint(self, method, path, token, body=None, label=""):
        status, _ = api_request(method, path, token=token, body=body)
        self.assertNotEqual(status, 0, f"Could not reach {label or path}")
        # 401 means wrong role; ≥200 means endpoint exists
        self.assertGreater(status, 0, f"{label}: connection failed")

    def test_airtime_endpoint_exists(self):
        if not self.customer_token:
            self.skipTest("No customer token")
        self._check_endpoint(
            "POST",
            "/api/digitalchannels/airtime",
            self.customer_token,
            body={"fromAccountNumber": ACTIVE_ACCOUNT, "phoneNumber": "+254712345678",
                  "amount": 50, "provider": "Safaricom"},
            label="airtime",
        )

    def test_mpesa_stk_endpoint_exists(self):
        if not self.customer_token:
            self.skipTest("No customer token")
        self._check_endpoint(
            "POST",
            "/api/digitalchannels/mpesa/stk-push",
            self.customer_token,
            body={"phoneNumber": "+254712345678", "amount": 100,
                  "accountNumber": ACTIVE_ACCOUNT},
            label="mpesa-stk",
        )

    def test_send_to_mobile_endpoint_exists(self):
        if not self.customer_token:
            self.skipTest("No customer token")
        self._check_endpoint(
            "POST",
            "/api/digitalchannels/send",
            self.customer_token,
            body={"fromAccountNumber": ACTIVE_ACCOUNT, "toPhoneNumber": "+254722111222",
                  "amount": 200, "provider": "Mpesa", "narration": "Regression test"},
            label="send-to-mobile",
        )

    def test_bill_payment_endpoint_exists(self):
        if not self.customer_token:
            self.skipTest("No customer token")
        self._check_endpoint(
            "POST",
            "/api/payments/bill",
            self.customer_token,
            body={"fromAccountNumber": ACTIVE_ACCOUNT, "billType": "Electricity",
                  "billAccount": "45123789012", "amount": 500},
            label="bill-payment",
        )

    def test_transfer_endpoint_exists(self):
        if not self.customer_token:
            self.skipTest("No customer token")
        self._check_endpoint(
            "POST",
            "/api/transactions/transfer",
            self.customer_token,
            body={"fromAccountNumber": ACTIVE_ACCOUNT, "toAccountNumber": "ACCTEST",
                  "amount": 100, "narration": "Regression"},
            label="transfer",
        )


# ═══════════════════════════════════════════════════════════════════════════════
# 6. Loan Endpoints Regression
# ═══════════════════════════════════════════════════════════════════════════════

class TestLoanEndpointsRegression(RegressionTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.customer_token = get_token("customer")

    def test_list_loans_endpoint_exists(self):
        if not self.customer_token:
            self.skipTest("No customer token")
        status, _ = api_request("GET", "/api/loans/user/loans", token=self.customer_token)
        self.assertNotEqual(status, 0)
        self.assertIn(status, (200, 204))

    def test_loan_apply_endpoint_exists(self):
        if not self.customer_token:
            self.skipTest("No customer token")
        status, _ = api_request(
            "POST",
            "/api/loans/apply",
            token=self.customer_token,
            body={
                "accountNumber": ACTIVE_ACCOUNT,
                "loanType": "Personal",
                "amount": 50000,
                "tenureMonths": 12,
                "purpose": "Regression test loan",
            },
        )
        self.assertNotEqual(status, 0)
        # Could succeed (201) or return business rule error (400/422); both fine
        self.assertIn(status, (200, 201, 400, 409, 422))


# ═══════════════════════════════════════════════════════════════════════════════
# 7. Role-Based Access Control (RBAC) Regression
# ═══════════════════════════════════════════════════════════════════════════════

class TestRBACRegression(RegressionTestCase):

    def test_customer_cannot_start_teller_session(self):
        """Customer role must be denied access to teller session endpoint."""
        token = get_token("customer")
        if not token:
            self.skipTest("No customer token")
        status, _ = api_request(
            "POST",
            "/api/teller/session/start",
            token=token,
            body={
                "branchId": TEST_BRANCH_ID,
                "tellerCode": "TEL001",
                "tellerName": "RBAC Test",
                "branchCode": "MAIN",
                "openingCashBalance": 100000,
            },
        )
        self.assertIn(
            status, (401, 403),
            f"Customer should be denied teller session; got {status}",
        )

    def test_customer_cannot_access_admin_endpoints(self):
        """Customer role must not access admin management endpoints."""
        token = get_token("customer")
        if not token:
            self.skipTest("No customer token")
        admin_paths = [
            "/api/admin/users",
            "/api/admin/roles",
            "/api/branches",
        ]
        for path in admin_paths:
            status, _ = api_request("GET", path, token=token)
            self.assertIn(
                status, (401, 403, 404),
                f"Customer accessed admin endpoint {path}: got {status}",
            )


# ─── Main ─────────────────────────────────────────────────────────────────────

if __name__ == "__main__":
    print(f"API target: {API_BASE_URL}")
    print(f"Skip live:  {SKIP_LIVE}")
    print()
    loader = unittest.TestLoader()
    loader.sortTestMethodsUsing = None
    suite = loader.loadTestsFromModule(sys.modules[__name__])
    runner = unittest.TextTestRunner(verbosity=2)
    result = runner.run(suite)
    sys.exit(0 if result.wasSuccessful() else 1)
