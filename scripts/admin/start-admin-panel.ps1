#!/usr/bin/env pwsh

Write-Host "🏦 Starting Wekeza Bank Administrator Panel..." -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Navigate to the Core API project
$projectPath = "Core/Wekeza.Core.Api"
if (-not (Test-Path $projectPath)) {
    Write-Host "❌ Project path not found: $projectPath" -ForegroundColor Red
    exit 1
}

Write-Host "📁 Project Path: $projectPath" -ForegroundColor Yellow

# Build the project
Write-Host "🔨 Building the project..." -ForegroundColor Yellow
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
Write-Host "🚀 Starting Wekeza Core Banking System..." -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Administrator Panel will be available at:" -ForegroundColor Cyan
Write-Host "   http://localhost:5000/admin" -ForegroundColor White
Write-Host ""
Write-Host "🔐 Test Login Credentials:" -ForegroundColor Cyan
Write-Host "   Administrator: admin / password" -ForegroundColor White
Write-Host "   Teller: teller / password" -ForegroundColor White
Write-Host "   Loan Officer: loanofficer / password" -ForegroundColor White
Write-Host "   Risk Officer: riskofficer / password" -ForegroundColor White
Write-Host ""
Write-Host "📚 API Documentation: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host "================================================" -ForegroundColor Cyan

try {
    dotnet run --urls "http://localhost:5000"
} catch {
    Write-Host "❌ Failed to start the application: $_" -ForegroundColor Red
    exit 1
} finally {
    Set-Location "../.."
}