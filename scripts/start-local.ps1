# Wekeza Bank - Local Development Startup Script
# This script starts the application with proper environment setup

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Wekeza Bank - Starting Application" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get solution root
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = Split-Path -Parent $scriptPath
$apiProject = Join-Path $solutionRoot "Core\Wekeza.Core.Api"

# Set environment
$env:ASPNETCORE_ENVIRONMENT = "Development"

Write-Host "Environment: Development" -ForegroundColor Yellow
Write-Host "Project: $apiProject" -ForegroundColor Yellow
Write-Host ""

# Check if PostgreSQL is running
Write-Host "Checking PostgreSQL service..." -ForegroundColor Yellow
$pgService = Get-Service -Name "postgresql*" -ErrorAction SilentlyContinue

if ($pgService) {
    if ($pgService.Status -eq "Running") {
        Write-Host "✓ PostgreSQL is running" -ForegroundColor Green
    } else {
        Write-Host "⚠ PostgreSQL service found but not running" -ForegroundColor Yellow
        Write-Host "Attempting to start PostgreSQL..." -ForegroundColor Yellow
        Start-Service $pgService.Name
        Start-Sleep -Seconds 2
        Write-Host "✓ PostgreSQL started" -ForegroundColor Green
    }
} else {
    Write-Host "⚠ PostgreSQL service not found" -ForegroundColor Yellow
    Write-Host "Please ensure PostgreSQL is installed and running" -ForegroundColor Yellow
}
Write-Host ""

# Build the solution
Write-Host "Building solution..." -ForegroundColor Yellow
Set-Location $solutionRoot
dotnet build --configuration Debug

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build successful" -ForegroundColor Green
Write-Host ""

# Run the application
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Starting Wekeza Bank API..." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Swagger UI will be available at:" -ForegroundColor Yellow
Write-Host "  https://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host "  http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Yellow
Write-Host ""

Set-Location $apiProject
dotnet run
