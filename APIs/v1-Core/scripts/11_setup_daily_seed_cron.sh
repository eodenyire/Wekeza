#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/../../.." && pwd)"
LOG_DIR="${SCRIPT_DIR}/logs"
mkdir -p "${LOG_DIR}"

# Default python: prefer workspace venv, fall back to system python3
_DEFAULT_PYTHON="${ROOT_DIR}/.venv/bin/python"
if [[ ! -f "${_DEFAULT_PYTHON}" || ! -x "${_DEFAULT_PYTHON}" ]]; then
  _DEFAULT_PYTHON="$(command -v python3)"
fi
PYTHON_BIN="${PYTHON_BIN:-${_DEFAULT_PYTHON}}"
CRON_SCHEDULE="${CRON_SCHEDULE:-5 0 * * *}"
CRON_COMMAND="cd ${SCRIPT_DIR}/.. && ${PYTHON_BIN} scripts/10_daily_realtime_seed.py >> ${LOG_DIR}/daily_seed.log 2>&1"

if ! command -v crontab >/dev/null 2>&1; then
  echo "[error] crontab command not found. Install cron and retry."
  exit 1
fi

EXISTING="$(crontab -l 2>/dev/null || true)"
FILTERED="$(printf '%s\n' "${EXISTING}" | grep -v '10_daily_realtime_seed.py' || true)"

{
  printf '%s\n' "${FILTERED}"
  printf '%s %s\n' "${CRON_SCHEDULE}" "${CRON_COMMAND}"
} | crontab -

echo "[ok] Daily realtime seeding cron installed"
echo "[info] Schedule: ${CRON_SCHEDULE}"
echo "[info] Command : ${CRON_COMMAND}"
