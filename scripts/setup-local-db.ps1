# Wekeza Bank - Local Database Setup Script
# This script sets up PostgreSQL database for local development

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Wekeza Bank - Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$DB_NAME = "WekezaCoreDB"
$DB_USER = "wekeza_app"
$DB_PASSWORD = "WekeZa2026!SecurePass"
$POSTGRES_USER = "postgres"

# Check if PostgreSQL is installed
Write-Host "Checking PostgreSQL installation..." -ForegroundColor Yellow
$pgVersion = psql --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: PostgreSQL is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install PostgreSQL from: https://www.postgresql.org/download/windows/" -ForegroundColor Yellow
    exit 1
}
Write-Host "✓ PostgreSQL found: $pgVersion" -ForegroundColor Green
Write-Host ""

# Prompt for postgres password
Write-Host "Enter PostgreSQL 'postgres' user password:" -ForegroundColor Yellow
$POSTGRES_PASSWORD = Read-Host -AsSecureString
$POSTGRES_PASSWORD_TEXT = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($POSTGRES_PASSWORD)
)

# Set environment variable for psql
$env:PGPASSWORD = $POSTGRES_PASSWORD_TEXT

# Test connection
Write-Host "Testing PostgreSQL connection..." -ForegroundColor Yellow
$testConnection = psql -U $POSTGRES_USER -h localhost -c "SELECT version();" 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Cannot connect to PostgreSQL" -ForegroundColor Red
    Write-Host "Please verify:" -ForegroundColor Yellow
    Write-Host "  1. PostgreSQL service is running" -ForegroundColor Yellow
    Write-Host "  2. Password is correct" -ForegroundColor Yellow
    Write-Host "  3. Port 5432 is accessible" -ForegroundColor Yellow
    exit 1
}
Write-Host "✓ Connection successful" -ForegroundColor Green
Write-Host ""

# Check if database exists
Write-Host "Checking if database exists..." -ForegroundColor Yellow
$dbExists = psql -U $POSTGRES_USER -h localhost -tAc "SELECT 1 FROM pg_database WHERE datname='$DB_NAME'" 2>$null

if ($dbExists -eq "1") {
    Write-Host "⚠ Database '$DB_NAME' already exists" -ForegroundColor Yellow
    $response = Read-Host "Do you want to drop and recreate it? (yes/no)"
    if ($response -eq "yes") {
        Write-Host "Dropping existing database..." -ForegroundColor Yellow
        psql -U $POSTGRES_USER -h localhost -c "DROP DATABASE IF EXISTS `"$DB_NAME`";" | Out-Null
        Write-Host "✓ Database dropped" -ForegroundColor Green
    } else {
        Write-Host "Keeping existing database" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Skipping to user creation..." -ForegroundColor Yellow
    }
}

# Create database
if ($dbExists -ne "1" -or $response -eq "yes") {
    Write-Host "Creating database '$DB_NAME'..." -ForegroundColor Yellow
    psql -U $POSTGRES_USER -h localhost -c "CREATE DATABASE `"$DB_NAME`";" | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database created successfully" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Failed to create database" -ForegroundColor Red
        exit 1
    }
}
Write-Host ""

# Check if user exists
Write-Host "Checking if application user exists..." -ForegroundColor Yellow
$userExists = psql -U $POSTGRES_USER -h localhost -tAc "SELECT 1 FROM pg_roles WHERE rolname='$DB_USER'" 2>$null

if ($userExists -eq "1") {
    Write-Host "⚠ User '$DB_USER' already exists" -ForegroundColor Yellow
    Write-Host "Updating password..." -ForegroundColor Yellow
    psql -U $POSTGRES_USER -h localhost -c "ALTER USER $DB_USER WITH PASSWORD '$DB_PASSWORD';" | Out-Null
} else {
    Write-Host "Creating application user '$DB_USER'..." -ForegroundColor Yellow
    psql -U $POSTGRES_USER -h localhost -c "CREATE USER $DB_USER WITH PASSWORD '$DB_PASSWORD';" | Out-Null
}

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ User configured successfully" -ForegroundColor Green
} else {
    Write-Host "ERROR: Failed to configure user" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Grant privileges
Write-Host "Granting privileges..." -ForegroundColor Yellow
psql -U $POSTGRES_USER -h localhost -c "GRANT ALL PRIVILEGES ON DATABASE `"$DB_NAME`" TO $DB_USER;" | Out-Null
psql -U $POSTGRES_USER -h localhost -d $DB_NAME -c "GRANT ALL ON SCHEMA public TO $DB_USER;" | Out-Null
psql -U $POSTGRES_USER -h localhost -d $DB_NAME -c "ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO $DB_USER;" | Out-Null

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Privileges granted successfully" -ForegroundColor Green
} else {
    Write-Host "WARNING: Some privileges may not have been granted" -ForegroundColor Yellow
}
Write-Host ""

# Clear password from environment
$env:PGPASSWORD = $null

# Display connection string
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Connection String:" -ForegroundColor Yellow
Write-Host "Host=localhost;Port=5432;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD" -ForegroundColor White
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Update appsettings.Development.json with the connection string above" -ForegroundColor White
Write-Host "2. Run: dotnet ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api" -ForegroundColor White
Write-Host "3. Run: dotnet run --project Core/Wekeza.Core.Api" -ForegroundColor White
Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
