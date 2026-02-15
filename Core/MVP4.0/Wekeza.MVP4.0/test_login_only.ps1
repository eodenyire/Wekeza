# Test Login Only
$baseUrl = "http://localhost:5004"

Write-Host "Testing Login..." -ForegroundColor Green

$loginData = @{
    username = "jacobodenyire"
    password = "admin123"
    role = "CustomerCareOfficer"
} | ConvertTo-Json

Write-Host "Login data: $loginData" -ForegroundColor Cyan

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json" -UseBasicParsing
    Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "Response: $($response.Content)" -ForegroundColor Cyan
    
    $loginResponse = $response.Content | ConvertFrom-Json
    $token = $loginResponse.token
    Write-Host "Token: $token" -ForegroundColor Yellow
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody" -ForegroundColor Red
    }
}