#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
COMPOSE_FILE="${ROOT_DIR}/docker-compose.yml"

cd "${ROOT_DIR}"

echo "[stop] Stopping v1-Core stack"
docker compose -f "${COMPOSE_FILE}" down

echo "[ok] Stack stopped"
