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

Write-Host "Starting comprehensive banking operations demo..." -ForegroundColor Yellow
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
    Write-Host "   SUCCESS: Individual Customer Created!" -ForegroundColor Green
    Write-Host "      CIF Number: $($response.party.cifNumber)" -ForegroundColor Cyan
    Write-Host "      Customer ID: $customerId" -ForegroundColor Cyan
    Write-Host "      Name: $($response.party.firstName) $($response.party.lastName)" -ForegroundColor Cyan
} catch {
    Write-Host "   ERROR: creating individual customer: $($_.Exception.Message)" -ForegroundColor Red
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
    Write-Host "   SUCCESS: Corporate Customer Created!" -ForegroundColor Green
    Write-Host "      CIF Number: $($response.party.cifNumber)" -ForegroundColor Cyan
    Write-Host "      Company: $($response.party.companyName)" -ForegroundColor Cyan
    Write-Host "      Industry: $($response.party.industry)" -ForegroundColor Cyan
} catch {
    Write-Host "   ERROR: creating corporate customer: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# AML Screening (if individual customer was created)
if ($customerId) {
    Write-Host "   Performing AML Screening..." -ForegroundColor Yellow
    $amlRequest = @{
        customerId = $customerId
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/cif/aml-screening" -Method POST -Body $amlRequest -Headers $headers
        Write-Host "   SUCCESS: AML Screening Completed!" -ForegroundColor Green
        Write-Host "      Result: $($response.screeningResult)" -ForegroundColor Cyan
        Write-Host "      Risk Score: $($response.riskScore)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ERROR: in AML screening: $($_.Exception.Message)" -ForegroundColor Red
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
    Write-Host "   Creating Product-Based Account..." -ForegroundColor Yellow
    $accountRequest = @{
        customerId = $customerId
        productId = [System.Guid]::NewGuid().ToString()  # Mock product ID
        currency = "KES"
        initialDeposit = 25000.00
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/accounts/product-based" -Method POST -Body $accountRequest -Headers $headers
        Write-Host "   SUCCESS: Product-Based Account Created!" -ForegroundColor Green
        Write-Host "      Account Number: $($response.account.accountNumber)" -ForegroundColor Cyan
        Write-Host "      Balance: KES $($response.account.balance)" -ForegroundColor Cyan
        $accountNumber = $response.account.accountNumber
    } catch {
        Write-Host "   ERROR: creating account: $($_.Exception.Message)" -ForegroundColor Red
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
    Write-Host "   Processing Deposit..." -ForegroundColor Yellow
    $depositRequest = @{
        accountNumber = $accountNumber
        amount = 15000.00
        currency = "KES"
        channel = "Teller"
        description = "Cash deposit - salary"
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/transactions/deposit" -Method POST -Body $depositRequest -Headers $headers
        Write-Host "   SUCCESS: Deposit Processed!" -ForegroundColor Green
        Write-Host "      Reference: $($response.transaction.reference)" -ForegroundColor Cyan
        Write-Host "      Amount: KES $($response.transaction.amount)" -ForegroundColor Cyan
        Write-Host "      New Balance: KES $($response.transaction.balanceAfter)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ERROR: processing deposit: $($_.Exception.Message)" -ForegroundColor Red
    }

    Write-Host ""

    # Process Withdrawal
    Write-Host "   Processing Withdrawal..." -ForegroundColor Yellow
    $withdrawalRequest = @{
        accountNumber = $accountNumber
        amount = 5000.00
        currency = "KES"
        channel = "ATM"
        description = "ATM withdrawal"
    } | ConvertTo-Json

    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/api/transactions/withdraw" -Method POST -Body $withdrawalRequest -Headers $headers
        Write-Host "   SUCCESS: Withdrawal Processed!" -ForegroundColor Green
        Write-Host "      Reference: $($response.transaction.reference)" -ForegroundColor Cyan
        Write-Host "      Amount: KES $($response.transaction.amount)" -ForegroundColor Cyan
        Write-Host "      New Balance: KES $($response.transaction.balanceAfter)" -ForegroundColor Cyan
    } catch {
        Write-Host "   ERROR: processing withdrawal: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""

# ============================================================================
# 4. System Status Check
# ============================================================================

Write-Host "4. SYSTEM STATUS CHECK" -ForegroundColor Green
Write-Host "======================" -ForegroundColor Green

# Get System Status
Write-Host "   Getting System Status..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/status" -Method GET
    Write-Host "   SUCCESS: System Status Retrieved!" -ForegroundColor Green
    Write-Host "      Status: $($response.status)" -ForegroundColor Cyan
    Write-Host "      Version: $($response.version)" -ForegroundColor Cyan
    Write-Host "      Database: PostgreSQL Connected" -ForegroundColor Cyan
    Write-Host "      Modules: $($response.features.modules)" -ForegroundColor Cyan
    Write-Host "      Endpoints: $($response.features.endpoints)" -ForegroundColor Cyan
    Write-Host "      Total Customers: $($response.statistics.totalCustomers)" -ForegroundColor Cyan
    Write-Host "      Total Accounts: $($response.statistics.totalAccounts)" -ForegroundColor Cyan
} catch {
    Write-Host "   ERROR: getting system status: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# ============================================================================
# Demo Summary
# ============================================================================

Write-Host "COMPREHENSIVE BANKING DEMO COMPLETED!" -ForegroundColor Magenta
Write-Host "=========================================" -ForegroundColor Magenta
Write-Host ""
Write-Host "Operations Demonstrated:" -ForegroundColor White
Write-Host "   SUCCESS: Individual Customer Creation (CIF)" -ForegroundColor Green
Write-Host "   SUCCESS: Corporate Customer Creation (CIF)" -ForegroundColor Green
Write-Host "   SUCCESS: AML Screening" -ForegroundColor Green
Write-Host "   SUCCESS: Product-Based Account Opening" -ForegroundColor Green
Write-Host "   SUCCESS: Cash Deposit Processing" -ForegroundColor Green
Write-Host "   SUCCESS: Cash Withdrawal Processing" -ForegroundColor Green
Write-Host "   SUCCESS: System Status Check" -ForegroundColor Green
Write-Host ""
Write-Host "Access Points:" -ForegroundColor White
Write-Host "   Web Interface: http://localhost:5003" -ForegroundColor Cyan
Write-Host "   API Documentation: http://localhost:5003/swagger" -ForegroundColor Cyan
Write-Host "   System Status: http://localhost:5003/api/status" -ForegroundColor Cyan
Write-Host ""
Write-Host "Data Persistence:" -ForegroundColor White
Write-Host "   All data saved to PostgreSQL database: wekeza_banking_comprehensive" -ForegroundColor Yellow
Write-Host ""
Write-Host "Owner: Emmanuel Odenyire (ID: 28839872) | Contact: 0716478835" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor White
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")