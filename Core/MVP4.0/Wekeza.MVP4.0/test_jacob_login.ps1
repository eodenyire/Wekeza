# Test Jacob's Customer Care login
$loginUrl = "http://localhost:5004/api/auth/login"
$loginData = @{
    username = "jacobodenyire"
    password = "admin123"
    role = "CustomerCareOfficer"
} | ConvertTo-Json

Write-Host "Testing Jacob's Customer Care login..." -ForegroundColor Yellow

try {
    $response = Invoke-RestMethod -Uri $loginUrl -Method POST -Body $loginData -ContentType "application/json"
    Write-Host "Login successful!" -ForegroundColor Green
    Write-Host "Token: $($response.token.Substring(0, 50))..." -ForegroundColor Cyan
    Write-Host "Role: $($response.user.role)" -ForegroundColor Cyan
    Write-Host "Full Name: $($response.user.fullName)" -ForegroundColor Cyan
    
    # Store token for further testing
    $global:authToken = $response.token
    
    # Test Customer Care dashboard stats
    Write-Host "`nTesting Customer Care dashboard stats..." -ForegroundColor Yellow
    $headers = @{
        "Authorization" = "Bearer $($response.token)"
        "Content-Type" = "application/json"
    }
    
    $statsUrl = "http://localhost:5004/api/customercare/dashboard/stats"
    $statsResponse = Invoke-RestMethod -Uri $statsUrl -Method GET -Headers $headers
    
    Write-Host "Dashboard stats retrieved successfully!" -ForegroundColor Green
    Write-Host "Active Inquiries: $($statsResponse.data.activeInquiries)" -ForegroundColor Cyan
    Write-Host "Resolved Today: $($statsResponse.data.resolvedToday)" -ForegroundColor Cyan
    Write-Host "Avg Response Time: $($statsResponse.data.avgResponseTime)" -ForegroundColor Cyan
    Write-Host "Satisfaction Score: $($statsResponse.data.satisfactionScore)" -ForegroundColor Cyan
    
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response: $responseBody" -ForegroundColor Red
    }
}