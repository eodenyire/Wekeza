# Wekeza Bank - Quick Start Guide

Get Wekeza Bank running in 5 minutes!

## Prerequisites Check

```powershell
# Check .NET 8
dotnet --version  # Should show 8.0.x

# Check PostgreSQL
psql --version    # Should show 15.x or higher
```

If missing, install from:
- .NET 8: https://dotnet.microsoft.com/download/dotnet/8.0
- PostgreSQL: https://www.postgresql.org/download/windows/

## 3-Step Setup

### Step 1: Setup Database (2 minutes)

```powershell
# Run automated setup script
.\scripts\setup-local-db.ps1

# When prompted, enter your PostgreSQL 'postgres' user password
# Script will create database and user automatically
```

### Step 2: Run Migrations (1 minute)

```powershell
# Create and apply database schema
.\scripts\run-migrations.ps1 -MigrationName "InitialCreate"
```

### Step 3: Start Application (1 minute)

```powershell
# Start the API
.\scripts\start-local.ps1

# Or manually:
cd Core/Wekeza.Core.Api
dotnet run
```

## Access the Application

Open your browser to:
- **Swagger UI**: https://localhost:5001/swagger
- **Health Check**: https://localhost:5001/health

## First API Call

1. In Swagger, expand `POST /api/authentication/login`
2. Click "Try it out"
3. Use credentials:
   ```json
   {
     "username": "admin",
     "password": "admin123"
   }
   ```
4. Copy the token from response
5. Click "Authorize" button at top
6. Paste token and click "Authorize"
7. Now you can test any endpoint!

## Troubleshooting

**Database connection failed?**
```powershell
# Check PostgreSQL is running
Get-Service postgresql*

# If stopped, start it
Start-Service postgresql-x64-15
```

**Port already in use?**
- Edit `Core/Wekeza.Core.Api/Properties/launchSettings.json`
- Change ports from 5000/5001 to 7000/7001

**Migration errors?**
```powershell
# Reset database
.\scripts\setup-local-db.ps1  # Answer 'yes' to recreate
.\scripts\run-migrations.ps1
```

## Next Steps

- Read full setup guide: `SETUP-LOCAL.md`
- Explore API documentation in Swagger
- Run tests: `dotnet test`
- Check logs in console output

---

**Need help?** Check `SETUP-LOCAL.md` for detailed instructions.
