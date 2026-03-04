#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

"${SCRIPT_DIR}/01_start_stack.sh"
"${SCRIPT_DIR}/03_run_sql_checks.sh"

if [[ -f "${SCRIPT_DIR}/.last_active_account.txt" ]]; then
  export ACTIVE_ACCOUNT_NUMBER="$(cat "${SCRIPT_DIR}/.last_active_account.txt")"
fi

/workspaces/Wekeza/.venv/bin/python "${SCRIPT_DIR}/12_backend_frontend_regression.py"
