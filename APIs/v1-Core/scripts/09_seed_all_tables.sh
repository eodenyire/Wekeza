#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DB_CONTAINER="${DB_CONTAINER:-wekeza-v1-postgres}"
DB_USER="${DB_USER:-wekeza_app}"
DB_NAME="${DB_NAME:-WekezaCoreDB}"

# Ensure baseline table setup + 1000-row seed for core entities
"${SCRIPT_DIR}/06_setup_database_1000_records.sh"

# Print exact row counts for every existing table
docker cp "${SCRIPT_DIR}/sql/13_all_table_row_counts.sql" "${DB_CONTAINER}:/tmp/13_all_table_row_counts.sql"

echo "[seed] Exact row counts for all existing tables"
docker exec -i "${DB_CONTAINER}" psql -v ON_ERROR_STOP=1 -U "${DB_USER}" -d "${DB_NAME}" -f /tmp/13_all_table_row_counts.sql

echo "[ok] All existing tables seeded/validated"
