#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
COMPOSE_FILE="${ROOT_DIR}/docker-compose.yml"
API_BASE_URL="${API_BASE_URL:-http://localhost:8080}"

cd "${ROOT_DIR}"

echo "[health] Docker services status"
docker compose -f "${COMPOSE_FILE}" ps

echo "[health] API /health"
curl -fsS "${API_BASE_URL}/health" | cat

echo "\n[health] API /api/authentication/me should return 401 without token"
status_code="$(curl -s -o /dev/null -w "%{http_code}" "${API_BASE_URL}/api/authentication/me")"
echo "status=${status_code}"

if [[ "${status_code}" != "401" ]]; then
  echo "[warn] Expected 401 on /api/authentication/me without token"
fi

echo "[ok] Basic health checks completed"
