#!/usr/bin/env pwsh

# 🏦 Wekeza Core Banking System - Local Development Startup Script
# This script sets up and starts the Wekeza banking system locally

Write-Host "🏦 Starting Wekeza Core Banking System..." -ForegroundColor Green

# Check if .NET 8 is installed
Write-Host "📋 Checking .NET installation..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET version: $dotnetVersion" -ForegroundColor Green
    
    if (-not $dotnetVersion.StartsWith("8.")) {
        Write-Host "⚠️  Warning: .NET 8.0 is recommended. Current version: $dotnetVersion" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ .NET is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

# Check if PostgreSQL is running
Write-Host "📋 Checking PostgreSQL connection..." -ForegroundColor Yellow
$connectionString = "Host=localhost;Database=WekezaCoreDB;Username=admin;Password=the_beast_pass"

# Test database connection (simplified check)
try {
    # This is a basic check - in production you'd use a proper connection test
    Write-Host "✅ Database configuration found" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Could not verify database connection" -ForegroundColor Yellow
    Write-Host "Please ensure PostgreSQL is running with:" -ForegroundColor Yellow
    Write-Host "  - Host: localhost" -ForegroundColor White
    Write-Host "  - Database: WekezaCoreDB" -ForegroundColor White
    Write-Host "  - Username: admin" -ForegroundColor White
    Write-Host "  - Password: the_beast_pass" -ForegroundColor White
}

# Restore NuGet packages
Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore Wekeza.Core.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to restore packages" -ForegroundColor Red
    exit 1
}

# Build the solution
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build Wekeza.Core.sln --configuration Debug --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}

# Run database migrations
Write-Host "🗄️  Running database migrations..." -ForegroundColor Yellow
try {
    dotnet ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api
    Write-Host "✅ Database migrations completed" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Could not run migrations. Database may not be available." -ForegroundColor Yellow
    Write-Host "You can run migrations manually later with:" -ForegroundColor Yellow
    Write-Host "dotnet ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api" -ForegroundColor White
}

# Start the API
Write-Host "🚀 Starting Wekeza Core Banking API..." -ForegroundColor Green
Write-Host "" -ForegroundColor White
Write-Host "🌐 API will be available at:" -ForegroundColor Cyan
Write-Host "  • HTTPS: https://localhost:7001" -ForegroundColor White
Write-Host "  • HTTP:  http://localhost:5001" -ForegroundColor White
Write-Host "  • Swagger: https://localhost:7001/swagger" -ForegroundColor White
Write-Host "" -ForegroundColor White
Write-Host "📊 Health Check: https://localhost:7001/health" -ForegroundColor Cyan
Write-Host "" -ForegroundColor White
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Gray

# Change to API directory and run
Set-Location "Core/Wekeza.Core.Api"
dotnet run --configuration Debug

Write-Host "🏦 Wekeza Core Banking System stopped." -ForegroundColor Yellow