# 🏦 Wekeza Core Banking System - Setup Guide

## Prerequisites

1. **.NET 8.0 SDK** - Already installed ✅
2. **PostgreSQL 15+** - Required for database
3. **Redis** (Optional) - For caching features

## Database Setup

### Option 1: Using Docker (Recommended)

```powershell
docker run --name wekeza-postgres `
  -e POSTGRES_USER=admin `
  -e POSTGRES_PASSWORD=the_beast_pass `
  -e POSTGRES_DB=WekezaCoreDB `
  -p 5432:5432 `
  -d postgres:15
```

### Option 2: Local PostgreSQL Installation

1. Download from: https://www.postgresql.org/download/windows/
2. Install with default settings
3. Create database manually:

```sql
CREATE DATABASE "WekezaCoreDB";
CREATE USER admin WITH PASSWORD 'the_beast_pass';
GRANT ALL PRIVILEGES ON DATABASE "WekezaCoreDB" TO admin;
```

## Configuration

The API is configured to connect to:
- **Host**: localhost
- **Port**: 5432
- **Database**: WekezaCoreDB
- **Username**: admin
- **Password**: the_beast_pass

Configuration is in: `Wekeza.Core.Api/appsettings.json`

## Starting the API

### Quick Start (Simplest)

```powershell
.\quick-start.ps1
```

### Full Start (With checks and migrations)

```powershell
.\start-wekeza-api.ps1
```

### Manual Start

```powershell
# Apply migrations
dotnet ef database update --project Wekeza.Core.Infrastructure --startup-project Wekeza.Core.Api

# Run the API
dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj
```

## Accessing the API

Once started, access:

- **Swagger Documentation**: https://localhost:5001/swagger
- **System Status**: https://localhost:5001/
- **API Overview**: https://localhost:5001/api
- **Health Checks**: https://localhost:5001/health

## Available Modules

The API includes these banking modules:

- **Administration** - `/api/administrator` - User and system management
- **Teller Operations** - `/api/teller` - Branch teller operations
- **Customer Portal** - `/api/customer-portal` - Customer self-service
- **Dashboard** - `/api/dashboard` - Real-time analytics
- **Accounts** - `/api/accounts` - Account management
- **CIF** - `/api/cif` - Customer Information File
- **Loans** - `/api/loans` - Loan management
- **Payments** - `/api/payments` - Payment processing
- **Transactions** - `/api/transactions` - Transaction processing
- **Cards** - `/api/cards` - Card management
- **Digital Channels** - `/api/digitalchannels` - Channel enrollment
- **Branch Operations** - `/api/branchoperations` - Branch operations
- **Compliance** - `/api/compliance` - AML and compliance
- **Trade Finance** - `/api/tradefinance` - Letters of credit
- **Treasury** - `/api/treasury` - FX and money markets
- **Reporting** - `/api/reporting` - Reports and MIS
- **Workflows** - `/api/workflows` - Approval workflows

## Troubleshooting

### Database Connection Issues

If you see database connection errors:

1. Verify PostgreSQL is running:
   ```powershell
   # For Docker
   docker ps | findstr wekeza-postgres
   
   # For local installation
   Get-Service postgresql*
   ```

2. Test connection manually:
   ```powershell
   $env:PGPASSWORD="the_beast_pass"
   psql -h localhost -p 5432 -U admin -d WekezaCoreDB
   ```

### Migration Issues

If migrations fail:

```powershell
# Remove existing migrations (if needed)
dotnet ef database drop --project Wekeza.Core.Infrastructure --startup-project Wekeza.Core.Api --force

# Reapply migrations
dotnet ef database update --project Wekeza.Core.Infrastructure --startup-project Wekeza.Core.Api
```

### Port Already in Use

If port 5001 is already in use, modify `launchSettings.json`:

```json
"applicationUrl": "https://localhost:7001;http://localhost:7000"
```

## Next Steps

1. Start the API using one of the methods above
2. Open Swagger at https://localhost:5001/swagger
3. Explore the available endpoints
4. Test authentication and authorization flows
5. Review the system status at https://localhost:5001/

## Support

For issues or questions, refer to the comprehensive documentation in the project.
