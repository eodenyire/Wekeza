#!/usr/bin/env pwsh

Write-Host "🏦 Starting Wekeza Comprehensive Banking System with Admin Panel..." -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Cyan

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Navigate to the ComprehensiveWekezaApi project
$projectPath = "ComprehensiveWekezaApi"
if (-not (Test-Path $projectPath)) {
    Write-Host "❌ Project path not found: $projectPath" -ForegroundColor Red
    exit 1
}

Write-Host "📁 Project Path: $projectPath" -ForegroundColor Yellow

# Build the project
Write-Host "🔨 Building the Comprehensive Banking System..." -ForegroundColor Yellow
Set-Location $projectPath

try {
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Build failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Build successful" -ForegroundColor Green
} catch {
    Write-Host "❌ Build error: $_" -ForegroundColor Red
    exit 1
}

# Start the application
Write-Host "🚀 Starting Wekeza Comprehensive Banking System..." -ForegroundColor Green
Write-Host ""
Write-Host "🌐 System will be available at:" -ForegroundColor Cyan
Write-Host "   Main System: http://localhost:5003" -ForegroundColor White
Write-Host "   Administrator Panel: http://localhost:5003/admin" -ForegroundColor White
Write-Host ""
Write-Host "🔐 Admin Panel Login Credentials:" -ForegroundColor Cyan
Write-Host "   Administrator: admin / password - Full System Access" -ForegroundColor White
Write-Host "   Branch Manager: manager / password - Branch Operations" -ForegroundColor White
Write-Host "   Teller: teller / password - Teller Operations" -ForegroundColor White
Write-Host "   Loan Officer: loanofficer / password - Loan Management" -ForegroundColor White
Write-Host "   Treasury Officer: treasury / password - Treasury Operations" -ForegroundColor White
Write-Host "   Compliance Officer: compliance / password - Compliance `& Risk" -ForegroundColor White
Write-Host "   Payment Officer: payments / password - Payment Processing" -ForegroundColor White
Write-Host "   Trade Finance Officer: tradefinance / password - Trade Finance" -ForegroundColor White
Write-Host ""
Write-Host "📚 Documentation: http://localhost:5003/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "🏦 System Features:" -ForegroundColor Cyan
Write-Host "   • 18 Banking Modules" -ForegroundColor White
Write-Host "   • 85+ API Endpoints" -ForegroundColor White
Write-Host "   • Role-Based Access Control" -ForegroundColor White
Write-Host "   • Comprehensive Admin Panel" -ForegroundColor White
Write-Host "   • PostgreSQL Database" -ForegroundColor White
Write-Host "   • Real-time Operations" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host "=================================================================" -ForegroundColor Cyan

try {
    dotnet run --urls "http://localhost:5003"
} catch {
    Write-Host "❌ Failed to start the application: $_" -ForegroundColor Red
    exit 1
} finally {
    Set-Location ".."
}