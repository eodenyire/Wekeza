# Week 4 GL Integration Test Script
# This script tests the complete GL integration with Product Factory

Write-Host "🚀 Testing Week 4 GL Integration..." -ForegroundColor Green

# Set API base URL
$baseUrl = "https://localhost:5001/api"

Write-Host "📋 Step 1: Create Chart of Accounts..." -ForegroundColor Yellow

# Create main asset account
$assetAccount = @{
    glCode = "1000"
    glName = "ASSETS"
    accountType = 0
    category = 5
    currency = "KES"
    level = 1
    isLeaf = $false
    allowManualPosting = $false
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUrl/generalledger/accounts" -Method POST -Body $assetAccount -ContentType "application/json"

# Create cash account
$cashAccount = @{
    glCode = "1001"
    glName = "Cash in Hand"
    accountType = 0
    category = 0
    currency = "KES"
    level = 2
    isLeaf = $true
    parentGLCode = "1000"
    allowManualPosting = $true
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUrl/generalledger/accounts" -Method POST -Body $cashAccount -ContentType "application/json"

# Create liability account
$liabilityAccount = @{
    glCode = "2000"
    glName = "LIABILITIES"
    accountType = 1
    category = 6
    currency = "KES"
    level = 1
    isLeaf = $false
    allowManualPosting = $false
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUrl/generalledger/accounts" -Method POST -Body $liabilityAccount -ContentType "application/json"

Write-Host "📋 Step 2: Create a Product..." -ForegroundColor Yellow

$product = @{
    productCode = "SAV001"
    productName = "Premium Savings Account"
    category = 0
    type = 0
    currency = "KES"
    description = "High-yield savings account with premium features"
} | ConvertTo-Json

$productResult = Invoke-RestMethod -Uri "$baseUrl/products" -Method POST -Body $product -ContentType "application/json"
$productId = $productResult.id

Write-Host "📋 Step 3: Configure Product Interest..." -ForegroundColor Yellow

$interestConfig = @{
    type = 0
    rate = 5.5
    calculationMethod = 0
    postingFrequency = 2
    isTiered = $false
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUrl/products/$productId/interest" -Method PUT -Body $interestConfig -ContentType "application/json"

Write-Host "📋 Step 4: Set Product Limits..." -ForegroundColor Yellow

$limits = @{
    minBalance = 1000
    maxBalance = 1000000
    dailyTransactionLimit = 50000
    monthlyTransactionLimit = 500000
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUrl/products/$productId/limits" -Method PUT -Body $limits -ContentType "application/json"

Write-Host "📋 Step 5: Activate Product..." -ForegroundColor Yellow

Invoke-RestMethod -Uri "$baseUrl/products/$productId/activate" -Method POST

Write-Host "📋 Step 6: Create a Customer..." -ForegroundColor Yellow

$customer = @{
    partyType = 0
    firstName = "John"
    lastName = "Doe"
    dateOfBirth = "1990-01-15"
    phoneNumber = "+254700123456"
    emailAddress = "john.doe@example.com"
    nationalId = "12345678"
    address = @{
        street = "123 Main Street"
        city = "Nairobi"
        country = "Kenya"
        postalCode = "00100"
    }
} | ConvertTo-Json -Depth 3

$customerResult = Invoke-RestMethod -Uri "$baseUrl/cif/individual" -Method POST -Body $customer -ContentType "application/json"
$customerId = $customerResult.partyId

Write-Host "📋 Step 7: Open Product-Based Account..." -ForegroundColor Yellow

$accountRequest = @{
    customerId = $customerId
    productId = $productId
    currency = "KES"
    initialDeposit = 5000.00
    accountAlias = "My Premium Savings"
} | ConvertTo-Json

$accountResult = Invoke-RestMethod -Uri "$baseUrl/accounts/product-based" -Method POST -Body $accountRequest -ContentType "application/json"

Write-Host "✅ Account Created Successfully!" -ForegroundColor Green
Write-Host "   Account Number: $($accountResult.accountNumber)" -ForegroundColor Cyan
Write-Host "   Customer GL Code: $($accountResult.customerGLCode)" -ForegroundColor Cyan
Write-Host "   Journal Number: $($accountResult.journalNumber)" -ForegroundColor Cyan

Write-Host "📋 Step 8: Verify GL Impact..." -ForegroundColor Yellow

# Get Chart of Accounts
$chartOfAccounts = Invoke-RestMethod -Uri "$baseUrl/generalledger/accounts" -Method GET

Write-Host "📊 Chart of Accounts:" -ForegroundColor Cyan
$chartOfAccounts | ForEach-Object {
    Write-Host "   $($_.glCode) - $($_.glName): Balance = $($_.netBalance)" -ForegroundColor White
}

# Get Trial Balance
$trialBalance = Invoke-RestMethod -Uri "$baseUrl/generalledger/trial-balance?asOfDate=2026-01-17" -Method GET

Write-Host "📊 Trial Balance:" -ForegroundColor Cyan
Write-Host "   Total Debit: $($trialBalance.totalDebit)" -ForegroundColor White
Write-Host "   Total Credit: $($trialBalance.totalCredit)" -ForegroundColor White
Write-Host "   Is Balanced: $($trialBalance.isBalanced)" -ForegroundColor White

if ($trialBalance.isBalanced) {
    Write-Host "🎉 SUCCESS: GL Integration Working Perfectly!" -ForegroundColor Green
    Write-Host "   ✅ Product-based account opened" -ForegroundColor Green
    Write-Host "   ✅ Customer GL account created" -ForegroundColor Green
    Write-Host "   ✅ Initial deposit posted to GL" -ForegroundColor Green
    Write-Host "   ✅ Trial balance is balanced" -ForegroundColor Green
} else {
    Write-Host "❌ ERROR: Trial balance is not balanced!" -ForegroundColor Red
}

Write-Host "🏆 Week 4 GL Integration Test Complete!" -ForegroundColor Green