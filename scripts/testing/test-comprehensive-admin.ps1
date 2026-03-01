#!/usr/bin/env pwsh

Write-Host "🧪 Testing Wekeza Comprehensive Banking Admin Panel..." -ForegroundColor Green
Write-Host "=====================================================" -ForegroundColor Cyan

$baseUrl = "http://localhost:5003"
$adminUrl = "$baseUrl/admin"

# Test if server is running
Write-Host "🔍 Checking if Comprehensive Banking System is running..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri $baseUrl -Method Get -TimeoutSec 5
    Write-Host "✅ Comprehensive Banking System is running" -ForegroundColor Green
    Write-Host "   Port: 5003" -ForegroundColor White
    Write-Host "   Status: Operational" -ForegroundColor White
} catch {
    Write-Host "❌ Server is not running. Please start it first with: ./start-comprehensive-admin.ps1" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Test authentication endpoint
Write-Host "🔐 Testing Admin Authentication..." -ForegroundColor Yellow
try {
    $loginData = @{
        username = "admin"
        password = "password"
    } | ConvertTo-Json

    $authResponse = Invoke-RestMethod -Uri "$adminUrl/login" -Method Post -Body $loginData -ContentType "application/json"
    $token = $authResponse.token
    Write-Host "✅ Admin authentication successful" -ForegroundColor Green
    Write-Host "   Username: $($authResponse.username)" -ForegroundColor White
    Write-Host "   Roles: $($authResponse.roles -join ', ')" -ForegroundColor White
    Write-Host "   System Access: $($authResponse.systemAccess.accessibleModules)/$($authResponse.systemAccess.totalModules) modules" -ForegroundColor White
} catch {
    Write-Host "❌ Admin authentication failed: $($_.Exception.Message)" -ForegroundColor Red
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
    Write-Host "   System Version: $($dashboardResponse.systemInfo.version)" -ForegroundColor White
    Write-Host "   Modules: $($dashboardResponse.systemInfo.modules)" -ForegroundColor White
} catch {
    Write-Host "❌ Dashboard endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test banking module endpoints
$modules = @(
    @{ Name = "CIF"; Endpoint = "/cif" },
    @{ Name = "Accounts"; Endpoint = "/accounts" },
    @{ Name = "Transactions"; Endpoint = "/transactions" },
    @{ Name = "Loans"; Endpoint = "/loans" },
    @{ Name = "Deposits"; Endpoint = "/deposits" },
    @{ Name = "Teller"; Endpoint = "/teller" },
    @{ Name = "Treasury"; Endpoint = "/treasury" },
    @{ Name = "Trade Finance"; Endpoint = "/trade-finance" },
    @{ Name = "Payments"; Endpoint = "/payments" },
    @{ Name = "Compliance"; Endpoint = "/compliance" }
)

Write-Host ""
Write-Host "🏦 Testing Banking Module Endpoints..." -ForegroundColor Yellow

foreach ($module in $modules) {
    try {
        $moduleResponse = Invoke-RestMethod -Uri "$adminUrl$($module.Endpoint)" -Method Get -Headers $headers
        Write-Host "✅ $($module.Name) module endpoint working" -ForegroundColor Green
        Write-Host "   Operations: $($moduleResponse.operations.Count)" -ForegroundColor White
    } catch {
        Write-Host "❌ $($module.Name) module endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Test user management
Write-Host ""
Write-Host "👥 Testing User Management..." -ForegroundColor Yellow
try {
    $userMgmtResponse = Invoke-RestMethod -Uri "$adminUrl/users" -Method Get -Headers $headers
    Write-Host "✅ User Management endpoint working" -ForegroundColor Green
    Write-Host "   Available Role Categories: $($userMgmtResponse.availableRoles.PSObject.Properties.Count)" -ForegroundColor White
} catch {
    Write-Host "❌ User Management endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test staff creation
try {
    $staffCreateResponse = Invoke-RestMethod -Uri "$adminUrl/staff/create" -Method Get -Headers $headers
    Write-Host "✅ Staff Creation endpoint working" -ForegroundColor Green
    Write-Host "   Staff Categories: $($staffCreateResponse.staffTypes.Count)" -ForegroundColor White
    Write-Host "   Available Branches: $($staffCreateResponse.branches.Count)" -ForegroundColor White
    Write-Host "   Departments: $($staffCreateResponse.departments.Count)" -ForegroundColor White
} catch {
    Write-Host "❌ Staff Creation endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test different user roles
Write-Host "🎭 Testing Different User Roles..." -ForegroundColor Yellow

$testRoles = @(
    @{ Username = "manager"; Role = "Branch Manager" },
    @{ Username = "teller"; Role = "Teller" },
    @{ Username = "loanofficer"; Role = "Loan Officer" },
    @{ Username = "treasury"; Role = "Treasury Officer" },
    @{ Username = "compliance"; Role = "Compliance Officer" }
)

foreach ($testRole in $testRoles) {
    try {
        $roleLoginData = @{
            username = $testRole.Username
            password = "password"
        } | ConvertTo-Json

        $roleAuthResponse = Invoke-RestMethod -Uri "$adminUrl/login" -Method Post -Body $roleLoginData -ContentType "application/json"
        Write-Host "✅ $($testRole.Role) login successful" -ForegroundColor Green
        Write-Host "   Module Access: $($roleAuthResponse.systemAccess.accessibleModules)/$($roleAuthResponse.systemAccess.totalModules)" -ForegroundColor White
    } catch {
        Write-Host "❌ $($testRole.Role) login failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# Test API endpoints
Write-Host "🔗 Testing Core API Endpoints..." -ForegroundColor Yellow

$apiEndpoints = @(
    @{ Name = "System Status"; Endpoint = "/api/status" },
    @{ Name = "CIF Individual"; Endpoint = "/api/cif/individual"; Method = "POST"; Body = @{ FirstName = "Test"; LastName = "User"; Email = "test@example.com"; PhoneNumber = "+254700000000"; IdentificationNumber = "12345678" } },
    @{ Name = "Account Opening"; Endpoint = "/api/accounts/product-based"; Method = "POST"; Body = @{ CustomerId = [Guid]::NewGuid(); ProductId = [Guid]::NewGuid(); Currency = "KES"; InitialDeposit = 1000 } }
)

foreach ($endpoint in $apiEndpoints) {
    try {
        if ($endpoint.Method -eq "POST") {
            $body = $endpoint.Body | ConvertTo-Json
            $apiResponse = Invoke-RestMethod -Uri "$baseUrl$($endpoint.Endpoint)" -Method Post -Body $body -ContentType "application/json"
        } else {
            $apiResponse = Invoke-RestMethod -Uri "$baseUrl$($endpoint.Endpoint)" -Method Get
        }
        Write-Host "✅ $($endpoint.Name) API endpoint working" -ForegroundColor Green
    } catch {
        Write-Host "❌ $($endpoint.Name) API endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "🎉 Testing Complete!" -ForegroundColor Green
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "🌐 Open your browser and navigate to:" -ForegroundColor Cyan
Write-Host "   Admin Panel: http://localhost:5003/admin" -ForegroundColor White
Write-Host "   System Overview: http://localhost:5003" -ForegroundColor White
Write-Host "   API Documentation: http://localhost:5003/swagger" -ForegroundColor White
Write-Host ""
Write-Host "🔐 Login with any of these credentials:" -ForegroundColor Cyan
Write-Host "   admin/password - Full System Access" -ForegroundColor White
Write-Host "   manager/password - Branch Manager Access" -ForegroundColor White
Write-Host "   teller/password - Teller Operations" -ForegroundColor White
Write-Host "   loanofficer/password - Loan Management" -ForegroundColor White
Write-Host "   treasury/password - Treasury Operations" -ForegroundColor White
Write-Host "   compliance/password - Compliance & Risk" -ForegroundColor White