# Test script to create a user with unique credentials
$timestamp = [DateTimeOffset]::Now.ToUnixTimeSeconds()
$userData = @{
    fullName = "Test User $timestamp"
    username = "testuser$timestamp"
    email = "testuser$timestamp@example.com"
    password = "password123"
    role = "Teller"
    isActive = $true
} | ConvertTo-Json

Write-Host "Creating user with data:"
Write-Host $userData

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5004/api/admin/create-user" -Method POST -Body $userData -ContentType "application/json"
    Write-Host "SUCCESS: User created successfully!"
    Write-Host "Response: $($response | ConvertTo-Json)"
} catch {
    Write-Host "ERROR: Failed to create user"
    Write-Host "Status Code: $($_.Exception.Response.StatusCode)"
    Write-Host "Error Message: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $responseBody = $reader.ReadToEnd()
        Write-Host "Response Body: $responseBody"
    }
}