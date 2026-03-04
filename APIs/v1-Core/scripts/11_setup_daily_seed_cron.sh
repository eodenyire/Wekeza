#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LOG_DIR="${SCRIPT_DIR}/logs"
mkdir -p "${LOG_DIR}"

PYTHON_BIN="${PYTHON_BIN:-/workspaces/Wekeza/.venv/bin/python}"
CRON_SCHEDULE="${CRON_SCHEDULE:-5 0 * * *}"
CRON_COMMAND="cd /workspaces/Wekeza/APIs/v1-Core && ${PYTHON_BIN} scripts/10_daily_realtime_seed.py >> /workspaces/Wekeza/APIs/v1-Core/scripts/logs/daily_seed.log 2>&1"

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
