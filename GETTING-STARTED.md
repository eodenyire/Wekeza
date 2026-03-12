# Wekeza Bank — Getting Started

Everything you need to run the full Wekeza Core Banking Platform on your own machine.

## Prerequisites

| Tool | Version | Install |
|------|---------|---------|
| Docker Desktop | 24+ | https://docs.docker.com/get-docker/ |
| Docker Compose | v2 (built in) | included with Docker Desktop |
| Git | any | https://git-scm.com/ |

> **No other tools required.** Node, .NET, and PostgreSQL all run inside Docker containers.

---

## Quick Start (3 commands)

```bash
# 1. Clone
git clone https://github.com/eodenyire/Wekeza.git
cd Wekeza

# 2. Start the full stack (builds images on first run — ~5 min)
docker compose -f docker-compose.full.yml up -d

# 3. Open portals
open http://localhost        # macOS
xdg-open http://localhost    # Linux
start http://localhost       # Windows
```

Wait ~30 seconds for all services to become healthy, then log in with any portal credential below.

---

## What Gets Started

| Container | Service | Port | Purpose |
|-----------|---------|------|---------|
| `wekeza-postgres` | PostgreSQL 15 | 5432 | Primary database |
| `wekeza-redis` | Redis 7 | 6379 | Session cache |
| `wekeza-api` | .NET 10 API | 8080 (internal) | Core Banking REST API |
| `wekeza-frontend` | nginx (React SPA) | 3000 (internal) | Unified Portal Shell |
| `wekeza-nginx` | nginx reverse proxy | **80** | Routes `/api/` → API, `/` → Frontend |

---

## Portal Credentials

All portal users share password: **`Admin@123`**

| Username | Role | Portal Access |
|----------|------|---------------|
| `admin` | SystemAdministrator | Enterprise Administration, Workflow |
| `manager1` | BranchManager | Branch Manager, Branch Operations, Teller, Supervisor, Staff Self-Service, Workflow |
| `teller1` | Teller | Teller Portal, Staff Self-Service |
| `supervisor1` | Supervisor | Supervisor, Teller, Workflow, Staff Self-Service |
| `compliance1` | ComplianceManager | Compliance & Risk |
| `treasury1` | TreasuryDealer | Treasury & Markets |
| `tradeFinance1` | TradeFinanceOfficer | Trade Finance |
| `payments1` | PaymentsOfficer | Payments & Clearing |
| `productGL1` | ProductManager | Product & GL |
| `vaultOfficer1` | VaultOfficer | Branch Operations |
| `executive1` | CEO | Executive & Board |
| `customer1` | RetailCustomer | Customer Digital Portal |

---

## Seeded Data

After the first `docker compose up`, the database is automatically populated:

| Table | Rows | Details |
|-------|------|---------|
| Users | 12 | All 12 portal users (see above) |
| Customers | 20 | `Alice Customer` (linked to `customer1`), plus 19 others |
| Accounts | 28 | Savings, Current, Business, Fixed Deposit accounts |
| Transactions | 28 | Deposits, Withdrawals, Transfers, Payments from today |
| Branches | 5 | Nairobi HQ, CBD, Kisumu, Mombasa, Nakuru |

The seed runs from these scripts (in order):
1. `scripts/init-db.sql` — extensions + audit schema
2. `scripts/seed-banking-data.sql` — core tables (Users, Customers, Accounts, Transactions)
3. `scripts/seed-comprehensive-data.sql` — full demo dataset

---

## Verify Health

```bash
# Check all containers are healthy
docker compose -f docker-compose.full.yml ps

# Test the API
curl http://localhost/api/health

# Check database row counts
docker exec wekeza-postgres psql -U wekeza_app -d WekezaCoreDB -c "
SELECT 'Users' AS t, COUNT(*) FROM \"Users\"
UNION ALL SELECT 'Customers', COUNT(*) FROM \"Customers\"
UNION ALL SELECT 'Accounts', COUNT(*) FROM \"Accounts\"
UNION ALL SELECT 'Transactions', COUNT(*) FROM \"Transactions\"
UNION ALL SELECT 'Branches', COUNT(*) FROM \"Branches\";"
```

---

## Stopping and Restarting

```bash
# Stop (preserves data volumes)
docker compose -f docker-compose.full.yml down

# Stop and wipe all data (fresh start)
docker compose -f docker-compose.full.yml down -v

# Restart
docker compose -f docker-compose.full.yml up -d
```

---

## Re-running Seed Data

```bash
# Seed comprehensive demo data manually
docker exec -i wekeza-postgres psql -U wekeza_app -d WekezaCoreDB \
    < scripts/seed-comprehensive-data.sql
```

---

## Building Images from Source

```bash
# Build all images
docker compose -f docker-compose.full.yml build

# Or use the build helper script
chmod +x scripts/docker/build-and-export.sh
./scripts/docker/build-and-export.sh

# Build and export as .tar.gz for offline distribution
./scripts/docker/build-and-export.sh --export

# Build and push to GitHub Container Registry
echo $GITHUB_TOKEN | docker login ghcr.io -u <your-github-username> --password-stdin
./scripts/docker/build-and-export.sh --push --tag v1.0.0
```

### Loading Exported Images (Offline)

```bash
# If you received the .tar.gz exports
for f in docker-exports/*.tar.gz; do docker load -i "$f"; done
docker compose -f docker-compose.full.yml up -d
```

---

## Architecture

```
Browser
  │
  ▼
nginx (port 80)
  ├─ /api/* ──────────► .NET 10 API (port 8080)
  │                          │
  │                      PostgreSQL (port 5432)
  │                      Redis (port 6379)
  │
  └─ /* ─────────────► React SPA / nginx (port 3000)
```

---

## Docker Images

| Image | Base | Size (approx) |
|-------|------|---------------|
| `wekeza-api:local` | `mcr.microsoft.com/dotnet/aspnet:10.0` | ~350 MB |
| `wekeza-frontend:local` | `nginx:1.25-alpine` | ~30 MB |

---

## File Layout

```
Wekeza/
├── docker-compose.full.yml          ← PRIMARY — use this to start everything
├── docker-compose.yml               ← Development variant
├── GETTING-STARTED.md               ← This file
│
├── APIs/v1-Core/                    ← .NET 10 Core Banking API
│   ├── Dockerfile
│   └── Wekeza.Core.Api/
│       └── Controllers/             ← Authentication, Teller, BranchManager, CustomerPortal …
│
├── Portals/wekeza-unified-shell/    ← React 18 + Vite + Ant Design frontend
│   ├── Dockerfile
│   └── src/portals/                 ← teller/, branch-manager/, customer/, admin/, …
│
├── scripts/
│   ├── init-db.sql                  ← DB init (extensions, audit schema)
│   ├── seed-banking-data.sql        ← Core tables DDL + seed
│   ├── seed-portal-users.sql        ← 12 portal users (BCrypt hashes)
│   ├── seed-comprehensive-data.sql  ← Full demo dataset (1000 rows)
│   ├── database/
│   │   └── full-schema-ddl.sql      ← Complete schema reference
│   └── docker/
│       └── build-and-export.sh      ← Build, tag, push, export images
│
└── nginx/
    └── nginx.conf                   ← Reverse proxy config
```

---

## Troubleshooting

| Symptom | Fix |
|---------|-----|
| Login fails | Ensure `wekeza-api` is healthy: `docker compose -f docker-compose.full.yml ps` |
| Portal shows zeros | The API may be starting — wait 30 s and refresh |
| Database empty | Re-run: `docker exec -i wekeza-postgres psql -U wekeza_app -d WekezaCoreDB < scripts/seed-comprehensive-data.sql` |
| Port 80 in use | Stop other web servers or change port in `docker-compose.full.yml` |
| Image build fails | Ensure Docker Desktop has internet access for pulling base images |

---

*Wekeza Bank Tier-1 Core Banking Platform — © 2026 eodenyire*
