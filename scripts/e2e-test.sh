#!/usr/bin/env bash
# =============================================================================
# Wekeza Banking System — End-to-End API Test Script
# Tests every major endpoint across all 14 portals.
# =============================================================================
# Usage:
#   API_BASE_URL=http://localhost:8080 ./scripts/e2e-test.sh
# =============================================================================
set -euo pipefail

API_BASE_URL="${API_BASE_URL:-http://localhost:8080}"
PASS=0; FAIL=0; SKIP=0
REPORT_FILE="${REPORT_FILE:-/tmp/wekeza-e2e-report.txt}"

# ── Colours ───────────────────────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'
CYAN='\033[0;36m'; BOLD='\033[1m'; NC='\033[0m'

ok()     { echo -e "${GREEN}  ✔${NC}  $*"; ((PASS++)) || true; }
fail()   { echo -e "${RED}  ✘${NC}  $*"; ((FAIL++)) || true; }
skip()   { echo -e "${YELLOW}  -${NC}  $*  ${YELLOW}(skipped)${NC}"; ((SKIP++)) || true; }
section(){ echo -e "\n${BOLD}${CYAN}── $* ──${NC}"; }

# ── HTTP helper ───────────────────────────────────────────────────────────────
# Returns: HTTP status code; response body in $RESP_BODY
call() {
  local method="$1"
  local path="$2"
  local token="${3:-}"
  local body="${4:-}"

  local curl_args=(-s -o /tmp/wekeza_resp.json -w "%{http_code}")
  [[ -n "$token" ]] && curl_args+=(-H "Authorization: Bearer $token")
  [[ -n "$body"  ]] && curl_args+=(-H "Content-Type: application/json" -d "$body")

  local status
  status=$(curl "${curl_args[@]}" -X "$method" "${API_BASE_URL}${path}" 2>/dev/null || echo "000")
  RESP_BODY=$(cat /tmp/wekeza_resp.json 2>/dev/null || echo "{}")
  echo "$status"
}

# Extract a JWT token from $RESP_BODY (handles multiple response shapes)
extract_token() {
  echo "$RESP_BODY" | python3 -c \
    "import sys,json; d=json.load(sys.stdin); print(d.get('token') or d.get('accessToken') or d.get('data',{}).get('token',''))" \
    2>/dev/null || echo ""
}

check() {
  local label="$1"
  local status="$2"
  local expected="${3:-200}"
  if [[ "$status" == "$expected" || "$status" =~ ^2[0-9]{2}$ && "$expected" == "2xx" ]]; then
    ok "$label ($status)"
  else
    fail "$label — expected $expected, got $status"
  fi
}

login() {
  local username="$1"
  local password="$2"
  local status
  status=$(call POST /api/auth/login "" "{\"username\":\"$username\",\"password\":\"$password\"}")
  if [[ "$status" =~ ^2 ]]; then
    extract_token
  else
    echo ""
  fi
}

# =============================================================================
# Tests begin
# =============================================================================
echo -e "${BOLD}Wekeza Banking System — End-to-End API Tests${NC}"
echo -e "API: ${CYAN}${API_BASE_URL}${NC}\n"
echo "Started: $(date)" | tee "$REPORT_FILE"

# ── Infrastructure health ─────────────────────────────────────────────────────
section "Infrastructure"
s=$(call GET /health)
check "GET /health" "$s" "200"

s=$(call GET /swagger/v1/swagger.json)
check "GET /swagger (OpenAPI spec)" "$s" "200"

# ── Auth — obtain tokens for every portal role ────────────────────────────────
section "Authentication"
declare -A TOKENS

PORTAL_CREDS=(
  "admin:Admin@123"
  "manager1:Manager@123"
  "teller1:Teller@123"
  "supervisor1:Supervisor@123"
  "compliance1:Compliance@123"
  "treasury1:Treasury@123"
  "tradeFinance1:Trade@123"
  "payments1:Payments@123"
  "productGL1:Product@123"
  "customer1:Customer@123"
  "vaultOfficer1:Vault@123"
  "executive1:Executive@123"
)

for cred in "${PORTAL_CREDS[@]}"; do
  user="${cred%%:*}"
  pass="${cred##*:}"
  s=$(call POST /api/auth/login "" "{\"username\":\"$user\",\"password\":\"$pass\"}")
  if [[ "$s" =~ ^2 ]]; then
    TOKENS["$user"]=$(extract_token)
    ok "Login $user ($s)"
  else
    fail "Login $user ($s)"
    TOKENS["$user"]=""
  fi
done

ADM="${TOKENS[admin]:-}"
MGR="${TOKENS[manager1]:-}"
TLR="${TOKENS[teller1]:-}"
SUP="${TOKENS[supervisor1]:-}"
COM="${TOKENS[compliance1]:-}"
TRS="${TOKENS[treasury1]:-}"
TRD="${TOKENS[tradeFinance1]:-}"
PAY="${TOKENS[payments1]:-}"
PRD="${TOKENS[productGL1]:-}"
CUS="${TOKENS[customer1]:-}"
VLT="${TOKENS[vaultOfficer1]:-}"
EXC="${TOKENS[executive1]:-}"

# ── Teller Portal ─────────────────────────────────────────────────────────────
section "Teller Portal"
[[ -n "$TLR" ]] || { skip "Teller (no token)"; } && {
  s=$(call GET /api/teller/session "$TLR");            check "GET /api/teller/session" "$s"
  s=$(call GET /api/teller/cash-drawer "$TLR");        check "GET /api/teller/cash-drawer" "$s"
  s=$(call GET /api/transactions/recent "$TLR");       check "GET /api/transactions/recent" "$s"
}

# ── Branch Manager Portal ─────────────────────────────────────────────────────
section "Branch Manager Portal"
[[ -n "$MGR" ]] || { skip "Branch Manager (no token)"; } && {
  s=$(call GET /api/branch-manager/cash-position "$MGR"); check "GET /api/branch-manager/cash-position" "$s"
  s=$(call GET /api/branch-manager/team "$MGR");          check "GET /api/branch-manager/team" "$s"
  s=$(call GET /api/branch-manager/performance "$MGR");   check "GET /api/branch-manager/performance" "$s"
}

# ── Branch Operations Portal ──────────────────────────────────────────────────
section "Branch Operations Portal"
[[ -n "$VLT" ]] || { skip "Branch Ops (no token)"; } && {
  s=$(call GET /api/branch-manager/cash-position "$VLT"); check "GET /api/branch-manager/cash-position (vault)" "$s"
  s=$(call POST /api/branchoperations/bod "$VLT" \
    "{\"branchId\":\"11111111-1111-1111-1111-111111111111\",\"processedBy\":\"vaultOfficer1\"}");
  check "POST /api/branchoperations/bod" "$s" "200"
}

# ── Supervisor Portal ─────────────────────────────────────────────────────────
section "Supervisor Portal"
[[ -n "$SUP" ]] || { skip "Supervisor (no token)"; } && {
  s=$(call GET /api/supervisor/pending-approvals "$SUP"); check "GET /api/supervisor/pending-approvals" "$s"
  s=$(call GET /api/supervisor/teller-queue "$SUP");      check "GET /api/supervisor/teller-queue" "$s"
}

# ── Admin Portal ──────────────────────────────────────────────────────────────
section "Admin Portal"
[[ -n "$ADM" ]] || { skip "Admin (no token)"; } && {
  s=$(call GET /api/admin/users "$ADM");           check "GET /api/admin/users" "$s"
  s=$(call GET /api/admin/security/events "$ADM"); check "GET /api/admin/security/events" "$s"
  s=$(call GET /api/admin/system/config "$ADM");   check "GET /api/admin/system/config" "$s"
}

# ── Compliance Portal ─────────────────────────────────────────────────────────
section "Compliance & Risk Portal"
[[ -n "$COM" ]] || { skip "Compliance (no token)"; } && {
  s=$(call GET /api/compliance/aml-alerts "$COM");   check "GET /api/compliance/aml-alerts" "$s"
  s=$(call GET /api/compliance/risk-matrix "$COM");  check "GET /api/compliance/risk-matrix" "$s"
}

# ── Treasury Portal ───────────────────────────────────────────────────────────
section "Treasury & Markets Portal"
[[ -n "$TRS" ]] || { skip "Treasury (no token)"; } && {
  s=$(call GET /api/treasury/liquidity "$TRS");      check "GET /api/treasury/liquidity" "$s"
  s=$(call GET /api/treasury/fx-rates "$TRS");       check "GET /api/treasury/fx-rates" "$s"
}

# ── Trade Finance Portal ──────────────────────────────────────────────────────
section "Trade Finance Portal"
[[ -n "$TRD" ]] || { skip "Trade Finance (no token)"; } && {
  s=$(call GET /api/trade-finance/letters-of-credit "$TRD"); check "GET /api/trade-finance/letters-of-credit" "$s"
  s=$(call GET /api/trade-finance/guarantees "$TRD");        check "GET /api/trade-finance/guarantees" "$s"
}

# ── Payments Portal ───────────────────────────────────────────────────────────
section "Payments & Clearing Portal"
[[ -n "$PAY" ]] || { skip "Payments (no token)"; } && {
  s=$(call GET /api/payments/queue "$PAY");   check "GET /api/payments/queue" "$s"
  s=$(call GET /api/payments/metrics "$PAY"); check "GET /api/payments/metrics" "$s"
}

# ── Product & GL Portal ───────────────────────────────────────────────────────
section "Product & GL Portal"
[[ -n "$PRD" ]] || { skip "Product & GL (no token)"; } && {
  s=$(call GET /api/product-gl/catalog "$PRD");  check "GET /api/product-gl/catalog" "$s"
  s=$(call GET /api/product-gl/controls "$PRD"); check "GET /api/product-gl/controls" "$s"
}

# ── Customer Portal ───────────────────────────────────────────────────────────
section "Customer Digital Portal"
[[ -n "$CUS" ]] || { skip "Customer (no token)"; } && {
  s=$(call GET /api/customer/accounts "$CUS");  check "GET /api/customer/accounts" "$s"
  s=$(call GET /api/customer/activity "$CUS");  check "GET /api/customer/activity" "$s"
}

# ── Executive Portal ──────────────────────────────────────────────────────────
section "Executive & Board Portal"
[[ -n "$EXC" ]] || { skip "Executive (no token)"; } && {
  s=$(call GET /api/executive/kpis "$EXC");        check "GET /api/executive/kpis" "$s"
  s=$(call GET /api/executive/board-pack "$EXC");  check "GET /api/executive/board-pack" "$s"
}

# ── Staff Self-Service Portal ─────────────────────────────────────────────────
section "Staff Self-Service Portal"
[[ -n "$TLR" ]] || { skip "Staff (no token)"; } && {
  s=$(call GET /api/staff/requests "$TLR");  check "GET /api/staff/requests" "$s"
  s=$(call GET /api/staff/payslips "$TLR");  check "GET /api/staff/payslips" "$s"
}

# ── Workflow Portal ───────────────────────────────────────────────────────────
section "Workflow & Task Portal"
[[ -n "$MGR" ]] || { skip "Workflow (no token)"; } && {
  s=$(call GET /api/workflow/tasks "$MGR");   check "GET /api/workflow/tasks" "$s"
  s=$(call GET /api/workflow/monitor "$MGR"); check "GET /api/workflow/monitor" "$s"
}

# ── Accounts & Transactions ───────────────────────────────────────────────────
section "Core Banking — Accounts & Transactions"
[[ -n "$ADM" ]] || { skip "Core banking (no token)"; } && {
  s=$(call GET /api/accounts "$ADM");                      check "GET /api/accounts" "$s"
  s=$(call GET /api/accounts/search?query=ACC "$ADM");     check "GET /api/accounts/search" "$s"
  s=$(call GET /api/transactions "$ADM");                  check "GET /api/transactions" "$s"
  s=$(call GET /api/customers "$ADM");                     check "GET /api/customers" "$s"
  s=$(call GET /api/branches "$ADM");                      check "GET /api/branches" "$s"
}

# =============================================================================
# Summary
# =============================================================================
TOTAL=$((PASS + FAIL + SKIP))
echo ""
echo -e "${BOLD}═══════════════════════════════════════${NC}"
echo -e "${BOLD}Test Summary${NC}"
echo -e "  Total:   $TOTAL"
echo -e "  ${GREEN}Passed:  $PASS${NC}"
echo -e "  ${RED}Failed:  $FAIL${NC}"
echo -e "  ${YELLOW}Skipped: $SKIP${NC}"
echo -e "${BOLD}═══════════════════════════════════════${NC}"
echo "" | tee -a "$REPORT_FILE"
echo "Finished: $(date)" | tee -a "$REPORT_FILE"
echo "Results: $PASS passed, $FAIL failed, $SKIP skipped (of $TOTAL)" | tee -a "$REPORT_FILE"

[[ "$FAIL" -eq 0 ]]
