#!/usr/bin/env node
/**
 * screenshot-portals.js
 * ─────────────────────
 * Launches a headless Chromium browser, logs into the Wekeza Unified Shell,
 * navigates to each of the 14 portals, and saves a screenshot of each one.
 *
 * Requirements:
 *   npm install playwright  (one-time; installs bundled Chromium automatically)
 *   npx playwright install chromium
 *
 * Usage:
 *   FRONTEND_URL=http://localhost:3000 node scripts/screenshot-portals.js
 *   node scripts/screenshot-portals.js --output docs/screenshots
 *
 * Environment variables:
 *   FRONTEND_URL   Base URL of the React frontend (default: http://localhost:3000)
 *   SCREENSHOT_DIR Override output directory
 */

const { chromium } = require("playwright");
const fs = require("fs");
const path = require("path");

// ── Config ────────────────────────────────────────────────────────────────────
const FRONTEND_URL =
  process.env.FRONTEND_URL || process.env.VITE_API_URL?.replace("/api", "") || "http://localhost:3000";

// Parse --output from CLI args
const outputArg = process.argv.indexOf("--output");
const SCREENSHOT_DIR =
  process.env.SCREENSHOT_DIR ||
  (outputArg !== -1 ? process.argv[outputArg + 1] : null) ||
  path.join(__dirname, "..", "docs", "screenshots");

// Portal definitions: route, login user, description
const PORTALS = [
  { route: "/portals/admin",            user: "admin",         pass: "Admin@123",     label: "01-admin-portal" },
  { route: "/portals/executive",        user: "executive1",    pass: "Executive@123", label: "02-executive-portal" },
  { route: "/portals/branch-manager",   user: "manager1",      pass: "Manager@123",   label: "03-branch-manager-portal" },
  { route: "/portals/branch-operations",user: "vaultOfficer1", pass: "Vault@123",     label: "04-branch-operations-portal" },
  { route: "/portals/teller",           user: "teller1",       pass: "Teller@123",    label: "05-teller-portal" },
  { route: "/portals/supervisor",       user: "supervisor1",   pass: "Supervisor@123",label: "06-supervisor-portal" },
  { route: "/portals/compliance",       user: "compliance1",   pass: "Compliance@123",label: "07-compliance-portal" },
  { route: "/portals/treasury",         user: "treasury1",     pass: "Treasury@123",  label: "08-treasury-portal" },
  { route: "/portals/trade-finance",    user: "tradeFinance1", pass: "Trade@123",     label: "09-trade-finance-portal" },
  { route: "/portals/product-gl",       user: "productGL1",    pass: "Product@123",   label: "10-product-gl-portal" },
  { route: "/portals/payments",         user: "payments1",     pass: "Payments@123",  label: "11-payments-portal" },
  { route: "/portals/customer",         user: "customer1",     pass: "Customer@123",  label: "12-customer-portal" },
  { route: "/portals/staff",            user: "teller1",       pass: "Teller@123",    label: "13-staff-portal" },
  { route: "/portals/workflow",         user: "manager1",      pass: "Manager@123",   label: "14-workflow-portal" },
];

// Convenience: shared admin for dashboard screenshot
const DASHBOARD = { route: "/dashboard", user: "admin", pass: "Admin@123", label: "00-dashboard" };
const LOGIN_PAGE = { label: "00-login-page" };

// ── Helpers ───────────────────────────────────────────────────────────────────
function log(msg) { process.stdout.write(`[screenshot] ${msg}\n`); }

async function saveScreenshot(page, name) {
  const filepath = path.join(SCREENSHOT_DIR, `${name}.png`);
  await page.screenshot({ path: filepath, fullPage: true });
  log(`  ✔  Saved: ${filepath}`);
  return filepath;
}

async function loginAs(page, username, password) {
  await page.goto(`${FRONTEND_URL}/login`, { waitUntil: "networkidle" });
  await page.fill('input[type="text"], input[name="username"], #username', username);
  await page.fill('input[type="password"], input[name="password"], #password', password);
  await page.click('button[type="submit"]');
  // Wait for navigation away from /login
  await page.waitForURL((url) => !url.pathname.includes("/login"), { timeout: 15000 });
  await page.waitForLoadState("networkidle", { timeout: 15000 });
}

// ── Main ──────────────────────────────────────────────────────────────────────
(async () => {
  // Ensure output directory exists
  fs.mkdirSync(SCREENSHOT_DIR, { recursive: true });
  log(`Screenshots will be saved to: ${SCREENSHOT_DIR}`);
  log(`Frontend URL: ${FRONTEND_URL}`);

  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({
    viewport: { width: 1440, height: 900 },
    locale: "en-US",
    timezoneId: "Africa/Nairobi",
  });

  const results = [];

  try {
    // ── Screenshot 0: Login page ─────────────────────────────────────────────
    log("\nCapturing login page ...");
    const loginPage = await context.newPage();
    await loginPage.goto(`${FRONTEND_URL}/login`, { waitUntil: "networkidle" });
    await saveScreenshot(loginPage, LOGIN_PAGE.label);
    results.push({ label: LOGIN_PAGE.label, status: "ok" });
    await loginPage.close();

    // ── Screenshot: Dashboard ────────────────────────────────────────────────
    log("\nCapturing dashboard ...");
    const dashPage = await context.newPage();
    try {
      await loginAs(dashPage, DASHBOARD.user, DASHBOARD.pass);
      await dashPage.goto(`${FRONTEND_URL}${DASHBOARD.route}`, { waitUntil: "networkidle", timeout: 20000 });
      await dashPage.waitForTimeout(1500);
      await saveScreenshot(dashPage, DASHBOARD.label);
      results.push({ label: DASHBOARD.label, status: "ok" });
    } catch (err) {
      log(`  ✘  Dashboard failed: ${err.message}`);
      await saveScreenshot(dashPage, `${DASHBOARD.label}-error`);
      results.push({ label: DASHBOARD.label, status: "error", error: err.message });
    }
    await dashPage.close();

    // ── Screenshots: all 14 portals ──────────────────────────────────────────
    for (const portal of PORTALS) {
      log(`\nCapturing ${portal.label} (${portal.user}) ...`);
      const page = await context.newPage();
      try {
        await loginAs(page, portal.user, portal.pass);
        await page.goto(`${FRONTEND_URL}${portal.route}`, {
          waitUntil: "networkidle",
          timeout: 20000,
        });
        // Wait a moment for data to load
        await page.waitForTimeout(2000);
        await saveScreenshot(page, portal.label);
        results.push({ label: portal.label, status: "ok" });
      } catch (err) {
        log(`  ✘  ${portal.label} failed: ${err.message}`);
        await saveScreenshot(page, `${portal.label}-error`);
        results.push({ label: portal.label, status: "error", error: err.message });
      }
      await page.close();
    }
  } finally {
    await browser.close();
  }

  // ── Write index HTML ──────────────────────────────────────────────────────
  const indexHtml = `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Wekeza Portal Screenshots</title>
  <style>
    body { font-family: system-ui, sans-serif; max-width: 1400px; margin: 0 auto; padding: 20px; background: #f5f5f5; }
    h1 { color: #1890ff; }
    .grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(400px, 1fr)); gap: 20px; margin-top: 20px; }
    .card { background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .card img { width: 100%; display: block; }
    .card-title { padding: 10px 15px; font-weight: bold; background: #1890ff; color: white; }
    .card-title.error { background: #ff4d4f; }
    .meta { padding: 8px 15px; font-size: 0.85em; color: #666; }
    .timestamp { text-align: right; color: #999; font-size: 0.85em; margin-top: 20px; }
  </style>
</head>
<body>
  <h1>🏦 Wekeza Banking System — Portal Screenshots</h1>
  <p>Frontend: <strong>${FRONTEND_URL}</strong></p>
  <div class="grid">
${results
  .map(
    (r) => `    <div class="card">
      <div class="card-title ${r.status === "error" ? "error" : ""}">${r.label}${r.status === "error" ? " ⚠️" : ""}</div>
      <img src="${r.label}${r.status === "error" ? "-error" : ""}.png" alt="${r.label}" loading="lazy" />
      <div class="meta">${r.error || "✔ captured successfully"}</div>
    </div>`
  )
  .join("\n")}
  </div>
  <div class="timestamp">Generated: ${new Date().toISOString()}</div>
</body>
</html>`;

  const indexPath = path.join(SCREENSHOT_DIR, "index.html");
  fs.writeFileSync(indexPath, indexHtml);
  log(`\nIndex page: ${indexPath}`);

  // ── Summary ───────────────────────────────────────────────────────────────
  const ok = results.filter((r) => r.status === "ok").length;
  const errors = results.filter((r) => r.status === "error").length;
  log(`\n${"═".repeat(50)}`);
  log(`Screenshots: ${ok} captured, ${errors} failed`);
  log(`Output:      ${SCREENSHOT_DIR}`);
  log(`Index:       ${indexPath}`);
  log(`${"═".repeat(50)}\n`);

  process.exit(errors > 0 ? 1 : 0);
})();
