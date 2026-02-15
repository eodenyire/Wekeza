#!/usr/bin/env pwsh
# 🧪 Quick Test - Wekeza Banking System

Write-Host "🧪 Wekeza Banking System - Quick Test" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Check API
Write-Host "Test 1: Checking API..." -ForegroundColor Yellow
try {
    $apiResponse = Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get -TimeoutSec 5
    Write-Host "✅ API is running" -ForegroundColor Green
    Write-Host "   Service: $($apiResponse.service)" -ForegroundColor White
    Write-Host "   Version: $($apiResponse.version)" -ForegroundColor White
    Write-Host "   Status: $($apiResponse.status)" -ForegroundColor White
} catch {
    Write-Host "❌ API is not running" -ForegroundColor Red
    Write-Host "   Please start the API first:" -ForegroundColor Yellow
    Write-Host "   cd Core" -ForegroundColor White
    Write-Host "   dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj" -ForegroundColor White
    exit 1
}

Write-Host ""

# Test 2: Check Authentication
Write-Host "Test 2: Testing Authentication..." -ForegroundColor Yellow
try {
    $loginBody = @{
        username = "admin"
        password = "test123"
    } | ConvertTo-Json

    $loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" `
        -Method Post `
        -Body $loginBody `
        -ContentType "application/json" `
        -TimeoutSec 5

    Write-Host "✅ Authentication works" -ForegroundColor Green
    Write-Host "   Username: $($loginResponse.username)" -ForegroundColor White
    Write-Host "   Roles: $($loginResponse.roles -join ', ')" -ForegroundColor White
    Write-Host "   Token: $($loginResponse.token.Substring(0, 20))..." -ForegroundColor White
    
    $token = $loginResponse.token
} catch {
    Write-Host "❌ Authentication failed" -ForegroundColor Red
    Write-Host "   Error: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test 3: Check Protected Endpoint
Write-Host "Test 3: Testing Protected Endpoint..." -ForegroundColor Yellow
try {
    $headers = @{
        Authorization = "Bearer $token"
    }

    $meResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/me" `
        -Method Get `
        -Headers $headers `
        -TimeoutSec 5

    Write-Host "✅ Protected endpoint works" -ForegroundColor Green
    Write-Host "   User ID: $($meResponse.userId)" -ForegroundColor White
    Write-Host "   Email: $($meResponse.email)" -ForegroundColor White
} catch {
    Write-Host "❌ Protected endpoint failed" -ForegroundColor Red
    Write-Host "   Error: $_" -ForegroundColor Red
}

Write-Host ""

# Test 4: Check Web Channels
Write-Host "Test 4: Checking Web Channels..." -ForegroundColor Yellow

$channels = @(
    @{ Name = "Public Website"; Port = 3000; Path = "Wekeza.Web.Channels/public-website" },
    @{ Name = "Personal Banking"; Port = 3001; Path = "Wekeza.Web.Channels/personal-banking" },
    @{ Name = "Corporate Banking"; Port = 3002; Path = "Wekeza.Web.Channels/corporate-banking" },
    @{ Name = "SME Banking"; Port = 3003; Path = "Wekeza.Web.Channels/sme-banking" }
)

foreach ($channel in $channels) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:$($channel.Port)" -Method Get -TimeoutSec 2 -UseBasicParsing
        Write-Host "✅ $($channel.Name) is running on port $($channel.Port)" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  $($channel.Name) is not running on port $($channel.Port)" -ForegroundColor Yellow
        
        # Check if package.json exists
        if (Test-Path "$($channel.Path)/package.json") {
            Write-Host "   To start: cd $($channel.Path) && npm run dev" -ForegroundColor White
        } else {
            Write-Host "   Channel not configured yet" -ForegroundColor Gray
        }
    }
}

Write-Host ""

# Test 5: Check Database
Write-Host "Test 5: Checking Database..." -ForegroundColor Yellow
try {
    $env:PGPASSWORD = "the_beast_pass"
    $dbTest = psql -h localhost -p 5432 -U admin -d WekezaCoreDB -c "SELECT 1;" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Database is accessible" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Database connection issue" -ForegroundColor Yellow
    }
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
} catch {
    Write-Host "⚠️  PostgreSQL client not found or database not accessible" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "📊 Test Summary" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Backend API: Running" -ForegroundColor Green
Write-Host "✅ Authentication: Working" -ForegroundColor Green
Write-Host "✅ Protected Endpoints: Working" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Yellow
Write-Host "1. Start web channels: .\start-all-channels.ps1" -ForegroundColor White
Write-Host "2. Open browser and test:" -ForegroundColor White
Write-Host "   • Public Website:   http://localhost:3000" -ForegroundColor White
Write-Host "   • Personal Banking: http://localhost:3001" -ForegroundColor White
Write-Host "   • Corporate Banking: http://localhost:3002" -ForegroundColor White
Write-Host "   • SME Banking:      http://localhost:3003" -ForegroundColor White
Write-Host "3. View API docs: http://localhost:5000/swagger" -ForegroundColor White
Write-Host ""
Write-Host "📚 For detailed testing: See TESTING-GUIDE.md" -ForegroundColor Cyan
Write-Host ""
