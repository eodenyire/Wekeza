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
python3 "${SCRIPT_DIR}/04_portal_smoke_tests.py"

echo "[ok] Full test workflow completed"
