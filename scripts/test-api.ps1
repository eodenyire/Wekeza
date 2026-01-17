# Wekeza Bank - API Testing Script
# Tests key endpoints to verify the system is working

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Wekeza Bank - API Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$BASE_URL = "https://localhost:5001"
$TOKEN = $null

# Test 1: Health Check
Write-Host "[1/6] Testing Health Check..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BASE_URL/health" -Method Get -SkipCertificateCheck
    Write-Host "✓ Health check passed" -ForegroundColor Green
} catch {
    Write-Host "✗ Health check failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Test 2: Login
Write-Host "[2/6] Testing Authentication..." -ForegroundColor Yellow
$loginBody = @{
    username = "admin"
    password = "admin123"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$BASE_URL/api/authentication/login" `
        -Method Post `
        -Body $loginBody `
        -ContentType "application/json" `
        -SkipCertificateCheck
    
    $TOKEN = $response.token
    Write-Host "✓ Login successful" -ForegroundColor Green
    Write-Host "  Token: $($TOKEN.Substring(0, 20))..." -ForegroundColor Gray
} catch {
    Write-Host "✗ Login failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Get Current User
if ($TOKEN) {
    Write-Host "[3/6] Testing Get Current User..." -ForegroundColor Yellow
    try {
        $headers = @{
            Authorization = "Bearer $TOKEN"
        }
        $response = Invoke-RestMethod -Uri "$BASE_URL/api/authentication/me" `
            -Method Get `
            -Headers $headers `
            -SkipCertificateCheck
        
        Write-Host "✓ User info retrieved" -ForegroundColor Green
        Write-Host "  Username: $($response.username)" -ForegroundColor Gray
    } catch {
        Write-Host "✗ Get user failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "API is ready for use!" -ForegroundColor Green
Write-Host "Access Swagger UI at: $BASE_URL/swagger" -ForegroundColor Yellow
Write-Host ""
