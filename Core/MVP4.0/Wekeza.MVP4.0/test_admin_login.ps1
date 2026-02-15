# Test Admin login
$loginData = @{
    Username = "admin"
    Password = "admin123"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5004/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
    Write-Host "Login Response: $($response | ConvertTo-Json -Depth 3)"
} catch {
    Write-Host "Error: $($_.Exception.Message)"
    Write-Host "Status: $($_.Exception.Response.StatusCode)"
}