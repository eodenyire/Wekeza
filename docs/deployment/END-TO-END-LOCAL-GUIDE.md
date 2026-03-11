# Wekeza Banking System — End-to-End Local Deployment Guide

> **All 14 portals verified ✅ | Build: 3,944 modules | Screenshots: [docs/screenshots/](screenshots/index.html)**

This guide covers three deployment strategies for the Wekeza Core Banking System on a local machine:

| Option | Technology | Use case |
|---|---|---|
| **1** | Docker Compose — simple (API + DB) | Back-end development, quick API testing |
| **2** | Docker Compose — full stack (all services + frontend + Nginx) | Full system local testing, UAT, demo |
| **3** | Kubernetes (Minikube / kind) | Production-parity testing, k8s validation |

---

## Prerequisites

| Tool | Version | Install |
|---|---|---|
| Docker | 24+ | https://docs.docker.com/get-docker/ |
| Docker Compose | v2 (plugin) | bundled with Docker Desktop |
| Node.js | 18+ | https://nodejs.org |
| kubectl | 1.28+ | https://kubernetes.io/docs/tasks/tools/ |
| Minikube _(Option 3 only)_ | 1.32+ | https://minikube.sigs.k8s.io/docs/start/ |

---

## Quick start — master script

```bash
# Clone and enter the repository
git clone https://github.com/eodenyire/Wekeza.git
cd Wekeza

# Option 1 — Docker simple (API + DB + Redis only)
./scripts/deploy.sh 1 up

# Option 2 — Docker full stack (+ Frontend + Nginx)
./scripts/deploy.sh 2 up

# Option 3 — Kubernetes
./scripts/deploy.sh 3 up
```

The script handles: image builds → container startup → health-waiting → database seeding → printing access URLs.

---

## Option 1: Docker — Simple

### What runs
| Container | Image | Port | Description |
|---|---|---|---|
| `wekeza-v1-postgres` | postgres:15-alpine | 5432 | PostgreSQL 15 database |
| `wekeza-v1-redis` | redis:7-alpine | 6379 | Redis cache |
| `wekeza-v1-api` | Built locally | 8080 | .NET 8 Core Banking API |

### Step-by-step

```bash
# 1. Navigate to the API directory
cd APIs/v1-Core

# 2. Copy the environment file
cp ../../.env.example .env
# Edit .env if you need custom credentials (defaults work out of the box)

# 3. Start the stack
docker compose up -d --build

# 4. Verify health
curl http://localhost:8080/health

# 5. Open Swagger UI
open http://localhost:8080/swagger
```

### Database setup & seeding

```bash
# The PostgreSQL container auto-runs init-db.sql on first start.
# To run the full 1000-record seed manually:
cd APIs/v1-Core
./scripts/09_seed_all_tables.sh

# Or via the master script:
./scripts/deploy.sh 1 seed
```

### Running all API tests

```bash
# Quick smoke tests
./APIs/v1-Core/scripts/05_run_all_tests.sh

# Full 14-portal regression
API_BASE_URL=http://localhost:8080 python3 APIs/v1-Core/scripts/16_all_portal_e2e_regression.py

# Or via the master script:
./scripts/deploy.sh 1 test
```

---

## Option 2: Docker — Full Stack

### What runs
| Container | Image | Port | Description |
|---|---|---|---|
| `wekeza-postgres` | postgres:15-alpine | 5432 | PostgreSQL 15 |
| `wekeza-redis` | redis:7-alpine | 6379 | Redis 7 |
| `wekeza-api` | Built locally | 8080 | .NET 8 API |
| `wekeza-frontend` | Built locally | 3000 | React 18 SPA |
| `wekeza-nginx` | nginx:1.25-alpine | 80 | Reverse proxy |

### Step-by-step

```bash
# 1. From the repository root, copy environment file
cp .env.example .env

# 2. (Optional) Set frontend auth mode — 'mock' skips the backend for UI testing
#    For real API integration use 'real'
echo "VITE_AUTH_MODE=real" >> .env

# 3. Build and start everything
docker compose -f docker-compose.full.yml up -d --build

# 4. Wait for all services (script does this automatically)
./scripts/deploy.sh 2 up

# 5. Access points
#    Frontend  → http://localhost:3000
#    API       → http://localhost:8080
#    Swagger   → http://localhost:8080/swagger
#    Nginx     → http://localhost:80  (routes /api → backend, / → frontend)
```

### Optional: start pgAdmin database manager

```bash
docker compose -f docker-compose.full.yml --profile tools up -d pgadmin
# Then open: http://localhost:5050
# Email: admin@wekeza.com  |  Password: admin123
# The Wekeza server is pre-configured — just click on it
```

### Capture portal screenshots (after stack is running)

```bash
# Requires: npm install playwright && npx playwright install chromium
npm install playwright
npx playwright install chromium

FRONTEND_URL=http://localhost:3000 node scripts/screenshot-portals.js
# Screenshots saved to: docs/screenshots/
```

---

## Option 3: Kubernetes

### Cluster options

| Cluster | Setup command |
|---|---|
| Minikube (recommended) | `minikube start --memory=4096 --cpus=4` |
| kind | `kind create cluster --name wekeza` |
| Remote k8s | Set `kubectl config use-context <your-context>` |

### Step-by-step

```bash
# 1. Start Minikube
minikube start --memory=4096 --cpus=4

# 2. Enable ingress addon (for host-based routing)
minikube addons enable ingress

# 3. Deploy everything via master script
./scripts/deploy.sh 3 up

# 4. Get Minikube IP
MINIKUBE_IP=$(minikube ip)

# 5. Access via NodePort
#    Frontend  → http://$MINIKUBE_IP:30080
#    API       → http://$MINIKUBE_IP:30088
#    Swagger   → http://$MINIKUBE_IP:30088/swagger

# Or use port-forwarding
kubectl port-forward svc/wekeza-api-service      8080:8080 -n wekeza-bank &
kubectl port-forward svc/wekeza-frontend-service 3000:3000 -n wekeza-bank &
```

### Optional: Hosts file for ingress

```bash
# Add to /etc/hosts for browser access via hostname
echo "$(minikube ip)  wekeza.local api.wekeza.local" | sudo tee -a /etc/hosts

# Then access:
#   http://wekeza.local        → Frontend
#   http://api.wekeza.local    → API + Swagger
```

### Kubernetes resource overview

```bash
# Show all deployed resources
kubectl get all -n wekeza-bank

# Watch rollout
kubectl rollout status deployment/wekeza-api -n wekeza-bank

# Pod logs
kubectl logs -l app=wekeza-api -n wekeza-bank --tail=50 -f

# Run tests against k8s
./scripts/deploy.sh 3 test
```

### Kubernetes manifests reference

| File | Purpose |
|---|---|
| `kubernetes/00-namespace.yml` | `wekeza-bank` namespace |
| `kubernetes/01-configmap.yml` | Non-sensitive env variables |
| `kubernetes/02-secrets.yml` | DB password, JWT secret (base64 encoded) |
| `kubernetes/03-postgres.yml` | PostgreSQL PVC + Deployment + Service |
| `kubernetes/04-redis.yml` | Redis PVC + Deployment + Service |
| `kubernetes/05-api.yml` | Backend API Deployment + Service + HPA |
| `kubernetes/06-frontend.yml` | Frontend Deployment + Service |
| `kubernetes/07-ingress.yml` | Nginx Ingress + NodePort fallback services |

> **Security note**: Update the base64-encoded values in `kubernetes/02-secrets.yml` before any non-local deployment. Generate real secrets with `echo -n 'my-secret' | base64`.

---

## Database Setup Details

### Schema initialization

When PostgreSQL starts for the first time, `scripts/init-db.sql` runs automatically. It:
- Creates extensions: `uuid-ossp`, `pg_trgm`
- Creates schemas: `public`, `audit`, `reporting`
- Sets up the audit log table

### Data seeding

`scripts/seed-banking-data.sql` seeds:
- Users table with the 12 portal test accounts
- Customers, Accounts, Transactions (sample data)

`APIs/v1-Core/scripts/sql/10_setup_all_tables_and_seed_1000.sql` seeds:
- **1,000 records** in each core table: Branches, Users, Customers, Accounts, Transactions

### Manual database access

```bash
# Option 1 / 2 — via Docker
docker exec -it wekeza-postgres psql -U wekeza_app -d WekezaCoreDB

# Or via docker compose
docker compose -f docker-compose.full.yml exec postgres psql -U wekeza_app -d WekezaCoreDB

# Kubernetes — via port-forward
kubectl port-forward svc/postgres-service 5432:5432 -n wekeza-bank &
psql -h localhost -U wekeza_app -d WekezaCoreDB

# Useful queries
\dt                          -- list all tables
SELECT count(*) FROM "Users";
SELECT count(*) FROM "Accounts";
SELECT count(*) FROM "Transactions";
```

---

## Portal Test Credentials

All portals require a logged-in user. Use these credentials:

| Portal | Username | Password | Role |
|---|---|---|---|
| Enterprise Admin | `admin` | `Admin@123` | SystemAdministrator |
| Executive & Board | `executive1` | `Executive@123` | CEO |
| Branch Manager | `manager1` | `Manager@123` | BranchManager |
| Branch Operations | `vaultOfficer1` | `Vault@123` | VaultOfficer |
| Teller | `teller1` | `Teller@123` | Teller |
| Supervisor | `supervisor1` | `Supervisor@123` | Supervisor |
| Compliance & Risk | `compliance1` | `Compliance@123` | ComplianceOfficer |
| Treasury & Markets | `treasury1` | `Treasury@123` | TreasuryDealer |
| Trade Finance | `tradeFinance1` | `Trade@123` | TradeFinanceOfficer |
| Product & GL | `productGL1` | `Product@123` | ProductManager |
| Payments & Clearing | `payments1` | `Payments@123` | PaymentsOfficer |
| Customer Digital | `customer1` | `Customer@123` | RetailCustomer |
| Staff Self-Service | `teller1` | `Teller@123` | Teller |
| Workflow & Task | `manager1` | `Manager@123` | BranchManager |

---

## End-to-End Testing

### API regression (all endpoints)

```bash
# Run from any environment — just set API_BASE_URL
API_BASE_URL=http://localhost:8080 ./scripts/e2e-test.sh

# For Kubernetes (port-forward first)
kubectl port-forward svc/wekeza-api-service 8080:8080 -n wekeza-bank &
API_BASE_URL=http://localhost:8080 ./scripts/e2e-test.sh
```

### Full portal regression (Python)

```bash
# Tests authentication + all portal endpoints for all 14 portals
API_BASE_URL=http://localhost:8080 python3 APIs/v1-Core/scripts/16_all_portal_e2e_regression.py
```

### Portal screenshots

```bash
# Capture all 14 portals (requires running frontend + Node.js + Playwright)
npm install playwright && npx playwright install chromium
FRONTEND_URL=http://localhost:3000 node scripts/screenshot-portals.js
# → docs/screenshots/index.html (gallery)
```

---

## Troubleshooting

### Container won't start

```bash
# Check logs
docker compose -f docker-compose.full.yml logs api
docker compose -f docker-compose.full.yml logs postgres

# Check health
docker compose -f docker-compose.full.yml ps

# Port already in use?
lsof -i :8080   # or :5432, :3000, :6379
```

### Database connection issues

```bash
# Verify postgres is healthy
docker compose -f docker-compose.full.yml exec postgres pg_isready -U wekeza_app

# Check container network
docker network inspect wekeza-net

# Test connection from API container
docker compose -f docker-compose.full.yml exec api wget -qO- http://localhost:8080/health
```

### Frontend not loading

```bash
# Check frontend build
cd Portals/wekeza-unified-shell && npm run build

# Check container
docker compose -f docker-compose.full.yml logs frontend

# Test directly
curl http://localhost:3000/
```

### Full reset (wipe all data)

```bash
# Docker
./scripts/deploy.sh 2 reset   # or: 1 reset

# Kubernetes
./scripts/deploy.sh 3 down
./scripts/deploy.sh 3 up
```

### Kubernetes pod stuck in Pending

```bash
# Check events
kubectl describe pod -l app=wekeza-api -n wekeza-bank

# Resource constraints?
kubectl top nodes

# Image not found?
minikube image load wekeza-api:local
```

---

## Deployment Files Reference

```
Wekeza/
├── docker-compose.full.yml          ← Option 2: Full local stack
├── APIs/v1-Core/docker-compose.yml  ← Option 1: API + DB only
├── kubernetes/                      ← Option 3: All K8s manifests
│   ├── 00-namespace.yml
│   ├── 01-configmap.yml
│   ├── 02-secrets.yml
│   ├── 03-postgres.yml
│   ├── 04-redis.yml
│   ├── 05-api.yml
│   ├── 06-frontend.yml
│   └── 07-ingress.yml
├── nginx/
│   ├── nginx.conf                   ← Nginx reverse proxy config
│   └── pgadmin-servers.json         ← pgAdmin auto-connect config
├── scripts/
│   ├── deploy.sh                    ← Master deployment script
│   ├── e2e-test.sh                  ← End-to-end API test script
│   ├── screenshot-portals.js        ← Portal screenshot script (real auth)
│   ├── screenshot-portals-local.js  ← Portal screenshot script (mock auth)
│   ├── init-db.sql                  ← Database initialization
│   └── seed-banking-data.sql        ← Sample data seeding
└── docs/screenshots/                ← Captured portal screenshots
    └── index.html                   ← Screenshot gallery
```
