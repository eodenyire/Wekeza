# Release Notes — 2026-03-04

## Release Summary

- **Commit:** `ed1e3b2c0bf1ebe0f3784f0f82d38b1e8313d796`
- **Branch:** `main`
- **Title:** Add VIF journey APIs, portal wiring, and scripts/sql backups
- **Status:** Pushed to GitHub (`origin/main`)

## Key Outcomes

- Delivered a full **VIF-assisted customer journey** backend flow:
  - Customer registration + CIF generation
  - Account registration
  - Assisted teller operations (deposit, withdrawal, transfer, airtime, M-Pesa, cheque deposit)
  - Investment actions (shares and treasury purchases)
  - Service requests (ATM card lock, mobile loan block)
  - Statement + balance retrieval
- Added/updated **portal controllers** for compliance, payments, product/GL, trade finance, and treasury.
- Wired unified shell portal pages to the new API patterns and introduced VIF teller journey panel.
- Added resilience/compatibility paths for teller cash operations where mediator-based flow may fail.

## Frontend Scope

- Updated portal route pages and live data integrations for:
  - Admin dashboard
  - Compliance portal
  - Payments portal
  - Product/GL portal
  - Trade finance portal
  - Treasury portal
  - Teller portal (including VIF assisted journey panel)

## Scripts, SQL, and Operational Backups

- Consolidated reproducible scripts under `APIs/v1-Core/scripts`:
  - stack start/stop, health checks, SQL checks, smoke tests, regressions, DB setup/seeding, daily seed automation
- Added SQL assets under `APIs/v1-Core/scripts/sql`:
  - user/account sanity checks
  - heavy 1000-record bootstrap
  - table structure/count reports
  - daily realtime seed SQL
- Added command log backup:
  - `APIs/v1-Core/scripts/COMMANDS-USED-BACKUP.md`
- Added timestamped script+SQL archive backup:
  - `APIs/v1-Core/scripts/backups/scripts_sql_backup_20260304_124126.tar.gz`

## Validation Notes

- End-to-end VIF script completed successfully in prior run:
  - `scripts/15_vif_customer_journey.py`
- Commit includes full backend + frontend + scripts/sql backup payload in one release set.

## Affected Areas

- API controllers and persistence configuration
- Unified shell portal pages and teller services/components
- DevOps/testing scripts and SQL data tooling

## Recommended Next Step

- Run one post-release smoke cycle in target environment:
  - `./scripts/05_run_all_tests.sh`
  - `python3 scripts/15_vif_customer_journey.py`
