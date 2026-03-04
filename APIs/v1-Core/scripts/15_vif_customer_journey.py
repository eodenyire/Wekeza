#!/usr/bin/env python3
import json
import os
import random
import string
import sys
import urllib.error
import urllib.request
import urllib.parse
from datetime import datetime, timedelta, timezone

API_BASE_URL = os.getenv("API_BASE_URL", "http://localhost:8080").rstrip("/")
USERNAME = os.getenv("TELLER_USERNAME", "teller1")
PASSWORD = os.getenv("TELLER_PASSWORD", "Teller@123")


def request_json(method: str, path: str, token: str | None = None, body: dict | None = None):
    url = f"{API_BASE_URL}{path}"
    headers = {"Content-Type": "application/json"}
    data = None

    if token:
        headers["Authorization"] = f"Bearer {token}"

    if body is not None:
        data = json.dumps(body).encode("utf-8")

    req = urllib.request.Request(url=url, method=method, headers=headers, data=data)

    try:
        with urllib.request.urlopen(req, timeout=30) as resp:
            raw = resp.read().decode("utf-8")
            payload = json.loads(raw) if raw else {}
            return resp.status, payload
    except urllib.error.HTTPError as exc:
        raw = exc.read().decode("utf-8") if exc.fp else ""
        try:
            payload = json.loads(raw) if raw else {}
        except json.JSONDecodeError:
            payload = {"raw": raw}
        return exc.code, payload


def expect_2xx(step: str, status: int, payload: dict):
    if status not in (200, 201, 204):
        raise RuntimeError(f"{step} failed: status={status}, payload={payload}")


def random_suffix(length: int = 6) -> str:
    return "".join(random.choice(string.digits) for _ in range(length))


def pick(payload: dict, *keys: str):
    for key in keys:
        if key in payload:
            return payload[key]
    return None


def main() -> int:
    status, auth = request_json(
        "POST",
        "/api/authentication/login",
        body={"Username": USERNAME, "Password": PASSWORD},
    )
    token = auth.get("token") or auth.get("Token")
    if status != 200 or not token:
        raise RuntimeError(f"login failed: status={status}, payload={auth}")

    marker = random_suffix()

    status, customer = request_json(
        "POST",
        "/api/vif/customers/register",
        token=token,
        body={
            "firstName": "Vif",
            "lastName": f"Customer{marker}",
            "identificationNumber": f"IDVIF{marker}",
            "email": f"vif.customer{marker}@example.com",
            "phoneNumber": f"+254700{marker}",
            "riskRating": 1,
        },
    )
    expect_2xx("register customer", status, customer)
    cif_number = pick(customer, "CifNumber", "cifNumber")
    if not cif_number:
        raise RuntimeError(f"CIF not returned in customer payload: {customer}")

    status, account = request_json(
        "POST",
        "/api/vif/accounts/register",
        token=token,
        body={
            "cifNumber": cif_number,
            "accountType": "Savings",
            "currency": "KES",
            "initialDeposit": 5000,
            "branchCode": "BR100001",
        },
    )
    expect_2xx("register account", status, account)
    account_number = pick(account, "AccountNumber", "accountNumber")
    if not account_number:
        raise RuntimeError(f"AccountNumber not returned in account payload: {account}")

    status, _ = request_json(
        "POST",
        "/api/vif/transactions/cash-deposit",
        token=token,
        body={"accountNumber": account_number, "amount": 1500, "currency": "KES", "description": "Counter cash deposit"},
    )
    expect_2xx("cash deposit", status, _)

    status, _ = request_json(
        "POST",
        "/api/vif/transactions/cash-withdrawal",
        token=token,
        body={"accountNumber": account_number, "amount": 500, "currency": "KES", "description": "Counter cash withdrawal"},
    )
    expect_2xx("cash withdrawal", status, _)

    status, _ = request_json(
        "POST",
        "/api/vif/transactions/airtime",
        token=token,
        body={
            "accountNumber": account_number,
            "amount": 100,
            "currency": "KES",
            "phoneNumber": f"+254711{marker}",
            "provider": "Safaricom",
        },
    )
    expect_2xx("buy airtime", status, _)

    status, _ = request_json(
        "POST",
        "/api/vif/transactions/mpesa",
        token=token,
        body={"accountNumber": account_number, "amount": 250, "currency": "KES", "phoneNumber": f"+254712{marker}"},
    )
    expect_2xx("send mpesa", status, _)

    status, _ = request_json(
        "POST",
        "/api/vif/transactions/cheque-deposit",
        token=token,
        body={
            "accountNumber": account_number,
            "amount": 1200,
            "currency": "KES",
            "chequeNumber": f"CHQ{marker}",
            "drawerBank": "Demo Bank",
        },
    )
    expect_2xx("cheque deposit", status, _)

    status, _ = request_json(
        "POST",
        "/api/vif/investments/shares/buy",
        token=token,
        body={
            "accountNumber": account_number,
            "amount": 300,
            "currency": "KES",
            "instrumentCode": "SCOM",
            "quantity": 10,
        },
    )
    expect_2xx("buy shares", status, _)

    status, _ = request_json(
        "POST",
        "/api/vif/investments/treasury/buy",
        token=token,
        body={
            "accountNumber": account_number,
            "amount": 700,
            "currency": "KES",
            "instrumentType": "TBill",
            "instrumentCode": "TBILL-91D",
            "quantity": 1,
            "tenorDays": 91,
        },
    )
    expect_2xx("buy treasury", status, _)

    status, _ = request_json(
        "POST",
        "/api/vif/services/atm-card/lock",
        token=token,
        body={"accountNumber": account_number, "reason": "Customer requested temporary lock"},
    )
    expect_2xx("lock atm card", status, _)

    status, _ = request_json(
        "POST",
        "/api/vif/services/mobile-loan/block",
        token=token,
        body={"accountNumber": account_number, "reason": "Customer opted out"},
    )
    expect_2xx("block mobile loan", status, _)

    from_date = (datetime.now(timezone.utc) - timedelta(days=30)).isoformat()
    to_date = datetime.now(timezone.utc).isoformat()
    statement_query = urllib.parse.urlencode(
        {
            "from": from_date,
            "to": to_date,
            "pageNumber": 1,
            "pageSize": 20,
        }
    )
    status, statement = request_json(
        "GET",
        f"/api/vif/transactions/statement/{account_number}?{statement_query}",
        token=token,
    )
    expect_2xx("statement", status, statement)

    status, balance = request_json("GET", f"/api/vif/accounts/{account_number}/balance", token=token)
    expect_2xx("balance", status, balance)

    print("=== VIF CUSTOMER JOURNEY PASSED ===")
    print(json.dumps({
        "cifNumber": cif_number,
        "accountNumber": account_number,
        "finalBalance": pick(balance, "Balance", "balance"),
        "statementCount": pick(statement, "TotalRecords", "totalRecords"),
    }, indent=2))

    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except Exception as exc:
        print(f"VIF journey failed: {exc}")
        sys.exit(1)
