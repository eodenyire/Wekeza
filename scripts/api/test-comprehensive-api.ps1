#!/usr/bin/env pwsh

# Test Script for ComprehensiveWekezaApi (Port 5003)
# This script demonstrates all major banking operations

Write-Host "🏦 TESTING COMPREHENSIVE WEKEZA API (PORT 5003)" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5003"

# Test 1: Create Individual Customer
Write-Host "1. 👤 Creating Individual Customer..." -ForegroundColor Yellow
$customerBody = @{
    firstName = "John"
    lastName = "Doe"
    email = "john.doe@wekeza.com"
    phoneNumber = "+254712345678"
    identificationNumber = "12345678"
} | ConvertTo-Json

try {
    $customerResult = Invoke-RestMethod -Uri "$baseUrl/api/cif/individual" -Method POST -Body $customerBody -ContentType "application/json"
    Write-Host "   ✅ Customer Created: $($customerResult.party.cifNumber)" -ForegroundColor Green
    $customerId = $customerResult.party.id
} catch {
    Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 2: Create Corporate Customer
Write-Host "2. 🏢 Creating Corporate Customer..." -ForegroundColor Yellow
$corporateBody = @{
    companyName = "Tech Solutions Ltd"
    registrationNumber = "REG123456"
    taxId = "TAX789012"
    industry = "Technology"
    contactPerson = "Jane Smith"
    email = "contact@techsolutions.com"
    phoneNumber = "+254700123456"
} | ConvertTo-Json

try {
    $corporateResult = Invoke-RestMethod -Uri "$baseUrl/api/cif/corporate" -Method POST -Body $corporateBody -ContentType "application/json"
    Write-Host "   ✅ Corporate Customer Created: $($corporateResult.party.cifNumber)" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 3: Get System Status
Write-Host "3. 📊 Getting System Status..." -ForegroundColor Yellow
try {
    $statusResult = Invoke-RestMethod -Uri "$baseUrl/api/status" -Method GET
    Write-Host "   ✅ System Status: $($statusResult.status)" -ForegroundColor Green
    Write-Host "   📈 Total Customers: $($statusResult.statistics.totalCustomers)" -ForegroundColor Cyan
    Write-Host "   🏦 Total Accounts: $($statusResult.statistics.totalAccounts)" -ForegroundColor Cyan
} catch {
    Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test 4: AML Screening (if customer was created)
if ($customerId) {
    Write-Host "4. 🔍 Performing AML Screening..." -ForegroundColor Yellow
    $amlBody = @{
        customerId = $customerId
    } | ConvertTo-Json

    try {
        $amlResult = Invoke-RestMethod -Uri "$baseUrl/api/cif/aml-screening" -Method POST -Body $amlBody -ContentType "application/json"
        Write-Host "   ✅ AML Screening: $($amlResult.screeningResult)" -ForegroundColor Green
        Write-Host "   📊 Risk Score: $($amlResult.riskScore)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "🎉 COMPREHENSIVE API TESTING COMPLETE!" -ForegroundColor Magenta
Write-Host ""
Write-Host "📚 Available Endpoints:" -ForegroundColor White
Write-Host "   🌐 Web Interface: http://localhost:5003" -ForegroundColor Cyan
Write-Host "   📖 API Documentation: http://localhost:5003/swagger" -ForegroundColor Cyan
Write-Host "   📊 System Status: http://localhost:5003/api/status" -ForegroundColor Cyan
Write-Host ""
Write-Host "👤 Owner: Emmanuel Odenyire (ID: 28839872) | Contact: 0716478835" -ForegroundColor Gray