#!/usr/bin/env pwsh

Write-Host "🧪 Testing Wekeza Bank Administrator Panel..." -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Cyan

$baseUrl = "http://localhost:5000"
$adminUrl = "$baseUrl/admin"
$apiUrl = "$baseUrl/api"

# Test if server is running
Write-Host "🔍 Checking if server is running..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri $baseUrl -Method Get -TimeoutSec 5
    Write-Host "✅ Server is running" -ForegroundColor Green
    Write-Host "   Service: $($response.Service)" -ForegroundColor White
    Write-Host "   Version: $($response.Version)" -ForegroundColor White
    Write-Host "   Status: $($response.Status)" -ForegroundColor White
} catch {
    Write-Host "❌ Server is not running. Please start it first with: ./start-admin-panel.ps1" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test authentication endpoint
Write-Host "🔐 Testing Authentication..." -ForegroundColor Yellow
try {
    $loginData = @{
        username = "admin"
        password = "password"
    } | ConvertTo-Json

    $authResponse = Invoke-RestMethod -Uri "$apiUrl/authentication/login" -Method Post -Body $loginData -ContentType "application/json"
    $token = $authResponse.token
    Write-Host "✅ Authentication successful" -ForegroundColor Green
    Write-Host "   Username: $($authResponse.username)" -ForegroundColor White
    Write-Host "   Roles: $($authResponse.roles -join ', ')" -ForegroundColor White
} catch {
    Write-Host "❌ Authentication failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test admin panel endpoints
Write-Host "🏛️ Testing Admin Panel Endpoints..." -ForegroundColor Yellow

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Test dashboard
try {
    $dashboardResponse = Invoke-RestMethod -Uri $adminUrl -Method Get -Headers $headers
    Write-Host "✅ Dashboard endpoint working" -ForegroundColor Green
    Write-Host "   Title: $($dashboardResponse.title)" -ForegroundColor White
    Write-Host "   Dashboard Items: $($dashboardResponse.dashboard.Count)" -ForegroundColor White
} catch {
    Write-Host "❌ Dashboard endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test user management
try {
    $userMgmtResponse = Invoke-RestMethod -Uri "$adminUrl/users" -Method Get -Headers $headers
    Write-Host "✅ User Management endpoint working" -ForegroundColor Green
    Write-Host "   Title: $($userMgmtResponse.title)" -ForegroundColor White
} catch {
    Write-Host "❌ User Management endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test staff creation
try {
    $staffCreateResponse = Invoke-RestMethod -Uri "$adminUrl/staff/create" -Method Get -Headers $headers
    Write-Host "✅ Staff Creation endpoint working" -ForegroundColor Green
    Write-Host "   Title: $($staffCreateResponse.title)" -ForegroundColor White
} catch {
    Write-Host "❌ Staff Creation endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test teller operations
try {
    $tellerResponse = Invoke-RestMethod -Uri "$adminUrl/teller" -Method Get -Headers $headers
    Write-Host "✅ Teller Operations endpoint working" -ForegroundColor Green
    Write-Host "   Operations: $($tellerResponse.operations.Count)" -ForegroundColor White
} catch {
    Write-Host "❌ Teller Operations endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test staff management API
Write-Host "👥 Testing Staff Management API..." -ForegroundColor Yellow

try {
    $staffResponse = Invoke-RestMethod -Uri "$apiUrl/staffmanagement" -Method Get -Headers $headers
    Write-Host "✅ Staff Management API working" -ForegroundColor Green
    Write-Host "   Staff Count: $($staffResponse.data.Count)" -ForegroundColor White
} catch {
    Write-Host "❌ Staff Management API failed: $($_.Exception.Message)" -ForegroundColor Red
}

try {
    $statsResponse = Invoke-RestMethod -Uri "$apiUrl/staffmanagement/statistics" -Method Get -Headers $headers
    Write-Host "✅ Staff Statistics API working" -ForegroundColor Green
    Write-Host "   Total Staff: $($statsResponse.data.totalStaff)" -ForegroundColor White
    Write-Host "   Active Staff: $($statsResponse.data.activeStaff)" -ForegroundColor White
} catch {
    Write-Host "❌ Staff Statistics API failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎉 Testing Complete!" -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "🌐 Open your browser and navigate to:" -ForegroundColor Cyan
Write-Host "   $adminUrl" -ForegroundColor White
Write-Host ""
Write-Host "🔐 Login with:" -ForegroundColor Cyan
Write-Host "   Username: admin" -ForegroundColor White
Write-Host "   Password: password" -ForegroundColor White