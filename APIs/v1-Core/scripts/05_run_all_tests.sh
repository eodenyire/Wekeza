#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

"${SCRIPT_DIR}/01_start_stack.sh"
"${SCRIPT_DIR}/02_smoke_health.sh"
"${SCRIPT_DIR}/03_run_sql_checks.sh"

if [[ -f "${SCRIPT_DIR}/.last_active_account.txt" ]]; then
  export ACTIVE_ACCOUNT_NUMBER="$(cat "${SCRIPT_DIR}/.last_active_account.txt")"
fi

echo "[run] Using ACTIVE_ACCOUNT_NUMBER=${ACTIVE_ACCOUNT_NUMBER:-<default>}"

echo ""
echo "=== Step 1: Portal smoke tests (admin / manager / teller) ==="
python3 "${SCRIPT_DIR}/04_portal_smoke_tests.py"

echo ""
echo "=== Step 2: Frontend form/API matrix ==="
python3 "${SCRIPT_DIR}/14_frontend_form_matrix.py"

echo ""
echo "=== Step 3: All-portal end-to-end regression (all 14 portals) ==="
python3 "${SCRIPT_DIR}/16_all_portal_e2e_regression.py"

echo ""
echo "[ok] Full test workflow completed"
