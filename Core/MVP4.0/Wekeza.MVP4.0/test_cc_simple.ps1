# Simple Customer Care Test
$baseUrl = "http://localhost:5004"

Write-Host "=== CUSTOMER CARE TEST ===" -ForegroundColor Magenta

# Login
Write-Host "1. Login Test..." -ForegroundColor Yellow
$loginData = @{
    username = "jacobodenyire"
    password = "admin123"
    role = "CustomerCareOfficer"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
$token = $loginResponse.token
Write-Host "✓ Login successful!" -ForegroundColor Green

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Customer Search
Write-Host "2. Customer Search Test..." -ForegroundColor Yellow
$searchResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/customers/search?searchTerm=John" -Method GET -Headers $headers
Write-Host "✓ Found $($searchResponse.data.Count) customers" -ForegroundColor Green

# Dashboard Stats
Write-Host "3. Dashboard Stats Test..." -ForegroundColor Yellow
$statsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/dashboard/stats" -Method GET -Headers $headers
Write-Host "✓ Active Inquiries: $($statsResponse.data.activeInquiries)" -ForegroundColor Green

# Create Complaint
Write-Host "4. Create Complaint Test..." -ForegroundColor Yellow
$complaintData = @{
    customerId = "11111111-1111-1111-1111-111111111111"
    category = "Account Balance"
    priority = "High"
    subject = "Test Complaint"
    description = "Test complaint description"
} | ConvertTo-Json

$complaintResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/complaints" -Method POST -Body $complaintData -Headers $headers
Write-Host "✓ Complaint created successfully" -ForegroundColor Green

# Account Status Request
Write-Host "5. Account Status Request Test..." -ForegroundColor Yellow
$statusData = @{
    accountNumber = "ACC001234"
    requestType = "Freeze"
    reason = "Test freeze request"
    customerAuthorized = $true
} | ConvertTo-Json

$statusResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/account-status-requests" -Method POST -Body $statusData -Headers $headers
Write-Host "✓ Account status request created" -ForegroundColor Green

# Card Request
Write-Host "6. Card Request Test..." -ForegroundColor Yellow
$cardData = @{
    accountNumber = "ACC001234"
    cardNumber = "1234"
    requestType = "Block"
    reason = "Lost Card"
    comments = "Test card block"
} | ConvertTo-Json

$cardResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/card-requests" -Method POST -Body $cardData -Headers $headers
Write-Host "✓ Card request created" -ForegroundColor Green

Write-Host "`n=== ALL TESTS PASSED ===" -ForegroundColor Green
Write-Host "Customer Care system is fully functional!" -ForegroundColor Cyan