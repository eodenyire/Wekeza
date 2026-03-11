#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
WORKSPACE_ROOT="$(cd "${SCRIPT_DIR}/../../.." && pwd)"

"${SCRIPT_DIR}/01_start_stack.sh"
"${SCRIPT_DIR}/03_run_sql_checks.sh"

if [[ -f "${SCRIPT_DIR}/.last_active_account.txt" ]]; then
  export ACTIVE_ACCOUNT_NUMBER="$(cat "${SCRIPT_DIR}/.last_active_account.txt")"
fi

# Resolve python3: prefer the workspace venv, fall back to system python3
PYTHON_BIN="${WORKSPACE_ROOT}/.venv/bin/python"
if [[ ! -f "${PYTHON_BIN}" || ! -x "${PYTHON_BIN}" ]]; then
  PYTHON_BIN="$(command -v python3)"
fi

"${PYTHON_BIN}" "${SCRIPT_DIR}/12_backend_frontend_regression.py"
