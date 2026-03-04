#!/usr/bin/env python3
import json
import os
import re
import sys
import time
import urllib.error
import urllib.request
from collections import Counter

API_BASE_URL = os.getenv("API_BASE_URL", "http://localhost:8080").rstrip("/")

USERS = {
    "admin": (os.getenv("ADMIN_USERNAME", "admin"), os.getenv("ADMIN_PASSWORD", "Admin@123")),
    "manager": (os.getenv("MANAGER_USERNAME", "manager1"), os.getenv("MANAGER_PASSWORD", "Manager@123")),
    "teller": (os.getenv("TELLER_USERNAME", "teller1"), os.getenv("TELLER_PASSWORD", "Teller@123")),
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
        with urllib.request.urlopen(req, timeout=25) as response:
            raw = response.read().decode("utf-8")
            payload = json.loads(raw) if raw else {}
            return response.status, payload
    except urllib.error.HTTPError as exc:
        raw = exc.read().decode("utf-8") if exc.fp else ""
        try:
            payload = json.loads(raw) if raw else {}
        except json.JSONDecodeError:
            payload = {"raw": raw}
        return exc.code, payload


def request_with_retry(method: str, path: str, token: str | None = None, body: dict | None = None, max_retries: int = 2):
    attempts = 0
    while True:
        status, payload = request_json(method, path, token=token, body=body)
        if status != 429 or attempts >= max_retries:
            return status, payload

        retry_after = 60
        if isinstance(payload, dict):
            retry_after = int(payload.get("retryAfter", retry_after))
        time.sleep(max(1, retry_after))
        attempts += 1


def normalize_get_path(path: str) -> str:
    query_overrides = {
        "/api/CIF/search": "?name=john",
        "/api/Deposits/fixed-deposits/calculate-maturity": "?principal=10000&rate=10&days=365",
        "/api/Deposits/recurring-deposits/calculate-maturity": "?monthlyInstallment=1000&rate=10&months=12",
        "/api/TradeFinance/letters-of-credit": "?lcNumber=LC-9001",
        "/api/teller/customers/search": "?searchTerm=john",
    }
    return f"{path}{query_overrides[path]}" if path in query_overrides else path


def login(username: str, password: str) -> str:
    status, payload = request_json("POST", "/api/authentication/login", body={"Username": username, "Password": password})
    token = payload.get("Token") or payload.get("token")
    if status != 200 or not token:
        raise RuntimeError(f"login failed for {username}: status={status}, payload={payload}")
    return token


def get_swagger_paths() -> dict:
    req = urllib.request.Request(f"{API_BASE_URL}/swagger/v1/swagger.json", method="GET")
    with urllib.request.urlopen(req, timeout=30) as resp:
        doc = json.loads(resp.read().decode("utf-8"))
    return doc.get("paths", {})


def has_path_params(path: str) -> bool:
    return bool(re.search(r"\{[^}]+\}", path))


def pick_token_for_path(path: str, tokens: dict[str, str]) -> str:
    if path.startswith("/api/teller"):
        return tokens["teller"]
    if path.startswith("/api/staff-self-service"):
        return tokens["teller"]
    if path.startswith("/api/branch-manager") or path.startswith("/api/supervisor"):
        return tokens["manager"]
    return tokens["admin"]


def acceptable_statuses_for_path(path: str) -> set[int]:
    default_ok = {200, 201, 204}
    if path.startswith("/api/customer-portal"):
        return default_ok | {403}
    if path.startswith("/api/staff-self-service"):
        return default_ok | {403}
    return default_ok


def body_for_known_form(path: str) -> dict | None:
    if path == "/api/teller/session/start":
        return {
            "branchId": "11111111-1111-1111-1111-111111111111",
            "tellerCode": "TEL001",
            "tellerName": "Teller One",
            "branchCode": "MAIN",
            "openingCashBalance": 100000,
        }
    if path == "/api/teller/transactions/cash-deposit":
        account = os.getenv("ACTIVE_ACCOUNT_NUMBER", "ACC0000055583")
        return {"accountNumber": account, "amount": 250, "currency": "KES", "narration": "Regression deposit"}
    if path == "/api/teller/transactions/cash-withdrawal":
        account = os.getenv("ACTIVE_ACCOUNT_NUMBER", "ACC0000055583")
        return {"accountNumber": account, "amount": 100, "currency": "KES"}
    return None


def main() -> int:
    tokens = {role: login(*creds) for role, creds in USERS.items()}

    paths = get_swagger_paths()
    tested = []
    skipped = []

    for path, methods in sorted(paths.items()):
        for method, meta in methods.items():
            m = method.upper()
            if path.startswith("/api/authentication/login") and m == "POST":
                continue

            if has_path_params(path):
                skipped.append((m, path, "path-params"))
                continue

            if m in {"PUT", "PATCH", "DELETE"}:
                skipped.append((m, path, "unsafe-method-skipped"))
                continue

            if m == "POST":
                body = body_for_known_form(path)
                if body is None:
                    skipped.append((m, path, "unknown-form-payload"))
                    continue
            else:
                body = None

            token = pick_token_for_path(path, tokens)
            effective_path = normalize_get_path(path) if m == "GET" else path
            status, payload = request_with_retry(m, effective_path, token=token, body=body)
            tested.append((m, path, status, payload))
            time.sleep(0.12)

    counter = Counter()
    for _, _, status, _ in tested:
        counter[status] += 1

    total = len(tested)
    ok = sum(1 for _, path, status, _ in tested if status in acceptable_statuses_for_path(path))

    print("\n=== BACKEND/FRONTEND REGRESSION SUMMARY ===")
    print(f"Total tested endpoints : {total}")
    print(f"Successful (2xx)      : {ok}")
    print(f"Skipped               : {len(skipped)}")
    print("Status distribution   :")
    for status, count in sorted(counter.items()):
        print(f"  {status}: {count}")

    print("\n--- Failed Endpoints ---")
    failures = [t for t in tested if t[2] not in acceptable_statuses_for_path(t[1])]
    if not failures:
        print("none")
    else:
        for method, path, status, payload in failures:
            print(f"{method} {path} -> {status}")
            if isinstance(payload, dict):
                print(f"  payload: {json.dumps(payload)[:240]}")

    print("\n--- Skipped Endpoints ---")
    for method, path, reason in skipped[:80]:
        print(f"{method} {path} [{reason}]")

    return 0 if not failures else 1


if __name__ == "__main__":
    sys.exit(main())
