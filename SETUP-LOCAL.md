# Wekeza Bank - Local Development Setup Guide

This guide will help you run Wekeza Bank on your local Windows machine without Docker.

## Prerequisites

### Required Software

1. **.NET 8 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation: `dotnet --version` (should show 8.0.x)

2. **PostgreSQL 15+**
   - Download: https://www.postgresql.org/download/windows/
   - During installation:
     - Set password for postgres user (remember this!)
     - Port: 5432 (default)
     - Install pgAdmin 4 (optional but recommended)

3. **Visual Studio 2022** or **VS Code**
   - VS 2022: https://visualstudio.microsoft.com/downloads/
   - VS Code: https://code.visualstudio.com/ + C# extension

4. **Git** (if not already installed)
   - Download: https://git-scm.com/download/win

## Step 1: Database Setup

### Option A: Using pgAdmin (GUI)

1. Open pgAdmin 4
2. Connect to PostgreSQL (localhost:5432)
3. Right-click "Databases" → Create → Database
4. Database name: `WekezaCoreDB`
5. Owner: `postgres`
6. Click "Save"

### Option B: Using Command Line (psql)

```powershell
# Open PowerShell as Administrator
psql -U postgres

# In psql prompt:
CREATE DATABASE "WekezaCoreDB";
\q
```

### Create Application User (Recommended for Security)

```sql
-- In pgAdmin Query Tool or psql:
CREATE USER wekeza_app WITH PASSWORD 'WekeZa2026!SecurePass';
GRANT ALL PRIVILEGES ON DATABASE "WekezaCoreDB" TO wekeza_app;
```

## Step 2: Configure Connection String

1. Navigate to `Core/Wekeza.Core.Api/appsettings.json`

2. Update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=WekezaCoreDB;Username=wekeza_app;Password=WekeZa2026!SecurePass"
  }
}
```

**For Development Only** - Create `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=WekezaCoreDB;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## Step 3: Install EF Core Tools

```powershell
# Install globally (one-time)
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

## Step 4: Create and Run Migrations

### Navigate to Solution Root

```powershell
cd C:\path\to\Wekeza
```

### Create Initial Migration

```powershell
# Create migration
dotnet ef migrations add InitialCreate `
  --project Core/Wekeza.Core.Infrastructure `
  --startup-project Core/Wekeza.Core.Api `
  --output-dir Persistence/Migrations

# Apply migration to database
dotnet ef database update `
  --project Core/Wekeza.Core.Infrastructure `
  --startup-project Core/Wekeza.Core.Api
```

### Verify Database Creation

```sql
-- In pgAdmin or psql:
\c WekezaCoreDB
\dt

-- You should see tables:
-- Accounts, Customers, Transactions, Loans, Cards
```

## Step 5: Build the Solution

```powershell
# Restore NuGet packages
dotnet restore

# Build entire solution
dotnet build

# Check for errors
# If successful, you'll see "Build succeeded"
```

## Step 6: Run the Application

### Option A: Using Visual Studio 2022

1. Open `Wekeza.sln`
2. Set `Wekeza.Core.Api` as startup project (right-click → Set as Startup Project)
3. Press `F5` or click "Run"
4. Browser will open to Swagger UI

### Option B: Using Command Line

```powershell
# Navigate to API project
cd Core/Wekeza.Core.Api

# Run the application
dotnet run

# You'll see output like:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:5001
#       Now listening on: http://localhost:5000
```

### Option C: Using VS Code

1. Open folder in VS Code
2. Press `F5` (will create launch.json automatically)
3. Select ".NET Core Launch (web)"
4. Application will start

## Step 7: Access the Application

### Swagger UI (API Documentation)
- **HTTPS**: https://localhost:5001/swagger
- **HTTP**: http://localhost:5000/swagger

### Health Check
- https://localhost:5001/health

## Step 8: Test Authentication

### 1. Login to Get Token

In Swagger UI:
1. Expand `POST /api/authentication/login`
2. Click "Try it out"
3. Enter credentials:
```json
{
  "username": "admin",
  "password": "admin123"
}
```
4. Click "Execute"
5. Copy the `token` from response

### 2. Authorize Swagger

1. Click the "Authorize" button (🔒 icon at top)
2. Paste your token (just the token, no "Bearer" prefix)
3. Click "Authorize"
4. Click "Close"

### 3. Test an Endpoint

Try opening an account:
1. Expand `POST /api/accounts/open`
2. Click "Try it out"
3. Enter sample data:
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "254712345678",
  "identificationNumber": "12345678",
  "currencyCode": "KES",
  "initialDeposit": 1000
}
```
4. Click "Execute"

## Step 9: Seed Test Data (Optional)

Create a seed script to populate test data:

```powershell
# Run seed script
dotnet run --project Core/Wekeza.Core.Api -- seed
```

Or manually insert test data via pgAdmin.

## Common Issues & Solutions

### Issue 1: "Connection refused" or "Could not connect to database"

**Solution:**
- Verify PostgreSQL is running: Open Services (services.msc), find "postgresql-x64-15"
- Check connection string matches your PostgreSQL setup
- Test connection: `psql -U postgres -h localhost`

### Issue 2: "Login failed for user"

**Solution:**
- Verify username/password in connection string
- Check user has permissions: `GRANT ALL PRIVILEGES ON DATABASE "WekezaCoreDB" TO wekeza_app;`

### Issue 3: "No migrations found"

**Solution:**
```powershell
# Ensure you're in the solution root
dotnet ef migrations add InitialCreate `
  --project Core/Wekeza.Core.Infrastructure `
  --startup-project Core/Wekeza.Core.Api
```

### Issue 4: Port already in use (5001/5000)

**Solution:**
Edit `Core/Wekeza.Core.Api/Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:7001;http://localhost:7000"
}
```

### Issue 5: SSL Certificate Error

**Solution:**
```powershell
# Trust the development certificate
dotnet dev-certs https --trust
```

## Development Workflow

### Making Changes

1. **Code Changes**: Edit files in VS Code/Visual Studio
2. **Hot Reload**: Changes apply automatically (for most files)
3. **Restart**: Press `Ctrl+C` and run `dotnet run` again if needed

### Database Changes

```powershell
# 1. Modify entity in Domain layer
# 2. Create migration
dotnet ef migrations add YourMigrationName `
  --project Core/Wekeza.Core.Infrastructure `
  --startup-project Core/Wekeza.Core.Api

# 3. Apply migration
dotnet ef database update `
  --project Core/Wekeza.Core.Infrastructure `
  --startup-project Core/Wekeza.Core.Api

# 4. Rollback if needed
dotnet ef database update PreviousMigrationName `
  --project Core/Wekeza.Core.Infrastructure `
  --startup-project Core/Wekeza.Core.Api
```

### Running Tests

```powershell
# Run all tests
dotnet test

# Run specific test project
dotnet test Tests/Wekeza.Core.UnitTests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Next Steps

Once you have the system running locally:

1. ✅ Test all API endpoints
2. ✅ Create test accounts and transactions
3. ✅ Verify M-Pesa integration (with test credentials)
4. ✅ Run the test suite
5. ✅ Review logs and performance

## Moving to Docker

After confirming everything works locally, see `SETUP-DOCKER.md` for containerization.

## Support

If you encounter issues:
1. Check logs in console output
2. Review `appsettings.Development.json` configuration
3. Verify PostgreSQL is running
4. Check firewall settings

---

**Ready to go live?** Once local development is stable, proceed to Docker setup for production deployment.
