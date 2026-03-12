Write-Host "Testing Staff Creation API Endpoint..." -ForegroundColor Cyan

$staffData = @{
    firstName = "John"
    lastName = "Doe"
    email = "john.doe@wekeza.com"
    phone = "+254712345678"
    role = "Teller"
    branchId = 1
    departmentId = 2
    employeeId = ""
} | ConvertTo-Json

Write-Host "Test Data:" -ForegroundColor Green
Write-Host $staffData -ForegroundColor White

try {
    Write-Host "Sending POST request..." -ForegroundColor Cyan
    
    $response = Invoke-RestMethod -Uri "http://localhost:5003/admin/staff/create" -Method POST -Body $staffData -ContentType "application/json" -ErrorAction Stop
    
    Write-Host "SUCCESS! Staff creation API is working!" -ForegroundColor Green
    Write-Host "Response:" -ForegroundColor Yellow
    $response | ConvertTo-Json -Depth 10 | Write-Host -ForegroundColor White
    
    if ($response.databasePersistence -eq $true) {
        Write-Host "DATABASE PERSISTENCE: ENABLED" -ForegroundColor Green
    } else {
        Write-Host "DATABASE PERSISTENCE: DISABLED" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "ERROR: Staff creation failed!" -ForegroundColor Red
    Write-Host "Error Details:" -ForegroundColor Yellow
    Write-Host $_.Exception.Message -ForegroundColor Red
}

Write-Host "Test completed!" -ForegroundColor Magenta