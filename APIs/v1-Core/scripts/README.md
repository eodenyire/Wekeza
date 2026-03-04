# v1-Core Reproducible Test Scripts

This folder captures the exact categories of scripts used during portal verification:

- Docker commands (bring up/tear down/check health)
- SQL scripts (user/account/data sanity checks)
- Python smoke tests (role login + endpoint checks)

## Script Index

- `01_start_stack.sh` - starts postgres/redis/api and waits for `/health`
- `02_smoke_health.sh` - basic container + API health verification
- `03_run_sql_checks.sh` - runs SQL checks and resolves an active account number
- `04_portal_smoke_tests.py` - runs admin/manager/teller login + endpoint smoke matrix
- `05_run_all_tests.sh` - orchestrates all steps above
- `06_setup_database_1000_records.sh` - heavy database bootstrap + seeds >=1000 records for core entities
- `07_verify_database_1000.sh` - verifies 1000-record threshold and prints all table names
- `08_export_table_structures.sh` - exports full schema/column/constraint structure for all tables
- `09_seed_all_tables.sh` - runs baseline seeding and prints exact row counts for all existing tables
- `10_daily_realtime_seed.py` - Python realtime daily seeder (new branch/staff/customer/account/transaction activity)
- `11_setup_daily_seed_cron.sh` - installs a cron schedule for daily Python realtime seeding
- `12_backend_frontend_regression.py` - broad backend API regression from frontend-facing endpoints
- `13_run_backend_frontend_regression.sh` - wrapper to run full regression setup + execution
- `14_frontend_form_matrix.py` - targeted form/API payload matrix checks
- `15_vif_customer_journey.py` - end-to-end VIF assisted customer journey (CIF, account, transactions, services)
- `99_stop_stack.sh` - stops the v1-Core stack
- `sql/01_list_test_users.sql`
- `sql/00_seed_test_users_legacy.sql`
- `sql/02_get_active_account.sql`
- `sql/03_portal_data_sanity.sql`
- `sql/10_setup_all_tables_and_seed_1000.sql`
- `sql/11_verify_core_counts.sql`
- `sql/12_describe_all_tables.sql`
- `sql/13_all_table_row_counts.sql`
- `sql/14_daily_realtime_seed.sql`

## Quick Start

From `APIs/v1-Core`:

```bash
chmod +x scripts/*.sh
./scripts/05_run_all_tests.sh
```

## Heavy Database Setup (1000 Records)

From `APIs/v1-Core`:

```bash
chmod +x scripts/*.sh
./scripts/06_setup_database_1000_records.sh
```

This script ensures core starter data exists with at least 1000 rows each for:

- `Branches`
- `Users` (staff)
- `Customers`
- `Accounts`
- `Balances`
- `Transactions`

Run verification only:

```bash
./scripts/07_verify_database_1000.sh
```

## All-Table Structure + Counts

```bash
./scripts/08_export_table_structures.sh
./scripts/09_seed_all_tables.sh
```

The structure export writes timestamped reports into `scripts/reports/`.

## Daily Realtime Seeding

Run once manually:

```bash
/workspaces/Wekeza/.venv/bin/python scripts/10_daily_realtime_seed.py
```

Install daily cron job (default `00:05` every day):

```bash
./scripts/11_setup_daily_seed_cron.sh
```

Customize schedule and python path:

```bash
CRON_SCHEDULE="15 1 * * *" PYTHON_BIN="/workspaces/Wekeza/.venv/bin/python" ./scripts/11_setup_daily_seed_cron.sh
```

## Full Backend/Frontend Regression

```bash
./scripts/13_run_backend_frontend_regression.sh
/workspaces/Wekeza/.venv/bin/python scripts/14_frontend_form_matrix.py

# VIF assisted-customer journey
python3 scripts/15_vif_customer_journey.py
```

## Default Credentials Used by Smoke Script

- Admin: `admin / Admin@123`
- Manager: `manager1 / Manager@123`
- Teller: `teller1 / Teller@123`

You can override with environment variables:

```bash
export ADMIN_USERNAME=admin
export ADMIN_PASSWORD=Admin@123
export MANAGER_USERNAME=manager1
export MANAGER_PASSWORD=Manager@123
export TELLER_USERNAME=teller1
export TELLER_PASSWORD=Teller@123
```

## Environment Variables

- `API_BASE_URL` (default `http://localhost:8080`)
- `DB_CONTAINER` (default `wekeza-v1-postgres`)
- `DB_USER` (default `wekeza_app`)
- `DB_NAME` (default `WekezaCoreDB`)
- `ACTIVE_ACCOUNT_NUMBER` (auto-resolved by SQL checks; can be manually set)

## Notes

- `03_run_sql_checks.sh` writes `.last_active_account.txt` and `05_run_all_tests.sh` reuses it.
- The smoke checks validate status codes and minimal API behavior for portal readiness.
- If running in an environment with existing services, ensure ports `8080`, `5432`, and `6379` are available.
