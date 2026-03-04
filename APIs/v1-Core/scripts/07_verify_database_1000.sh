#!/usr/bin/env bash
set -euo pipefail

DB_CONTAINER="${DB_CONTAINER:-wekeza-v1-postgres}"
DB_USER="${DB_USER:-wekeza_app}"
DB_NAME="${DB_NAME:-WekezaCoreDB}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "[verify] Core record thresholds (>=1000)"
docker cp "${SCRIPT_DIR}/sql/11_verify_core_counts.sql" "${DB_CONTAINER}:/tmp/11_verify_core_counts.sql"
docker exec -i "${DB_CONTAINER}" psql -U "${DB_USER}" -d "${DB_NAME}" -f /tmp/11_verify_core_counts.sql
