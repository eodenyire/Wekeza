#!/usr/bin/env pwsh

# 🏦 Wekeza Core Banking System - Startup Script
# This script starts the Wekeza banking system locally

Write-Host "🏦 Starting Wekeza Core Banking System..." -ForegroundColor Green
Write-Host ""

# Set the dotnet path
$dotnetPath = "C:\Program Files\dotnet\dotnet.exe"

# Check if .NET is available
Write-Host "📋 Checking .NET installation..." -ForegroundColor Yellow
try {
    $version = & $dotnetPath --version
    Write-Host "✅ .NET version: $version" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET not found at expected location" -ForegroundColor Red
    Write-Host "Please ensure .NET 8.0 is installed and accessible" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""

# Restore packages
Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
try {
    & $dotnetPath restore Wekeza.Core.sln
    if ($LASTEXITCODE -ne 0) { throw "Restore failed" }
    Write-Host "✅ Packages restored successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to restore packages" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""

# Build solution
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
try {
    & $dotnetPath build Wekeza.Core.sln --configuration Debug --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    Write-Host "✅ Build completed successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Build failed" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""

# Database setup
Write-Host "🗄️ Setting up database (if needed)..." -ForegroundColor Yellow
Write-Host "Note: This requires PostgreSQL to be running" -ForegroundColor Cyan
Write-Host "Database: WekezaCoreDB, User: admin, Password: the_beast_pass" -ForegroundColor Cyan
Write-Host ""

# Try to run migrations
Write-Host "🔄 Running database migrations..." -ForegroundColor Yellow
try {
    & $dotnetPath ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Database migrations completed successfully" -ForegroundColor Green
    } else {
        throw "Migration failed"
    }
} catch {
    Write-Host "⚠️ Database migrations failed - PostgreSQL may not be running" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "You can start PostgreSQL with Docker:" -ForegroundColor Yellow
    Write-Host "docker run --name wekeza-postgres -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=the_beast_pass -e POSTGRES_DB=WekezaCoreDB -p 5432:5432 -d postgres:15" -ForegroundColor White
    Write-Host ""
    Write-Host "Or continue without database (some features won't work)" -ForegroundColor Yellow
    Write-Host ""
}

# Start the API
Write-Host "🚀 Starting Wekeza Core Banking API..." -ForegroundColor Green
Write-Host ""
Write-Host "🌐 API will be available at:" -ForegroundColor Cyan
Write-Host "  • HTTPS: https://localhost:7001" -ForegroundColor White
Write-Host "  • HTTP:  http://localhost:5001" -ForegroundColor White
Write-Host "  • Swagger: https://localhost:7001/swagger" -ForegroundColor White
Write-Host ""
Write-Host "📊 Health Check: https://localhost:7001/health" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Gray
Write-Host ""

# Change to API directory and run
try {
    Set-Location "Core\Wekeza.Core.Api"
    & $dotnetPath run --configuration Debug
} catch {
    Write-Host "❌ Failed to start the API: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    Set-Location "..\..\"
    Write-Host ""
    Write-Host "🏦 Wekeza Core Banking System stopped." -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
}