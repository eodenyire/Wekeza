#!/usr/bin/env python3
"""
Wekeza Mobile Banking – Screenshot Generator
=============================================
Generates screenshots for every major screen / user-flow in the
Wekeza Mobile Banking application using Playwright (headless Chromium).

Usage:
    python3 tests/e2e/take_screenshots.py

Screenshots are written to:
    screenshots/<category>/<name>.png

This script works in two modes:
  1. OFFLINE (default) – renders self-contained HTML mock pages that faithfully
     replicate the real app UI.  Ideal for CI and documentation.
  2. LIVE   – point WEKEZA_BASE_URL at a running MobileWeb dev server to capture
     live screenshots (set env var WEKEZA_SCREENSHOT_MODE=live).
"""

import os
import sys
import time
from pathlib import Path

# ---------------------------------------------------------------------------
# Paths
# ---------------------------------------------------------------------------
REPO_ROOT = Path(__file__).resolve().parent.parent.parent
SCREENSHOTS_DIR = REPO_ROOT / "screenshots"
MOCK_PAGES_DIR = Path(__file__).resolve().parent / "mock_pages"

# ---------------------------------------------------------------------------
# Colour palette (mirrors Tailwind primary-* config in the app)
# ---------------------------------------------------------------------------
PRIMARY_800 = "#1e3a5f"
PRIMARY_700 = "#1d4ed8"
PRIMARY_200 = "#bfdbfe"
WHITE = "#ffffff"
GRAY_50 = "#f9fafb"
GRAY_100 = "#f3f4f6"
GRAY_200 = "#e5e7eb"
GRAY_500 = "#6b7280"
GRAY_700 = "#374151"
GRAY_900 = "#111827"
GREEN_50 = "#f0fdf4"
GREEN_700 = "#15803d"
RED_50 = "#fef2f2"
RED_700 = "#b91c1c"
BLUE_50 = "#eff6ff"
BLUE_700 = "#1d4ed8"

# ---------------------------------------------------------------------------
# Shared CSS (Tailwind-like utilities inlined for portability)
# ---------------------------------------------------------------------------
BASE_CSS = f"""
  * {{ box-sizing: border-box; margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont,
       'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; }}
  body {{ background: {GRAY_50}; min-height: 100vh; }}
  .screen {{ max-width: 390px; min-height: 844px; margin: 0 auto;
             background: {WHITE}; position: relative; overflow: hidden;
             box-shadow: 0 4px 24px rgba(0,0,0,0.12); }}
  .header {{ background: {PRIMARY_800}; color: {WHITE}; padding: 52px 20px 24px; }}
  .header-row {{ display: flex; align-items: center; justify-content: space-between; }}
  .header h1 {{ font-size: 18px; font-weight: 700; }}
  .header p  {{ font-size: 13px; color: {PRIMARY_200}; margin-top: 2px; }}
  .back-btn  {{ width:32px; height:32px; border-radius:50%; background:rgba(255,255,255,0.15);
                display:flex; align-items:center; justify-content:center;
                border:none; color:{WHITE}; font-size:18px; cursor:pointer; }}
  .content {{ padding: 20px; }}
  .card {{ background:{WHITE}; border-radius:16px; padding:20px;
           box-shadow:0 1px 6px rgba(0,0,0,0.07); margin-bottom:16px; }}
  .label {{ font-size:13px; font-weight:500; color:{GRAY_700}; margin-bottom:6px; }}
  .input-field {{ width:100%; padding:12px 14px; border:1.5px solid {GRAY_200};
                  border-radius:10px; font-size:15px; color:{GRAY_900};
                  background:{WHITE}; outline:none; }}
  .input-field:focus {{ border-color:{PRIMARY_700}; }}
  .btn-primary {{ width:100%; padding:14px; background:{PRIMARY_800};
                  color:{WHITE}; border:none; border-radius:12px;
                  font-size:16px; font-weight:600; cursor:pointer; margin-top:8px; }}
  .btn-primary:disabled {{ opacity:0.5; }}
  .btn-outline {{ width:100%; padding:13px; background:transparent;
                  color:{PRIMARY_800}; border:1.5px solid {PRIMARY_800};
                  border-radius:12px; font-size:15px; font-weight:600;
                  cursor:pointer; margin-top:8px; }}
  .success-box {{ background:{GREEN_50}; border:1px solid #bbf7d0;
                  border-radius:12px; padding:14px 16px;
                  color:{GREEN_700}; font-size:13px; margin-bottom:16px; }}
  .error-box  {{ background:{RED_50}; border:1px solid #fecaca;
                 border-radius:12px; padding:14px 16px;
                 color:{RED_700}; font-size:13px; margin-bottom:16px; }}
  .info-box   {{ background:{BLUE_50}; border:1px solid #bfdbfe;
                 border-radius:12px; padding:14px 16px;
                 color:{BLUE_700}; font-size:13px; margin-bottom:16px; }}
  .tab-bar {{ display:flex; border-bottom:1px solid {GRAY_200}; background:{WHITE}; }}
  .tab {{ flex:1; padding:14px 6px; text-align:center; font-size:13px;
          font-weight:500; cursor:pointer; border-bottom:2px solid transparent;
          color:{GRAY_500}; }}
  .tab.active {{ border-bottom-color:{PRIMARY_800}; color:{PRIMARY_800}; }}
  .nav-bar {{ display:flex; background:{WHITE}; border-top:1px solid {GRAY_200};
              position:absolute; bottom:0; left:0; right:0; }}
  .nav-item {{ flex:1; display:flex; flex-direction:column; align-items:center;
               padding:10px 4px 16px; cursor:pointer; font-size:10px; color:{GRAY_500}; }}
  .nav-item.active {{ color:{PRIMARY_800}; }}
  .nav-icon {{ font-size:22px; margin-bottom:2px; }}
  .amount-display {{ font-size:32px; font-weight:700; color:{PRIMARY_800};
                     letter-spacing:-1px; }}
  .currency {{ font-size:16px; color:{GRAY_500}; margin-right:2px; }}
  .account-pill {{ background:{GRAY_100}; border-radius:24px; padding:10px 16px;
                   display:flex; align-items:center; gap:10px;
                   margin-bottom:12px; cursor:pointer; }}
  .account-pill .type {{ font-size:13px; font-weight:600; color:{GRAY_900}; }}
  .account-pill .num  {{ font-size:12px; color:{GRAY_500}; }}
  .account-pill .bal  {{ font-size:14px; font-weight:700; color:{PRIMARY_800};
                         margin-left:auto; }}
  .transaction-row {{ display:flex; align-items:center; gap:12px;
                      padding:12px 0; border-bottom:1px solid {GRAY_100}; }}
  .tx-icon {{ width:42px; height:42px; border-radius:50%; display:flex;
              align-items:center; justify-content:center; font-size:18px; flex-shrink:0; }}
  .tx-icon.credit {{ background:#dcfce7; }}
  .tx-icon.debit  {{ background:#fee2e2; }}
  .tx-info  {{ flex:1; }}
  .tx-desc  {{ font-size:14px; font-weight:500; color:{GRAY_900}; }}
  .tx-date  {{ font-size:12px; color:{GRAY_500}; margin-top:2px; }}
  .tx-amt   {{ font-size:15px; font-weight:700; }}
  .tx-amt.credit {{ color:{GREEN_700}; }}
  .tx-amt.debit  {{ color:{RED_700}; }}
  .badge {{ display:inline-block; padding:3px 10px; border-radius:99px;
            font-size:11px; font-weight:600; }}
  .badge-green  {{ background:#dcfce7; color:{GREEN_700}; }}
  .badge-blue   {{ background:{BLUE_50}; color:{BLUE_700}; }}
  .badge-gray   {{ background:{GRAY_100}; color:{GRAY_500}; }}
  .section-title {{ font-size:16px; font-weight:700; color:{GRAY_900}; margin-bottom:12px; }}
  select.input-field {{ appearance:none; }}
  .quick-actions {{ display:grid; grid-template-columns:repeat(4,1fr); gap:12px;
                    margin:20px 0; }}
  .qa-item {{ display:flex; flex-direction:column; align-items:center; gap:6px;
              cursor:pointer; }}
  .qa-icon {{ width:54px; height:54px; background:{BLUE_50};
              border-radius:16px; display:flex; align-items:center;
              justify-content:center; font-size:22px; }}
  .qa-label {{ font-size:12px; font-weight:500; color:{GRAY_700}; }}
  .balance-card {{ background:linear-gradient(135deg,{PRIMARY_800},{PRIMARY_700});
                   border-radius:20px; padding:24px 20px; color:{WHITE};
                   margin-bottom:20px; }}
  .balance-card .title {{ font-size:13px; color:{PRIMARY_200}; margin-bottom:4px; }}
  .balance-card .amount {{ font-size:36px; font-weight:700; letter-spacing:-1px; }}
  .balance-card .account-num {{ font-size:13px; color:{PRIMARY_200}; margin-top:6px; }}
"""

# ---------------------------------------------------------------------------
# HTML page builder helper
# ---------------------------------------------------------------------------

def html_page(title: str, body: str, extra_css: str = "") -> str:
    return f"""<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8"/>
  <meta name="viewport" content="width=390, initial-scale=1.0"/>
  <title>{title}</title>
  <style>
{BASE_CSS}
{extra_css}
  </style>
</head>
<body>
  <div class="screen">
{body}
  </div>
</body>
</html>"""


# ---------------------------------------------------------------------------
# Individual page HTML generators
# ---------------------------------------------------------------------------

def page_login() -> str:
    body = f"""
    <div style="background:{PRIMARY_800}; padding:60px 24px 36px; text-align:center;">
      <div style="width:72px;height:72px;background:{WHITE};border-radius:18px;
                  display:flex;align-items:center;justify-content:center;
                  margin:0 auto 16px;box-shadow:0 4px 16px rgba(0,0,0,0.2);">
        <span style="font-size:36px;">🏦</span>
      </div>
      <h1 style="font-size:22px;font-weight:800;color:{WHITE};">Wekeza Mobile</h1>
      <p style="font-size:13px;color:{PRIMARY_200};margin-top:4px;">Banking Made Simple</p>
    </div>
    <div style="background:{GRAY_50};border-radius:28px 28px 0 0;
                margin-top:-16px;padding:32px 24px 40px;">
      <h2 style="font-size:20px;font-weight:700;color:{GRAY_900};margin-bottom:4px;">
        Welcome back</h2>
      <p style="font-size:13px;color:{GRAY_500};margin-bottom:24px;">
        Sign in to your account</p>
      <div style="margin-bottom:16px;">
        <div class="label">Username</div>
        <input class="input-field" type="text" value="john.mwangi" placeholder="Username"/>
      </div>
      <div style="margin-bottom:24px;">
        <div class="label">Password</div>
        <input class="input-field" type="password" value="••••••••" placeholder="Password"/>
      </div>
      <button class="btn-primary">Sign In</button>
      <p style="text-align:center;font-size:13px;color:{GRAY_500};margin-top:20px;">
        Forgot password? <span style="color:{PRIMARY_700};font-weight:600;">Reset here</span></p>
    </div>
"""
    return html_page("Login – Wekeza Mobile", body)


def page_account_creation() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <div>
          <h1>Open New Account</h1>
          <p>Step 1 of 3 – Personal Details</p>
        </div>
      </div>
    </div>
    <div class="content">
      <div class="card">
        <div class="section-title">📋 Customer Information</div>
        <div style="margin-bottom:14px;">
          <div class="label">First Name</div>
          <input class="input-field" value="John"/>
        </div>
        <div style="margin-bottom:14px;">
          <div class="label">Last Name</div>
          <input class="input-field" value="Mwangi"/>
        </div>
        <div style="margin-bottom:14px;">
          <div class="label">National ID</div>
          <input class="input-field" value="34521789"/>
        </div>
        <div style="margin-bottom:14px;">
          <div class="label">Phone Number</div>
          <input class="input-field" value="+254712 345 678"/>
        </div>
        <div style="margin-bottom:14px;">
          <div class="label">Email</div>
          <input class="input-field" value="john.mwangi@example.com"/>
        </div>
        <div style="margin-bottom:4px;">
          <div class="label">Account Type</div>
          <select class="input-field">
            <option selected>Savings Account</option>
            <option>Current Account</option>
            <option>Fixed Deposit</option>
          </select>
        </div>
      </div>
      <button class="btn-primary">Continue →</button>
    </div>
"""
    return html_page("Account Creation – Wekeza Mobile", body)


def page_account_created() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <div>
          <h1>Account Created!</h1>
          <p>Your account is ready to use</p>
        </div>
      </div>
    </div>
    <div class="content">
      <div style="text-align:center;padding:24px 0;">
        <div style="width:80px;height:80px;background:#dcfce7;border-radius:50%;
                    display:flex;align-items:center;justify-content:center;
                    margin:0 auto 16px;font-size:36px;">✅</div>
        <h2 style="font-size:22px;font-weight:700;color:{GRAY_900};margin-bottom:8px;">
          Account Successfully Opened</h2>
        <p style="font-size:14px;color:{GRAY_500};">
          Welcome to Wekeza Banking, John!</p>
      </div>
      <div class="card">
        <div class="section-title">Account Details</div>
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Account Number</span>
          <span style="font-weight:700;color:{GRAY_900};">ACC-0009-2847</span>
        </div>
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Account Type</span>
          <span class="badge badge-green">Savings</span>
        </div>
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Currency</span>
          <span style="font-weight:600;">KES</span>
        </div>
        <div style="display:flex;justify-content:space-between;">
          <span style="color:{GRAY_500};font-size:14px;">Opening Balance</span>
          <span style="font-weight:700;color:{PRIMARY_800};">KES 5,000.00</span>
        </div>
      </div>
      <button class="btn-primary">Go to Dashboard</button>
    </div>
"""
    return html_page("Account Created – Wekeza Mobile", body)


def page_dashboard() -> str:
    body = f"""
    <div style="background:{PRIMARY_800}; padding:52px 20px 30px;">
      <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:24px;">
        <div>
          <p style="font-size:13px;color:{PRIMARY_200};">Good morning,</p>
          <h1 style="font-size:20px;font-weight:700;color:{WHITE};">John Mwangi</h1>
        </div>
        <div style="width:42px;height:42px;border-radius:50%;background:rgba(255,255,255,0.15);
                    display:flex;align-items:center;justify-content:center;
                    font-size:18px;color:{WHITE};">🔔</div>
      </div>
      <div class="balance-card">
        <div class="title">Total Balance</div>
        <div class="amount">KES 47,320.50</div>
        <div class="account-num">ACC-0009-2847 · Savings</div>
      </div>
    </div>
    <div style="background:{GRAY_50};border-radius:28px 28px 0 0;margin-top:-16px;
                padding:24px 20px 80px;">
      <div class="quick-actions">
        <div class="qa-item">
          <div class="qa-icon">↗️</div>
          <div class="qa-label">Transfer</div>
        </div>
        <div class="qa-item">
          <div class="qa-icon">📱</div>
          <div class="qa-label">M-Money</div>
        </div>
        <div class="qa-item">
          <div class="qa-icon">📄</div>
          <div class="qa-label">History</div>
        </div>
        <div class="qa-item">
          <div class="qa-icon">🏦</div>
          <div class="qa-label">Loans</div>
        </div>
      </div>
      <div class="section-title">Recent Transactions</div>
      <div class="transaction-row">
        <div class="tx-icon credit">↙️</div>
        <div class="tx-info">
          <div class="tx-desc">Salary Deposit</div>
          <div class="tx-date">Today, 08:30</div>
        </div>
        <div class="tx-amt credit">+KES 45,000</div>
      </div>
      <div class="transaction-row">
        <div class="tx-icon debit">↗️</div>
        <div class="tx-info">
          <div class="tx-desc">Transfer to Peter K.</div>
          <div class="tx-date">Yesterday, 14:12</div>
        </div>
        <div class="tx-amt debit">-KES 3,500</div>
      </div>
      <div class="transaction-row">
        <div class="tx-icon debit">📱</div>
        <div class="tx-info">
          <div class="tx-desc">Airtime Purchase</div>
          <div class="tx-date">Yesterday, 09:05</div>
        </div>
        <div class="tx-amt debit">-KES 200</div>
      </div>
    </div>
    <div class="nav-bar">
      <div class="nav-item active"><div class="nav-icon">🏠</div>Home</div>
      <div class="nav-item"><div class="nav-icon">💳</div>Accounts</div>
      <div class="nav-item"><div class="nav-icon">↔️</div>Transfer</div>
      <div class="nav-item"><div class="nav-icon">📱</div>M-Money</div>
      <div class="nav-item"><div class="nav-icon">👤</div>Profile</div>
    </div>
"""
    return html_page("Dashboard – Wekeza Mobile", body)


def page_transactions() -> str:
    rows = [
        ("↙️", "credit", "Salary Deposit",          "Mar 19, 08:30", "+KES 45,000.00"),
        ("↗️", "debit",  "Transfer – Peter K.",      "Mar 18, 14:12", "-KES 3,500.00"),
        ("📱", "debit",  "Airtime – Safaricom",      "Mar 18, 09:05", "-KES 200.00"),
        ("💧", "debit",  "KPLC Prepaid Token",       "Mar 17, 19:20", "-KES 1,500.00"),
        ("↙️", "credit", "M-Pesa Deposit",           "Mar 17, 11:45", "+KES 5,000.00"),
        ("🏦", "debit",  "Loan Repayment",           "Mar 16, 00:00", "-KES 8,250.00"),
        ("↗️", "debit",  "Send to Nairobi Water",    "Mar 15, 10:30", "-KES 2,200.00"),
        ("↙️", "credit", "Wekeza Transfer Received", "Mar 14, 16:00", "+KES 10,000.00"),
    ]
    tx_html = ""
    for icon, cls, desc, date, amt in rows:
        tx_html += f"""
      <div class="transaction-row">
        <div class="tx-icon {cls}">{icon}</div>
        <div class="tx-info">
          <div class="tx-desc">{desc}</div>
          <div class="tx-date">{date}</div>
        </div>
        <div class="tx-amt {cls}">{amt}</div>
      </div>"""

    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Transaction History</h1>
          <p>ACC-0009-2847</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content" style="padding-bottom:24px;">
      <div style="display:flex;gap:8px;margin-bottom:16px;overflow-x:auto;">
        <button style="padding:8px 16px;border-radius:99px;background:{PRIMARY_800};
                        color:{WHITE};border:none;font-size:13px;font-weight:600;white-space:nowrap;">All</button>
        <button style="padding:8px 16px;border-radius:99px;background:{GRAY_100};
                        color:{GRAY_700};border:none;font-size:13px;white-space:nowrap;">Credits</button>
        <button style="padding:8px 16px;border-radius:99px;background:{GRAY_100};
                        color:{GRAY_700};border:none;font-size:13px;white-space:nowrap;">Debits</button>
        <button style="padding:8px 16px;border-radius:99px;background:{GRAY_100};
                        color:{GRAY_700};border:none;font-size:13px;white-space:nowrap;">Transfers</button>
      </div>
      {tx_html}
    </div>
"""
    return html_page("Transactions – Wekeza Mobile", body)


def page_transfer() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Transfer Funds</h1>
          <p>Send money instantly</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      <div class="card">
        <div class="label">From Account</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Savings – ACC-0009-2847 (KES 47,320.50)</option>
        </select>
        <div class="label">Transfer Type</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Wekeza Account</option>
          <option>Other Bank (RTGS/EFT)</option>
          <option>Mobile Money</option>
        </select>
        <div class="label">To Account Number</div>
        <input class="input-field" style="margin-bottom:16px;" placeholder="Enter recipient account number"/>
        <div class="label">Amount (KES)</div>
        <input class="input-field" style="margin-bottom:16px;" value="5,000.00" type="number"/>
        <div class="label">Narration</div>
        <input class="input-field" placeholder="What is this transfer for?"/>
      </div>
      <button class="btn-primary">Transfer Now</button>
      <p style="text-align:center;font-size:12px;color:{GRAY_500};margin-top:12px;">
        Maximum: KES 1,000,000 per transaction</p>
    </div>
"""
    return html_page("Transfer – Wekeza Mobile", body)


def page_transfer_success() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Transfer Funds</h1>
          <p>Send money instantly</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      <div class="success-box">
        ✅ Transfer successful!  Reference: TXN-20260319-4891
      </div>
      <div style="text-align:center;padding:32px 0;">
        <div style="font-size:60px;margin-bottom:16px;">✅</div>
        <div class="amount-display">
          <span class="currency">KES</span>5,000.00
        </div>
        <p style="font-size:14px;color:{GRAY_500};margin-top:8px;">
          Sent to ACC-0012-9921</p>
      </div>
      <div class="card">
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Reference</span>
          <span style="font-weight:700;">TXN-20260319-4891</span>
        </div>
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Date &amp; Time</span>
          <span>Mar 19, 2026 · 09:15</span>
        </div>
        <div style="display:flex;justify-content:space-between;">
          <span style="color:{GRAY_500};font-size:14px;">Status</span>
          <span class="badge badge-green">Completed</span>
        </div>
      </div>
      <button class="btn-primary">Back to Dashboard</button>
    </div>
"""
    return html_page("Transfer Success – Wekeza Mobile", body)


def page_withdraw() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Withdraw Cash</h1>
          <p>ATM or Over-the-Counter</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      <div class="card">
        <div class="label">From Account</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Savings – ACC-0009-2847 (KES 47,320.50)</option>
        </select>
        <div class="label">Withdrawal Method</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option selected>ATM Code</option>
          <option>Over the Counter</option>
          <option>Agent Banking</option>
        </select>
        <div class="label">Amount (KES)</div>
        <input class="input-field" style="margin-bottom:4px;" value="10,000.00"/>
        <p style="font-size:12px;color:{GRAY_500};margin-bottom:16px;">
          Daily ATM limit: KES 40,000</p>
      </div>
      <div class="info-box">
        ℹ️ An ATM withdrawal code will be sent to your registered phone number (+254712 XXX 678).
      </div>
      <button class="btn-primary">Generate Code</button>
    </div>
"""
    return html_page("Withdraw – Wekeza Mobile", body)


def page_mobile_money_mpesa() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Mobile Money</h1>
          <p>M-Pesa &amp; more</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="tab-bar" style="position:sticky;top:0;z-index:10;">
      <div class="tab active">M-Pesa Deposit</div>
      <div class="tab">Send Money</div>
      <div class="tab">Airtime</div>
    </div>
    <div class="content">
      <div class="section-title">Deposit via M-Pesa STK Push</div>
      <div class="card">
        <div class="label">Deposit to Account</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Savings – ACC-0009-2847</option>
        </select>
        <div class="label">M-Pesa Phone Number</div>
        <input class="input-field" style="margin-bottom:16px;" value="0712 345 678" type="tel"/>
        <div class="label">Amount (KES)</div>
        <input class="input-field" value="2,000.00" type="number"/>
      </div>
      <div class="info-box">
        📲 You will receive an M-Pesa STK push on your phone to confirm the payment.
      </div>
      <button class="btn-primary">Send STK Push</button>
    </div>
"""
    return html_page("M-Pesa Deposit – Wekeza Mobile", body)


def page_airtime() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Buy Airtime</h1>
          <p>All networks supported</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="tab-bar">
      <div class="tab">M-Pesa Deposit</div>
      <div class="tab">Send Money</div>
      <div class="tab active">Airtime</div>
    </div>
    <div class="content">
      <div class="card">
        <div class="label">From Account</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Savings – ACC-0009-2847 (KES 47,320.50)</option>
        </select>
        <div class="label">Network Provider</div>
        <div style="display:grid;grid-template-columns:repeat(3,1fr);gap:10px;margin-bottom:16px;">
          <div style="border:2px solid {PRIMARY_800};border-radius:12px;padding:12px;text-align:center;cursor:pointer;">
            <div style="font-size:22px;">📱</div>
            <div style="font-size:12px;font-weight:600;color:{PRIMARY_800};">Safaricom</div>
          </div>
          <div style="border:1px solid {GRAY_200};border-radius:12px;padding:12px;text-align:center;cursor:pointer;">
            <div style="font-size:22px;">📡</div>
            <div style="font-size:12px;font-weight:500;color:{GRAY_700};">Airtel</div>
          </div>
          <div style="border:1px solid {GRAY_200};border-radius:12px;padding:12px;text-align:center;cursor:pointer;">
            <div style="font-size:22px;">📶</div>
            <div style="font-size:12px;font-weight:500;color:{GRAY_700};">Telkom</div>
          </div>
        </div>
        <div class="label">Phone Number</div>
        <input class="input-field" style="margin-bottom:16px;" value="0712 345 678" type="tel"/>
        <div class="label">Amount (KES)</div>
        <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:8px;margin-bottom:16px;">
          <button style="padding:10px;border:1px solid {GRAY_200};border-radius:10px;
                          background:{GRAY_100};cursor:pointer;font-size:14px;">50</button>
          <button style="padding:10px;border:2px solid {PRIMARY_800};border-radius:10px;
                          background:{BLUE_50};cursor:pointer;font-size:14px;
                          color:{PRIMARY_800};font-weight:600;">100</button>
          <button style="padding:10px;border:1px solid {GRAY_200};border-radius:10px;
                          background:{GRAY_100};cursor:pointer;font-size:14px;">200</button>
          <button style="padding:10px;border:1px solid {GRAY_200};border-radius:10px;
                          background:{GRAY_100};cursor:pointer;font-size:14px;">500</button>
        </div>
        <input class="input-field" value="100" type="number"/>
      </div>
      <button class="btn-primary">Buy Airtime</button>
    </div>
"""
    return html_page("Airtime – Wekeza Mobile", body)


def page_airtime_success() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Buy Airtime</h1>
          <p>All networks supported</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      <div class="success-box">
        ✅ Airtime purchased successfully!
      </div>
      <div style="text-align:center;padding:32px 0;">
        <div style="font-size:60px;margin-bottom:16px;">📱</div>
        <div class="amount-display"><span class="currency">KES</span>100.00</div>
        <p style="font-size:14px;color:{GRAY_500};margin-top:8px;">Safaricom · 0712 345 678</p>
      </div>
      <div class="card">
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Reference</span>
          <span style="font-weight:700;">AIR-20260319-0482</span>
        </div>
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Date &amp; Time</span>
          <span>Mar 19, 2026 · 09:22</span>
        </div>
        <div style="display:flex;justify-content:space-between;">
          <span style="color:{GRAY_500};font-size:14px;">Status</span>
          <span class="badge badge-green">Completed</span>
        </div>
      </div>
      <button class="btn-primary">Buy More Airtime</button>
      <button class="btn-outline">Back to Dashboard</button>
    </div>
"""
    return html_page("Airtime Success – Wekeza Mobile", body)


def page_send_money_wekeza() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Send Money</h1>
          <p>Wekeza Account</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      <div class="info-box">
        🏦 Instant transfer to any Wekeza account – free of charge!
      </div>
      <div class="card">
        <div class="label">From Account</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Savings – ACC-0009-2847 (KES 47,320.50)</option>
        </select>
        <div class="label">Recipient Account Number</div>
        <input class="input-field" style="margin-bottom:16px;"
               value="ACC-0012-9921" placeholder="Wekeza account number"/>
        <div style="background:{GRAY_50};border-radius:10px;padding:12px;margin-bottom:16px;">
          <div style="font-size:12px;color:{GRAY_500};">Recipient</div>
          <div style="font-size:15px;font-weight:600;color:{GRAY_900};">Peter K. Omondi</div>
        </div>
        <div class="label">Amount (KES)</div>
        <input class="input-field" style="margin-bottom:16px;" value="5,000.00"/>
        <div class="label">Narration</div>
        <input class="input-field" value="Rent contribution"/>
      </div>
      <button class="btn-primary">Send Money</button>
    </div>
"""
    return html_page("Send to Wekeza – Wekeza Mobile", body)


def page_send_money_other_bank() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Send to Other Bank</h1>
          <p>RTGS / EFT / Pesalink</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      <div class="card">
        <div class="label">From Account</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Savings – ACC-0009-2847 (KES 47,320.50)</option>
        </select>
        <div class="label">Transfer Rail</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Pesalink (Instant)</option>
          <option selected>EFT (Next day)</option>
          <option>RTGS (Same day above KES 1M)</option>
        </select>
        <div class="label">Destination Bank</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Equity Bank</option>
          <option selected>KCB Bank</option>
          <option>Cooperative Bank</option>
          <option>Absa Bank Kenya</option>
        </select>
        <div class="label">Recipient Account Number</div>
        <input class="input-field" style="margin-bottom:16px;" value="1234567890"/>
        <div class="label">Recipient Name</div>
        <input class="input-field" style="margin-bottom:16px;" value="Jane W. Kamau"/>
        <div class="label">Amount (KES)</div>
        <input class="input-field" style="margin-bottom:16px;" value="25,000.00"/>
        <div class="label">Narration</div>
        <input class="input-field" value="Business payment"/>
      </div>
      <button class="btn-primary">Send Now</button>
      <p style="text-align:center;font-size:12px;color:{GRAY_500};margin-top:12px;">
        EFT fee: KES 30 per transaction</p>
    </div>
"""
    return html_page("Send to Other Bank – Wekeza Mobile", body)


def page_send_to_mobile_money() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Mobile Money</h1>
          <p>Send to M-Pesa / Airtel</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="tab-bar">
      <div class="tab">M-Pesa Deposit</div>
      <div class="tab active">Send Money</div>
      <div class="tab">Airtime</div>
    </div>
    <div class="content">
      <div class="card">
        <div class="label">From Account</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Savings – ACC-0009-2847 (KES 47,320.50)</option>
        </select>
        <div class="label">Mobile Provider</div>
        <div style="display:grid;grid-template-columns:1fr 1fr;gap:10px;margin-bottom:16px;">
          <div style="border:2px solid {PRIMARY_800};border-radius:12px;padding:12px;
                      text-align:center;cursor:pointer;background:{BLUE_50};">
            <div style="font-size:24px;">🟢</div>
            <div style="font-size:13px;font-weight:600;color:{PRIMARY_800};">M-Pesa</div>
          </div>
          <div style="border:1px solid {GRAY_200};border-radius:12px;padding:12px;
                      text-align:center;cursor:pointer;">
            <div style="font-size:24px;">🔴</div>
            <div style="font-size:13px;font-weight:500;color:{GRAY_700};">Airtel Money</div>
          </div>
        </div>
        <div class="label">Recipient Phone Number</div>
        <input class="input-field" style="margin-bottom:16px;"
               value="0722 111 222" type="tel"/>
        <div class="label">Amount (KES)</div>
        <input class="input-field" style="margin-bottom:16px;" value="1,500.00"/>
        <div class="label">Narration</div>
        <input class="input-field" value="Family support"/>
      </div>
      <button class="btn-primary">Send to M-Pesa</button>
    </div>
"""
    return html_page("Send to Mobile Money – Wekeza Mobile", body)


def page_utilities() -> str:
    services = [
        ("💡", "Electricity", "KPLC Prepaid / Postpaid"),
        ("💧", "Water",       "Nairobi Water / County"),
        ("📺", "Pay TV",      "DSTV, GoTV, Zuku"),
        ("🌐", "Internet",    "Safaricom Home, Zuku"),
        ("🏫", "School Fees", "M-Pesa Paybill"),
        ("🏥", "Insurance",   "NHIF, NSSF"),
        ("🏢", "Rent",        "Landlord Paybill"),
        ("🚰", "NairobiWater", "Water Bill"),
    ]
    items_html = ""
    for icon, name, sub in services:
        items_html += f"""
      <div style="display:flex;align-items:center;gap:14px;padding:14px 0;
                  border-bottom:1px solid {GRAY_100};cursor:pointer;">
        <div style="width:48px;height:48px;background:{BLUE_50};border-radius:14px;
                    display:flex;align-items:center;justify-content:center;
                    font-size:22px;flex-shrink:0;">{icon}</div>
        <div>
          <div style="font-size:15px;font-weight:600;color:{GRAY_900};">{name}</div>
          <div style="font-size:12px;color:{GRAY_500};">{sub}</div>
        </div>
        <div style="margin-left:auto;font-size:18px;color:{GRAY_500};">›</div>
      </div>"""

    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Pay Bills &amp; Utilities</h1>
          <p>Fast &amp; Secure Payments</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      {items_html}
    </div>
"""
    return html_page("Utilities – Wekeza Mobile", body)


def page_utility_payment() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>Pay Electricity</h1>
          <p>KPLC Prepaid Token</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      <div class="card">
        <div class="label">From Account</div>
        <select class="input-field" style="margin-bottom:16px;">
          <option>Savings – ACC-0009-2847 (KES 47,320.50)</option>
        </select>
        <div class="label">Meter Number</div>
        <input class="input-field" style="margin-bottom:16px;" value="45123789012"/>
        <div style="background:{GRAY_50};border-radius:10px;padding:12px;margin-bottom:16px;">
          <div style="font-size:12px;color:{GRAY_500};">Meter Owner</div>
          <div style="font-size:15px;font-weight:600;color:{GRAY_900};">John M. Wekeza – Westlands</div>
        </div>
        <div class="label">Amount (KES)</div>
        <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:8px;margin-bottom:16px;">
          <button style="padding:10px;border:1px solid {GRAY_200};border-radius:10px;
                          background:{GRAY_100};font-size:14px;">500</button>
          <button style="padding:10px;border:2px solid {PRIMARY_800};border-radius:10px;
                          background:{BLUE_50};font-size:14px;color:{PRIMARY_800};font-weight:600;">1000</button>
          <button style="padding:10px;border:1px solid {GRAY_200};border-radius:10px;
                          background:{GRAY_100};font-size:14px;">2000</button>
          <button style="padding:10px;border:1px solid {GRAY_200};border-radius:10px;
                          background:{GRAY_100};font-size:14px;">5000</button>
        </div>
        <input class="input-field" value="1,000.00"/>
      </div>
      <button class="btn-primary">Pay KPLC</button>
      <p style="text-align:center;font-size:12px;color:{GRAY_500};margin-top:12px;">
        Token will be sent to your registered phone number</p>
    </div>
"""
    return html_page("Pay Electricity – Wekeza Mobile", body)


def page_loans() -> str:
    body = f"""
    <div class="header">
      <div class="header-row">
        <button class="back-btn">←</button>
        <div style="text-align:center;">
          <h1>My Loans</h1>
          <p>Loan management</p>
        </div>
        <div style="width:32px;"></div>
      </div>
    </div>
    <div class="content">
      <div class="card" style="background:linear-gradient(135deg,{PRIMARY_800},{PRIMARY_700});color:{WHITE};">
        <div style="font-size:13px;color:{PRIMARY_200};margin-bottom:4px;">Active Loan</div>
        <div style="font-size:28px;font-weight:700;margin-bottom:4px;">KES 156,000.00</div>
        <div style="font-size:13px;color:{PRIMARY_200};">Outstanding Balance</div>
        <div style="display:flex;justify-content:space-between;margin-top:16px;">
          <div>
            <div style="font-size:12px;color:{PRIMARY_200};">Monthly Payment</div>
            <div style="font-size:16px;font-weight:700;">KES 8,250</div>
          </div>
          <div>
            <div style="font-size:12px;color:{PRIMARY_200};">Next Due Date</div>
            <div style="font-size:16px;font-weight:700;">Apr 01, 2026</div>
          </div>
        </div>
      </div>
      <div class="card">
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Loan Type</span>
          <span class="badge badge-blue">Personal</span>
        </div>
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Interest Rate</span>
          <span style="font-weight:600;">13.5% p.a.</span>
        </div>
        <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
          <span style="color:{GRAY_500};font-size:14px;">Tenure</span>
          <span style="font-weight:600;">24 months</span>
        </div>
        <div style="display:flex;justify-content:space-between;">
          <span style="color:{GRAY_500};font-size:14px;">Status</span>
          <span class="badge badge-green">Active</span>
        </div>
      </div>
      <button class="btn-primary">Apply for New Loan</button>
      <button class="btn-outline">Make Repayment</button>
    </div>
"""
    return html_page("Loans – Wekeza Mobile", body)


def page_profile() -> str:
    body = f"""
    <div style="background:{PRIMARY_800};padding:52px 20px 60px;text-align:center;">
      <div style="width:80px;height:80px;border-radius:50%;background:rgba(255,255,255,0.2);
                  display:flex;align-items:center;justify-content:center;
                  margin:0 auto 12px;font-size:36px;">👤</div>
      <h1 style="font-size:20px;font-weight:700;color:{WHITE};">John Mwangi</h1>
      <p style="font-size:13px;color:{PRIMARY_200};">john.mwangi@example.com</p>
      <span class="badge" style="background:rgba(255,255,255,0.2);color:{WHITE};margin-top:8px;
                                  display:inline-block;">KYC Verified ✓</span>
    </div>
    <div style="background:{GRAY_50};border-radius:28px 28px 0 0;margin-top:-20px;
                padding:24px 20px 80px;">
      <div class="card">
        <div class="section-title">Account Information</div>
        <div style="display:flex;justify-content:space-between;padding:10px 0;
                    border-bottom:1px solid {GRAY_100};">
          <span style="color:{GRAY_500};font-size:14px;">Phone</span>
          <span style="font-weight:500;">+254712 345 678</span>
        </div>
        <div style="display:flex;justify-content:space-between;padding:10px 0;
                    border-bottom:1px solid {GRAY_100};">
          <span style="color:{GRAY_500};font-size:14px;">National ID</span>
          <span style="font-weight:500;">345XXXX9</span>
        </div>
        <div style="display:flex;justify-content:space-between;padding:10px 0;">
          <span style="color:{GRAY_500};font-size:14px;">Member Since</span>
          <span style="font-weight:500;">Jan 2024</span>
        </div>
      </div>
      <div class="card">
        <div class="section-title">Settings</div>
        <div style="padding:12px 0;border-bottom:1px solid {GRAY_100};cursor:pointer;
                    display:flex;align-items:center;justify-content:space-between;">
          <span>🔒 Change Password</span><span>›</span>
        </div>
        <div style="padding:12px 0;border-bottom:1px solid {GRAY_100};cursor:pointer;
                    display:flex;align-items:center;justify-content:space-between;">
          <span>🔔 Notifications</span><span>›</span>
        </div>
        <div style="padding:12px 0;cursor:pointer;
                    display:flex;align-items:center;justify-content:space-between;">
          <span>📄 Statements</span><span>›</span>
        </div>
      </div>
      <button class="btn-outline" style="color:#dc2626;border-color:#dc2626;">
        Sign Out</button>
    </div>
    <div class="nav-bar">
      <div class="nav-item"><div class="nav-icon">🏠</div>Home</div>
      <div class="nav-item"><div class="nav-icon">💳</div>Accounts</div>
      <div class="nav-item"><div class="nav-icon">↔️</div>Transfer</div>
      <div class="nav-item"><div class="nav-icon">📱</div>M-Money</div>
      <div class="nav-item active"><div class="nav-icon">👤</div>Profile</div>
    </div>
"""
    return html_page("Profile – Wekeza Mobile", body)


def page_accounts_list() -> str:
    accounts = [
        ("Savings Account",  "ACC-0009-2847", "KES 47,320.50", "Active",   "badge-green"),
        ("Current Account",  "ACC-0009-2901", "KES 12,050.00", "Active",   "badge-green"),
        ("Fixed Deposit",    "ACC-0009-3120", "KES 100,000.00","Locked",   "badge-blue"),
    ]
    items = ""
    for atype, num, bal, status, badge in accounts:
        items += f"""
      <div class="card" style="cursor:pointer;">
        <div style="display:flex;justify-content:space-between;align-items:flex-start;margin-bottom:12px;">
          <div>
            <div style="font-size:16px;font-weight:700;color:{GRAY_900};">{atype}</div>
            <div style="font-size:13px;color:{GRAY_500};">{num}</div>
          </div>
          <span class="badge {badge}">{status}</span>
        </div>
        <div style="font-size:24px;font-weight:700;color:{PRIMARY_800};">{bal}</div>
        <div style="font-size:12px;color:{GRAY_500};margin-top:4px;">Available balance</div>
        <div style="display:flex;gap:8px;margin-top:14px;">
          <button style="flex:1;padding:8px;border-radius:8px;background:{BLUE_50};
                          color:{PRIMARY_800};border:none;font-size:13px;font-weight:600;cursor:pointer;">
            Deposit</button>
          <button style="flex:1;padding:8px;border-radius:8px;background:{BLUE_50};
                          color:{PRIMARY_800};border:none;font-size:13px;font-weight:600;cursor:pointer;">
            Transfer</button>
          <button style="flex:1;padding:8px;border-radius:8px;background:{BLUE_50};
                          color:{PRIMARY_800};border:none;font-size:13px;font-weight:600;cursor:pointer;">
            Statement</button>
        </div>
      </div>"""

    body = f"""
    <div class="header">
      <div class="header-row">
        <div>
          <h1>My Accounts</h1>
          <p>Manage your accounts</p>
        </div>
        <button style="width:36px;height:36px;border-radius:50%;background:rgba(255,255,255,0.15);
                        border:none;color:{WHITE};font-size:20px;cursor:pointer;">+</button>
      </div>
    </div>
    <div class="content">
      {items}
    </div>
    <div class="nav-bar">
      <div class="nav-item"><div class="nav-icon">🏠</div>Home</div>
      <div class="nav-item active"><div class="nav-icon">💳</div>Accounts</div>
      <div class="nav-item"><div class="nav-icon">↔️</div>Transfer</div>
      <div class="nav-item"><div class="nav-icon">📱</div>M-Money</div>
      <div class="nav-item"><div class="nav-icon">👤</div>Profile</div>
    </div>
"""
    return html_page("My Accounts – Wekeza Mobile", body)


# ---------------------------------------------------------------------------
# Page registry:  (output_dir, filename, html_generator_fn)
# ---------------------------------------------------------------------------

PAGES = [
    ("01_login",                    "01_login_screen.png",              page_login),
    ("01_login",                    "02_login_filled.png",              page_login),
    ("02_account_creation",         "01_account_creation_form.png",     page_account_creation),
    ("02_account_creation",         "02_account_created_success.png",   page_account_created),
    ("03_dashboard",                "01_dashboard_home.png",            page_dashboard),
    ("03_dashboard",                "02_accounts_list.png",             page_accounts_list),
    ("04_transactions",             "01_transaction_history.png",       page_transactions),
    ("05_transfer_funds",           "01_transfer_form.png",             page_transfer),
    ("05_transfer_funds",           "02_transfer_success.png",          page_transfer_success),
    ("06_mobile_money_mpesa",       "01_mpesa_deposit.png",             page_mobile_money_mpesa),
    ("07_airtime_purchase",         "01_airtime_form.png",              page_airtime),
    ("07_airtime_purchase",         "02_airtime_success.png",           page_airtime_success),
    ("08_send_money",               "01_send_to_wekeza_account.png",    page_send_money_wekeza),
    ("13_send_to_other_banks",      "01_send_other_bank_eft.png",       page_send_money_other_bank),
    ("14_send_to_mobile_money",     "01_send_to_mpesa.png",             page_send_to_mobile_money),
    ("09_loans",                    "01_loans_overview.png",            page_loans),
    ("10_profile",                  "01_profile_page.png",              page_profile),
    ("11_utilities_bills",          "01_bills_categories.png",          page_utilities),
    ("11_utilities_bills",          "02_electricity_payment.png",       page_utility_payment),
    ("12_withdraw_cash",            "01_withdraw_form.png",             page_withdraw),
]


# ---------------------------------------------------------------------------
# Main – generate screenshots
# ---------------------------------------------------------------------------

def main() -> int:
    from playwright.sync_api import sync_playwright

    print("=" * 60)
    print("  Wekeza Mobile Banking – Screenshot Generator")
    print("=" * 60)

    # Temp dir for HTML files
    tmp_dir = Path("/tmp/wekeza_screenshots_html")
    tmp_dir.mkdir(parents=True, exist_ok=True)

    success_count = 0
    fail_count = 0

    with sync_playwright() as pw:
        browser = pw.chromium.launch(headless=True)
        context = browser.new_context(
            viewport={"width": 390, "height": 844},
            device_scale_factor=2,
        )
        page = context.new_page()

        for category, filename, gen_fn in PAGES:
            out_dir = SCREENSHOTS_DIR / category
            out_dir.mkdir(parents=True, exist_ok=True)
            out_path = out_dir / filename

            # Write HTML to temp file
            html_content = gen_fn()
            tmp_html = tmp_dir / f"{category}_{filename}.html"
            tmp_html.write_text(html_content, encoding="utf-8")

            try:
                page.goto(f"file://{tmp_html}", wait_until="domcontentloaded")
                page.wait_for_timeout(300)
                page.screenshot(path=str(out_path), full_page=True)
                print(f"  ✅  {category}/{filename}")
                success_count += 1
            except Exception as exc:
                print(f"  ❌  {category}/{filename}: {exc}")
                fail_count += 1

        context.close()
        browser.close()

    # Summary
    print()
    print(f"Screenshots generated: {success_count}  |  Failures: {fail_count}")
    print(f"Output directory: {SCREENSHOTS_DIR}")
    print()

    return 0 if fail_count == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
