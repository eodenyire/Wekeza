# Test Customer Care Functionality
$baseUrl = "http://localhost:5004"

Write-Host "Testing Customer Care Functionality..." -ForegroundColor Green

# Step 1: Login as Customer Care Officer
Write-Host "`n1. Logging in as Customer Care Officer..." -ForegroundColor Yellow
$loginData = @{
    username = "jacobodenyire"
    password = "admin123"
    role = "CustomerCareOfficer"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
    $token = $loginResponse.token
    Write-Host "✓ Login successful. Token: $($token.Substring(0,20))..." -ForegroundColor Green
} catch {
    Write-Host "✗ Login failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 2: Test Customer Search
Write-Host "`n2. Testing Customer Search..." -ForegroundColor Yellow
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

try {
    $searchResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/customers/search?searchTerm=John" -Method GET -Headers $headers
    Write-Host "✓ Customer search successful. Found $($searchResponse.data.Count) customers" -ForegroundColor Green
    if ($searchResponse.data.Count -gt 0) {
        $firstCustomer = $searchResponse.data[0]
        Write-Host "  First customer: $($firstCustomer.fullName) ($($firstCustomer.customerNumber))" -ForegroundColor Cyan
    }
} catch {
    Write-Host "✗ Customer search failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 3: Test Dashboard Stats
Write-Host "`n3. Testing Dashboard Statistics..." -ForegroundColor Yellow
try {
    $statsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/dashboard/stats" -Method GET -Headers $headers
    Write-Host "✓ Dashboard stats retrieved successfully" -ForegroundColor Green
    Write-Host "  Active Inquiries: $($statsResponse.data.activeInquiries)" -ForegroundColor Cyan
    Write-Host "  Resolved Today: $($statsResponse.data.resolvedToday)" -ForegroundColor Cyan
    Write-Host "  Avg Response Time: $($statsResponse.data.avgResponseTime)" -ForegroundColor Cyan
    Write-Host "  Satisfaction Score: $($statsResponse.data.satisfactionScore)" -ForegroundColor Cyan
} catch {
    Write-Host "✗ Dashboard stats failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 4: Test Account Status Requests
Write-Host "`n4. Testing Account Status Requests..." -ForegroundColor Yellow
try {
    $requestsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/account-status-requests" -Method GET -Headers $headers
    Write-Host "✓ Account status requests retrieved. Found $($requestsResponse.data.Count) requests" -ForegroundColor Green
} catch {
    Write-Host "✗ Account status requests failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 5: Test Card Requests
Write-Host "`n5. Testing Card Requests..." -ForegroundColor Yellow
try {
    $cardRequestsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/card-requests" -Method GET -Headers $headers
    Write-Host "✓ Card requests retrieved. Found $($cardRequestsResponse.data.Count) requests" -ForegroundColor Green
} catch {
    Write-Host "✗ Card requests failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 6: Test Complaints
Write-Host "`n6. Testing Complaints..." -ForegroundColor Yellow
try {
    $complaintsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/complaints" -Method GET -Headers $headers
    Write-Host "✓ Complaints retrieved. Found $($complaintsResponse.data.Count) complaints" -ForegroundColor Green
} catch {
    Write-Host "✗ Complaints failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nCustomer Care API Testing Complete!" -ForegroundColor Green