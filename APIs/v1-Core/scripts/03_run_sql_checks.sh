#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
SQL_DIR="${SCRIPT_DIR}/sql"
DB_CONTAINER="${DB_CONTAINER:-wekeza-v1-postgres}"
DB_USER="${DB_USER:-wekeza_app}"
DB_NAME="${DB_NAME:-WekezaCoreDB}"

run_sql_file() {
  local sql_file="$1"
  echo "[sql] Running ${sql_file}"
  docker exec -i "${DB_CONTAINER}" psql -U "${DB_USER}" -d "${DB_NAME}" -f "${sql_file}"
}

echo "[sql] Copying SQL scripts into container and executing"
docker cp "${SQL_DIR}/01_list_test_users.sql" "${DB_CONTAINER}:/tmp/01_list_test_users.sql"
docker cp "${SQL_DIR}/02_get_active_account.sql" "${DB_CONTAINER}:/tmp/02_get_active_account.sql"
docker cp "${SQL_DIR}/03_portal_data_sanity.sql" "${DB_CONTAINER}:/tmp/03_portal_data_sanity.sql"

run_sql_file "/tmp/01_list_test_users.sql"
run_sql_file "/tmp/03_portal_data_sanity.sql"

ACTIVE_ACCOUNT_NUMBER="$({
  docker exec -i "${DB_CONTAINER}" psql -U "${DB_USER}" -d "${DB_NAME}" -t -A -f /tmp/02_get_active_account.sql;
} | head -n 1 | tr -d '[:space:]')"

if [[ -z "${ACTIVE_ACCOUNT_NUMBER}" ]]; then
  echo "[error] No active account was found"
  exit 1
fi

echo "[ok] Active account: ${ACTIVE_ACCOUNT_NUMBER}"
echo "${ACTIVE_ACCOUNT_NUMBER}" > "${SCRIPT_DIR}/.last_active_account.txt"

echo "[ok] SQL checks completed"
