#!/usr/bin/env python3
"""
Wekeza v1-Core Banking API – Comprehensive Screenshot Generator
===============================================================
Generates 120+ screenshots for every major portal, role, and workflow
in the Wekeza v1-Core Banking API system using Playwright (headless Chromium).

Coverage:
  01_login/               – Landing page, login flows, all role logins
  02_customer_registration/ – Full registration / account-opening flow
  03_sysadmin_portal/     – System Administrator full portal
  04_branch_manager/      – Branch Manager operations
  05_teller_operations/   – Deposit, withdrawal, receipts, till
  06_customer_service/    – Customer-care portal & CIF management
  07_accounts/            – Account types, statements, management
  08_transactions/        – Transfers, payments, reversals
  09_loans/               – Loan products, application, approval, repayment
  10_customer_portal/     – Self-service customer portal
  11_digital_banking/     – M-Pesa, USSD, mobile & internet banking
  12_compliance_aml/      – AML alerts, KYC, sanctions, SAR
  13_reports/             – Financial statements, branch performance
  14_general_ledger/      – Chart of accounts, journals, trial balance
  15_payments/            – RTGS, EFT, M-Pesa gateway
  16_api_docs/            – Swagger / OpenAPI documentation

Usage:
    python3 APIs/v1-Core/tests/e2e/take_screenshots.py

Screenshots are written to:
    APIs/v1-Core/screenshots/<category>/<name>.png

This script works in OFFLINE mode – it renders self-contained HTML mock pages
that faithfully replicate the v1-Core admin/portal web UI.  Ideal for CI and
documentation without needing the actual server running.
"""

import os
import sys
from pathlib import Path

# ---------------------------------------------------------------------------
# Paths
# ---------------------------------------------------------------------------
# Script is at: APIs/v1-Core/tests/e2e/take_screenshots.py
# Three parents up brings us to: APIs/v1-Core/
V1_CORE_DIR = Path(__file__).resolve().parent.parent.parent
SCREENSHOTS_DIR = V1_CORE_DIR / "screenshots"

# ---------------------------------------------------------------------------
# Colour palette (mirrors the v1-Core admin panel CSS)
# ---------------------------------------------------------------------------
PRIMARY     = "#2c5aa0"
PRIMARY_DRK = "#1e3d72"
ACCENT      = "#28a745"
WARNING     = "#ffc107"
DANGER      = "#dc3545"
WHITE       = "#ffffff"
BG_LIGHT    = "#f8f9fa"
GRAY_200    = "#e9ecef"
GRAY_400    = "#ced4da"
GRAY_600    = "#6c757d"
GRAY_800    = "#343a40"
GRAY_900    = "#212529"
BLUE_LIGHT  = "#e8f0fe"
GREEN_LIGHT = "#d4edda"
RED_LIGHT   = "#f8d7da"
YELLOW_LIGHT = "#fff3cd"

GRAY_700    = "#495057"

# ---------------------------------------------------------------------------
# Shared CSS – Bootstrap-like, matching v1-Core admin panel aesthetics
# ---------------------------------------------------------------------------
BASE_CSS = f"""
  * {{ box-sizing: border-box; margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }}
  body {{ background: {BG_LIGHT}; min-height: 100vh; color: {GRAY_900}; font-size: 14px; }}

  /* ── Navbar ── */
  .navbar {{ background: {WHITE}; box-shadow: 0 2px 8px rgba(0,0,0,0.1);
             display: flex; align-items: center; padding: 0 24px;
             height: 56px; position: relative; z-index: 100; }}
  .navbar-brand {{ font-size: 18px; font-weight: 700; color: {PRIMARY}; text-decoration: none;
                   display: flex; align-items: center; gap: 8px; }}
  .navbar-actions {{ margin-left: auto; display: flex; align-items: center; gap: 12px; }}
  .nav-user {{ display: flex; align-items: center; gap: 8px; font-size: 13px; color: {GRAY_600}; }}
  .avatar {{ width: 32px; height: 32px; border-radius: 50%; background: {PRIMARY};
             color: {WHITE}; display: flex; align-items: center; justify-content: center;
             font-size: 13px; font-weight: 600; }}
  .badge-notif {{ background: {DANGER}; color: {WHITE}; border-radius: 99px;
                  padding: 2px 7px; font-size: 11px; font-weight: 600; }}

  /* ── Sidebar ── */
  .layout {{ display: flex; min-height: calc(100vh - 56px); }}
  .sidebar {{ width: 240px; background: linear-gradient(160deg, {PRIMARY}, {PRIMARY_DRK});
              color: {WHITE}; padding: 20px 0; flex-shrink: 0; }}
  .sidebar-user {{ padding: 0 20px 20px; border-bottom: 1px solid rgba(255,255,255,0.15); margin-bottom: 12px; }}
  .sidebar-user .name {{ font-weight: 600; font-size: 14px; }}
  .sidebar-user .role {{ font-size: 11px; color: rgba(255,255,255,0.7); margin-top: 2px; }}
  .sidebar .nav-link {{ display: flex; align-items: center; gap: 10px;
                        padding: 10px 20px; color: rgba(255,255,255,0.8);
                        text-decoration: none; border-radius: 8px; margin: 2px 10px;
                        font-size: 13px; transition: background 0.2s; cursor: pointer; }}
  .sidebar .nav-link:hover, .sidebar .nav-link.active {{
    background: rgba(255,255,255,0.15); color: {WHITE}; }}
  .sidebar .nav-section {{ padding: 16px 20px 6px; font-size: 10px; letter-spacing: 1px;
                           text-transform: uppercase; color: rgba(255,255,255,0.5); }}

  /* ── Main content ── */
  .main {{ flex: 1; padding: 24px; overflow: auto; background: {BG_LIGHT}; }}
  .page-title {{ font-size: 20px; font-weight: 700; color: {GRAY_800}; margin-bottom: 4px; }}
  .page-subtitle {{ font-size: 13px; color: {GRAY_600}; margin-bottom: 24px; }}

  /* ── Cards ── */
  .card {{ background: {WHITE}; border-radius: 12px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);
           overflow: hidden; margin-bottom: 20px; }}
  .card-header {{ padding: 14px 20px; border-bottom: 1px solid {GRAY_200};
                  display: flex; align-items: center; justify-content: space-between; }}
  .card-header h5 {{ font-size: 15px; font-weight: 600; margin: 0; color: {GRAY_800}; }}
  .card-body {{ padding: 20px; }}

  /* ── Stat cards ── */
  .stat-grid {{ display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 24px; }}
  .stat-card {{ background: {WHITE}; border-radius: 12px; padding: 20px;
                box-shadow: 0 2px 8px rgba(0,0,0,0.08);
                display: flex; flex-direction: column; gap: 8px; }}
  .stat-icon {{ width: 44px; height: 44px; border-radius: 10px;
                display: flex; align-items: center; justify-content: center; font-size: 20px; }}
  .stat-value {{ font-size: 22px; font-weight: 700; color: {GRAY_900}; }}
  .stat-label {{ font-size: 12px; color: {GRAY_600}; }}
  .stat-trend {{ font-size: 12px; font-weight: 600; }}
  .trend-up {{ color: {ACCENT}; }}
  .trend-dn {{ color: {DANGER}; }}

  /* ── Tables ── */
  table {{ width: 100%; border-collapse: collapse; font-size: 13px; }}
  thead th {{ background: {BG_LIGHT}; padding: 10px 14px; font-weight: 600;
              color: {GRAY_600}; border-bottom: 2px solid {GRAY_200};
              white-space: nowrap; }}
  tbody td {{ padding: 10px 14px; border-bottom: 1px solid {GRAY_200}; color: {GRAY_800}; }}
  tbody tr:hover {{ background: #fafbff; }}

  /* ── Badges ── */
  .badge {{ display: inline-block; padding: 3px 10px; border-radius: 99px;
            font-size: 11px; font-weight: 600; }}
  .badge-success {{ background: {GREEN_LIGHT}; color: #155724; }}
  .badge-danger  {{ background: {RED_LIGHT};   color: #721c24; }}
  .badge-warning {{ background: {YELLOW_LIGHT}; color: #856404; }}
  .badge-info    {{ background: {BLUE_LIGHT};   color: {PRIMARY}; }}
  .badge-secondary {{ background: {GRAY_200}; color: {GRAY_600}; }}

  /* ── Buttons ── */
  .btn {{ padding: 8px 16px; border-radius: 8px; border: none; cursor: pointer;
          font-size: 13px; font-weight: 600; }}
  .btn-primary {{ background: {PRIMARY}; color: {WHITE}; }}
  .btn-success {{ background: {ACCENT}; color: {WHITE}; }}
  .btn-outline-primary {{ background: transparent; border: 1.5px solid {PRIMARY}; color: {PRIMARY}; }}
  .btn-sm {{ padding: 5px 10px; font-size: 12px; }}
  .btn-danger {{ background: {DANGER}; color: {WHITE}; }}

  /* ── Form controls ── */
  .form-label {{ font-size: 13px; font-weight: 500; color: {GRAY_700}; margin-bottom: 5px; display: block; }}
  .form-control {{ width: 100%; padding: 10px 12px; border: 1.5px solid {GRAY_400};
                   border-radius: 8px; font-size: 14px; color: {GRAY_900};
                   background: {WHITE}; outline: none; }}
  .form-control:focus {{ border-color: {PRIMARY}; }}
  .form-group {{ margin-bottom: 16px; }}
  .form-row {{ display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }}
  .form-row-3 {{ display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 16px; }}

  /* ── Misc ── */
  .text-muted {{ color: {GRAY_600}; }}
  .text-primary {{ color: {PRIMARY}; }}
  .text-success {{ color: {ACCENT}; }}
  .text-danger  {{ color: {DANGER}; }}
  .fw-bold {{ font-weight: 700; }}
  .mb-0 {{ margin-bottom: 0; }}
  .mt-2 {{ margin-top: 8px; }}
  .d-flex {{ display: flex; }}
  .align-items-center {{ align-items: center; }}
  .gap-2 {{ gap: 8px; }}
  .icon {{ font-size: 16px; }}
"""

# ---------------------------------------------------------------------------
# HTML page builder helpers
# ---------------------------------------------------------------------------

def desktop_page(title: str, body: str, extra_css: str = "") -> str:
    return f"""<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8"/>
  <meta name="viewport" content="width=1440, initial-scale=1.0"/>
  <title>{title} – Wekeza v1-Core</title>
  <style>
{BASE_CSS}
{extra_css}
  </style>
</head>
<body>
{body}
</body>
</html>"""


def navbar_html(user: str = "admin", role: str = "System Administrator",
                notif: int = 3) -> str:
    initials = "".join(w[0].upper() for w in user.split(".")[:2]) or "AD"
    return f"""
  <nav class="navbar">
    <a class="navbar-brand" href="#">
      🏦 Wekeza Core Banking
    </a>
    <div class="navbar-actions">
      <span class="badge-notif">{notif}</span>
      <div class="nav-user">
        <div class="avatar">{initials}</div>
        <div>
          <div style="font-weight:600;font-size:13px;">{user}</div>
          <div style="font-size:11px;color:{GRAY_600};">{role}</div>
        </div>
      </div>
    </div>
  </nav>"""


def sidebar_html(active: str = "") -> str:
    links = [
        ("📊", "Dashboard",          "dashboard"),
        ("👤", "CIF / Customers",    "cif"),
        ("🏦", "Accounts",           "accounts"),
        ("💸", "Transactions",       "transactions"),
        ("💰", "Deposits",           "deposits"),
        ("📋", "Loans",              "loans"),
        ("💳", "Cards",              "cards"),
        ("📱", "Digital Channels",   "digital"),
        ("💹", "Payments",           "payments"),
        ("📈", "Trade Finance",      "trade"),
        ("🏛️",  "Treasury",          "treasury"),
        ("📝", "General Ledger",     "gl"),
        ("📉", "Compliance",         "compliance"),
        ("🔄", "Workflows",          "workflows"),
        ("👥", "Staff Management",   "staff"),
        ("📊", "Reporting",          "reporting"),
        ("⚙️", "Admin Panel",        "admin"),
    ]
    items = ""
    for icon, label, key in links:
        cls = "active" if key == active else ""
        items += f'<a class="nav-link {cls}" href="#">{icon} {label}</a>\n'
    return f"""
  <div class="sidebar">
    <div class="sidebar-user">
      <div class="name">admin</div>
      <div class="role">System Administrator</div>
    </div>
    {items}
  </div>"""


def layout(active: str, page_content: str,
           user: str = "admin", role: str = "System Administrator") -> str:
    return f"""
  {navbar_html(user, role)}
  <div class="layout">
    {sidebar_html(active)}
    <div class="main">
      {page_content}
    </div>
  </div>"""


# ---------------------------------------------------------------------------
# Individual page HTML generators
# ---------------------------------------------------------------------------

# ── 01 – Login ──────────────────────────────────────────────────────────────

def page_login() -> str:
    body = f"""
  <div style="min-height:100vh;display:flex;align-items:center;
              justify-content:center;background:{BG_LIGHT};">
    <div style="width:420px;">
      <div style="text-align:center;margin-bottom:24px;">
        <div style="width:64px;height:64px;background:{PRIMARY};border-radius:14px;
                    display:flex;align-items:center;justify-content:center;
                    margin:0 auto 12px;font-size:30px;">🏦</div>
        <h1 style="font-size:22px;font-weight:700;color:{GRAY_800};">Wekeza Core Banking</h1>
        <p style="font-size:13px;color:{GRAY_600};margin-top:4px;">v1-Core Banking Platform</p>
      </div>
      <div class="card">
        <div class="card-header" style="background:{PRIMARY};">
          <h5 style="color:{WHITE};margin:0;">🔐 Administrator Login</h5>
        </div>
        <div class="card-body">
          <div class="form-group">
            <label class="form-label">Username</label>
            <input class="form-control" type="text" value="admin"/>
            <div style="font-size:11px;color:{GRAY_600};margin-top:4px;">
              Roles: admin · teller · loanofficer · riskofficer
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">Password</label>
            <input class="form-control" type="password" value="••••••••••"/>
          </div>
          <button class="btn btn-primary" style="width:100%;padding:11px;">
            🔑 Login
          </button>
        </div>
      </div>
      <p style="text-align:center;font-size:12px;color:{GRAY_600};margin-top:16px;">
        Wekeza Bank © 2026 – All rights reserved
      </p>
    </div>
  </div>"""
    return desktop_page("Login", body)


def page_login_error() -> str:
    body = f"""
  <div style="min-height:100vh;display:flex;align-items:center;
              justify-content:center;background:{BG_LIGHT};">
    <div style="width:420px;">
      <div style="text-align:center;margin-bottom:24px;">
        <div style="width:64px;height:64px;background:{PRIMARY};border-radius:14px;
                    display:flex;align-items:center;justify-content:center;
                    margin:0 auto 12px;font-size:30px;">🏦</div>
        <h1 style="font-size:22px;font-weight:700;color:{GRAY_800};">Wekeza Core Banking</h1>
        <p style="font-size:13px;color:{GRAY_600};margin-top:4px;">v1-Core Banking Platform</p>
      </div>
      <div class="card">
        <div class="card-header" style="background:{PRIMARY};">
          <h5 style="color:{WHITE};margin:0;">🔐 Administrator Login</h5>
        </div>
        <div class="card-body">
          <div style="background:{RED_LIGHT};border:1px solid #f5c6cb;border-radius:8px;
                      padding:10px 14px;margin-bottom:16px;font-size:13px;color:#721c24;">
            ⚠️ Invalid username or password. Please try again.
          </div>
          <div class="form-group">
            <label class="form-label">Username</label>
            <input class="form-control" type="text" value="unknown_user"
                   style="border-color:{DANGER};"/>
          </div>
          <div class="form-group">
            <label class="form-label">Password</label>
            <input class="form-control" type="password" value="••••••••"/>
          </div>
          <button class="btn btn-primary" style="width:100%;padding:11px;">
            🔑 Login
          </button>
        </div>
      </div>
    </div>
  </div>"""
    return desktop_page("Login – Error", body)


# ── 02 – Dashboard ──────────────────────────────────────────────────────────

def page_dashboard() -> str:
    content = f"""
      <div class="page-title">📊 System Dashboard</div>
      <div class="page-subtitle">Welcome back, admin — Wednesday, 19 March 2026</div>

      <div class="stat-grid">
        <div class="stat-card">
          <div class="stat-icon" style="background:{BLUE_LIGHT};">👤</div>
          <div class="stat-value">12,847</div>
          <div class="stat-label">Total Customers</div>
          <div class="stat-trend trend-up">▲ 2.4% this month</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{GREEN_LIGHT};">🏦</div>
          <div class="stat-value">34,521</div>
          <div class="stat-label">Active Accounts</div>
          <div class="stat-trend trend-up">▲ 1.8% this month</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{YELLOW_LIGHT};">💸</div>
          <div class="stat-value">KES 4.2B</div>
          <div class="stat-label">Total Deposits</div>
          <div class="stat-trend trend-up">▲ 5.1% this month</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{RED_LIGHT};">📋</div>
          <div class="stat-value">8,134</div>
          <div class="stat-label">Active Loans</div>
          <div class="stat-trend trend-dn">▼ 0.3% this month</div>
        </div>
      </div>

      <div style="display:grid;grid-template-columns:2fr 1fr;gap:20px;">
        <div class="card">
          <div class="card-header">
            <h5>💸 Recent Transactions</h5>
            <button class="btn btn-outline-primary btn-sm">View All</button>
          </div>
          <div class="card-body" style="padding:0;">
            <table>
              <thead>
                <tr>
                  <th>Ref</th><th>Customer</th><th>Type</th><th>Amount</th>
                  <th>Status</th><th>Date</th>
                </tr>
              </thead>
              <tbody>
                <tr><td>TXN-2026-001</td><td>John Mwangi</td>
                    <td>Deposit</td><td style="color:{ACCENT};">+ KES 50,000</td>
                    <td><span class="badge badge-success">Completed</span></td>
                    <td>19 Mar 2026 09:14</td></tr>
                <tr><td>TXN-2026-002</td><td>Wanjiku Kamau</td>
                    <td>Transfer</td><td style="color:{DANGER};">- KES 12,500</td>
                    <td><span class="badge badge-success">Completed</span></td>
                    <td>19 Mar 2026 09:08</td></tr>
                <tr><td>TXN-2026-003</td><td>David Otieno</td>
                    <td>Withdrawal</td><td style="color:{DANGER};">- KES 8,000</td>
                    <td><span class="badge badge-warning">Pending</span></td>
                    <td>19 Mar 2026 08:55</td></tr>
                <tr><td>TXN-2026-004</td><td>Amina Hassan</td>
                    <td>M-Pesa</td><td style="color:{ACCENT};">+ KES 25,000</td>
                    <td><span class="badge badge-success">Completed</span></td>
                    <td>19 Mar 2026 08:42</td></tr>
                <tr><td>TXN-2026-005</td><td>Peter Kariuki</td>
                    <td>Loan Repay</td><td style="color:{ACCENT};">+ KES 15,000</td>
                    <td><span class="badge badge-success">Completed</span></td>
                    <td>19 Mar 2026 08:30</td></tr>
              </tbody>
            </table>
          </div>
        </div>

        <div>
          <div class="card">
            <div class="card-header"><h5>🔔 Alerts</h5></div>
            <div class="card-body" style="padding:12px;">
              <div style="padding:10px;background:{RED_LIGHT};border-radius:8px;
                          margin-bottom:8px;font-size:13px;color:#721c24;">
                ⚠️ 3 loan applications pending review
              </div>
              <div style="padding:10px;background:{YELLOW_LIGHT};border-radius:8px;
                          margin-bottom:8px;font-size:13px;color:#856404;">
                ⏰ KYC expiry: 12 customers
              </div>
              <div style="padding:10px;background:{BLUE_LIGHT};border-radius:8px;
                          font-size:13px;color:{PRIMARY};">
                ℹ️ System backup scheduled: 02:00 AM
              </div>
            </div>
          </div>

          <div class="card">
            <div class="card-header"><h5>👥 Online Staff</h5></div>
            <div class="card-body" style="padding:12px;">
              {"".join(f'<div style="display:flex;align-items:center;gap:8px;margin-bottom:8px;">'
                       f'<div class="avatar" style="width:28px;height:28px;font-size:11px;">{i[0]}</div>'
                       f'<div><div style="font-size:13px;font-weight:600;">{n}</div>'
                       f'<div style="font-size:11px;color:{GRAY_600};">{r}</div></div>'
                       f'<div style="margin-left:auto;width:8px;height:8px;border-radius:50%;'
                       f'background:{ACCENT};"></div></div>'
                       for i, n, r in [
                           ("A", "admin", "Sys Admin"),
                           ("T", "teller01", "Teller"),
                           ("L", "loan.officer", "Loans"),
                           ("R", "risk.officer", "Risk"),
                       ])}
            </div>
          </div>
        </div>
      </div>"""
    return desktop_page("Dashboard", layout("dashboard", content))


# ── 03 – Accounts ────────────────────────────────────────────────────────────

def page_accounts_list() -> str:
    rows = [
        ("ACC-001234", "John Mwangi",     "Savings",  "KES 128,450.00", "Active",   "19 Jan 2024"),
        ("ACC-001235", "Wanjiku Kamau",   "Current",  "KES 64,200.00",  "Active",   "12 Feb 2024"),
        ("ACC-001236", "David Otieno",    "Fixed Dep","KES 500,000.00", "Active",   "05 Mar 2024"),
        ("ACC-001237", "Amina Hassan",    "Savings",  "KES 22,000.00",  "Dormant",  "18 Jun 2023"),
        ("ACC-001238", "Peter Kariuki",   "Current",  "KES 312,700.00", "Active",   "22 Aug 2024"),
        ("ACC-001239", "Grace Njeri",     "Savings",  "KES 45,100.00",  "Active",   "10 Oct 2024"),
        ("ACC-001240", "Samuel Owino",    "Current",  "KES 9,800.00",   "Suspended","01 Dec 2023"),
    ]
    tbody = ""
    for acc, name, atype, bal, status, opened in rows:
        s_cls = "badge-success" if status == "Active" else ("badge-warning" if status == "Dormant" else "badge-danger")
        tbody += f"""
        <tr>
          <td><span style="font-family:monospace;font-size:12px;">{acc}</span></td>
          <td>{name}</td><td>{atype}</td>
          <td style="font-weight:600;">{bal}</td>
          <td><span class="badge {s_cls}">{status}</span></td>
          <td style="color:{GRAY_600};font-size:12px;">{opened}</td>
          <td>
            <button class="btn btn-outline-primary btn-sm">View</button>
            <button class="btn btn-sm" style="background:{GRAY_200};color:{GRAY_700};">Edit</button>
          </td>
        </tr>"""
    content = f"""
      <div class="page-title">🏦 Accounts Management</div>
      <div class="page-subtitle">All customer accounts across the bank</div>
      <div class="card">
        <div class="card-header">
          <h5>Account Registry</h5>
          <div style="display:flex;gap:8px;">
            <input class="form-control" style="width:220px;padding:7px 10px;"
                   type="text" placeholder="🔍  Search accounts…"/>
            <button class="btn btn-primary">+ Open Account</button>
          </div>
        </div>
        <div class="card-body" style="padding:0;">
          <table>
            <thead>
              <tr><th>Account No.</th><th>Customer</th><th>Type</th><th>Balance</th>
                  <th>Status</th><th>Opened</th><th>Actions</th></tr>
            </thead>
            <tbody>{tbody}</tbody>
          </table>
        </div>
        <div style="padding:12px 20px;font-size:12px;color:{GRAY_600};
                    border-top:1px solid {GRAY_200};display:flex;
                    justify-content:space-between;align-items:center;">
          <span>Showing 1–7 of 34,521 accounts</span>
          <div style="display:flex;gap:6px;">
            <button class="btn btn-sm" style="background:{GRAY_200};color:{GRAY_700};">‹ Prev</button>
            <button class="btn btn-primary btn-sm">Next ›</button>
          </div>
        </div>
      </div>"""
    return desktop_page("Accounts", layout("accounts", content))


def page_account_detail() -> str:
    content = f"""
      <div class="page-title">🏦 Account Detail</div>
      <div class="page-subtitle">Account No. ACC-001234 – John Mwangi</div>

      <div style="display:grid;grid-template-columns:1fr 2fr;gap:20px;">
        <div>
          <div class="card">
            <div class="card-header"><h5>👤 Customer Profile</h5></div>
            <div class="card-body">
              <div style="text-align:center;margin-bottom:16px;">
                <div class="avatar" style="width:56px;height:56px;font-size:22px;
                             margin:0 auto 10px;">JM</div>
                <div style="font-weight:700;font-size:15px;">John Mwangi</div>
                <div style="font-size:12px;color:{GRAY_600};">CIF-000456</div>
              </div>
              {"".join(f'<div style="display:flex;justify-content:space-between;padding:8px 0;border-bottom:1px solid {GRAY_200};font-size:13px;"><span style="color:{GRAY_600};">{k}</span><span style="font-weight:600;">{v}</span></div>'
                       for k, v in [
                           ("Phone", "+254 712 345 678"),
                           ("Email", "john.m@email.com"),
                           ("ID No.", "34521789"),
                           ("KYC", "✅ Verified"),
                           ("Branch", "Nairobi CBD"),
                       ])}
            </div>
          </div>
          <div class="card">
            <div class="card-header"><h5>⚡ Quick Actions</h5></div>
            <div class="card-body" style="display:flex;flex-direction:column;gap:8px;">
              <button class="btn btn-primary" style="width:100%;">💸 Deposit Funds</button>
              <button class="btn btn-outline-primary" style="width:100%;">💳 Withdraw</button>
              <button class="btn btn-outline-primary" style="width:100%;">🔄 Transfer</button>
              <button class="btn btn-sm btn-danger" style="width:100%;">🔒 Suspend Account</button>
            </div>
          </div>
        </div>

        <div>
          <div class="card">
            <div class="card-header"><h5>💰 Account Summary</h5></div>
            <div class="card-body">
              <div style="display:grid;grid-template-columns:repeat(3,1fr);gap:16px;margin-bottom:20px;">
                {"".join(f'<div style="background:{bg};border-radius:10px;padding:16px;">'
                         f'<div style="font-size:11px;color:{GRAY_600};">{lbl}</div>'
                         f'<div style="font-size:18px;font-weight:700;color:{col};margin-top:4px;">{val}</div></div>'
                         for bg, lbl, val, col in [
                             (BLUE_LIGHT, "Available Balance", "KES 128,450", PRIMARY),
                             (GREEN_LIGHT, "Total Credited", "KES 540,000", ACCENT),
                             (RED_LIGHT, "Total Debited", "KES 411,550", DANGER),
                         ])}
              </div>
            </div>
          </div>
          <div class="card">
            <div class="card-header">
              <h5>📋 Transaction History</h5>
              <button class="btn btn-outline-primary btn-sm">Export CSV</button>
            </div>
            <div class="card-body" style="padding:0;">
              <table>
                <thead><tr><th>Date</th><th>Description</th><th>Credit</th><th>Debit</th><th>Balance</th></tr></thead>
                <tbody>
                  <tr><td>19 Mar 2026</td><td>Salary Credit</td><td style="color:{ACCENT};">KES 50,000</td><td>—</td><td>KES 128,450</td></tr>
                  <tr><td>15 Mar 2026</td><td>Utility Bill – KPLC</td><td>—</td><td style="color:{DANGER};">KES 3,200</td><td>KES 78,450</td></tr>
                  <tr><td>12 Mar 2026</td><td>M-Pesa Deposit</td><td style="color:{ACCENT};">KES 10,000</td><td>—</td><td>KES 81,650</td></tr>
                  <tr><td>10 Mar 2026</td><td>Cash Withdrawal</td><td>—</td><td style="color:{DANGER};">KES 8,000</td><td>KES 71,650</td></tr>
                  <tr><td>05 Mar 2026</td><td>Transfer to ACC-001240</td><td>—</td><td style="color:{DANGER};">KES 5,000</td><td>KES 79,650</td></tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>"""
    return desktop_page("Account Detail", layout("accounts", content))


# ── 04 – Transactions ────────────────────────────────────────────────────────

def page_transactions() -> str:
    entries = [
        ("TXN-2026-001", "John Mwangi",     "Deposit",    "+50,000", "success", "19 Mar 09:14", "Teller-01"),
        ("TXN-2026-002", "Wanjiku Kamau",   "Transfer",   "-12,500", "success", "19 Mar 09:08", "Online"),
        ("TXN-2026-003", "David Otieno",    "Withdrawal", "-8,000",  "warning", "19 Mar 08:55", "ATM"),
        ("TXN-2026-004", "Amina Hassan",    "M-Pesa",     "+25,000", "success", "19 Mar 08:42", "Digital"),
        ("TXN-2026-005", "Peter Kariuki",   "Loan Repay", "+15,000", "success", "19 Mar 08:30", "Online"),
        ("TXN-2026-006", "Grace Njeri",     "Bill Pay",   "-4,500",  "success", "19 Mar 07:58", "Digital"),
        ("TXN-2026-007", "Samuel Owino",    "Deposit",    "+30,000", "success", "19 Mar 07:42", "Teller-02"),
        ("TXN-2026-008", "James Mwenda",    "Transfer",   "-7,000",  "danger",  "19 Mar 07:30", "Online"),
    ]
    tbody = ""
    for ref, cust, ttype, amt, status, ts, channel in entries:
        color = ACCENT if amt.startswith("+") else DANGER
        s_map = {"success": ("Completed", "badge-success"),
                 "warning": ("Pending",   "badge-warning"),
                 "danger":  ("Failed",    "badge-danger")}
        s_lbl, s_cls = s_map[status]
        tbody += f"""
        <tr>
          <td><span style="font-family:monospace;font-size:12px;">{ref}</span></td>
          <td>{cust}</td><td>{ttype}</td>
          <td style="font-weight:700;color:{color};">KES {amt}</td>
          <td><span class="badge {s_cls}">{s_lbl}</span></td>
          <td style="font-size:12px;color:{GRAY_600};">{ts}</td>
          <td>{channel}</td>
          <td><button class="btn btn-outline-primary btn-sm">Details</button></td>
        </tr>"""
    content = f"""
      <div class="page-title">💸 Transactions</div>
      <div class="page-subtitle">All bank transactions – real-time ledger view</div>
      <div class="card">
        <div class="card-header">
          <h5>Transaction Ledger</h5>
          <div style="display:flex;gap:8px;">
            <input class="form-control" style="width:200px;padding:7px 10px;"
                   type="date" value="2026-03-19"/>
            <input class="form-control" style="width:220px;padding:7px 10px;"
                   placeholder="🔍  Search…" type="text"/>
            <select class="form-control" style="width:150px;padding:7px 10px;">
              <option>All Types</option>
              <option>Deposit</option>
              <option>Withdrawal</option>
              <option>Transfer</option>
            </select>
            <button class="btn btn-outline-primary">Export</button>
          </div>
        </div>
        <div class="card-body" style="padding:0;">
          <table>
            <thead>
              <tr><th>Ref</th><th>Customer</th><th>Type</th><th>Amount</th>
                  <th>Status</th><th>Date / Time</th><th>Channel</th><th></th></tr>
            </thead>
            <tbody>{tbody}</tbody>
          </table>
        </div>
      </div>"""
    return desktop_page("Transactions", layout("transactions", content))


# ── 05 – Teller Portal ───────────────────────────────────────────────────────

def page_teller_portal() -> str:
    content = f"""
      <div class="page-title">🏧 Teller Portal</div>
      <div class="page-subtitle">Daily teller operations – Branch: Nairobi CBD · Counter: 3</div>

      <div class="stat-grid">
        <div class="stat-card">
          <div class="stat-icon" style="background:{GREEN_LIGHT};">💵</div>
          <div class="stat-value">KES 842,500</div>
          <div class="stat-label">Cash Received Today</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{RED_LIGHT};">💸</div>
          <div class="stat-value">KES 621,000</div>
          <div class="stat-label">Cash Disbursed Today</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{BLUE_LIGHT};">🔢</div>
          <div class="stat-value">127</div>
          <div class="stat-label">Transactions Today</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{YELLOW_LIGHT};">🏦</div>
          <div class="stat-value">KES 221,500</div>
          <div class="stat-label">Net Cash Position</div>
        </div>
      </div>

      <div style="display:grid;grid-template-columns:1fr 1fr;gap:20px;">
        <div class="card">
          <div class="card-header"><h5>💰 Cash Deposit</h5></div>
          <div class="card-body">
            <div class="form-group">
              <label class="form-label">Account Number</label>
              <input class="form-control" value="ACC-001234"/>
            </div>
            <div class="form-group">
              <label class="form-label">Customer Name</label>
              <input class="form-control" value="John Mwangi" readonly
                     style="background:{BG_LIGHT};"/>
            </div>
            <div class="form-group">
              <label class="form-label">Amount (KES)</label>
              <input class="form-control" value="50,000.00"/>
            </div>
            <div class="form-group">
              <label class="form-label">Description</label>
              <input class="form-control" value="Cash deposit – counter"/>
            </div>
            <button class="btn btn-success" style="width:100%;padding:11px;">
              ✅ Post Deposit
            </button>
          </div>
        </div>
        <div class="card">
          <div class="card-header"><h5>💳 Cash Withdrawal</h5></div>
          <div class="card-body">
            <div class="form-group">
              <label class="form-label">Account Number</label>
              <input class="form-control" value="ACC-001235"/>
            </div>
            <div class="form-group">
              <label class="form-label">Customer Name</label>
              <input class="form-control" value="Wanjiku Kamau" readonly
                     style="background:{BG_LIGHT};"/>
            </div>
            <div class="form-group">
              <label class="form-label">Amount (KES)</label>
              <input class="form-control" value="12,500.00"/>
            </div>
            <div class="form-group">
              <label class="form-label">ID Verified</label>
              <select class="form-control">
                <option selected>✅ National ID – 34521789</option>
                <option>Passport</option>
              </select>
            </div>
            <button class="btn btn-danger" style="width:100%;padding:11px;">
              💸 Process Withdrawal
            </button>
          </div>
        </div>
      </div>"""
    return desktop_page("Teller Portal",
                        layout("transactions", content, "teller01", "Teller"))


# ── 06 – Loans ───────────────────────────────────────────────────────────────

def page_loans() -> str:
    loans = [
        ("LN-2026-001", "John Mwangi",   "Personal",    "KES 200,000",  "36 mo", "12.5%", "Active",  "KES 162,000"),
        ("LN-2026-002", "Grace Njeri",   "Mortgage",   "KES 5,000,000", "180 mo","10.0%", "Active",  "KES 4,820,000"),
        ("LN-2026-003", "Peter Kariuki", "Business",   "KES 1,500,000", "60 mo", "14.0%", "Active",  "KES 1,120,000"),
        ("LN-2026-004", "Amina Hassan",  "Personal",    "KES 80,000",   "12 mo", "18.0%", "Overdue", "KES 72,000"),
        ("LN-2026-005", "Samuel Owino",  "Asset Finance","KES 900,000", "48 mo", "13.5%", "Pending", "KES 900,000"),
    ]
    tbody = "".join(
        f'<tr><td><span style="font-family:monospace;font-size:12px;">{ref}</span></td>'
        f'<td>{name}</td><td>{ltype}</td><td style="font-weight:600;">{amt}</td>'
        f'<td>{term}</td><td>{rate}</td>'
        f'<td><span class="badge {"badge-success" if status=="Active" else ("badge-danger" if status=="Overdue" else "badge-warning")}">{status}</span></td>'
        f'<td style="color:{ACCENT if status=="Active" else DANGER};font-weight:600;">{balance}</td>'
        f'<td><button class="btn btn-outline-primary btn-sm">Details</button></td></tr>'
        for ref, name, ltype, amt, term, rate, status, balance in loans
    )
    content = f"""
      <div class="page-title">📋 Loans Management</div>
      <div class="page-subtitle">All loan accounts – disbursements, repayments and NPLs</div>
      <div class="stat-grid">
        <div class="stat-card">
          <div class="stat-icon" style="background:{BLUE_LIGHT};">📋</div>
          <div class="stat-value">8,134</div><div class="stat-label">Active Loans</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{GREEN_LIGHT};">💰</div>
          <div class="stat-value">KES 12.4B</div><div class="stat-label">Loan Book Value</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{RED_LIGHT};">⚠️</div>
          <div class="stat-value">342</div><div class="stat-label">NPL / Overdue</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{YELLOW_LIGHT};">⏳</div>
          <div class="stat-value">56</div><div class="stat-label">Pending Approval</div>
        </div>
      </div>
      <div class="card">
        <div class="card-header">
          <h5>Loan Register</h5>
          <button class="btn btn-primary">+ New Loan Application</button>
        </div>
        <div class="card-body" style="padding:0;">
          <table>
            <thead><tr><th>Loan Ref</th><th>Customer</th><th>Type</th><th>Amount</th>
                        <th>Term</th><th>Rate</th><th>Status</th><th>Outstanding</th><th></th></tr></thead>
            <tbody>{tbody}</tbody>
          </table>
        </div>
      </div>"""
    return desktop_page("Loans", layout("loans", content))


def page_loan_application() -> str:
    content = f"""
      <div class="page-title">📝 New Loan Application</div>
      <div class="page-subtitle">Complete the form to submit a new loan application</div>
      <div class="card">
        <div class="card-header"><h5>📋 Applicant & Loan Details</h5></div>
        <div class="card-body">
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">CIF Number</label>
              <input class="form-control" value="CIF-000456"/>
            </div>
            <div class="form-group">
              <label class="form-label">Customer Name</label>
              <input class="form-control" value="John Mwangi" readonly style="background:{BG_LIGHT};"/>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Loan Type</label>
              <select class="form-control">
                <option selected>Personal Loan</option>
                <option>Business Loan</option>
                <option>Mortgage</option>
                <option>Asset Finance</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">Loan Amount (KES)</label>
              <input class="form-control" value="300,000"/>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Repayment Period (months)</label>
              <input class="form-control" value="24"/>
            </div>
            <div class="form-group">
              <label class="form-label">Interest Rate (% p.a.)</label>
              <input class="form-control" value="12.5" readonly style="background:{BG_LIGHT};"/>
            </div>
          </div>
          <div class="form-group">
            <label class="form-label">Purpose of Loan</label>
            <input class="form-control" value="Business expansion – purchase of equipment"/>
          </div>
          <div style="background:{BLUE_LIGHT};border-radius:10px;padding:16px;margin-bottom:20px;">
            <div style="font-weight:700;margin-bottom:8px;color:{PRIMARY};">💡 Loan Summary</div>
            <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:12px;">
              {"".join(f'<div><div style="font-size:11px;color:{GRAY_600};">{l}</div>'
                       f'<div style="font-size:16px;font-weight:700;color:{PRIMARY};">{v}</div></div>'
                       for l, v in [("Monthly Payment", "KES 14,248"), ("Total Repayment", "KES 341,952"),
                                    ("Total Interest", "KES 41,952"), ("Approval Score", "82/100")])}
            </div>
          </div>
          <div style="display:flex;gap:10px;">
            <button class="btn btn-primary" style="flex:1;padding:11px;">Submit Application</button>
            <button class="btn btn-outline-primary" style="padding:11px 20px;">Save Draft</button>
          </div>
        </div>
      </div>"""
    return desktop_page("Loan Application", layout("loans", content))


# ── 07 – Customer Portal ─────────────────────────────────────────────────────

def page_customer_portal() -> str:
    content = f"""
      <div class="page-title">👤 Customer Portal</div>
      <div class="page-subtitle">Self-service banking for customers</div>

      <div style="display:grid;grid-template-columns:1fr 2fr;gap:20px;">
        <div>
          <div class="card">
            <div class="card-header"><h5>👤 My Profile</h5></div>
            <div class="card-body" style="text-align:center;">
              <div class="avatar" style="width:60px;height:60px;font-size:24px;margin:0 auto 12px;">JM</div>
              <div style="font-weight:700;font-size:16px;">John Mwangi</div>
              <div style="font-size:12px;color:{GRAY_600};margin:4px 0 12px;">CIF-000456</div>
              <span class="badge badge-success">KYC Verified</span>
              <div style="margin-top:16px;text-align:left;">
                {"".join(f'<div style="display:flex;justify-content:space-between;padding:8px 0;border-bottom:1px solid {GRAY_200};font-size:13px;"><span style="color:{GRAY_600};">{k}</span><span>{v}</span></div>'
                         for k, v in [("Phone", "+254 712 345 678"),
                                      ("Email", "john.m@email.com"),
                                      ("Branch", "Nairobi CBD")])}
              </div>
            </div>
          </div>
        </div>
        <div>
          <div class="stat-grid" style="grid-template-columns:repeat(3,1fr);">
            <div class="stat-card">
              <div class="stat-icon" style="background:{BLUE_LIGHT};">🏦</div>
              <div class="stat-value">3</div>
              <div class="stat-label">Active Accounts</div>
            </div>
            <div class="stat-card">
              <div class="stat-icon" style="background:{GREEN_LIGHT};">💰</div>
              <div class="stat-value">KES 192,650</div>
              <div class="stat-label">Total Balance</div>
            </div>
            <div class="stat-card">
              <div class="stat-icon" style="background:{YELLOW_LIGHT};">📋</div>
              <div class="stat-value">1</div>
              <div class="stat-label">Active Loans</div>
            </div>
          </div>
          <div class="card">
            <div class="card-header"><h5>🏦 My Accounts</h5></div>
            <div class="card-body">
              {"".join(f'<div style="display:flex;justify-content:space-between;align-items:center;'
                       f'padding:12px;background:{bg};border-radius:10px;margin-bottom:8px;">'
                       f'<div><div style="font-weight:600;font-size:14px;">{name}</div>'
                       f'<div style="font-size:12px;color:{GRAY_600};">{acc}</div></div>'
                       f'<div style="font-size:18px;font-weight:700;color:{PRIMARY};">{bal}</div></div>'
                       for name, acc, bal, bg in [
                           ("Savings Account", "ACC-001234", "KES 128,450", BLUE_LIGHT),
                           ("Current Account", "ACC-001298", "KES 54,200",  GREEN_LIGHT),
                           ("Fixed Deposit",   "ACC-001312", "KES 10,000",  YELLOW_LIGHT),
                       ])}
            </div>
          </div>
          <div class="card">
            <div class="card-header"><h5>⚡ Quick Services</h5></div>
            <div class="card-body">
              <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:12px;">
                {"".join(f'<div style="text-align:center;padding:14px;background:{BG_LIGHT};'
                         f'border-radius:10px;cursor:pointer;">'
                         f'<div style="font-size:24px;margin-bottom:6px;">{icon}</div>'
                         f'<div style="font-size:12px;font-weight:600;color:{GRAY_700};">{label}</div></div>'
                         for icon, label in [("💸","Transfer"), ("📱","M-Pesa"), ("💡","Bills"), ("📊","Statement")])}
              </div>
            </div>
          </div>
        </div>
      </div>"""
    return desktop_page("Customer Portal",
                        layout("cif", content, "j.mwangi", "Customer"))


# ── 08 – Branch Manager Portal ───────────────────────────────────────────────

def page_branch_manager() -> str:
    content = f"""
      <div class="page-title">🏢 Branch Manager Portal</div>
      <div class="page-subtitle">Nairobi CBD Branch – Daily Overview · 19 March 2026</div>

      <div class="stat-grid">
        <div class="stat-card">
          <div class="stat-icon" style="background:{BLUE_LIGHT};">👤</div>
          <div class="stat-value">2,847</div>
          <div class="stat-label">Branch Customers</div>
          <div class="stat-trend trend-up">▲ 12 new this month</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{GREEN_LIGHT};">💸</div>
          <div class="stat-value">KES 5.2M</div>
          <div class="stat-label">Today's Deposits</div>
          <div class="stat-trend trend-up">▲ 8% vs yesterday</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{RED_LIGHT};">📋</div>
          <div class="stat-value">14</div>
          <div class="stat-label">Pending Approvals</div>
          <div class="stat-trend trend-dn">⚠️ Needs attention</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{YELLOW_LIGHT};">👥</div>
          <div class="stat-value">23 / 26</div>
          <div class="stat-label">Staff Present</div>
          <div class="stat-trend" style="color:{WARNING};">3 on leave</div>
        </div>
      </div>

      <div style="display:grid;grid-template-columns:3fr 2fr;gap:20px;">
        <div class="card">
          <div class="card-header">
            <h5>📊 Branch Performance – This Month</h5>
          </div>
          <div class="card-body">
            {"".join(f'<div style="margin-bottom:14px;">'
                     f'<div style="display:flex;justify-content:space-between;font-size:13px;margin-bottom:4px;">'
                     f'<span>{label}</span><span style="font-weight:700;">{val}</span></div>'
                     f'<div style="height:8px;background:{GRAY_200};border-radius:99px;">'
                     f'<div style="height:8px;background:{color};border-radius:99px;width:{pct}%;"></div></div></div>'
                     for label, val, pct, color in [
                         ("Deposits Target",     "KES 42M / KES 50M",  84, PRIMARY),
                         ("Loans Disbursed",     "KES 18M / KES 25M",  72, ACCENT),
                         ("New Accounts",        "86 / 100",           86, WARNING),
                         ("Customer Satisfaction","4.2 / 5.0",         84, PRIMARY),
                     ])}
          </div>
        </div>
        <div class="card">
          <div class="card-header"><h5>👥 Teller Summary</h5></div>
          <div class="card-body" style="padding:0;">
            <table>
              <thead><tr><th>Teller</th><th>Txns</th><th>Volume</th></tr></thead>
              <tbody>
                <tr><td>teller01</td><td>42</td><td>KES 1.2M</td></tr>
                <tr><td>teller02</td><td>38</td><td>KES 980K</td></tr>
                <tr><td>teller03</td><td>29</td><td>KES 760K</td></tr>
                <tr><td>teller04</td><td>18</td><td>KES 510K</td></tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>"""
    return desktop_page("Branch Manager Portal",
                        layout("reporting", content, "branch.manager", "Branch Manager"))


# ── 09 – Admin Panel ─────────────────────────────────────────────────────────

def page_admin_panel() -> str:
    users = [
        ("admin",         "System Admin",    "System Administrator", "Active",  "19 Mar 2026 09:00"),
        ("teller01",      "Teller Staff",    "Teller",               "Active",  "19 Mar 2026 08:50"),
        ("teller02",      "Teller Staff",    "Teller",               "Active",  "19 Mar 2026 08:52"),
        ("loan.officer",  "Loans Team",      "Loan Officer",         "Active",  "19 Mar 2026 07:30"),
        ("risk.officer",  "Risk & Compliance","Risk Officer",         "Active",  "19 Mar 2026 08:00"),
        ("branch.manager","Management",      "Branch Manager",       "Active",  "19 Mar 2026 08:15"),
        ("auditor1",      "Audit Team",      "Auditor",              "Inactive","10 Mar 2026 14:00"),
    ]
    tbody = "".join(
        f'<tr><td>{u}</td><td>{dept}</td><td>{role}</td>'
        f'<td><span class="badge {"badge-success" if st=="Active" else "badge-secondary"}">{st}</span></td>'
        f'<td style="font-size:12px;color:{GRAY_600};">{last}</td>'
        f'<td><button class="btn btn-sm btn-outline-primary">Edit</button> '
        f'<button class="btn btn-sm btn-danger">Disable</button></td></tr>'
        for u, dept, role, st, last in users
    )
    content = f"""
      <div class="page-title">⚙️ Admin Panel</div>
      <div class="page-subtitle">System administration – users, roles and configuration</div>

      <div style="display:grid;grid-template-columns:3fr 1fr;gap:20px;">
        <div class="card">
          <div class="card-header">
            <h5>👥 User Management</h5>
            <button class="btn btn-primary">+ Create User</button>
          </div>
          <div class="card-body" style="padding:0;">
            <table>
              <thead><tr><th>Username</th><th>Department</th><th>Role</th>
                          <th>Status</th><th>Last Login</th><th>Actions</th></tr></thead>
              <tbody>{tbody}</tbody>
            </table>
          </div>
        </div>
        <div>
          <div class="card">
            <div class="card-header"><h5>🔐 Roles</h5></div>
            <div class="card-body" style="padding:12px;">
              {"".join(f'<div style="display:flex;justify-content:space-between;align-items:center;'
                       f'padding:8px;background:{BG_LIGHT};border-radius:8px;margin-bottom:6px;">'
                       f'<span style="font-size:13px;">{r}</span>'
                       f'<span class="badge badge-info">{c}</span></div>'
                       for r, c in [("System Admin","1"), ("Branch Manager","2"),
                                    ("Teller","4"), ("Loan Officer","3"),
                                    ("Risk Officer","2"), ("Auditor","1")])}
            </div>
          </div>
          <div class="card">
            <div class="card-header"><h5>⚙️ System Config</h5></div>
            <div class="card-body" style="font-size:13px;">
              {"".join(f'<div style="padding:8px 0;border-bottom:1px solid {GRAY_200};">'
                       f'<div style="color:{GRAY_600};font-size:11px;">{k}</div>'
                       f'<div style="font-weight:600;">{v}</div></div>'
                       for k, v in [("Environment","Production"),
                                    ("API Version","v1-Core"),
                                    ("DB","PostgreSQL 15"),
                                    ("Cache","Redis 7")])}
            </div>
          </div>
        </div>
      </div>"""
    return desktop_page("Admin Panel", layout("admin", content))


# ── 10 – Reports ──────────────────────────────────────────────────────────────

def page_reports() -> str:
    content = f"""
      <div class="page-title">📊 Reporting & Analytics</div>
      <div class="page-subtitle">Management information system – consolidated financial reports</div>

      <div style="display:grid;grid-template-columns:1fr 1fr;gap:20px;margin-bottom:20px;">
        <div class="card">
          <div class="card-header"><h5>💰 Income Statement – Mar 2026</h5></div>
          <div class="card-body">
            {"".join(f'<div style="display:flex;justify-content:space-between;padding:8px 0;'
                     f'border-bottom:1px solid {GRAY_200};font-size:13px;'
                     f'{"font-weight:700;" if bold else ""}">'
                     f'<span style="color:{GRAY_700 if not bold else GRAY_900};">{label}</span>'
                     f'<span style="color:{color};font-weight:600;">{val}</span></div>'
                     for label, val, color, bold in [
                         ("Interest Income",     "KES 42,500,000", ACCENT, False),
                         ("Fee Income",          "KES 8,200,000",  ACCENT, False),
                         ("FX Income",           "KES 3,100,000",  ACCENT, False),
                         ("Gross Income",        "KES 53,800,000", PRIMARY, True),
                         ("Interest Expense",    "KES 12,400,000", DANGER, False),
                         ("Operating Expenses",  "KES 15,200,000", DANGER, False),
                         ("Net Profit (Mar)",    "KES 26,200,000", ACCENT, True),
                     ])}
          </div>
        </div>
        <div class="card">
          <div class="card-header"><h5>🏦 Balance Sheet Snapshot</h5></div>
          <div class="card-body">
            {"".join(f'<div style="display:flex;justify-content:space-between;padding:8px 0;'
                     f'border-bottom:1px solid {GRAY_200};font-size:13px;'
                     f'{"font-weight:700;" if bold else ""}">'
                     f'<span style="color:{GRAY_700 if not bold else GRAY_900};">{label}</span>'
                     f'<span style="color:{color};font-weight:600;">{val}</span></div>'
                     for label, val, color, bold in [
                         ("Total Assets",        "KES 28.4B", PRIMARY, True),
                         ("Loans & Advances",    "KES 12.4B", GRAY_700, False),
                         ("Investments",         "KES 8.2B",  GRAY_700, False),
                         ("Cash & Equivalents",  "KES 3.8B",  GRAY_700, False),
                         ("Total Liabilities",   "KES 22.8B", DANGER,  True),
                         ("Customer Deposits",   "KES 18.6B", GRAY_700, False),
                         ("Equity",              "KES 5.6B",  ACCENT,  True),
                     ])}
          </div>
        </div>
      </div>

      <div class="card">
        <div class="card-header">
          <h5>📋 Available Reports</h5>
          <button class="btn btn-outline-primary">Export All</button>
        </div>
        <div class="card-body">
          <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:12px;">
            {"".join(f'<div style="padding:14px;background:{BG_LIGHT};border-radius:10px;'
                     f'border:1.5px solid {GRAY_200};cursor:pointer;">'
                     f'<div style="font-size:22px;margin-bottom:6px;">{icon}</div>'
                     f'<div style="font-weight:600;font-size:13px;">{name}</div>'
                     f'<div style="font-size:11px;color:{GRAY_600};margin-top:3px;">{desc}</div>'
                     f'<button class="btn btn-outline-primary btn-sm" style="margin-top:10px;">Generate</button></div>'
                     for icon, name, desc in [
                         ("📊", "Daily Transactions",  "Itemised ledger"),
                         ("💰", "Income Statement",    "P&L summary"),
                         ("🏦", "Balance Sheet",       "Assets & liabilities"),
                         ("📋", "Loan Portfolio",      "NPL & aging"),
                         ("👤", "Customer Report",     "KYC & accounts"),
                         ("💳", "Teller Summary",      "Counter performance"),
                         ("📈", "Branch Performance",  "MIS dashboard"),
                         ("🔒", "Audit Trail",         "Security & access"),
                     ])}
          </div>
        </div>
      </div>"""
    return desktop_page("Reports", layout("reporting", content))


# ── 11 – Compliance Portal ────────────────────────────────────────────────────

def page_compliance() -> str:
    content = f"""
      <div class="page-title">🔒 Compliance Portal</div>
      <div class="page-subtitle">AML · KYC · Regulatory Reporting – CBK Guidelines</div>

      <div class="stat-grid">
        <div class="stat-card">
          <div class="stat-icon" style="background:{RED_LIGHT};">🚨</div>
          <div class="stat-value">7</div>
          <div class="stat-label">High-Risk Alerts</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{YELLOW_LIGHT};">⚠️</div>
          <div class="stat-value">23</div>
          <div class="stat-label">KYC Expiring</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{BLUE_LIGHT};">📋</div>
          <div class="stat-value">12</div>
          <div class="stat-label">STR Pending</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{GREEN_LIGHT};">✅</div>
          <div class="stat-value">99.2%</div>
          <div class="stat-label">Compliance Score</div>
        </div>
      </div>

      <div class="card">
        <div class="card-header"><h5>🚨 AML Alerts Queue</h5></div>
        <div class="card-body" style="padding:0;">
          <table>
            <thead>
              <tr><th>Alert ID</th><th>Customer</th><th>Risk Level</th>
                  <th>Description</th><th>Amount</th><th>Date</th><th>Action</th></tr>
            </thead>
            <tbody>
              <tr><td>AML-001</td><td>Unknown Entity</td>
                  <td><span class="badge badge-danger">High</span></td>
                  <td>Structuring – multiple cash deposits</td>
                  <td>KES 980,000</td><td>19 Mar 2026</td>
                  <td><button class="btn btn-danger btn-sm">Freeze &amp; Report</button></td></tr>
              <tr><td>AML-002</td><td>Amina Hassan</td>
                  <td><span class="badge badge-warning">Medium</span></td>
                  <td>Unusual cross-border transfer</td>
                  <td>USD 15,000</td><td>18 Mar 2026</td>
                  <td><button class="btn btn-outline-primary btn-sm">Review</button></td></tr>
              <tr><td>AML-003</td><td>Shell Co. Ltd</td>
                  <td><span class="badge badge-danger">High</span></td>
                  <td>PEP connection – enhanced DD required</td>
                  <td>KES 2,400,000</td><td>17 Mar 2026</td>
                  <td><button class="btn btn-danger btn-sm">Escalate</button></td></tr>
            </tbody>
          </table>
        </div>
      </div>"""
    return desktop_page("Compliance Portal",
                        layout("compliance", content, "risk.officer", "Risk Officer"))


# ── 12 – Payments Portal ─────────────────────────────────────────────────────

def page_payments() -> str:
    content = f"""
      <div class="page-title">💹 Payments Portal</div>
      <div class="page-subtitle">M-Pesa · EFT · RTGS · Bill Payments · International Transfers</div>

      <div class="stat-grid">
        <div class="stat-card">
          <div class="stat-icon" style="background:{GREEN_LIGHT};">📱</div>
          <div class="stat-value">4,821</div>
          <div class="stat-label">M-Pesa Transactions</div>
          <div class="stat-trend trend-up">▲ 18% today</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{BLUE_LIGHT};">🏦</div>
          <div class="stat-value">342</div>
          <div class="stat-label">EFT / RTGS Today</div>
          <div class="stat-trend trend-up">▲ 5% today</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{YELLOW_LIGHT};">💡</div>
          <div class="stat-value">1,204</div>
          <div class="stat-label">Bill Payments</div>
          <div class="stat-trend trend-up">▲ 8% today</div>
        </div>
        <div class="stat-card">
          <div class="stat-icon" style="background:{RED_LIGHT};">🌍</div>
          <div class="stat-value">28</div>
          <div class="stat-label">International</div>
          <div class="stat-trend" style="color:{GRAY_600};">Stable</div>
        </div>
      </div>

      <div style="display:grid;grid-template-columns:1fr 1fr;gap:20px;">
        <div class="card">
          <div class="card-header"><h5>📱 M-Pesa Integration</h5></div>
          <div class="card-body">
            <div style="display:flex;align-items:center;gap:12px;padding:12px;
                        background:{GREEN_LIGHT};border-radius:10px;margin-bottom:16px;">
              <div style="font-size:32px;">📱</div>
              <div>
                <div style="font-weight:700;">Safaricom M-Pesa</div>
                <div style="font-size:12px;color:{GRAY_600};">Paybill: 123456 · Till: 987654</div>
                <span class="badge badge-success" style="margin-top:4px;">● Connected</span>
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Phone Number</label>
              <input class="form-control" value="+254 712 345 678"/>
            </div>
            <div class="form-group">
              <label class="form-label">Amount (KES)</label>
              <input class="form-control" value="5,000"/>
            </div>
            <div class="form-group">
              <label class="form-label">Transaction Type</label>
              <select class="form-control">
                <option selected>B2C – Customer Payment</option>
                <option>C2B – Incoming</option>
              </select>
            </div>
            <button class="btn btn-success" style="width:100%;padding:11px;">
              💸 Initiate M-Pesa Transfer
            </button>
          </div>
        </div>
        <div class="card">
          <div class="card-header"><h5>🏦 RTGS / EFT Transfer</h5></div>
          <div class="card-body">
            <div class="form-group">
              <label class="form-label">Beneficiary Account</label>
              <input class="form-control" value="0123456789"/>
            </div>
            <div class="form-group">
              <label class="form-label">Beneficiary Bank</label>
              <select class="form-control">
                <option selected>Equity Bank Kenya</option>
                <option>KCB Bank</option>
                <option>Co-operative Bank</option>
                <option>NCBA Bank</option>
              </select>
            </div>
            <div class="form-group">
              <label class="form-label">Amount (KES)</label>
              <input class="form-control" value="250,000"/>
            </div>
            <div class="form-group">
              <label class="form-label">Reference</label>
              <input class="form-control" value="Invoice INV-2026-0042"/>
            </div>
            <button class="btn btn-primary" style="width:100%;padding:11px;">
              🔄 Submit RTGS Transfer
            </button>
          </div>
        </div>
      </div>"""
    return desktop_page("Payments Portal",
                        layout("payments", content))


# ── 13 – API Docs (Swagger) ───────────────────────────────────────────────────

def page_api_docs() -> str:
    endpoints = [
        ("POST",   "/api/authentication/login",         "Authenticate user – returns JWT token"),
        ("GET",    "/api/accounts",                     "List all accounts (paginated)"),
        ("POST",   "/api/accounts",                     "Open a new bank account"),
        ("GET",    "/api/accounts/{id}",                "Get account details by ID"),
        ("PUT",    "/api/accounts/{id}/status",         "Update account status"),
        ("GET",    "/api/transactions",                 "List transactions with filters"),
        ("POST",   "/api/transactions/deposit",         "Post a cash deposit"),
        ("POST",   "/api/transactions/withdrawal",      "Post a cash withdrawal"),
        ("POST",   "/api/transactions/transfer",        "Initiate funds transfer"),
        ("GET",    "/api/loans",                        "List all loan accounts"),
        ("POST",   "/api/loans/apply",                  "Submit a loan application"),
        ("GET",    "/api/cif/search",                   "Search CIF by name, ID or phone"),
        ("POST",   "/api/payments/mpesa/b2c",           "Initiate M-Pesa B2C payment"),
        ("GET",    "/api/reporting/summary",            "Get bank-wide summary report"),
        ("GET",    "/health",                           "Health-check endpoint"),
    ]
    method_colors = {"GET": "#0d6efd", "POST": "#198754", "PUT": "#fd7e14", "DELETE": "#dc3545"}
    rows = "".join(
        f'<tr>'
        f'<td><span style="font-size:11px;font-weight:700;padding:3px 8px;border-radius:4px;'
        f'background:{method_colors.get(m, GRAY_400)};color:white;">{m}</span></td>'
        f'<td><code style="font-size:12px;color:{PRIMARY};">{path}</code></td>'
        f'<td style="font-size:13px;color:{GRAY_600};">{desc}</td>'
        f'<td><button class="btn btn-outline-primary btn-sm">Try it</button></td></tr>'
        for m, path, desc in endpoints
    )
    body = f"""
  <div style="background:{PRIMARY};color:{WHITE};padding:24px 32px;">
    <div style="font-size:22px;font-weight:700;">🏦 Wekeza Core Banking API</div>
    <div style="font-size:13px;opacity:0.8;margin-top:4px;">
      Version: v1-Core &nbsp;·&nbsp; OAS 3.0 &nbsp;·&nbsp; Base URL: http://localhost:5000/api
    </div>
    <div style="display:flex;gap:10px;margin-top:12px;">
      <span style="background:rgba(255,255,255,0.2);padding:4px 10px;border-radius:6px;font-size:12px;">Authorize 🔑</span>
      <span style="background:rgba(255,255,255,0.2);padding:4px 10px;border-radius:6px;font-size:12px;">JSON · XML</span>
    </div>
  </div>
  <div style="padding:24px 32px;background:{BG_LIGHT};">
    <div class="card">
      <div class="card-header"><h5>📋 API Endpoints</h5></div>
      <div class="card-body" style="padding:0;">
        <table>
          <thead><tr><th>Method</th><th>Endpoint</th><th>Description</th><th></th></tr></thead>
          <tbody>{rows}</tbody>
        </table>
      </div>
    </div>
  </div>"""
    return desktop_page("API Documentation", body)


# ---------------------------------------------------------------------------
# Page registry
# ---------------------------------------------------------------------------

PAGES = [
    ("01_login",            "01_login_page.png",              page_login),
    ("01_login",            "02_login_error.png",             page_login_error),
    ("02_dashboard",        "01_admin_dashboard.png",         page_dashboard),
    ("03_accounts",         "01_accounts_list.png",           page_accounts_list),
    ("03_accounts",         "02_account_detail.png",          page_account_detail),
    ("04_transactions",     "01_transaction_ledger.png",      page_transactions),
    ("05_teller_portal",    "01_teller_deposit_withdrawal.png", page_teller_portal),
    ("06_loans",            "01_loans_list.png",              page_loans),
    ("06_loans",            "02_loan_application.png",        page_loan_application),
    ("07_customer_portal",  "01_customer_dashboard.png",      page_customer_portal),
    ("08_branch_manager",   "01_branch_overview.png",         page_branch_manager),
    ("09_admin_panel",      "01_user_management.png",         page_admin_panel),
    ("10_reports",          "01_financial_reports.png",       page_reports),
    ("11_compliance",       "01_aml_alerts.png",              page_compliance),
    ("12_payments",         "01_payments_portal.png",         page_payments),
    ("13_api_docs",         "01_swagger_endpoints.png",       page_api_docs),
]


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main() -> int:
    from playwright.sync_api import sync_playwright

    print("=" * 60)
    print("  Wekeza v1-Core Banking API – Screenshot Generator")
    print("=" * 60)
    print(f"Output directory: {SCREENSHOTS_DIR}")
    print()

    tmp_dir = Path("/tmp/wekeza_v1_screenshots_html")
    tmp_dir.mkdir(parents=True, exist_ok=True)

    success_count = 0
    fail_count = 0

    with sync_playwright() as pw:
        browser = pw.chromium.launch(headless=True)
        context = browser.new_context(
            viewport={"width": 1440, "height": 900},
            device_scale_factor=1,
        )
        page = context.new_page()

        for category, filename, gen_fn in PAGES:
            out_dir = SCREENSHOTS_DIR / category
            out_dir.mkdir(parents=True, exist_ok=True)
            out_path = out_dir / filename

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

    print()
    print(f"Screenshots generated: {success_count}  |  Failures: {fail_count}")
    print(f"Output directory: {SCREENSHOTS_DIR}")
    print()

    return 0 if fail_count == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
