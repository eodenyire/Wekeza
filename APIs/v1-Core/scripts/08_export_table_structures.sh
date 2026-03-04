#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
DB_CONTAINER="${DB_CONTAINER:-wekeza-v1-postgres}"
DB_USER="${DB_USER:-wekeza_app}"
DB_NAME="${DB_NAME:-WekezaCoreDB}"
REPORT_DIR="${SCRIPT_DIR}/reports"
TIMESTAMP="$(date +%Y%m%d_%H%M%S)"
REPORT_FILE="${REPORT_DIR}/table_structures_${TIMESTAMP}.txt"

mkdir -p "${REPORT_DIR}"

"${SCRIPT_DIR}/01_start_stack.sh"

docker cp "${SCRIPT_DIR}/sql/12_describe_all_tables.sql" "${DB_CONTAINER}:/tmp/12_describe_all_tables.sql"

echo "[report] Exporting full table structures to ${REPORT_FILE}"
docker exec -i "${DB_CONTAINER}" psql -v ON_ERROR_STOP=1 -U "${DB_USER}" -d "${DB_NAME}" -f /tmp/12_describe_all_tables.sql | tee "${REPORT_FILE}"

echo "[ok] Structure report generated: ${REPORT_FILE}"
