#!/usr/bin/env node
/**
 * screenshot-portals-local.js
 * Quick screenshot capture using the built frontend with mock auth.
 * No backend required — uses localStorage to inject a mock auth token.
 */

const { chromium } = require("playwright");
const fs = require("fs");
const path = require("path");

const FRONTEND_URL = process.env.FRONTEND_URL || "http://localhost:3001";
const SCREENSHOT_DIR = process.env.SCREENSHOT_DIR ||
  path.join(__dirname, "..", "docs", "screenshots");

// Mock auth token (same JWT secret as the default in .env.example, signed for admin)
// We inject auth state directly into localStorage to bypass the login page
const MOCK_AUTH_STATE = JSON.stringify({
  state: {
    user: {
      id: "550e8400-e29b-41d4-a716-446655440002",
      username: "admin",
      email: "admin@wekeza.com",
      firstName: "System",
      lastName: "Admin",
      role: "Administrator",
      roles: ["Administrator", "Manager", "Teller", "Supervisor", "ComplianceOfficer",
               "TreasuryDealer", "TradeFinanceOfficer", "PaymentsOfficer", "ProductManager",
               "Customer", "VaultOfficer", "CEO"],
      department: "IT Administration",
      branch: "HQ",
      permissions: ["*"],
      isActive: true,
    },
    token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbiIsInJvbGUiOiJBZG1pbmlzdHJhdG9yIiwiZXhwIjo5OTk5OTk5OTk5fQ.placeholder",
    isAuthenticated: true,
  },
  version: 0,
});

const PORTALS = [
  { route: "/dashboard",                    label: "00-dashboard" },
  { route: "/portals/admin",                label: "01-admin-portal" },
  { route: "/portals/executive",            label: "02-executive-portal" },
  { route: "/portals/branch-manager",       label: "03-branch-manager-portal" },
  { route: "/portals/branch-operations",    label: "04-branch-operations-portal" },
  { route: "/portals/teller",               label: "05-teller-portal" },
  { route: "/portals/supervisor",           label: "06-supervisor-portal" },
  { route: "/portals/compliance",           label: "07-compliance-portal" },
  { route: "/portals/treasury",             label: "08-treasury-portal" },
  { route: "/portals/trade-finance",        label: "09-trade-finance-portal" },
  { route: "/portals/product-gl",           label: "10-product-gl-portal" },
  { route: "/portals/payments",             label: "11-payments-portal" },
  { route: "/portals/customer",             label: "12-customer-portal" },
  { route: "/portals/staff",                label: "13-staff-portal" },
  { route: "/portals/workflow",             label: "14-workflow-portal" },
];

(async () => {
  fs.mkdirSync(SCREENSHOT_DIR, { recursive: true });
  console.log(`[screenshots] Output: ${SCREENSHOT_DIR}`);
  console.log(`[screenshots] Frontend: ${FRONTEND_URL}`);

  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    viewport: { width: 1440, height: 900 },
    locale: "en-US",
  });

  // Pre-seed localStorage with auth state so ProtectedRoute passes
  await context.addInitScript((authState) => {
    localStorage.setItem("wekeza-auth-storage", authState);
    // Also set the plain token key used by some portal fetch() calls
    try {
      const parsed = JSON.parse(authState);
      if (parsed?.state?.token) {
        localStorage.setItem("authToken", parsed.state.token);
      }
    } catch (_) {}
  }, MOCK_AUTH_STATE);

  const results = [];

  // Screenshot login page first (no auth injection needed)
  {
    const page = await context.newPage();
    // Create a clean context for the login page
    const loginCtx = await browser.newContext({ viewport: { width: 1440, height: 900 } });
    const loginPage = await loginCtx.newPage();
    await loginPage.goto(`${FRONTEND_URL}/login`, { waitUntil: "networkidle", timeout: 15000 });
    const fp = path.join(SCREENSHOT_DIR, "00-login-page.png");
    await loginPage.screenshot({ path: fp, fullPage: true });
    console.log(`  ✔  00-login-page.png`);
    results.push({ label: "00-login-page", status: "ok" });
    await loginCtx.close();
    await page.close();
  }

  for (const portal of PORTALS) {
    const page = await context.newPage();
    try {
      await page.goto(`${FRONTEND_URL}${portal.route}`, {
        waitUntil: "networkidle",
        timeout: 20000,
      });
      // Give React a moment to render data
      await page.waitForTimeout(1500);
      const fp = path.join(SCREENSHOT_DIR, `${portal.label}.png`);
      await page.screenshot({ path: fp, fullPage: true });
      console.log(`  ✔  ${portal.label}.png`);
      results.push({ label: portal.label, status: "ok" });
    } catch (err) {
      const fp = path.join(SCREENSHOT_DIR, `${portal.label}-error.png`);
      await page.screenshot({ path: fp, fullPage: true });
      console.log(`  ✘  ${portal.label} — ${err.message}`);
      results.push({ label: portal.label, status: "error", error: err.message });
    }
    await page.close();
  }

  await browser.close();

  // Write HTML index
  const ts = new Date().toISOString();
  const cards = results.map((r) => {
    const imgName = r.status === "error" ? `${r.label}-error` : r.label;
    return `<div class="card">
  <div class="card-title ${r.status === "error" ? "err" : ""}">${r.label.replace(/-/g, " ").replace(/^\d+ /, "")}</div>
  <a href="${imgName}.png" target="_blank"><img src="${imgName}.png" alt="${r.label}" loading="lazy"/></a>
  <div class="meta">${r.error || "✔ captured"}</div>
</div>`;
  }).join("\n");

  const html = `<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="UTF-8">
<title>Wekeza Portal Screenshots — ${ts}</title>
<style>
  body{font-family:system-ui,sans-serif;margin:0;padding:20px;background:#f0f2f5}
  h1{color:#1890ff;margin-bottom:4px}
  h2{color:#555;font-weight:400;margin-top:0}
  .grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(420px,1fr));gap:16px;margin-top:20px}
  .card{background:#fff;border-radius:8px;overflow:hidden;box-shadow:0 1px 4px rgba(0,0,0,.15)}
  .card img{width:100%;display:block;border-bottom:1px solid #eee}
  .card-title{padding:10px 14px;font-weight:700;font-size:.9rem;background:#1890ff;color:#fff}
  .card-title.err{background:#ff4d4f}
  .meta{padding:6px 14px;font-size:.8rem;color:#888}
  footer{text-align:right;color:#aaa;font-size:.8rem;margin-top:20px}
</style>
</head>
<body>
<h1>🏦 Wekeza Banking System</h1>
<h2>Portal Screenshots — ${ts}</h2>
<p>Frontend: <code>${FRONTEND_URL}</code> &nbsp;|&nbsp; 
   Portals captured: <strong>${results.filter(r=>r.status==='ok').length}</strong> / ${results.length}</p>
<div class="grid">
${cards}
</div>
<footer>Generated by scripts/screenshot-portals-local.js on ${ts}</footer>
</body>
</html>`;

  fs.writeFileSync(path.join(SCREENSHOT_DIR, "index.html"), html);
  console.log(`\n[screenshots] Index: ${path.join(SCREENSHOT_DIR, "index.html")}`);

  const ok = results.filter(r => r.status === "ok").length;
  const err = results.filter(r => r.status === "error").length;
  console.log(`[screenshots] ${ok} captured, ${err} failed\n`);
  process.exit(err > 0 ? 1 : 0);
})();
