#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
COMPOSE_FILE="${ROOT_DIR}/docker-compose.yml"
API_BASE_URL="${API_BASE_URL:-http://localhost:8080}"

cd "${ROOT_DIR}"

echo "[start] Starting v1-Core dependencies (postgres, redis)..."
docker compose -f "${COMPOSE_FILE}" up -d postgres redis

echo "[start] Building and starting v1-Core API..."
docker compose -f "${COMPOSE_FILE}" up -d --build api

echo "[start] Waiting for API health at ${API_BASE_URL}/health ..."
for i in {1..40}; do
  if curl -fsS "${API_BASE_URL}/health" >/dev/null 2>&1; then
    echo "[ok] API is healthy"
    exit 0
  fi
  sleep 3
  echo "[wait] Attempt ${i}/40"
done

echo "[error] API did not become healthy in time"
exit 1
