#!/usr/bin/env python3
import json
import os
import sys
import urllib.error
import urllib.request

API_BASE_URL = os.getenv("API_BASE_URL", "http://localhost:8080").rstrip("/")
ACCOUNT = os.getenv("ACTIVE_ACCOUNT_NUMBER", "ACC0000055583")


def request_json(method: str, path: str, token: str | None = None, body: dict | None = None):
    url = f"{API_BASE_URL}{path}"
    data = json.dumps(body).encode("utf-8") if body is not None else None
    headers = {"Content-Type": "application/json"}
    if token:
        headers["Authorization"] = f"Bearer {token}"
    req = urllib.request.Request(url=url, method=method, data=data, headers=headers)
    try:
        with urllib.request.urlopen(req, timeout=25) as response:
            raw = response.read().decode("utf-8")
            return response.status, json.loads(raw) if raw else {}
    except urllib.error.HTTPError as exc:
        raw = exc.read().decode("utf-8") if exc.fp else ""
        try:
            payload = json.loads(raw) if raw else {}
        except json.JSONDecodeError:
            payload = {"raw": raw}
        return exc.code, payload


def login(username: str, password: str) -> str:
    status, payload = request_json("POST", "/api/authentication/login", body={"Username": username, "Password": password})
    token = payload.get("Token") or payload.get("token")
    if status != 200 or not token:
        raise RuntimeError(f"Login failed for {username}: {status}")
    return token


def main() -> int:
    admin = login(os.getenv("ADMIN_USERNAME", "admin"), os.getenv("ADMIN_PASSWORD", "Admin@123"))
    manager = login(os.getenv("MANAGER_USERNAME", "manager1"), os.getenv("MANAGER_PASSWORD", "Manager@123"))
    teller = login(os.getenv("TELLER_USERNAME", "teller1"), os.getenv("TELLER_PASSWORD", "Teller@123"))

    # Each entry: (label, method, path, token, body, expected_status)
    form_calls = [
        # -- Teller Portal --------------------------------------------------------
        ("Teller session start", "POST", "/api/teller/session/start", teller, {
            "branchId": "11111111-1111-1111-1111-111111111111",
            "tellerCode": "TEL001",
            "tellerName": "Teller One",
            "branchCode": "MAIN",
            "openingCashBalance": 100000,
        }, 200),
        ("Teller cash deposit", "POST", "/api/teller/transactions/cash-deposit", teller, {
            "accountNumber": ACCOUNT,
            "amount": 150,
            "currency": "KES",
            "narration": "Form matrix deposit",
        }, 200),
        ("Teller cash withdrawal", "POST", "/api/teller/transactions/cash-withdrawal", teller, {
            "accountNumber": ACCOUNT,
            "amount": 100,
            "currency": "KES",
        }, 200),
        # -- Branch Manager Portal ------------------------------------------------
        ("Branch compliance view", "GET", "/api/branch-manager/compliance", manager, None, 200),
        ("Branch dashboard view", "GET", "/api/branch-manager/dashboard", manager, None, 200),
        ("Branch staff list", "GET", "/api/branch-manager/staff", manager, None, 200),
        # -- Supervisor Portal ----------------------------------------------------
        ("Supervisor team view", "GET", "/api/supervisor/team", manager, None, 200),
        ("Supervisor pending approvals", "GET", "/api/supervisor/approvals/pending", manager, None, 200),
        ("Supervisor daily metrics", "GET", "/api/supervisor/operations/daily-metrics", manager, None, 200),
        # -- Admin Portal ---------------------------------------------------------
        ("Admin system stats", "GET", "/api/administrator/system/stats", admin, None, 200),
        # -- Compliance Portal ----------------------------------------------------
        ("Compliance metrics view", "GET", "/api/compliance-portal/risk-metrics", admin, None, 200),
        # -- Payments Portal -------------------------------------------------------
        ("Payments status view", "GET", "/api/payments-portal/payment-status", admin, None, 200),
        # -- Product GL Portal -----------------------------------------------------
        ("Product GL products", "GET", "/api/product-gl-portal/products", admin, None, 200),
        # -- Treasury Portal -------------------------------------------------------
        ("Treasury liquidity", "GET", "/api/treasury-portal/liquidity", admin, None, 200),
        # -- Trade Finance Portal --------------------------------------------------
        ("Trade finance LCs", "GET", "/api/trade-finance-portal/letters-of-credit", admin, None, 200),
        # -- Staff Self-Service Portal (Teller role is authorized) ----------------
        ("Staff profile", "GET", "/api/staff-self-service/profile", teller, None, 200),
        ("Staff leave balance", "GET", "/api/staff-self-service/leave/balance", teller, None, 200),
        ("Staff payroll current", "GET", "/api/staff-self-service/payroll/current", teller, None, 200),
    ]

    failures = []
    print("\n=== FRONTEND FORM/API MATRIX ===")
    for label, method, path, token, body, expected in form_calls:
        status, payload = request_json(method, path, token=token, body=body)
        acceptable = expected if isinstance(expected, set) else {expected}
        ok = status in acceptable
        print(f"[{'PASS' if ok else 'FAIL'}] {label} -> {status}")
        if not ok:
            failures.append((label, path, status, expected, payload))

    print(f"\nTotal checked: {len(form_calls)}  Failures: {len(failures)}")
    if failures:
        print("\nFailures:")
        for failure in failures:
            label, path, status, expected, payload = failure
            print(f" - {label} ({path}) status={status}, expected={expected}, payload={str(payload)[:220]}")
        return 1

    print("All checked frontend form/API flows passed")
    return 0


if __name__ == "__main__":
    sys.exit(main())
