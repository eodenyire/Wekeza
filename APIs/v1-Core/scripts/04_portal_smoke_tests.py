#!/usr/bin/env python3
import json
import os
import sys
import urllib.error
import urllib.parse
import urllib.request

API_BASE_URL = os.getenv("API_BASE_URL", "http://localhost:8080").rstrip("/")
ACTIVE_ACCOUNT_NUMBER = os.getenv("ACTIVE_ACCOUNT_NUMBER", "ACC0008083435")

CREDENTIALS = {
    "admin": {
        "username": os.getenv("ADMIN_USERNAME", "admin"),
        "password": os.getenv("ADMIN_PASSWORD", "Admin@123"),
    },
    "manager": {
        "username": os.getenv("MANAGER_USERNAME", "manager1"),
        "password": os.getenv("MANAGER_PASSWORD", "Manager@123"),
    },
    "teller": {
        "username": os.getenv("TELLER_USERNAME", "teller1"),
        "password": os.getenv("TELLER_PASSWORD", "Teller@123"),
    },
}


def request_json(method: str, path: str, token: str | None = None, body: dict | None = None):
    url = f"{API_BASE_URL}{path}"
    data = None
    headers = {"Content-Type": "application/json"}

    if token:
        headers["Authorization"] = f"Bearer {token}"

    if body is not None:
        data = json.dumps(body).encode("utf-8")

    req = urllib.request.Request(url=url, method=method, data=data, headers=headers)

    try:
        with urllib.request.urlopen(req, timeout=20) as response:
            raw = response.read().decode("utf-8")
            payload = json.loads(raw) if raw else {}
            return response.status, payload
    except urllib.error.HTTPError as e:
        raw = e.read().decode("utf-8") if e.fp else ""
        try:
            payload = json.loads(raw) if raw else {}
        except json.JSONDecodeError:
            payload = {"raw": raw}
        return e.code, payload


def login(role_key: str):
    creds = CREDENTIALS[role_key]
    status, payload = request_json(
        "POST",
        "/api/authentication/login",
        body={"Username": creds["username"], "Password": creds["password"]},
    )

    token = payload.get("Token") or payload.get("token")
    if status != 200 or not token:
        raise RuntimeError(f"Login failed for {role_key}: status={status}, payload={payload}")
    return token


def run_checks(role_name: str, token: str, checks: list[tuple[str, str, str, dict | None, int]]):
    failures = []
    print(f"\n=== {role_name.upper()} CHECKS ===")
    for label, method, path, body, expected_status in checks:
        status, payload = request_json(method, path, token=token, body=body)
        ok = status == expected_status
        marker = "PASS" if ok else "FAIL"
        print(f"[{marker}] {label} -> {method} {path} => {status}")
        if not ok:
            failures.append((label, method, path, status, expected_status, payload))
    return failures


def main():
    all_failures = []

    admin_token = login("admin")
    manager_token = login("manager")
    teller_token = login("teller")

    admin_checks = [
        ("Current user", "GET", "/api/authentication/me", None, 200),
        ("Compliance metrics", "GET", "/api/compliance-portal/risk-metrics", None, 200),
        ("Trade finance LCs", "GET", "/api/trade-finance-portal/letters-of-credit", None, 200),
        ("Payments status", "GET", "/api/payments-portal/payment-status", None, 200),
        ("Product GL products", "GET", "/api/product-gl-portal/products", None, 200),
        ("Treasury liquidity", "GET", "/api/treasury-portal/liquidity", None, 200),
        ("Admin stats", "GET", "/api/administrator/system/stats", None, 200),
    ]

    manager_checks = [
        ("Branch dashboard", "GET", "/api/branch-manager/dashboard", None, 200),
        ("Branch staff", "GET", "/api/branch-manager/staff", None, 200),
        ("Supervisor team", "GET", "/api/supervisor/team", None, 200),
        ("Branch compliance", "GET", "/api/branch-manager/compliance", None, 200),
    ]

    teller_checks = [
        ("Teller dashboard", "GET", "/api/teller/dashboard", None, 200),
        ("Recent transactions", "GET", "/api/teller/transactions/recent", None, 200),
        ("Cash drawer balance", "GET", "/api/teller/cash-drawer/balance", None, 200),
        ("Customer search", "GET", "/api/teller/customers/search?searchTerm=john", None, 200),
        (
            "Start session",
            "POST",
            "/api/teller/session/start",
            {
                "branchId": "11111111-1111-1111-1111-111111111111",
                "tellerCode": "TEL001",
                "tellerName": "Teller One",
                "branchCode": "MAIN",
                "openingCashBalance": 100000,
            },
            200,
        ),
        (
            "Cash withdrawal",
            "POST",
            "/api/teller/transactions/cash-withdrawal",
            {
                "accountNumber": ACTIVE_ACCOUNT_NUMBER,
                "amount": 100,
                "currency": "KES",
            },
            200,
        ),
        (
            "Cash deposit",
            "POST",
            "/api/teller/transactions/cash-deposit",
            {
                "accountNumber": ACTIVE_ACCOUNT_NUMBER,
                "amount": 100,
                "currency": "KES",
                "narration": "Smoke test",
            },
            200,
        ),
    ]

    all_failures.extend(run_checks("admin", admin_token, admin_checks))
    all_failures.extend(run_checks("manager", manager_token, manager_checks))
    all_failures.extend(run_checks("teller", teller_token, teller_checks))

    print("\n=== SUMMARY ===")
    if all_failures:
        print(f"FAILED: {len(all_failures)} checks")
        for item in all_failures:
            label, method, path, status, expected, payload = item
            print(f" - {label}: {method} {path} got {status}, expected {expected}, payload={payload}")
        sys.exit(1)

    print("PASSED: all checks returned expected status codes")
    sys.exit(0)


if __name__ == "__main__":
    main()
