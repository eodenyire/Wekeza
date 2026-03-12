# Wekeza Scripts

This directory contains all operational scripts organized by purpose.

## Directory Structure

### 📁 `/scripts/api`
Scripts for managing and testing APIs.

**Files:**
- `start-all-apis.bat` - Start all APIs (Windows batch)
- `start-all-apis.ps1` - Start all APIs (PowerShell)
- `test-comprehensive-api.ps1` - Comprehensive API testing

**Usage:**
```bash
# Start all APIs
./start-all-apis.ps1

# Test APIs
./test-comprehensive-api.ps1
```

### 📁 `/scripts/core`
Core system startup and demo scripts.

**Files:**
- `demo-wekeza-core-system.sh` - Core system demonstration
- `start-wekeza-complete.ps1` - Start complete Wekeza system
- `start-wekeza-simple.ps1` - Start simplified Wekeza system
- `start-wekeza.bat` - Start Wekeza (Windows batch)
- `start-wekeza.ps1` - Start Wekeza (PowerShell)
- `wekeza.ps1` - Main Wekeza launcher

**Usage:**
```bash
# Demo the core system
./demo-wekeza-core-system.sh

# Start complete system
./start-wekeza-complete.ps1

# Start simple version
./start-wekeza-simple.ps1
```

### 📁 `/scripts/database`
Database setup, migration, and management scripts.

**Files:**
- `create-staff-tables-direct.ps1` - Create staff tables directly
- `create-staff-tables.sql` - SQL for staff tables
- `reset-database.ps1` - Reset database to clean state
- `setup-staff-tables.ps1` - Setup staff tables
- `start-postgresql.ps1` - Start PostgreSQL service
- `test-database-connection.ps1` - Test database connectivity

**Usage:**
```bash
# Start PostgreSQL
./start-postgresql.ps1

# Setup database
./setup-staff-tables.ps1

# Test connection
./test-database-connection.ps1

# Reset if needed
./reset-database.ps1
```

### 📁 `/scripts/testing`
Testing and validation scripts.

**Files:**
- `test-admin-panel.ps1` - Test admin panel functionality
- `test-comprehensive-admin.ps1` - Comprehensive admin testing
- `test-minimal-build.ps1` - Test minimal build
- `test-staff-creation.ps1` - Test staff creation
- `run-comprehensive-tests.sh` - Run all comprehensive tests

**Usage:**
```bash
# Run comprehensive tests
./run-comprehensive-tests.sh

# Test specific components
./test-admin-panel.ps1
./test-staff-creation.ps1
```

### 📁 `/scripts/admin`
Admin panel startup scripts.

**Files:**
- `start-admin-panel.ps1` - Start admin panel
- `start-comprehensive-admin.ps1` - Start comprehensive admin interface

**Usage:**
```bash
# Start admin panel
./start-admin-panel.ps1

# Start comprehensive admin
./start-comprehensive-admin.ps1
```

### 📁 `/scripts/deployment`
Deployment and infrastructure setup scripts.

**Files:**
- `start-mvp5-local.sh` - Start MVP5 locally (bash)
- `start-mvp5.ps1` - Start MVP5 (PowerShell)
- `start-mvp5.sh` - Start MVP5 (bash)
- `setup-infrastructure.ps1` - Setup infrastructure
- `start-minimal.ps1` - Start minimal configuration

**Usage:**
```bash
# Setup infrastructure first
./setup-infrastructure.ps1

# Start MVP5
./start-mvp5.sh

# Or start minimal
./start-minimal.ps1
```

### 📁 `/scripts` (Existing)
General-purpose scripts that were already in the scripts folder:

- `deploy-production.ps1` - Production deployment
- `run-migrations.ps1` / `run-migrations.sh` - Database migrations
- `run-performance-tests.ps1` - Performance testing
- `run-security-tests.ps1` - Security testing
- `setup-database.ps1` - Database setup
- `setup-local-db.ps1` - Local database setup
- `setup-monitoring.ps1` - Monitoring setup
- `start-local-dev.ps1` - Local development environment
- `start-local.ps1` - Start local environment
- `test-api.ps1` - API testing
- `test-gl-integration.ps1` - General Ledger integration testing
- `test-system.ps1` - System testing

## Quick Reference

### Common Workflows

#### 🚀 First Time Setup
```bash
# 1. Setup infrastructure
./deployment/setup-infrastructure.ps1

# 2. Setup database
./database/start-postgresql.ps1
./database/setup-staff-tables.ps1

# 3. Start the system
./core/start-wekeza-complete.ps1
```

#### 🧪 Testing Workflow
```bash
# 1. Test database
./database/test-database-connection.ps1

# 2. Test APIs
./api/test-comprehensive-api.ps1

# 3. Run comprehensive tests
./testing/run-comprehensive-tests.sh
```

#### 🏃 Quick Start
```bash
# Start everything
./core/start-wekeza-complete.ps1

# Or just APIs
./api/start-all-apis.ps1

# Or admin panel
./admin/start-admin-panel.ps1
```

## Script Conventions

### File Extensions
- `.ps1` - PowerShell scripts (cross-platform, preferred)
- `.sh` - Bash scripts (Linux/Mac)
- `.bat` - Windows batch files (legacy)
- `.sql` - SQL scripts

### Naming Conventions
- `start-*` - Startup scripts
- `test-*` - Testing scripts
- `setup-*` - Setup/configuration scripts
- `run-*` - Execution scripts

## Platform Notes

### Windows
- Use PowerShell scripts (.ps1) for best experience
- May need to set execution policy: `Set-ExecutionPolicy RemoteSigned`

### Linux/Mac
- Use bash scripts (.sh)
- Ensure scripts are executable: `chmod +x script.sh`
- PowerShell Core is also available

### Docker
- Some scripts may interact with Docker containers
- Ensure Docker is running before executing container-related scripts

## Troubleshooting

### Script Won't Run
1. Check execution permissions (Linux/Mac): `chmod +x script.sh`
2. Check PowerShell execution policy (Windows): `Get-ExecutionPolicy`
3. Ensure you're in the correct directory

### Database Connection Issues
1. Check PostgreSQL is running: `./database/start-postgresql.ps1`
2. Test connection: `./database/test-database-connection.ps1`
3. Reset if needed: `./database/reset-database.ps1`

### API Startup Issues
1. Check all dependencies are installed
2. Verify database is initialized
3. Check ports are not already in use
4. Review logs in the respective API directories

## Contributing

When adding new scripts:
1. Place in the appropriate category folder
2. Use descriptive names following conventions
3. Add documentation at the top of the script
4. Update this README
5. Ensure cross-platform compatibility when possible

## Maintenance

These scripts are organized to maintain clarity and ease of use. Please:
- Keep related scripts in their category folders
- Document new scripts clearly
- Update this README when adding new categories
- Test scripts before committing
