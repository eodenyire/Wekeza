#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
DB_CONTAINER="${DB_CONTAINER:-wekeza-v1-postgres}"
DB_USER="${DB_USER:-wekeza_app}"
DB_NAME="${DB_NAME:-WekezaCoreDB}"

echo "[setup] Ensuring stack is running before DB bootstrap"
"${SCRIPT_DIR}/01_start_stack.sh"

echo "[setup] Copying heavy setup SQL into DB container"
docker cp "${SCRIPT_DIR}/sql/10_setup_all_tables_and_seed_1000.sql" "${DB_CONTAINER}:/tmp/10_setup_all_tables_and_seed_1000.sql"
docker cp "${SCRIPT_DIR}/sql/11_verify_core_counts.sql" "${DB_CONTAINER}:/tmp/11_verify_core_counts.sql"

echo "[setup] Executing heavy setup + seed script"
docker exec -i "${DB_CONTAINER}" psql -v ON_ERROR_STOP=1 -U "${DB_USER}" -d "${DB_NAME}" -f /tmp/10_setup_all_tables_and_seed_1000.sql

echo "[setup] Running verification script"
docker exec -i "${DB_CONTAINER}" psql -v ON_ERROR_STOP=1 -U "${DB_USER}" -d "${DB_NAME}" -f /tmp/11_verify_core_counts.sql

echo "[ok] Database bootstrap and 1000-record seed complete"
