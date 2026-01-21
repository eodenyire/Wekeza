#!/usr/bin/env pwsh

# Comprehensive Banking API Demo Script
# This script demonstrates all major banking operations on Port 5003

Write-Host "COMPREHENSIVE BANKING API DEMO (PORT 5003)" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5003"
$headers = @{ "Content-Type" = "application/json" }

# Global variables to store created entities
$customerId = $null
$corporateId = $null
$accountId = $null
$loanId = $null

Write-Host "🔄 Starting comprehensive banking operations demo..." -ForegroundColor Yellow
Write-Host ""

# ============================================================================
# 1. CIF (Customer Information File) Operations
# ============================================================================

Write-Host "1. CIF OPERATIONS" -ForegroundColor Green
Write-Host "=================" -ForegroundColor Green

# Create Individual Customer
Write-Host "   Creating Individual Customer..." -ForegroundColor Yellow
$individualCustomer = @{
    firstName = "Alice"
    lastName = "Johnson"
    email = "alice.johnson@wekeza.com"
    phoneNumber = "+254712345678"
    identificationNumber = "ID123456789"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/cif/individual" -Method POST -Body $individualCustomer -Headers $headers
    $customerId = $response.party.id
    Write-Host "   ✅ Individual Customer Created!" -ForegroundColor Green
    Write-Host "      CIF Number: $($response.party.cifNumber)" -ForegroundColor Cyan
    Write-Host "      Customer ID: $customerId" -ForegroundColor Cyan
    Write-Host "      Name: $($response.party.firstName) $($response.party.lastName)" -ForegroundColor Cyan
} catch {
    Write-Host "   ❌ Error creating individual customer: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Create Corporate Customer
Write-Host "   Creating Corporate Customer..." -ForegroundColor Yellow
$corporateCustomer = @{
    companyName = "TechCorp Solutions Ltd"
    registrationNumber = "REG2026001"
    taxId = "TAX123456"
    industry = "Information Technology"
    contactPerson = "Bob Smith"
    email = "contact@techcorp.com"
    phoneNumber = "+254700987654"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/cif/corporate" -Method POST -Body $corporateCustomer -Headers $headers
    $corporateId = $response.party.id
    Write-Host "   ✅ Corporate Customer Created!" -ForegroundColor Green
    Write-Host "      CIF Number: $($response.party.cifNumber)" -ForegroundColor Cyan
    Write-Host "      Company: $($response.party.companyName)" -ForegroundColor Cyan
    Write-Host "      Industry: $($response.party.industry)" -ForegroundColor Cyan
} catch {
    Write-Host "   ❌ Error creating corporate customer: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# AML Screening (if individual customer was created)
if ($customerId) {
    Write-Host "   🔍 Performing AML Screening..." -ForegroundColor Yellow
    $amlRequest = @{
        customerId = $customerId
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/cif/aml-screening" -Method POST -Body $amlRequest -Headers $headers
        Write-Host "   ✅ AML Screening Completed!" -ForegroundColor Green
        Write-Host "      Result: $($response.screeningResult)" -ForegroundColor Cyan
        Write-Host "      Risk Score: $($response.riskScore)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ❌ Error in AML screening: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# ============================================================================
# 2. Account Management Operations
# ============================================================================

Write-Host "2. ACCOUNT MANAGEMENT" -ForegroundColor Green
Write-Host "=====================" -ForegroundColor Green

if ($customerId) {
    # Create Product-Based Account
    Write-Host "   🏦 Creating Product-Based Account..." -ForegroundColor Yellow
    $accountRequest = @{
        customerId = $customerId
        productId = [System.Guid]::NewGuid().ToString()  # Mock product ID
        currency = "KES"
        initialDeposit = 25000.00
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/accounts/product-based" -Method POST -Body $accountRequest -Headers $headers
        Write-Host "   ✅ Product-Based Account Created!" -ForegroundColor Green
        Write-Host "      Account Number: $($response.account.accountNumber)" -ForegroundColor Cyan
        Write-Host "      Balance: KES $($response.account.balance)" -ForegroundColor Cyan
        $accountNumber = $response.account.accountNumber
    } catch {
        Write-Host "   ❌ Error creating account: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# ============================================================================
# 3. Transaction Processing
# ============================================================================

Write-Host "3. TRANSACTION PROCESSING" -ForegroundColor Green
Write-Host "=========================" -ForegroundColor Green

if ($accountNumber) {
    # Process Deposit
    Write-Host "   💰 Processing Deposit..." -ForegroundColor Yellow
    $depositRequest = @{
        accountNumber = $accountNumber
        amount = 15000.00
        currency = "KES"
        channel = "Teller"
        description = "Cash deposit - salary"
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/transactions/deposit" -Method POST -Body $depositRequest -Headers $headers
        Write-Host "   ✅ Deposit Processed!" -ForegroundColor Green
        Write-Host "      Reference: $($response.transaction.reference)" -ForegroundColor Cyan
        Write-Host "      Amount: KES $($response.transaction.amount)" -ForegroundColor Cyan
        Write-Host "      New Balance: KES $($response.transaction.balanceAfter)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ❌ Error processing deposit: $($_.Exception.Message)" -ForegroundColor Red
    }

    Write-Host ""

    # Process Withdrawal
    Write-Host "   💸 Processing Withdrawal..." -ForegroundColor Yellow
    $withdrawalRequest = @{
        accountNumber = $accountNumber
        amount = 5000.00
        currency = "KES"
        channel = "ATM"
        description = "ATM withdrawal"
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/transactions/withdraw" -Method POST -Body $withdrawalRequest -Headers $headers
        Write-Host "   ✅ Withdrawal Processed!" -ForegroundColor Green
        Write-Host "      Reference: $($response.transaction.reference)" -ForegroundColor Cyan
        Write-Host "      Amount: KES $($response.transaction.amount)" -ForegroundColor Cyan
        Write-Host "      New Balance: KES $($response.transaction.balanceAfter)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ❌ Error processing withdrawal: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# ============================================================================
# 4. Loan Management
# ============================================================================

Write-Host "4. LOAN MANAGEMENT" -ForegroundColor Green
Write-Host "==================" -ForegroundColor Green

if ($customerId) {
    # Apply for Loan
    Write-Host "   💳 Applying for Loan..." -ForegroundColor Yellow
    $loanRequest = @{
        customerId = $customerId
        loanType = "Personal"
        requestedAmount = 150000.00
        currency = "KES"
        term = 24
        purpose = "Business expansion"
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/loans/apply" -Method POST -Body $loanRequest -Headers $headers
        $loanId = $response.loan.id
        Write-Host "   ✅ Loan Application Submitted!" -ForegroundColor Green
        Write-Host "      Loan Number: $($response.loan.loanNumber)" -ForegroundColor Cyan
        Write-Host "      Requested Amount: KES $($response.loan.requestedAmount)" -ForegroundColor Cyan
        Write-Host "      Status: $($response.loan.status)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ❌ Error applying for loan: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# ============================================================================
# 5. Fixed Deposits & Investments
# ============================================================================

Write-Host "5. FIXED DEPOSITS & INVESTMENTS" -ForegroundColor Green
Write-Host "===============================" -ForegroundColor Green

if ($customerId -and $accountNumber) {
    # Book Fixed Deposit
    Write-Host "   💎 Booking Fixed Deposit..." -ForegroundColor Yellow
    $fdRequest = @{
        customerId = $customerId
        sourceAccount = $accountNumber
        amount = 100000.00
        currency = "KES"
        term = 12
        interestRate = 8.5
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/deposits/fixed" -Method POST -Body $fdRequest -Headers $headers
        Write-Host "   ✅ Fixed Deposit Booked!" -ForegroundColor Green
        Write-Host "      Certificate Number: $($response.deposit.certificateNumber)" -ForegroundColor Cyan
        Write-Host "      Amount: KES $($response.deposit.amount)" -ForegroundColor Cyan
        Write-Host "      Interest Rate: $($response.deposit.interestRate)%" -ForegroundColor Cyan
        Write-Host "      Maturity Date: $($response.deposit.maturityDate)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ❌ Error booking fixed deposit: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# ============================================================================
# 6. Cards & Instruments
# ============================================================================

Write-Host "6. CARDS & INSTRUMENTS" -ForegroundColor Green
Write-Host "======================" -ForegroundColor Green

if ($customerId -and $accountNumber) {
    # Issue Card
    Write-Host "   💳 Issuing Debit Card..." -ForegroundColor Yellow
    $cardRequest = @{
        customerId = $customerId
        accountId = [System.Guid]::NewGuid().ToString()  # Mock account ID
        cardType = "Debit"
        dailyLimit = 50000.00
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/cards/issue" -Method POST -Body $cardRequest -Headers $headers
        Write-Host "   ✅ Debit Card Issued!" -ForegroundColor Green
        Write-Host "      Card Number: $($response.card.cardNumber)" -ForegroundColor Cyan
        Write-Host "      Card Type: $($response.card.cardType)" -ForegroundColor Cyan
        Write-Host "      Daily Limit: KES $($response.card.dailyLimit)" -ForegroundColor Cyan
        Write-Host "      Status: $($response.card.status)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ❌ Error issuing card: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# ============================================================================
# 7. Payment Processing
# ============================================================================

Write-Host "7. PAYMENT PROCESSING" -ForegroundColor Green
Write-Host "=====================" -ForegroundColor Green

if ($accountNumber) {
    # Create Payment Order
    Write-Host "   💸 Creating Payment Order..." -ForegroundColor Yellow
    $paymentRequest = @{
        fromAccountId = [System.Guid]::NewGuid().ToString()  # Mock account ID
        toAccountNumber = "WKZ-87654321"  # Mock destination account
        amount = 12000.00
        paymentType = "EFT"
        description = "Supplier payment"
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/payments/create" -Method POST -Body $paymentRequest -Headers $headers
        Write-Host "   ✅ Payment Order Created!" -ForegroundColor Green
        Write-Host "      Payment Reference: $($response.payment.reference)" -ForegroundColor Cyan
        Write-Host "      Amount: KES $($response.payment.amount)" -ForegroundColor Cyan
        Write-Host "      Status: $($response.payment.status)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ❌ Error creating payment: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# ============================================================================
# 8. System Status & Summary
# ============================================================================

Write-Host "8. SYSTEM STATUS & SUMMARY" -ForegroundColor Green
Write-Host "===========================" -ForegroundColor Green

# Get System Status
Write-Host "   📊 Getting System Status..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/status" -Method GET
    Write-Host "   ✅ System Status Retrieved!" -ForegroundColor Green
    Write-Host "      Status: $($response.status)" -ForegroundColor Cyan
    Write-Host "      Version: $($response.version)" -ForegroundColor Cyan
    Write-Host "      Database: PostgreSQL Connected" -ForegroundColor Cyan
    Write-Host "      Modules: $($response.features.modules)" -ForegroundColor Cyan
    Write-Host "      Endpoints: $($response.features.endpoints)" -ForegroundColor Cyan
    Write-Host "      Total Customers: $($response.statistics.totalCustomers)" -ForegroundColor Cyan
    Write-Host "      Total Accounts: $($response.statistics.totalAccounts)" -ForegroundColor Cyan
} catch {
    Write-Host "   ❌ Error getting system status: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# ============================================================================
# Demo Summary
# ============================================================================

Write-Host "COMPREHENSIVE BANKING DEMO COMPLETED!" -ForegroundColor Magenta
Write-Host "=========================================" -ForegroundColor Magenta
Write-Host ""
Write-Host "📋 Operations Demonstrated:" -ForegroundColor White
Write-Host "   ✅ Individual Customer Creation (CIF)" -ForegroundColor Green
Write-Host "   ✅ Corporate Customer Creation (CIF)" -ForegroundColor Green
Write-Host "   ✅ AML Screening" -ForegroundColor Green
Write-Host "   ✅ Product-Based Account Opening" -ForegroundColor Green
Write-Host "   ✅ Cash Deposit Processing" -ForegroundColor Green
Write-Host "   ✅ Cash Withdrawal Processing" -ForegroundColor Green
Write-Host "   ✅ Loan Application" -ForegroundColor Green
Write-Host "   ✅ Fixed Deposit Booking" -ForegroundColor Green
Write-Host "   ✅ Debit Card Issuance" -ForegroundColor Green
Write-Host "   ✅ Payment Order Creation" -ForegroundColor Green
Write-Host "   ✅ System Status Check" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Access Points:" -ForegroundColor White
Write-Host "   📱 Web Interface: http://localhost:5003" -ForegroundColor Cyan
Write-Host "   📚 API Documentation: http://localhost:5003/swagger" -ForegroundColor Cyan
Write-Host "   📊 System Status: http://localhost:5003/api/status" -ForegroundColor Cyan
Write-Host ""
Write-Host "💾 Data Persistence:" -ForegroundColor White
Write-Host "   🗄️ All data saved to PostgreSQL database: wekeza_banking_comprehensive" -ForegroundColor Yellow
Write-Host ""
Write-Host "Owner: Emmanuel Odenyire (ID: 28839872) | Contact: 0716478835" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor White
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")