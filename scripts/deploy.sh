#!/usr/bin/env bash
# =============================================================================
# Wekeza Banking System — Master Deployment Script
# =============================================================================
# Usage:
#   ./scripts/deploy.sh [option] [command]
#
# Options:
#   1   Docker — simple (postgres + redis + api + frontend)
#   2   Docker — full stack with nginx reverse proxy and pgAdmin
#   3   Kubernetes — local cluster via Minikube or kind
#
# Commands (default: up):
#   up        Start all services (build images if needed)
#   down      Stop and remove containers
#   reset     Stop + remove volumes (FULL DATA WIPE) then restart
#   status    Show running containers / pods
#   logs      Tail logs
#   test      Run end-to-end API tests (requires running stack)
#   seed      Re-run database seed scripts
#   screenshots  Capture screenshots of all 14 portals (requires Node + Playwright)
#
# Examples:
#   ./scripts/deploy.sh 1 up          # Start with Docker (simple)
#   ./scripts/deploy.sh 2 up          # Start with Docker (full)
#   ./scripts/deploy.sh 3 up          # Start on Kubernetes
#   ./scripts/deploy.sh 1 test        # Run API tests (Docker simple)
#   ./scripts/deploy.sh 2 screenshots # Screenshot all portals
# =============================================================================

set -euo pipefail

# ── Colours ───────────────────────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'
BLUE='\033[0;34m'; CYAN='\033[0;36m'; BOLD='\033[1m'; NC='\033[0m'

info()    { echo -e "${CYAN}[INFO]${NC}  $*"; }
ok()      { echo -e "${GREEN}[OK]${NC}    $*"; }
warn()    { echo -e "${YELLOW}[WARN]${NC}  $*"; }
error()   { echo -e "${RED}[ERROR]${NC} $*" >&2; }
section() { echo -e "\n${BOLD}${BLUE}══ $* ══${NC}"; }

# ── Paths ─────────────────────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
SCREENSHOTS_DIR="${ROOT_DIR}/docs/screenshots"

COMPOSE_SIMPLE="${ROOT_DIR}/APIs/v1-Core/docker-compose.yml"
COMPOSE_FULL="${ROOT_DIR}/docker-compose.full.yml"
K8S_DIR="${ROOT_DIR}/kubernetes"

# ── Arguments ─────────────────────────────────────────────────────────────────
OPTION="${1:-}"
COMMAND="${2:-up}"

API_BASE_URL="${API_BASE_URL:-http://localhost:8080}"
FRONTEND_URL="${FRONTEND_URL:-http://localhost:3000}"
DB_CONTAINER="${DB_CONTAINER:-wekeza-postgres}"

# ── Helper: wait for HTTP endpoint ────────────────────────────────────────────
wait_for_url() {
  local url="$1"
  local label="${2:-$url}"
  local max_tries="${3:-60}"
  info "Waiting for ${label} ..."
  for i in $(seq 1 "$max_tries"); do
    if curl -fsS "$url" >/dev/null 2>&1; then
      ok "${label} is up"
      return 0
    fi
    printf "."
    sleep 3
  done
  echo ""
  error "${label} did not become healthy within $((max_tries * 3))s"
  return 1
}

# ── Helper: run DB seed ────────────────────────────────────────────────────────
run_seed() {
  local container="$1"
  local db_user="${2:-wekeza_app}"
  local db_name="${3:-WekezaCoreDB}"
  section "Seeding database"
  for sql_file in \
    "${ROOT_DIR}/scripts/init-db.sql" \
    "${ROOT_DIR}/scripts/seed-banking-data.sql" \
    "${ROOT_DIR}/APIs/v1-Core/scripts/sql/10_setup_all_tables_and_seed_1000.sql" \
    "${ROOT_DIR}/scripts/seed-portal-users.sql"; do
    if [[ -f "$sql_file" ]]; then
      info "Applying $(basename "$sql_file") ..."
      docker cp "$sql_file" "${container}:/tmp/$(basename "$sql_file")"
      docker exec -i "$container" psql -v ON_ERROR_STOP=0 \
        -U "$db_user" -d "$db_name" \
        -f "/tmp/$(basename "$sql_file")" && ok "$(basename "$sql_file") applied" || warn "$(basename "$sql_file") had errors (may be OK if rows already exist)"
    fi
  done
  ok "Database seeding complete"
}

# ── Helper: run e2e tests ─────────────────────────────────────────────────────
run_tests() {
  section "Running end-to-end API tests"
  if [[ -f "${ROOT_DIR}/scripts/e2e-test.sh" ]]; then
    API_BASE_URL="$API_BASE_URL" bash "${ROOT_DIR}/scripts/e2e-test.sh"
  fi
  if [[ -f "${ROOT_DIR}/APIs/v1-Core/scripts/16_all_portal_e2e_regression.py" ]]; then
    info "Running all-portal regression ..."
    API_BASE_URL="$API_BASE_URL" python3 "${ROOT_DIR}/APIs/v1-Core/scripts/16_all_portal_e2e_regression.py" || warn "Some API tests failed — check output above"
  fi
}

# ── Helper: take portal screenshots ───────────────────────────────────────────
take_screenshots() {
  section "Capturing portal screenshots"
  if [[ -f "${ROOT_DIR}/scripts/screenshot-portals.js" ]]; then
    mkdir -p "$SCREENSHOTS_DIR"
    FRONTEND_URL="$FRONTEND_URL" node "${ROOT_DIR}/scripts/screenshot-portals.js" \
      --output "$SCREENSHOTS_DIR" && ok "Screenshots saved to ${SCREENSHOTS_DIR}"
  else
    warn "screenshot-portals.js not found — skipping screenshots"
  fi
}

# =============================================================================
# OPTION 1 — Docker Simple
# =============================================================================
docker_simple() {
  local cmd="$1"
  COMPOSE_FILE="$COMPOSE_SIMPLE"
  section "Docker Deployment (Option 1 — Simple)"
  info "Compose file: ${COMPOSE_FILE}"
  DB_CONTAINER="wekeza-v1-postgres"

  case "$cmd" in
    up)
      info "Building and starting services ..."
      docker compose -f "$COMPOSE_FILE" up -d --build
      wait_for_url "${API_BASE_URL}/health" "Backend API"
      run_seed "$DB_CONTAINER"
      ok "Stack is ready!"
      echo -e "\n${BOLD}Access points:${NC}"
      echo -e "  API:     ${CYAN}${API_BASE_URL}${NC}"
      echo -e "  Swagger: ${CYAN}${API_BASE_URL}/swagger${NC}"
      ;;
    down)  docker compose -f "$COMPOSE_FILE" down ;;
    reset)
      warn "This will WIPE ALL DATA. Ctrl-C to abort ..."
      sleep 3
      docker compose -f "$COMPOSE_FILE" down -v
      docker_simple up
      ;;
    status)  docker compose -f "$COMPOSE_FILE" ps ;;
    logs)    docker compose -f "$COMPOSE_FILE" logs -f ;;
    seed)    run_seed "$DB_CONTAINER" ;;
    test)    run_tests ;;
    screenshots)  take_screenshots ;;
    *)  error "Unknown command: $cmd"; exit 1 ;;
  esac
}

# =============================================================================
# OPTION 2 — Docker Full Stack
# =============================================================================
docker_full() {
  local cmd="$1"
  COMPOSE_FILE="$COMPOSE_FULL"
  section "Docker Deployment (Option 2 — Full Stack with Nginx)"
  info "Compose file: ${COMPOSE_FILE}"
  DB_CONTAINER="wekeza-postgres"
  FRONTEND_URL="${FRONTEND_URL:-http://localhost:3000}"
  NGINX_URL="http://localhost:80"

  case "$cmd" in
    up)
      info "Building and starting full stack ..."
      docker compose -f "$COMPOSE_FILE" up -d --build
      wait_for_url "${API_BASE_URL}/health" "Backend API" 80
      wait_for_url "${FRONTEND_URL}" "Frontend" 30
      run_seed "$DB_CONTAINER"
      ok "Full stack is ready!"
      echo -e "\n${BOLD}Access points:${NC}"
      echo -e "  Frontend  : ${CYAN}${FRONTEND_URL}${NC}"
      echo -e "  API       : ${CYAN}${API_BASE_URL}${NC}"
      echo -e "  Swagger   : ${CYAN}${API_BASE_URL}/swagger${NC}"
      echo -e "  Nginx     : ${CYAN}${NGINX_URL}${NC}  (routes / → frontend, /api → backend)"
      echo -e "  pgAdmin   : ${YELLOW}(not started — run with --profile tools)${NC}"
      echo -e "\nTo start pgAdmin: docker compose -f docker-compose.full.yml --profile tools up -d pgadmin"
      ;;
    down)  docker compose -f "$COMPOSE_FILE" down ;;
    reset)
      warn "This will WIPE ALL DATA. Ctrl-C to abort ..."
      sleep 3
      docker compose -f "$COMPOSE_FILE" down -v
      docker_full up
      ;;
    status)  docker compose -f "$COMPOSE_FILE" ps ;;
    logs)    docker compose -f "$COMPOSE_FILE" logs -f ;;
    seed)    run_seed "$DB_CONTAINER" ;;
    test)    run_tests ;;
    screenshots)  take_screenshots ;;
    *)  error "Unknown command: $cmd"; exit 1 ;;
  esac
}

# =============================================================================
# OPTION 3 — Kubernetes (Minikube / kind)
# =============================================================================
kubernetes() {
  local cmd="$1"
  section "Kubernetes Deployment (Option 3)"

  # Check prerequisites
  if ! command -v kubectl &>/dev/null; then
    error "kubectl not found. Install: https://kubernetes.io/docs/tasks/tools/"
    exit 1
  fi

  case "$cmd" in
    up)
      info "Checking for Minikube / kind cluster ..."
      if command -v minikube &>/dev/null && minikube status &>/dev/null; then
        info "Using existing Minikube cluster"
        MINIKUBE_IP="$(minikube ip 2>/dev/null || echo "")"
        if [[ -z "$MINIKUBE_IP" ]]; then
          error "minikube ip returned empty. Is Minikube running? Try: minikube start"
          exit 1
        fi
        API_BASE_URL="http://${MINIKUBE_IP}:30088"
        FRONTEND_URL="http://${MINIKUBE_IP}:30080"
      else
        warn "No running Minikube detected. If using kind or a remote cluster, ensure kubectl context is set."
      fi

      info "Building Docker images ..."
      docker build -t wekeza-api:local "${ROOT_DIR}/APIs/v1-Core"
      docker build -t wekeza-frontend:local "${ROOT_DIR}/Portals/wekeza-unified-shell"

      if command -v minikube &>/dev/null && minikube status &>/dev/null; then
        info "Loading images into Minikube ..."
        minikube image load wekeza-api:local
        minikube image load wekeza-frontend:local
      fi

      info "Applying Kubernetes manifests ..."
      kubectl apply -f "${K8S_DIR}/00-namespace.yml"
      kubectl apply -f "${K8S_DIR}/01-configmap.yml"
      kubectl apply -f "${K8S_DIR}/02-secrets.yml"
      kubectl apply -f "${K8S_DIR}/03-postgres.yml"
      kubectl apply -f "${K8S_DIR}/04-redis.yml"

      info "Waiting for postgres and redis to be ready ..."
      kubectl rollout status deployment/postgres -n wekeza-bank --timeout=120s
      kubectl rollout status deployment/redis    -n wekeza-bank --timeout=60s

      kubectl apply -f "${K8S_DIR}/05-api.yml"
      kubectl apply -f "${K8S_DIR}/06-frontend.yml"
      kubectl apply -f "${K8S_DIR}/07-ingress.yml"

      info "Waiting for API and frontend to be ready ..."
      kubectl rollout status deployment/wekeza-api      -n wekeza-bank --timeout=180s
      kubectl rollout status deployment/wekeza-frontend -n wekeza-bank --timeout=120s

      ok "Kubernetes deployment complete!"
      echo -e "\n${BOLD}Access points:${NC}"
      if command -v minikube &>/dev/null; then
        echo -e "  Frontend  : ${CYAN}${FRONTEND_URL}${NC}"
        echo -e "  API       : ${CYAN}${API_BASE_URL}${NC}"
        echo -e "  Swagger   : ${CYAN}${API_BASE_URL}/swagger${NC}"
        echo -e "\nOr use port-forwarding:"
      fi
      echo -e "  kubectl port-forward svc/wekeza-api-service      8080:8080 -n wekeza-bank &"
      echo -e "  kubectl port-forward svc/wekeza-frontend-service 3000:3000 -n wekeza-bank &"
      ;;
    down)
      warn "Removing all Wekeza Kubernetes resources ..."
      kubectl delete namespace wekeza-bank --ignore-not-found=true
      ok "Namespace wekeza-bank deleted"
      ;;
    status)
      kubectl get all -n wekeza-bank
      ;;
    logs)
      kubectl logs -l app=wekeza-api -n wekeza-bank -f --all-containers --max-log-requests=5
      ;;
    test)
      info "Port-forwarding API for tests ..."
      kubectl port-forward svc/wekeza-api-service 8080:8080 -n wekeza-bank &
      PF_PID=$!
      sleep 5
      API_BASE_URL="http://localhost:8080" run_tests
      kill "$PF_PID" 2>/dev/null || true
      ;;
    *)  error "Unknown command: $cmd"; exit 1 ;;
  esac
}

# =============================================================================
# Main dispatcher
# =============================================================================
print_usage() {
  echo -e "${BOLD}Usage:${NC} $0 [1|2|3] [up|down|reset|status|logs|test|seed|screenshots]"
  echo ""
  echo -e "  ${BOLD}1${NC}  Docker — simple (API + DB only)"
  echo -e "  ${BOLD}2${NC}  Docker — full stack (API + DB + Redis + Frontend + Nginx)"
  echo -e "  ${BOLD}3${NC}  Kubernetes — Minikube / kind / remote cluster"
  echo ""
  echo -e "  ${BOLD}up${NC}           Build images and start all services"
  echo -e "  ${BOLD}down${NC}         Stop containers (keep volumes)"
  echo -e "  ${BOLD}reset${NC}        Wipe all data and restart from scratch"
  echo -e "  ${BOLD}status${NC}       Show running containers / pods"
  echo -e "  ${BOLD}logs${NC}         Tail container logs"
  echo -e "  ${BOLD}test${NC}         Run end-to-end API regression tests"
  echo -e "  ${BOLD}seed${NC}         Re-apply database seed scripts"
  echo -e "  ${BOLD}screenshots${NC}  Capture screenshots of all 14 portals"
}

case "$OPTION" in
  1)  docker_simple "$COMMAND" ;;
  2)  docker_full   "$COMMAND" ;;
  3)  kubernetes    "$COMMAND" ;;
  -h|--help|help|"")  print_usage ;;
  *)  error "Unknown option: $OPTION"; print_usage; exit 1 ;;
esac
