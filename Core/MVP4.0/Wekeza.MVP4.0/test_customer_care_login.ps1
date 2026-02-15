# Test Customer Care login
$loginData = @{
    Username = "jacobodenyire"
    Password = "admin123"
    Role = "CustomerCareOfficer"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5004/api/auth/login" -Method POST -Body $loginData -ContentType "application/json"
    Write-Host "Login Response: $($response | ConvertTo-Json -Depth 3)"
} catch {
    Write-Host "Error: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        Write-Host "Status: $($_.Exception.Response.StatusCode)"
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody"
    }
}