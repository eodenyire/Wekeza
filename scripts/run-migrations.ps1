# Wekeza Bank - Database Migration Script
# This script creates and applies EF Core migrations

param(
    [Parameter(Mandatory=$false)]
    [string]$MigrationName = "InitialCreate",
    
    [Parameter(Mandatory=$false)]
    [switch]$CreateOnly,
    
    [Parameter(Mandatory=$false)]
    [switch]$ApplyOnly,
    
    [Parameter(Mandatory=$false)]
    [switch]$Rollback,
    
    [Parameter(Mandatory=$false)]
    [string]$TargetMigration
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Wekeza Bank - Migration Manager" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get solution root (assuming script is in /scripts folder)
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = Split-Path -Parent $scriptPath

Write-Host "Solution Root: $solutionRoot" -ForegroundColor Yellow
Write-Host ""

# Check if dotnet ef is installed
Write-Host "Checking for EF Core tools..." -ForegroundColor Yellow
$efVersion = dotnet ef --version 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: EF Core tools not found" -ForegroundColor Red
    Write-Host "Installing EF Core tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to install EF Core tools" -ForegroundColor Red
        exit 1
    }
    Write-Host "✓ EF Core tools installed" -ForegroundColor Green
} else {
    Write-Host "✓ EF Core tools found: $efVersion" -ForegroundColor Green
}
Write-Host ""

# Set project paths
$infrastructureProject = Join-Path $solutionRoot "Core\Wekeza.Core.Infrastructure"
$apiProject = Join-Path $solutionRoot "Core\Wekeza.Core.Api"

# Verify projects exist
if (-not (Test-Path $infrastructureProject)) {
    Write-Host "ERROR: Infrastructure project not found at: $infrastructureProject" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $apiProject)) {
    Write-Host "ERROR: API project not found at: $apiProject" -ForegroundColor Red
    exit 1
}

# Change to solution root
Set-Location $solutionRoot

# Handle rollback
if ($Rollback) {
    if ([string]::IsNullOrEmpty($TargetMigration)) {
        Write-Host "ERROR: -TargetMigration parameter required for rollback" -ForegroundColor Red
        Write-Host "Usage: .\run-migrations.ps1 -Rollback -TargetMigration 'MigrationName'" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "Rolling back to migration: $TargetMigration" -ForegroundColor Yellow
    dotnet ef database update $TargetMigration `
        --project $infrastructureProject `
        --startup-project $apiProject
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Rollback successful" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Rollback failed" -ForegroundColor Red
        exit 1
    }
    exit 0
}

# Create migration
if (-not $ApplyOnly) {
    Write-Host "Creating migration: $MigrationName" -ForegroundColor Yellow
    Write-Host ""
    
    dotnet ef migrations add $MigrationName `
        --project $infrastructureProject `
        --startup-project $apiProject `
        --output-dir Persistence/Migrations
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to create migration" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    Write-Host "✓ Migration created successfully" -ForegroundColor Green
    Write-Host ""
}

# Apply migration
if (-not $CreateOnly) {
    Write-Host "Applying migrations to database..." -ForegroundColor Yellow
    Write-Host ""
    
    dotnet ef database update `
        --project $infrastructureProject `
        --startup-project $apiProject
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Failed to apply migrations" -ForegroundColor Red
        Write-Host ""
        Write-Host "Common issues:" -ForegroundColor Yellow
        Write-Host "  1. Database connection string is incorrect" -ForegroundColor White
        Write-Host "  2. PostgreSQL service is not running" -ForegroundColor White
        Write-Host "  3. Database user lacks permissions" -ForegroundColor White
        exit 1
    }
    
    Write-Host ""
    Write-Host "✓ Migrations applied successfully" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Migration Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# List applied migrations
Write-Host "Applied Migrations:" -ForegroundColor Yellow
dotnet ef migrations list `
    --project $infrastructureProject `
    --startup-project $apiProject

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "  Run the application: dotnet run --project Core\Wekeza.Core.Api" -ForegroundColor White
Write-Host ""
