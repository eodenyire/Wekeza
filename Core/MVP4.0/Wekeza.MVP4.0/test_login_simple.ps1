# Simple Login Test
$baseUrl = "http://localhost:5004"

Write-Host "Testing Customer Care Login..." -ForegroundColor Green

$loginData = @{
    username = "jacobodenyire"
    password = "admin123"
    role = "CustomerCareOfficer"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
    Write-Host "SUCCESS: Login worked!" -ForegroundColor Green
    Write-Host "User: $($response.user.fullName)" -ForegroundColor Cyan
    Write-Host "Role: $($response.user.role)" -ForegroundColor Cyan
    Write-Host "Token received: Yes" -ForegroundColor Cyan
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}