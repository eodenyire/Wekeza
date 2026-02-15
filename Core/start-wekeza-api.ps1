#!/usr/bin/env pwsh
# 🚀 Wekeza Core Banking API - Startup Script
# This script starts the Wekeza Core Banking API

Write-Host "🚀 Starting Wekeza Core Banking API..." -ForegroundColor Green
Write-Host ""

# Database connection parameters
$dbHost = "localhost"
$dbPort = "5432"
$dbName = "WekezaCoreDB"
$dbUser = "admin"
$dbPassword = "the_beast_pass"

Write-Host "📋 Configuration:" -ForegroundColor Yellow
Write-Host "  Database: $dbHost`:$dbPort/$dbName" -ForegroundColor White
Write-Host "  User: $dbUser" -ForegroundColor White
Write-Host ""

# Check if PostgreSQL is accessible
Write-Host "🔌 Checking PostgreSQL connection..." -ForegroundColor Yellow
$env:PGPASSWORD = $dbPassword

try {
    $testConnection = psql -h $dbHost -p $dbPort -U $dbUser -d postgres -c "SELECT 1;" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ PostgreSQL is accessible" -ForegroundColor Green
    } else {
        throw "Connection failed"
    }
} catch {
    Write-Host "⚠️  Cannot connect to PostgreSQL" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Please ensure PostgreSQL is running:" -ForegroundColor Cyan
    Write-Host "  Option 1 - Using Docker:" -ForegroundColor White
    Write-Host "    docker run --name wekeza-postgres -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=the_beast_pass -e POSTGRES_DB=WekezaCoreDB -p 5432:5432 -d postgres:15" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Option 2 - Local PostgreSQL:" -ForegroundColor White
    Write-Host "    Install from: https://www.postgresql.org/download/windows/" -ForegroundColor Gray
    Write-Host ""
    Write-Host "The API will attempt to start anyway..." -ForegroundColor Yellow
    Write-Host ""
}

# Check if database exists
Write-Host "🗄️  Checking database..." -ForegroundColor Yellow
try {
    $dbExists = psql -h $dbHost -p $dbPort -U $dbUser -d postgres -tAc "SELECT 1 FROM pg_database WHERE datname='$dbName';" 2>$null
    if ($dbExists -eq "1") {
        Write-Host "✅ Database '$dbName' exists" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Database '$dbName' does not exist" -ForegroundColor Yellow
        Write-Host "Creating database..." -ForegroundColor Yellow
        psql -h $dbHost -p $dbPort -U $dbUser -d postgres -c "CREATE DATABASE `"$dbName`";" 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Database created" -ForegroundColor Green
        }
    }
} catch {
    Write-Host "⚠️  Could not verify database" -ForegroundColor Yellow
}

# Clean up environment variable
Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "🔄 Applying database migrations..." -ForegroundColor Yellow
try {
    dotnet ef database update --project Wekeza.Core.Infrastructure --startup-project Wekeza.Core.Api --no-build
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Migrations applied successfully" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Migration may have failed, but continuing..." -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️  Could not apply migrations: $_" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🏗️  Building the API..." -ForegroundColor Yellow
dotnet build Wekeza.Core.Api/Wekeza.Core.Api.csproj --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful" -ForegroundColor Green
Write-Host ""
Write-Host "🚀 Starting Wekeza Core Banking API..." -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 Once started, you can access:" -ForegroundColor Yellow
Write-Host "  • API Documentation: https://localhost:7001/swagger" -ForegroundColor White
Write-Host "  • System Status: https://localhost:7001/" -ForegroundColor White
Write-Host "  • Health Checks: https://localhost:7001/health" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the API" -ForegroundColor Gray
Write-Host ""

# Start the API
dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj --configuration Release --no-build
