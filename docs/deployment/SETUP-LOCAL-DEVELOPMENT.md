# 🏦 Wekeza Core Banking System - Local Development Setup

## Overview
This guide will help you set up the Wekeza Core Banking System on your local machine for development and testing.

## Prerequisites

### 1. .NET 8.0 SDK
**Required Version:** .NET 8.0 (Latest LTS)

**Installation:**
- **Windows:** Download from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Verify Installation:** Open Command Prompt and run:
  ```cmd
  dotnet --version
  ```
  Should return: `8.0.x`

### 2. PostgreSQL Database
**Required Version:** PostgreSQL 15+

**Option A - Native Installation:**
1. Download from [PostgreSQL Downloads](https://www.postgresql.org/download/)
2. During installation, configure:
   - **Username:** `admin`
   - **Password:** `the_beast_pass`
   - **Port:** `5432` (default)
   - **Database:** Create `WekezaCoreDB`

**Option B - Docker (Recommended):**
```bash
docker run --name wekeza-postgres \
  -e POSTGRES_USER=admin \
  -e POSTGRES_PASSWORD=the_beast_pass \
  -e POSTGRES_DB=WekezaCoreDB \
  -p 5432:5432 \
  -d postgres:15
```

### 3. Redis (Optional - for caching)
**Option A - Docker:**
```bash
docker run --name wekeza-redis -p 6379:6379 -d redis:7-alpine
```

**Option B - Windows:** Download from [Redis releases](https://github.com/microsoftarchive/redis/releases)

## Quick Start

### 1. Clone and Navigate
```bash
git clone <repository-url>
cd Wekeza-Core
```

### 2. Automated Setup (Recommended)
```powershell
# Setup database
./scripts/setup-database.ps1

# Start the application
./scripts/start-local-dev.ps1
```

### 3. Manual Setup (Alternative)

**Step 1: Restore Packages**
```bash
dotnet restore Wekeza.Core.sln
```

**Step 2: Build Solution**
```bash
dotnet build Wekeza.Core.sln
```

**Step 3: Install EF Tools (if not installed)**
```bash
dotnet tool install --global dotnet-ef
```

**Step 4: Run Database Migrations**
```bash
dotnet ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api
```

**Step 5: Start the API**
```bash
cd Core/Wekeza.Core.Api
dotnet run
```

## Access Points

Once running, the system will be available at:

| Service | URL | Description |
|---------|-----|-------------|
| **API** | https://localhost:7001 | Main API endpoint |
| **Swagger** | https://localhost:7001/swagger | API documentation |
| **Health Check** | https://localhost:7001/health | System health status |

## System Architecture

```
🏗️ Wekeza Core Banking System
├── 🌐 API Layer (ASP.NET Core 8.0)
│   ├── Controllers (19 controllers)
│   ├── Authentication (JWT Bearer)
│   ├── Rate Limiting
│   └── Swagger Documentation
├── 🧠 Application Layer (CQRS + MediatR)
│   ├── Commands & Queries
│   ├── Validation (FluentValidation)
│   ├── Behaviors (Logging, Auth, Audit)
│   └── AutoMapper Profiles
├── 🏛️ Domain Layer (DDD)
│   ├── Aggregates (53 banking entities)
│   ├── Value Objects
│   ├── Domain Events
│   └── Business Rules
└── 🗄️ Infrastructure Layer
    ├── PostgreSQL (Entity Framework Core)
    ├── Redis Caching
    ├── External Services
    └── Background Jobs
```

## Core Banking Modules

The system implements 15 major banking modules:

| Module | Status | Description |
|--------|--------|-------------|
| **Customer & Party Management** | ✅ Complete | CIF, KYC, Customer onboarding |
| **Account Management** | ✅ Complete | CASA, Account operations |
| **Deposits & Investments** | ✅ Complete | Fixed deposits, Recurring deposits |
| **Loans & Credit Management** | ✅ Complete | Loan origination, Servicing |
| **Payments & Transfers** | ✅ Complete | RTGS, SWIFT, Internal transfers |
| **Teller & Branch Operations** | ✅ Complete | Cash management, EOD/BOD |
| **Cards & Channels** | ✅ Complete | Debit/Credit cards, ATM, POS |
| **Trade Finance** | ✅ Complete | Letters of Credit, Bank Guarantees |
| **Treasury & Markets** | ✅ Complete | FX deals, Money market |
| **General Ledger** | ✅ Complete | Chart of accounts, Journal entries |
| **Risk & Compliance** | ✅ Complete | AML, Sanctions screening |
| **Reporting & Analytics** | ✅ Complete | Regulatory reports, MIS |
| **Workflow & BPM** | ✅ Complete | Approval workflows, Task management |
| **Integration & Middleware** | ✅ Complete | API gateway, Message queues |
| **Security & Administration** | ✅ Complete | User management, Audit logs |

## Configuration

### Database Connection
Located in `Core/Wekeza.Core.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=WekezaCoreDB;Username=admin;Password=the_beast_pass"
  }
}
```

### JWT Authentication
```json
{
  "JwtSettings": {
    "Secret": "WekeZa-BaNk-SuPeR-SeCrEt-KeY-2026-MuSt-Be-At-LeAsT-32-ChArS-LoNg!",
    "Issuer": "https://api.wekeza.com",
    "Audience": "https://wekeza.com",
    "ExpiryMinutes": 60
  }
}
```

## Testing the API

### 1. Using Swagger UI
1. Navigate to https://localhost:7001/swagger
2. Click "Authorize" button
3. Login via `/api/authentication/login`
4. Copy the JWT token
5. Enter `Bearer {token}` in authorization

### 2. Using PowerShell
```powershell
# Test health endpoint
Invoke-RestMethod -Uri "https://localhost:7001/health" -Method GET

# Login (get JWT token)
$loginData = @{
    Email = "admin@wekeza.com"
    Password = "Admin123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:7001/api/authentication/login" -Method POST -Body $loginData -ContentType "application/json"
$token = $response.token

# Use authenticated endpoint
$headers = @{ Authorization = "Bearer $token" }
Invoke-RestMethod -Uri "https://localhost:7001/api/accounts" -Method GET -Headers $headers
```

## Development Features

### Hot Reload
The API supports hot reload during development:
```bash
dotnet watch run --project Core/Wekeza.Core.Api
```

### Database Migrations
Create new migration:
```bash
dotnet ef migrations add MigrationName --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api
```

Apply migrations:
```bash
dotnet ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api
```

### Logging
Logs are written to:
- **Console:** Structured JSON logs
- **File:** `logs/wekeza-{date}.txt`

## Troubleshooting

### Common Issues

**1. Database Connection Failed**
- Ensure PostgreSQL is running
- Verify connection string in `appsettings.json`
- Check firewall settings

**2. Migration Errors**
- Ensure database exists
- Check user permissions
- Verify EF tools installation

**3. Port Already in Use**
- Change ports in `launchSettings.json`
- Kill existing processes: `netstat -ano | findstr :7001`

**4. Package Restore Issues**
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Restore packages: `dotnet restore --force`

### Performance Monitoring
- **Health Checks:** https://localhost:7001/health
- **Metrics:** Built-in performance middleware logs slow requests (>500ms)
- **Audit Trail:** All transactions are logged for compliance

## Next Steps

1. **Explore the API:** Use Swagger UI to test endpoints
2. **Review Architecture:** Examine the Clean Architecture implementation
3. **Add Features:** Follow the established patterns for new functionality
4. **Run Tests:** Execute unit and integration tests
5. **Deploy:** Use Docker for containerized deployment

## Support

For development support:
- **Documentation:** Check the `/docs` folder
- **Issues:** Create GitHub issues for bugs
- **Architecture:** Review Clean Architecture and DDD patterns
- **Banking Domain:** Consult domain experts for business rules

---

🏦 **Wekeza Core Banking System** - Enterprise-grade banking platform built with .NET 8.0, Clean Architecture, and Domain-Driven Design.