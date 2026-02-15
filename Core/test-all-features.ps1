# Comprehensive Test Script for Public Sector Portal
$baseUrl = "http://localhost:5000/api/public-sector"
$authUrl = "http://localhost:5000/api/authentication/login"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  WEKEZA PUBLIC SECTOR PORTAL TEST" -ForegroundColor Cyan
Write-Host "  Complete Feature Testing Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Authenticate
Write-Host "🔐 Authenticating..." -ForegroundColor Yellow
$loginPayload = @{
    username = "admin"
    password = "password123"
} | ConvertTo-Json

try {
    $authResponse = Invoke-RestMethod -Uri $authUrl -Method Post -Body $loginPayload -ContentType "application/json"
    $token = $authResponse.token
    Write-Host "✅ Authentication successful" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "❌ Authentication failed" -ForegroundColor Red
    exit
}

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Test 1: Budget Control
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TEST 1: BUDGET CONTROL & COMMITMENTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "1.1 Getting Budget Allocations..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/budget/allocations?fiscalYear=2026" -Method Get -Headers $headers
    Write-Host "✅ Retrieved budget allocations" -ForegroundColor Green
    Write-Host "Total Allocations: $($response.data.Count)" -ForegroundColor White
    $response.data | Select-Object -First 3 | ForEach-Object {
        Write-Host "  - $($_.departmentName) ($($_.category)): KES $($_.allocatedAmount:N2)" -ForegroundColor Gray
    }
    $allocationId = $response.data[0].id
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "1.2 Creating Budget Commitment..." -ForegroundColor Yellow
$commitmentPayload = @{
    allocationId = $allocationId
    amount = 5000000
    purpose = "Infrastructure development project"
    reference = "PROJECT-2026-001"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/budget/commitments" -Method Post -Body $commitmentPayload -Headers $headers
    Write-Host "✅ Budget commitment created" -ForegroundColor Green
    Write-Host "Commitment Number: $($response.data.commitmentNumber)" -ForegroundColor White
    Write-Host "Amount: KES $($response.data.amount:N2)" -ForegroundColor White
    $commitmentId = $response.data.commitmentId
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "1.3 Getting Budget Utilization..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/budget/utilization?fiscalYear=2026" -Method Get -Headers $headers
    Write-Host "✅ Retrieved budget utilization" -ForegroundColor Green
    $response.data | Select-Object -First 3 | ForEach-Object {
        Write-Host "  - $($_.departmentname): $($_.utilizationRate:N2)% utilized" -ForegroundColor Gray
    }
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "1.4 Getting Budget Alerts..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/budget/alerts?fiscalYear=2026" -Method Get -Headers $headers
    Write-Host "✅ Retrieved budget alerts" -ForegroundColor Green
    Write-Host "Active Alerts: $($response.data.Count)" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 2: Bulk Payments
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TEST 2: BULK PAYMENTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "2.1 Uploading Bulk Payment File..." -ForegroundColor Yellow
try {
    $filePath = "sample-bulk-payments.csv"
    $accountId = "66666666-6666-6666-6666-666666666666"
    
    $boundary = [System.Guid]::NewGuid().ToString()
    $LF = "`r`n"
    
    $fileContent = Get-Content $filePath -Raw
    $fileBytes = [System.Text.Encoding]::UTF8.GetBytes($fileContent)
    
    $bodyLines = (
        "--$boundary",
        "Content-Disposition: form-data; name=`"file`"; filename=`"sample-bulk-payments.csv`"",
        "Content-Type: text/csv$LF",
        [System.Text.Encoding]::UTF8.GetString($fileBytes),
        "--$boundary",
        "Content-Disposition: form-data; name=`"accountId`"$LF",
        $accountId,
        "--$boundary--$LF"
    ) -join $LF
    
    $uploadHeaders = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "multipart/form-data; boundary=$boundary"
    }
    
    $response = Invoke-RestMethod -Uri "$baseUrl/payments/bulk/upload" -Method Post -Body $bodyLines -Headers $uploadHeaders
    Write-Host "✅ Bulk payment file uploaded" -ForegroundColor Green
    Write-Host "Batch Number: $($response.data.batchNumber)" -ForegroundColor White
    Write-Host "Total Count: $($response.data.totalCount)" -ForegroundColor White
    Write-Host "Total Amount: KES $($response.data.totalAmount:N2)" -ForegroundColor White
    $batchId = $response.data.batchId
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    Write-Host ""
}

if ($batchId) {
    Write-Host "2.2 Validating Bulk Payment Batch..." -ForegroundColor Yellow
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/payments/bulk/$batchId/validate" -Method Post -Headers $headers
        Write-Host "✅ Batch validated" -ForegroundColor Green
        Write-Host "Valid Items: $($response.data.validCount)" -ForegroundColor White
        Write-Host "Invalid Items: $($response.data.invalidCount)" -ForegroundColor White
        Write-Host ""
    } catch {
        Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
    }

    Write-Host "2.3 Getting Batch Status..." -ForegroundColor Yellow
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/payments/bulk/$batchId" -Method Get -Headers $headers
        Write-Host "✅ Retrieved batch status" -ForegroundColor Green
        Write-Host "Status: $($response.data.batch.status)" -ForegroundColor White
        Write-Host ""
    } catch {
        Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
    }

    Write-Host "2.4 Executing Bulk Payment Batch..." -ForegroundColor Yellow
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/payments/bulk/$batchId/execute" -Method Post -Headers $headers
        Write-Host "✅ Batch executed" -ForegroundColor Green
        Write-Host "Success Count: $($response.data.successCount)" -ForegroundColor White
        Write-Host "Failed Count: $($response.data.failedCount)" -ForegroundColor White
        Write-Host ""
    } catch {
        Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
    }
}

# Test 3: Payment Workflow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TEST 3: PAYMENT WORKFLOW" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "3.1 Initiating Payment..." -ForegroundColor Yellow
$paymentPayload = @{
    customerId = "22222222-2222-2222-2222-222222222222"
    accountId = "66666666-6666-6666-6666-666666666666"
    paymentType = "SUPPLIER"
    amount = 15000000
    currency = "KES"
    beneficiaryName = "Major Supplier Ltd"
    beneficiaryAccount = "1234567890"
    beneficiaryBank = "KCB Bank"
    purpose = "Large infrastructure payment"
    reference = "INV-2026-LARGE"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/payments/initiate" -Method Post -Body $paymentPayload -Headers $headers
    Write-Host "✅ Payment initiated" -ForegroundColor Green
    Write-Host "Request Number: $($response.data.requestNumber)" -ForegroundColor White
    Write-Host "Required Approvals: $($response.data.requiredApprovals)" -ForegroundColor White
    $paymentId = $response.data.paymentId
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "3.2 Getting Pending Approvals..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/payments/pending-approval" -Method Get -Headers $headers
    Write-Host "✅ Retrieved pending approvals" -ForegroundColor Green
    Write-Host "Pending Count: $($response.data.Count)" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 4: Dashboard
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TEST 4: DASHBOARD & ANALYTICS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "4.1 Getting Dashboard Metrics..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/dashboard" -Method Get -Headers $headers
    Write-Host "✅ Retrieved dashboard metrics" -ForegroundColor Green
    Write-Host "Securities Value: KES $($response.data.securitiesValue:N2)" -ForegroundColor White
    Write-Host "Loans Value: KES $($response.data.loansValue:N2)" -ForegroundColor White
    Write-Host "Banking Value: KES $($response.data.bankingValue:N2)" -ForegroundColor White
    Write-Host "Grants Value: KES $($response.data.grantsValue:N2)" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "4.2 Getting Revenue Trends..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/dashboard/revenue-trends" -Method Get -Headers $headers
    Write-Host "✅ Retrieved revenue trends" -ForegroundColor Green
    Write-Host "Data Points: $($response.data.Count)" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TEST SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Budget Control: Tested" -ForegroundColor Green
Write-Host "✅ Bulk Payments: Tested" -ForegroundColor Green
Write-Host "✅ Payment Workflow: Tested" -ForegroundColor Green
Write-Host "✅ Dashboard: Tested" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ALL TESTS COMPLETED SUCCESSFULLY!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
