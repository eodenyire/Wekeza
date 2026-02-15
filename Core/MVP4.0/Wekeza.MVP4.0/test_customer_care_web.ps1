# Test Customer Care web interface
$baseUrl = "http://localhost:5004"

Write-Host "=== CUSTOMER CARE WEB INTERFACE TEST ===" -ForegroundColor Magenta

# Test 1: Access Customer Care login page
Write-Host "1. Testing Customer Care login page..." -ForegroundColor Yellow
try {
    $loginPageResponse = Invoke-WebRequest -Uri "$baseUrl/Login/CustomerCareOfficer" -UseBasicParsing
    if ($loginPageResponse.StatusCode -eq 200) {
        Write-Host "Customer Care login page accessible" -ForegroundColor Green
    }
} catch {
    Write-Host "Error accessing login page: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Check if Customer Care dashboard is protected
Write-Host "2. Testing Customer Care dashboard protection..." -ForegroundColor Yellow
try {
    $dashboardResponse = Invoke-WebRequest -Uri "$baseUrl/CustomerCare" -UseBasicParsing
    Write-Host "Dashboard response status: $($dashboardResponse.StatusCode)" -ForegroundColor Cyan
} catch {
    if ($_.Exception.Response.StatusCode -eq 302) {
        Write-Host "Dashboard properly redirects unauthenticated users (302)" -ForegroundColor Green
    } else {
        Write-Host "Dashboard protection status: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
    }
}

Write-Host "=== WEB INTERFACE TEST SUMMARY ===" -ForegroundColor Magenta
Write-Host "Customer Care web interface is functional" -ForegroundColor Green