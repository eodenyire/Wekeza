# Simple Customer Care API Test
$baseUrl = "http://localhost:5004"

# Login
$loginData = @{
    username = "jacobodenyire"
    password = "admin123"
    role = "CustomerCareOfficer"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
$token = $loginResponse.token
Write-Host "Login successful. Token received." -ForegroundColor Green

# Test Customer Search
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Write-Host "`nTesting Customer Search..." -ForegroundColor Yellow
$searchResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/customers/search?searchTerm=Doe" -Method GET -Headers $headers
Write-Host "Customer search: Found $($searchResponse.data.Count) customers" -ForegroundColor Cyan

# Test Dashboard Stats
Write-Host "`nTesting Dashboard Stats..." -ForegroundColor Yellow
$statsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/dashboard/stats" -Method GET -Headers $headers
Write-Host "Dashboard stats retrieved successfully" -ForegroundColor Cyan
Write-Host "Active Inquiries: $($statsResponse.data.activeInquiries)" -ForegroundColor White
Write-Host "Resolved Today: $($statsResponse.data.resolvedToday)" -ForegroundColor White

# Test Account Status Requests
Write-Host "`nTesting Account Status Requests..." -ForegroundColor Yellow
$requestsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/account-status-requests" -Method GET -Headers $headers
Write-Host "Account status requests: Found $($requestsResponse.data.Count) requests" -ForegroundColor Cyan

# Test Card Requests
Write-Host "`nTesting Card Requests..." -ForegroundColor Yellow
$cardRequestsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/card-requests" -Method GET -Headers $headers
Write-Host "Card requests: Found $($cardRequestsResponse.data.Count) requests" -ForegroundColor Cyan

# Test Complaints
Write-Host "`nTesting Complaints..." -ForegroundColor Yellow
$complaintsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/complaints" -Method GET -Headers $headers
Write-Host "Complaints: Found $($complaintsResponse.data.Count) complaints" -ForegroundColor Cyan

Write-Host "`nAll Customer Care API tests completed successfully!" -ForegroundColor Green