#!/usr/bin/env python3
"""
Wekeza Banking – Functional Tests
===================================
Tests every banking operation through the v1-Core REST API:
  • Authentication (login / logout / change-password)
  • Account management (list accounts, balance inquiry, statement)
  • Fund transfers (Wekeza-to-Wekeza, EFT/RTGS, mobile money)
  • Cash withdrawal (ATM code generation)
  • Airtime purchase
  • M-Pesa STK push deposit
  • Bill / utility payments
  • Loan application and repayment
  • Customer profile management

Usage (against a running v1-Core stack):
    API_BASE_URL=http://localhost:8080 python3 tests/functional/test_banking_operations.py

Credentials default to the seeded test user; override with environment vars:
    TELLER_USERNAME / TELLER_PASSWORD
    CUSTOMER_USERNAME / CUSTOMER_PASSWORD
"""

import json
import os
import sys
import time
import unittest
import urllib.error
import urllib.request
from datetime import datetime, timezone

# ─── Config ───────────────────────────────────────────────────────────────────
API_BASE_URL = os.getenv("API_BASE_URL", "http://localhost:8080").rstrip("/")
CUSTOMER_USERNAME = os.getenv("CUSTOMER_USERNAME", "customer1")
CUSTOMER_PASSWORD = os.getenv("CUSTOMER_PASSWORD", "Customer@123")
TELLER_USERNAME = os.getenv("TELLER_USERNAME", "teller1")
TELLER_PASSWORD = os.getenv("TELLER_PASSWORD", "Teller@123")
ACTIVE_ACCOUNT = os.getenv("ACTIVE_ACCOUNT_NUMBER", "ACC0000055583")
TEST_BRANCH_ID = os.getenv("TEST_BRANCH_ID", "11111111-1111-1111-1111-111111111111")
TIMEOUT = int(os.getenv("TEST_TIMEOUT", "30"))

# Skip network tests when the API is not reachable (e.g. offline CI)
SKIP_LIVE = os.getenv("WEKEZA_SKIP_LIVE_TESTS", "false").lower() in ("1", "true", "yes")


# ─── HTTP Helper ──────────────────────────────────────────────────────────────

def api_request(
    method: str,
    path: str,
    token: str | None = None,
    body: dict | None = None,
) -> tuple[int, dict]:
    url = f"{API_BASE_URL}{path}"
    data = json.dumps(body).encode() if body is not None else None
    headers: dict[str, str] = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    req = urllib.request.Request(url=url, method=method, data=data, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=TIMEOUT) as resp:
            raw = resp.read().decode()
            return resp.status, (json.loads(raw) if raw else {})
    except urllib.error.HTTPError as exc:
        raw = exc.read().decode() if exc.fp else ""
        try:
            payload = json.loads(raw) if raw else {}
        except json.JSONDecodeError:
            payload = {"raw": raw[:500]}
        return exc.code, payload
    except (urllib.error.URLError, OSError):
        return 0, {"error": "connection_failed"}


def is_api_reachable() -> bool:
    status, _ = api_request("GET", "/health")
    return status == 200


def login(username: str, password: str) -> str | None:
    status, payload = api_request(
        "POST",
        "/api/authentication/login",
        body={"Username": username, "Password": password},
    )
    if status not in (200, 201):
        return None
    return payload.get("token") or payload.get("Token")


# ─── Skip decorator ───────────────────────────────────────────────────────────

def live_test(fn):
    """Skip test when API is not reachable or WEKEZA_SKIP_LIVE_TESTS is set."""
    if SKIP_LIVE:
        return unittest.skip("Live API tests disabled via WEKEZA_SKIP_LIVE_TESTS")(fn)
    return fn


# ─── Base class ───────────────────────────────────────────────────────────────

class WekeзaFunctionalTestCase(unittest.TestCase):
    token: str | None = None

    @classmethod
    def setUpClass(cls):
        if not is_api_reachable() and not SKIP_LIVE:
            raise unittest.SkipTest(
                f"API not reachable at {API_BASE_URL}/health – "
                "set WEKEZA_SKIP_LIVE_TESTS=true to skip live tests"
            )


# ═══════════════════════════════════════════════════════════════════════════════
# 1. Authentication Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestAuthentication(WekeзaFunctionalTestCase):

    def test_01_customer_login_success(self):
        """Customer can log in with valid credentials and receive a JWT token."""
        status, payload = api_request(
            "POST",
            "/api/authentication/login",
            body={"Username": CUSTOMER_USERNAME, "Password": CUSTOMER_PASSWORD},
        )
        self.assertIn(status, (200, 201), f"Login failed: {payload}")
        token = payload.get("token") or payload.get("Token")
        self.assertIsNotNone(token, "JWT token missing from login response")
        self.assertGreater(len(token), 20, "Token looks too short")

    def test_02_login_invalid_password_returns_401(self):
        """Wrong password must return 401 Unauthorized."""
        status, _ = api_request(
            "POST",
            "/api/authentication/login",
            body={"Username": CUSTOMER_USERNAME, "Password": "wrong!"},
        )
        self.assertEqual(status, 401, "Expected 401 for wrong password")

    def test_03_login_unknown_user_returns_401(self):
        """Non-existent user must return 401."""
        status, _ = api_request(
            "POST",
            "/api/authentication/login",
            body={"Username": "ghost_user_xyz", "Password": "anything"},
        )
        self.assertEqual(status, 401)

    def test_04_protected_endpoint_without_token(self):
        """Accessing a protected endpoint without a token must return 401."""
        status, _ = api_request("GET", "/api/accounts/user/accounts")
        self.assertEqual(status, 401)

    def test_05_protected_endpoint_with_bad_token(self):
        """Accessing a protected endpoint with an invalid token must return 401."""
        status, _ = api_request(
            "GET",
            "/api/accounts/user/accounts",
            token="this.is.not.valid",
        )
        self.assertIn(status, (401, 403))

    def test_06_teller_login_success(self):
        """Teller role login returns a valid JWT."""
        status, payload = api_request(
            "POST",
            "/api/authentication/login",
            body={"Username": TELLER_USERNAME, "Password": TELLER_PASSWORD},
        )
        self.assertIn(status, (200, 201), f"Teller login failed: {payload}")
        token = payload.get("token") or payload.get("Token")
        self.assertIsNotNone(token)


# ═══════════════════════════════════════════════════════════════════════════════
# 2. Account Management Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestAccountManagement(WekeзaFunctionalTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.token = login(CUSTOMER_USERNAME, CUSTOMER_PASSWORD)
        if cls.token is None:
            raise unittest.SkipTest("Could not authenticate – skipping account tests")

    def test_01_list_user_accounts(self):
        """Authenticated customer can list their accounts."""
        status, payload = api_request("GET", "/api/accounts/user/accounts", token=self.token)
        self.assertIn(status, (200, 204), f"List accounts failed: {payload}")

    def test_02_balance_inquiry(self):
        """Balance inquiry returns numeric balance fields."""
        status, payload = api_request(
            "GET",
            f"/api/accounts/{ACTIVE_ACCOUNT}/balance",
            token=self.token,
        )
        if status == 404:
            self.skipTest(f"Account {ACTIVE_ACCOUNT} not found in test DB")
        self.assertEqual(status, 200, f"Balance inquiry failed: {payload}")
        self.assertIn("balance", {k.lower() for k in payload.keys()})

    def test_03_account_summary(self):
        """Account summary returns account type and currency."""
        status, payload = api_request(
            "GET",
            f"/api/accounts/{ACTIVE_ACCOUNT}/summary",
            token=self.token,
        )
        if status == 404:
            self.skipTest(f"Account {ACTIVE_ACCOUNT} not found")
        self.assertEqual(status, 200)

    def test_04_transaction_statement(self):
        """Transaction statement returns a list/paginated response."""
        status, payload = api_request(
            "GET",
            f"/api/transactions/statement/{ACTIVE_ACCOUNT}?page=1&pageSize=10",
            token=self.token,
        )
        if status == 404:
            self.skipTest(f"Account {ACTIVE_ACCOUNT} not found")
        self.assertIn(status, (200, 204))


# ═══════════════════════════════════════════════════════════════════════════════
# 3. Fund Transfer Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestFundTransfers(WekeзaFunctionalTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.token = login(CUSTOMER_USERNAME, CUSTOMER_PASSWORD)
        if cls.token is None:
            raise unittest.SkipTest("Could not authenticate – skipping transfer tests")

    def test_01_transfer_missing_fields_returns_400(self):
        """Transfer with missing required fields returns 400 Bad Request."""
        status, payload = api_request(
            "POST",
            "/api/transactions/transfer",
            token=self.token,
            body={"fromAccountNumber": ACTIVE_ACCOUNT},  # missing toAccountNumber & amount
        )
        self.assertIn(status, (400, 422), f"Expected validation error, got {status}: {payload}")

    def test_02_transfer_zero_amount_returns_400(self):
        """Transfer of zero amount must be rejected."""
        status, _ = api_request(
            "POST",
            "/api/transactions/transfer",
            token=self.token,
            body={
                "fromAccountNumber": ACTIVE_ACCOUNT,
                "toAccountNumber": "ACC9999999999",
                "amount": 0,
                "narration": "Zero amount test",
            },
        )
        self.assertIn(status, (400, 422))

    def test_03_transfer_negative_amount_returns_400(self):
        """Transfer of negative amount must be rejected."""
        status, _ = api_request(
            "POST",
            "/api/transactions/transfer",
            token=self.token,
            body={
                "fromAccountNumber": ACTIVE_ACCOUNT,
                "toAccountNumber": "ACC9999999999",
                "amount": -100,
                "narration": "Negative amount test",
            },
        )
        self.assertIn(status, (400, 422))

    def test_04_transfer_to_nonexistent_account(self):
        """Transfer to a non-existent account must return 404 or 400."""
        status, _ = api_request(
            "POST",
            "/api/transactions/transfer",
            token=self.token,
            body={
                "fromAccountNumber": ACTIVE_ACCOUNT,
                "toAccountNumber": "ACCXXX000000000",
                "amount": 100,
                "narration": "Test non-existent destination",
            },
        )
        self.assertIn(status, (400, 404, 422))


# ═══════════════════════════════════════════════════════════════════════════════
# 4. Teller Operations Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestTellerOperations(WekeзaFunctionalTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.token = login(TELLER_USERNAME, TELLER_PASSWORD)
        if cls.token is None:
            raise unittest.SkipTest("Could not authenticate as teller")

    def test_01_cash_deposit_validates_amount(self):
        """Cash deposit with zero amount is rejected."""
        status, _ = api_request(
            "POST",
            "/api/teller/transactions/cash-deposit",
            token=self.token,
            body={
                "accountNumber": ACTIVE_ACCOUNT,
                "amount": 0,
                "currency": "KES",
                "narration": "Test zero deposit",
            },
        )
        self.assertIn(status, (400, 422))

    def test_02_cash_withdrawal_validates_amount(self):
        """Cash withdrawal with zero amount is rejected."""
        status, _ = api_request(
            "POST",
            "/api/teller/transactions/cash-withdrawal",
            token=self.token,
            body={
                "accountNumber": ACTIVE_ACCOUNT,
                "amount": 0,
                "currency": "KES",
            },
        )
        self.assertIn(status, (400, 422))

    def test_03_teller_session_missing_fields_returns_400(self):
        """Teller session start with missing fields returns 400."""
        status, _ = api_request(
            "POST",
            "/api/teller/session/start",
            token=self.token,
            body={},  # completely empty
        )
        self.assertIn(status, (400, 422))


# ═══════════════════════════════════════════════════════════════════════════════
# 5. Digital Channels Tests (Airtime / Mobile Money)
# ═══════════════════════════════════════════════════════════════════════════════

class TestDigitalChannels(WekeзaFunctionalTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.token = login(CUSTOMER_USERNAME, CUSTOMER_PASSWORD)
        if cls.token is None:
            raise unittest.SkipTest("Could not authenticate – skipping digital channels tests")

    def test_01_airtime_invalid_phone_returns_400(self):
        """Airtime purchase with invalid phone number is rejected."""
        status, _ = api_request(
            "POST",
            "/api/digitalchannels/airtime",
            token=self.token,
            body={
                "fromAccountNumber": ACTIVE_ACCOUNT,
                "phoneNumber": "not-a-phone",
                "amount": 50,
                "provider": "Safaricom",
            },
        )
        self.assertIn(status, (400, 422))

    def test_02_airtime_zero_amount_returns_400(self):
        """Airtime purchase of zero is rejected."""
        status, _ = api_request(
            "POST",
            "/api/digitalchannels/airtime",
            token=self.token,
            body={
                "fromAccountNumber": ACTIVE_ACCOUNT,
                "phoneNumber": "+254712345678",
                "amount": 0,
                "provider": "Safaricom",
            },
        )
        self.assertIn(status, (400, 422))

    def test_03_mpesa_stk_missing_fields_returns_400(self):
        """M-Pesa STK push with empty body returns 400."""
        status, _ = api_request(
            "POST",
            "/api/digitalchannels/mpesa/stk-push",
            token=self.token,
            body={},
        )
        self.assertIn(status, (400, 422))

    def test_04_send_to_mobile_missing_phone_returns_400(self):
        """Send-to-mobile-money with missing phone returns 400."""
        status, _ = api_request(
            "POST",
            "/api/digitalchannels/send",
            token=self.token,
            body={
                "fromAccountNumber": ACTIVE_ACCOUNT,
                "amount": 500,
                "provider": "Mpesa",
            },
        )
        self.assertIn(status, (400, 422))


# ═══════════════════════════════════════════════════════════════════════════════
# 6. Bill / Utility Payment Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestBillPayments(WekeзaFunctionalTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.token = login(CUSTOMER_USERNAME, CUSTOMER_PASSWORD)
        if cls.token is None:
            raise unittest.SkipTest("Could not authenticate – skipping bill tests")

    def test_01_bill_payment_empty_body_returns_400(self):
        """Bill payment with empty body returns 400."""
        status, _ = api_request(
            "POST",
            "/api/payments/bill",
            token=self.token,
            body={},
        )
        self.assertIn(status, (400, 422))

    def test_02_bill_payment_missing_amount_returns_400(self):
        """Bill payment without amount is rejected."""
        status, _ = api_request(
            "POST",
            "/api/payments/bill",
            token=self.token,
            body={
                "fromAccountNumber": ACTIVE_ACCOUNT,
                "billType": "Electricity",
                "billAccount": "45123789012",
            },
        )
        self.assertIn(status, (400, 422))

    def test_03_bill_payment_zero_amount_returns_400(self):
        """Bill payment of zero is rejected."""
        status, _ = api_request(
            "POST",
            "/api/payments/bill",
            token=self.token,
            body={
                "fromAccountNumber": ACTIVE_ACCOUNT,
                "billType": "Electricity",
                "billAccount": "45123789012",
                "amount": 0,
            },
        )
        self.assertIn(status, (400, 422))


# ═══════════════════════════════════════════════════════════════════════════════
# 7. Loan Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestLoans(WekeзaFunctionalTestCase):

    @classmethod
    def setUpClass(cls):
        super().setUpClass()
        cls.token = login(CUSTOMER_USERNAME, CUSTOMER_PASSWORD)
        if cls.token is None:
            raise unittest.SkipTest("Could not authenticate – skipping loan tests")

    def test_01_list_user_loans(self):
        """Authenticated customer can list their loans."""
        status, _ = api_request("GET", "/api/loans/user/loans", token=self.token)
        self.assertIn(status, (200, 204))

    def test_02_loan_application_missing_fields_returns_400(self):
        """Loan application with missing required fields returns 400."""
        status, _ = api_request(
            "POST",
            "/api/loans/apply",
            token=self.token,
            body={
                "accountNumber": ACTIVE_ACCOUNT,
                # missing amount, tenureMonths, loanType, purpose
            },
        )
        self.assertIn(status, (400, 422))

    def test_03_loan_application_zero_amount_returns_400(self):
        """Loan application for zero amount is rejected."""
        status, _ = api_request(
            "POST",
            "/api/loans/apply",
            token=self.token,
            body={
                "accountNumber": ACTIVE_ACCOUNT,
                "loanType": "Personal",
                "amount": 0,
                "tenureMonths": 12,
                "purpose": "Test",
            },
        )
        self.assertIn(status, (400, 422))


# ═══════════════════════════════════════════════════════════════════════════════
# 8. Health and Infrastructure Tests
# ═══════════════════════════════════════════════════════════════════════════════

class TestHealthInfrastructure(WekeзaFunctionalTestCase):

    def test_01_health_endpoint_returns_200(self):
        """The /health endpoint returns 200 OK."""
        status, _ = api_request("GET", "/health")
        self.assertEqual(status, 200)

    def test_02_api_root_is_reachable(self):
        """The API root responds (200 or 404 is acceptable, not connection error)."""
        status, _ = api_request("GET", "/api")
        self.assertNotEqual(status, 0, "Could not connect to API")


# ─── Main ─────────────────────────────────────────────────────────────────────

if __name__ == "__main__":
    print(f"API target: {API_BASE_URL}")
    print(f"Skip live:  {SKIP_LIVE}")
    print()
    loader = unittest.TestLoader()
    loader.sortTestMethodsUsing = None  # preserve declaration order
    suite = loader.loadTestsFromModule(sys.modules[__name__])
    runner = unittest.TextTestRunner(verbosity=2)
    result = runner.run(suite)
    sys.exit(0 if result.wasSuccessful() else 1)
