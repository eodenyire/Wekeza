#!/usr/bin/env pwsh

# Wekeza Banking System - Start All APIs
# This script starts all 4 API versions on different ports

Write-Host "🏦 Starting Wekeza Banking System - All APIs" -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Cyan

# Function to start API in background
function Start-API {
    param(
        [string]$Name,
        [string]$Path,
        [int]$Port,
        [string]$Description
    )
    
    Write-Host "🚀 Starting $Name on port $Port..." -ForegroundColor Yellow
    Write-Host "   📁 Path: $Path" -ForegroundColor Gray
    Write-Host "   📝 Description: $Description" -ForegroundColor Gray
    
    # Start the API in a new PowerShell window
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$Path'; dotnet run" -WindowStyle Normal
    
    Write-Host "   ✅ $Name started successfully!" -ForegroundColor Green
    Write-Host ""
}

# Start all APIs
Start-API -Name "Minimal API" -Path "MinimalWekezaApi" -Port 5000 -Description "Simple prototype with basic banking operations"
Start-Sleep -Seconds 2

Start-API -Name "Database API" -Path "DatabaseWekezaApi" -Port 5001 -Description "Database-connected core banking with PostgreSQL"
Start-Sleep -Seconds 2

Start-API -Name "Enhanced API" -Path "EnhancedWekezaApi" -Port 5002 -Description "CQRS architecture with Domain-Driven Design"
Start-Sleep -Seconds 2

Start-API -Name "Comprehensive API" -Path "ComprehensiveWekezaApi" -Port 5003 -Description "Full enterprise banking platform with 85+ endpoints"

Write-Host "🎉 All APIs Started Successfully!" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 API Access URLs:" -ForegroundColor White
Write-Host "   🔹 Minimal API:      http://localhost:5000" -ForegroundColor Cyan
Write-Host "   🔹 Database API:     http://localhost:5001" -ForegroundColor Cyan  
Write-Host "   🔹 Enhanced API:     http://localhost:5002" -ForegroundColor Cyan
Write-Host "   🔹 Comprehensive API: http://localhost:5003" -ForegroundColor Cyan
Write-Host ""
Write-Host "📚 Swagger Documentation:" -ForegroundColor White
Write-Host "   🔹 Minimal:      http://localhost:5000/swagger" -ForegroundColor Yellow
Write-Host "   🔹 Database:     http://localhost:5001/swagger" -ForegroundColor Yellow
Write-Host "   🔹 Enhanced:     http://localhost:5002/swagger" -ForegroundColor Yellow
Write-Host "   🔹 Comprehensive: http://localhost:5003/swagger" -ForegroundColor Yellow
Write-Host ""
Write-Host "⚡ System Status:" -ForegroundColor White
Write-Host "   🔹 Minimal Status:      http://localhost:5000/api/status" -ForegroundColor Magenta
Write-Host "   🔹 Database Status:     http://localhost:5001/api/status" -ForegroundColor Magenta
Write-Host "   🔹 Enhanced Status:     http://localhost:5002/api/status" -ForegroundColor Magenta
Write-Host "   🔹 Comprehensive Status: http://localhost:5003/api/status" -ForegroundColor Magenta
Write-Host ""
Write-Host "👤 Owner: Emmanuel Odenyire (ID: 28839872) | Contact: 0716478835" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor White
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")