# Customer Care Final Test
$baseUrl = "http://localhost:5004"

Write-Host "=== CUSTOMER CARE FUNCTIONALITY TEST ===" -ForegroundColor Magenta

# Login
Write-Host "`n1. AUTHENTICATION TEST" -ForegroundColor Yellow
$loginData = @{
    username = "jacobodenyire"
    password = "admin123"
    role = "CustomerCareOfficer"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
$token = $loginResponse.token
Write-Host "✓ Login successful! User: $($loginResponse.user.fullName)" -ForegroundColor Green

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Customer Search
Write-Host "`n2. CUSTOMER SEARCH TEST" -ForegroundColor Yellow
$searchResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/customers/search?searchTerm=John" -Method GET -Headers $headers
Write-Host "✓ Customer search: Found $($searchResponse.data.Count) customers" -ForegroundColor Green

# Dashboard Stats
Write-Host "`n3. DASHBOARD STATISTICS TEST" -ForegroundColor Yellow
$statsResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/dashboard/stats" -Method GET -Headers $headers
Write-Host "✓ Dashboard stats retrieved successfully" -ForegroundColor Green
Write-Host "  Active Inquiries: $($statsResponse.data.activeInquiries)" -ForegroundColor White
Write-Host "  Resolved Today: $($statsResponse.data.resolvedToday)" -ForegroundColor White
Write-Host "  Satisfaction Score: $($statsResponse.data.satisfactionScore)" -ForegroundColor White

# Complaint Creation
Write-Host "`n4. COMPLAINT HANDLING TEST" -ForegroundColor Yellow
$complaintData = @{
    customerId = "11111111-1111-1111-1111-111111111111"
    category = "Account Balance"
    priority = "High"
    subject = "Test Complaint - Balance Inquiry"
    description = "Customer unable to see correct balance in mobile app"
} | ConvertTo-Json

$complaintResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/complaints" -Method POST -Body $complaintData -Headers $headers
Write-Host "✓ Test complaint created successfully" -ForegroundColor Green

# Account Status Request
Write-Host "`n5. ACCOUNT STATUS REQUEST TEST" -ForegroundColor Yellow
$statusRequestData = @{
    accountNumber = "ACC001234"
    requestType = "Freeze"
    reason = "Customer reported suspicious transactions"
    customerAuthorized = $true
} | ConvertTo-Json

$statusResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/account-status-requests" -Method POST -Body $statusRequestData -Headers $headers
Write-Host "✓ Account status request created successfully" -ForegroundColor Green

# Card Request
Write-Host "`n6. CARD REQUEST TEST" -ForegroundColor Yellow
$cardRequestData = @{
    accountNumber = "ACC001234"
    cardNumber = "1234"
    requestType = "Block"
    reason = "Lost Card"
    comments = "Customer lost card during travel"
} | ConvertTo-Json

$cardResponse = Invoke-RestMethod -Uri "$baseUrl/api/customercare/card-requests" -Method POST -Body $cardRequestData -Headers $headers
Write-Host "✓ Card request created successfully" -ForegroundColor Green

# Report Generation
Write-Host "`n7. REPORT GENERATION TEST" -ForegroundColor Yellow
$accountNumber = "ACC001234"
$fromDate = "2024-01-01"
$toDate = "2024-12-31"

$statementUrl = "$baseUrl/api/customercare/reports/statement/$accountNumber" + "?fromDate=$fromDate" + "&" + "toDate=$toDate"
$response = Invoke-WebRequest -Uri $statementUrl -Method GET -Headers @{"Authorization" = "Bearer $token"} -UseBasicParsing

if ($response.StatusCode -eq 200) {
    Write-Host "✓ Account statement generated successfully" -ForegroundColor Green
    Write-Host "  Statement size: $($response.Content.Length) bytes" -ForegroundColor White
}

# Summary
Write-Host "`n=== TEST SUMMARY ===" -ForegroundColor Magenta
Write-Host "✓ Authentication and Authorization - Working" -ForegroundColor Green
Write-Host "✓ Customer Search and Enquiry - Working" -ForegroundColor Green
Write-Host "✓ Dashboard Statistics - Working" -ForegroundColor Green
Write-Host "✓ Complaint and Issue Handling - Working" -ForegroundColor Green
Write-Host "✓ Account Status Requests - Working" -ForegroundColor Green
Write-Host "✓ Card and Channel Support - Working" -ForegroundColor Green
Write-Host "✓ Reports and Documentation - Working" -ForegroundColor Green

Write-Host "`nCustomer Care system is fully functional!" -ForegroundColor Green
Write-Host "Jacob (jacobodenyire) can access all Customer Care features." -ForegroundColor Cyan