# Commands Used Backup

Date: 2026-03-04  
Scope: v1-Core stack bring-up, validation, seeding, regression, and VIF journey execution.

## Core Stack and Build

```bash
cd /workspaces/Wekeza/APIs/v1-Core
docker compose up -d postgres redis
docker compose up -d --build api
docker compose ps
docker compose down
```

## Health and Auth Smoke

```bash
curl -fsS http://localhost:8080/health
curl -s -o /dev/null -w "%{http_code}" http://localhost:8080/api/authentication/me
```

## Database SQL Execution and Inspection

```bash
docker cp scripts/sql/01_list_test_users.sql wekeza-v1-postgres:/tmp/01_list_test_users.sql
docker cp scripts/sql/02_get_active_account.sql wekeza-v1-postgres:/tmp/02_get_active_account.sql
docker cp scripts/sql/03_portal_data_sanity.sql wekeza-v1-postgres:/tmp/03_portal_data_sanity.sql

docker exec -i wekeza-v1-postgres psql -U wekeza_app -d WekezaCoreDB -f /tmp/01_list_test_users.sql
docker exec -i wekeza-v1-postgres psql -U wekeza_app -d WekezaCoreDB -f /tmp/02_get_active_account.sql
docker exec -i wekeza-v1-postgres psql -U wekeza_app -d WekezaCoreDB -f /tmp/03_portal_data_sanity.sql
```

## Scripted Test and Regression Commands

```bash
chmod +x scripts/*.sh
./scripts/01_start_stack.sh
./scripts/02_smoke_health.sh
./scripts/03_run_sql_checks.sh
./scripts/05_run_all_tests.sh

./scripts/06_setup_database_1000_records.sh
./scripts/07_verify_database_1000.sh
./scripts/08_export_table_structures.sh
./scripts/09_seed_all_tables.sh

/workspaces/Wekeza/.venv/bin/python scripts/10_daily_realtime_seed.py
./scripts/11_setup_daily_seed_cron.sh

./scripts/13_run_backend_frontend_regression.sh
/workspaces/Wekeza/.venv/bin/python scripts/14_frontend_form_matrix.py
python3 scripts/15_vif_customer_journey.py

./scripts/99_stop_stack.sh
```

## Direct VIF Validation Command

```bash
cd /workspaces/Wekeza/APIs/v1-Core
python3 scripts/15_vif_customer_journey.py
```

## Optional Environment Overrides Used by Scripts

```bash
export API_BASE_URL=http://localhost:8080
export DB_CONTAINER=wekeza-v1-postgres
export DB_USER=wekeza_app
export DB_NAME=WekezaCoreDB
export ACTIVE_ACCOUNT_NUMBER=ACC0000055583
```

## Frontend (Unified Shell) Verification Commands

```bash
cd /workspaces/Wekeza/Portals/wekeza-unified-shell
npm install
npm run build
# or
npm run typecheck
```
