#!/usr/bin/env pwsh

# 🗄️ Wekeza Core Banking System - Database Setup Script
# This script sets up the PostgreSQL database for local development

Write-Host "🗄️ Setting up Wekeza Core Banking Database..." -ForegroundColor Green

# Database connection parameters
$dbHost = "localhost"
$dbPort = "5432"
$dbName = "WekezaCoreDB"
$dbUser = "admin"
$dbPassword = "the_beast_pass"

Write-Host "📋 Database Configuration:" -ForegroundColor Yellow
Write-Host "  Host: $dbHost" -ForegroundColor White
Write-Host "  Port: $dbPort" -ForegroundColor White
Write-Host "  Database: $dbName" -ForegroundColor White
Write-Host "  Username: $dbUser" -ForegroundColor White
Write-Host "" -ForegroundColor White

# Check if PostgreSQL is installed
Write-Host "📋 Checking PostgreSQL installation..." -ForegroundColor Yellow
try {
    $pgVersion = psql --version
    Write-Host "✅ PostgreSQL found: $pgVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ PostgreSQL is not installed or not in PATH" -ForegroundColor Red
    Write-Host "" -ForegroundColor White
    Write-Host "Please install PostgreSQL:" -ForegroundColor Yellow
    Write-Host "  • Windows: https://www.postgresql.org/download/windows/" -ForegroundColor White
    Write-Host "  • Or use Docker: docker run --name wekeza-postgres -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=the_beast_pass -e POSTGRES_DB=WekezaCoreDB -p 5432:5432 -d postgres:15" -ForegroundColor White
    Write-Host "" -ForegroundColor White
    exit 1
}

# Test connection to PostgreSQL
Write-Host "🔌 Testing database connection..." -ForegroundColor Yellow
$env:PGPASSWORD = $dbPassword

try {
    # Try to connect to the database
    $result = psql -h $dbHost -p $dbPort -U $dbUser -d postgres -c "SELECT version();" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Successfully connected to PostgreSQL" -ForegroundColor Green
    } else {
        throw "Connection failed"
    }
} catch {
    Write-Host "❌ Could not connect to PostgreSQL" -ForegroundColor Red
    Write-Host "Please ensure PostgreSQL is running and accessible with the configured credentials." -ForegroundColor Yellow
    Write-Host "" -ForegroundColor White
    Write-Host "If using Docker, start PostgreSQL with:" -ForegroundColor Yellow
    Write-Host "docker run --name wekeza-postgres -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=the_beast_pass -e POSTGRES_DB=WekezaCoreDB -p 5432:5432 -d postgres:15" -ForegroundColor White
    Write-Host "" -ForegroundColor White
    exit 1
}

# Check if database exists, create if not
Write-Host "🗄️ Checking if database '$dbName' exists..." -ForegroundColor Yellow
$dbExists = psql -h $dbHost -p $dbPort -U $dbUser -d postgres -tAc "SELECT 1 FROM pg_database WHERE datname='$dbName';" 2>$null

if ($dbExists -eq "1") {
    Write-Host "✅ Database '$dbName' already exists" -ForegroundColor Green
} else {
    Write-Host "📝 Creating database '$dbName'..." -ForegroundColor Yellow
    psql -h $dbHost -p $dbPort -U $dbUser -d postgres -c "CREATE DATABASE `"$dbName`";" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Database '$dbName' created successfully" -ForegroundColor Green
    } else {
        Write-Host "❌ Failed to create database '$dbName'" -ForegroundColor Red
        exit 1
    }
}

# Install Entity Framework tools if not already installed
Write-Host "🔧 Checking Entity Framework tools..." -ForegroundColor Yellow
try {
    $efVersion = dotnet ef --version 2>$null
    Write-Host "✅ Entity Framework tools found: $efVersion" -ForegroundColor Green
} catch {
    Write-Host "📦 Installing Entity Framework tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Entity Framework tools installed" -ForegroundColor Green
    } else {
        Write-Host "❌ Failed to install Entity Framework tools" -ForegroundColor Red
        exit 1
    }
}

# Run database migrations
Write-Host "🔄 Running database migrations..." -ForegroundColor Yellow
try {
    Set-Location "../"  # Go back to solution root
    dotnet ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api --verbose
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Database migrations completed successfully" -ForegroundColor Green
    } else {
        Write-Host "❌ Database migrations failed" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Error running migrations: $_" -ForegroundColor Red
    exit 1
}

Write-Host "" -ForegroundColor White
Write-Host "🎉 Database setup completed successfully!" -ForegroundColor Green
Write-Host "" -ForegroundColor White
Write-Host "📊 Database Information:" -ForegroundColor Cyan
Write-Host "  • Connection String: Host=$dbHost;Database=$dbName;Username=$dbUser;Password=***" -ForegroundColor White
Write-Host "  • You can now start the Wekeza API with: ./scripts/start-local-dev.ps1" -ForegroundColor White
Write-Host "" -ForegroundColor White

# Clean up environment variable
Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue