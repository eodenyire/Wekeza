# Wekeza Core Banking System - Startup Script
Write-Host "Starting Wekeza Core Banking System - Complete Implementation" -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Green

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

# Check .NET 8
try {
    $dotnetVersion = dotnet --version
    Write-Host "OK .NET SDK: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "ERROR .NET 8 SDK not found. Please install .NET 8 SDK" -ForegroundColor Red
    exit 1
}

# Check Docker
try {
    $dockerVersion = docker --version
    Write-Host "OK Docker: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "WARNING Docker not found. Database services may not start automatically" -ForegroundColor Yellow
}

# Start infrastructure services
Write-Host "Starting infrastructure services..." -ForegroundColor Yellow

if (Get-Command docker -ErrorAction SilentlyContinue) {
    Write-Host "Starting PostgreSQL and Redis..." -ForegroundColor Cyan
    
    # Stop existing containers if they exist
    docker stop wekeza-postgres wekeza-redis 2>$null
    docker rm wekeza-postgres wekeza-redis 2>$null
    
    # Start PostgreSQL
    docker run -d --name wekeza-postgres `
        -e POSTGRES_DB=wekeza_core `
        -e POSTGRES_USER=postgres `
        -e POSTGRES_PASSWORD=password123 `
        -p 5432:5432 `
        postgres:15-alpine
    
    # Start Redis
    docker run -d --name wekeza-redis `
        -p 6379:6379 `
        redis:7-alpine
    
    Write-Host "OK Infrastructure services started" -ForegroundColor Green
    
    # Wait for services to be ready
    Write-Host "Waiting for services to be ready..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15
} else {
    Write-Host "WARNING Docker not available. Please ensure PostgreSQL and Redis are running manually" -ForegroundColor Yellow
    Write-Host "   PostgreSQL: localhost:5432 (Database: wekeza_core, User: postgres)" -ForegroundColor Cyan
    Write-Host "   Redis: localhost:6379" -ForegroundColor Cyan
}

# Build the solution
Write-Host "Building the solution..." -ForegroundColor Yellow
dotnet build Wekeza.Core.sln --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR Build failed" -ForegroundColor Red
    exit 1
}

Write-Host "OK Build successful" -ForegroundColor Green

# Run database migrations
Write-Host "Running database migrations..." -ForegroundColor Yellow
dotnet ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api

if ($LASTEXITCODE -ne 0) {
    Write-Host "WARNING Database migrations may have failed. Continuing..." -ForegroundColor Yellow
} else {
    Write-Host "OK Database migrations completed" -ForegroundColor Green
}

# Start the API
Write-Host "Starting Wekeza Core Banking API..." -ForegroundColor Yellow
Write-Host ""
Write-Host "System will be available at:" -ForegroundColor Cyan
Write-Host "   API Documentation: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "   System Status: http://localhost:5000/" -ForegroundColor White
Write-Host "   Health Checks: http://localhost:5000/health" -ForegroundColor White
Write-Host ""
Write-Host "Available Portals:" -ForegroundColor Cyan
Write-Host "   Administrator Portal: http://localhost:5000/api/administrator" -ForegroundColor White
Write-Host "   Teller Portal: http://localhost:5000/api/teller" -ForegroundColor White
Write-Host "   Customer Portal: http://localhost:5000/api/customer-portal" -ForegroundColor White
Write-Host "   Analytics Dashboard: http://localhost:5000/api/dashboard" -ForegroundColor White
Write-Host ""
Write-Host "Banking Modules Available:" -ForegroundColor Cyan
Write-Host "   Account Management: /api/accounts" -ForegroundColor White
Write-Host "   CIF Management: /api/cif" -ForegroundColor White
Write-Host "   Loan Management: /api/loans" -ForegroundColor White
Write-Host "   Payment Processing: /api/payments" -ForegroundColor White
Write-Host "   Transaction Processing: /api/transactions" -ForegroundColor White
Write-Host "   Card Management: /api/cards" -ForegroundColor White
Write-Host "   Digital Channels: /api/digitalchannels" -ForegroundColor White
Write-Host "   Branch Operations: /api/branchoperations" -ForegroundColor White
Write-Host "   Compliance & AML: /api/compliance" -ForegroundColor White
Write-Host "   Trade Finance: /api/tradefinance" -ForegroundColor White
Write-Host "   Treasury Operations: /api/treasury" -ForegroundColor White
Write-Host "   Reporting System: /api/reporting" -ForegroundColor White
Write-Host "   Workflow Engine: /api/workflows" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the system" -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Green

# Start the API server
dotnet run --project Core/Wekeza.Core.Api --configuration Release