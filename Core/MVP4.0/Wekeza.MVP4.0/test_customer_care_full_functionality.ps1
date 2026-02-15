# Comprehensive Customer Care functionality test
$baseUrl = "http://localhost:5004"

Write-Host "=== COMPREHENSIVE CUSTOMER CARE FUNCTIONALITY TEST ===" -ForegroundColor Magenta

# Test 1: Login and get token
Write-Host "1. Testing Customer Care Login..." -ForegroundColor Yellow
$loginData = @{
    username = "jacobodenyire"
    password = "admin123"
    role = "CustomerCareOfficer"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
    Write-Host "Login successful: $($response.user.fullName)" -ForegroundColor Green
    
    $authToken = $response.token
    $headers = @{
        "Authorization" = "Bearer $authToken"
        "Content-Type" = "application/json"
    }
    
    # Test 2: Customer Search
    Write-Host "2. Testing Customer Search..." -ForegroundColor Yellow
    $searchResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/customers/search?searchTerm=John" -Method GET -Headers $headers
    Write-Host "Customer search working: Found $($searchResponse.data.Count) customers" -ForegroundColor Green
    
    # Test 3: Dashboard Statistics
    Write-Host "3. Testing Dashboard Statistics..." -ForegroundColor Yellow
    $statsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/dashboard/stats" -Method GET -Headers $headers
    Write-Host "Dashboard stats loaded:" -ForegroundColor Green
    Write-Host "Active Inquiries: $($statsResponse.data.activeInquiries)" -ForegroundColor Cyan
    Write-Host "Resolved Today: $($statsResponse.data.resolvedToday)" -ForegroundColor Cyan
    
    # Test 4: Complaints
    Write-Host "4. Testing Complaints Management..." -ForegroundColor Yellow
    $complaintsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/complaints" -Method GET -Headers $headers
    Write-Host "Complaints loaded: $($complaintsResponse.data.Count) complaints" -ForegroundColor Green
    
    Write-Host "=== CUSTOMER CARE FUNCTIONALITY STATUS ===" -ForegroundColor Magenta
    Write-Host "Authentication: Working" -ForegroundColor Green
    Write-Host "Customer Search: Working" -ForegroundColor Green
    Write-Host "Dashboard Statistics: Working" -ForegroundColor Green
    Write-Host "Complaints Management: Working" -ForegroundColor Green
    
} catch {
    Write-Host "Test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Customer Care system testing completed!" -ForegroundColor Green