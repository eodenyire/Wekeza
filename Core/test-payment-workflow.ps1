# Test Payment Workflow API Endpoints
$baseUrl = "http://localhost:5000/api/public-sector/payments"
$authUrl = "http://localhost:5000/api/authentication/login"

Write-Host "=== Testing Payment Workflow API ===" -ForegroundColor Cyan
Write-Host ""

# First, authenticate to get token
Write-Host "0. Authenticating..." -ForegroundColor Yellow
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
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit
}

# Create headers with token
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Test 1: Initiate Payment
Write-Host "1. Testing Payment Initiation..." -ForegroundColor Yellow
$initiatePayload = @{
    customerId = "22222222-2222-2222-2222-222222222222"
    accountId = "66666666-6666-6666-6666-666666666666"
    paymentType = "SUPPLIER"
    amount = 5000000
    currency = "KES"
    beneficiaryName = "ABC Suppliers Ltd"
    beneficiaryAccount = "1234567890"
    beneficiaryBank = "KCB Bank"
    purpose = "Payment for office supplies"
    reference = "INV-2026-001"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/initiate" -Method Post -Body $initiatePayload -Headers $headers
    Write-Host "✅ Payment initiated successfully" -ForegroundColor Green
    Write-Host "Payment ID: $($response.data.paymentId)" -ForegroundColor White
    Write-Host "Request Number: $($response.data.requestNumber)" -ForegroundColor White
    Write-Host "Required Approvals: $($response.data.requiredApprovals)" -ForegroundColor White
    $paymentId = $response.data.paymentId
    Write-Host ""
} catch {
    Write-Host "❌ Failed to initiate payment" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
    Write-Host ""
    exit
}

# Test 2: Get Pending Approvals
Write-Host "2. Testing Get Pending Approvals..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/pending-approval" -Method Get -Headers $headers
    Write-Host "✅ Retrieved pending approvals" -ForegroundColor Green
    Write-Host "Pending Payments: $($response.data.Count)" -ForegroundColor White
    if ($response.data.Count -gt 0) {
        $response.data | ForEach-Object {
            Write-Host "  - $($_.requestnumber): $($_.currency) $($_.amount) to $($_.beneficiaryname)" -ForegroundColor Gray
        }
    }
    Write-Host ""
} catch {
    Write-Host "❌ Failed to get pending approvals" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
}

# Test 3: Get Payment Details
Write-Host "3. Testing Get Payment Details..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/$paymentId" -Method Get -Headers $headers
    Write-Host "✅ Retrieved payment details" -ForegroundColor Green
    Write-Host "Request Number: $($response.data.requestnumber)" -ForegroundColor White
    Write-Host "Amount: $($response.data.currency) $($response.data.amount)" -ForegroundColor White
    Write-Host "Beneficiary: $($response.data.beneficiaryname)" -ForegroundColor White
    Write-Host "Status: $($response.data.status)" -ForegroundColor White
    Write-Host "Current Level: $($response.data.currentapprovallevel) / $($response.data.requiredapprovallevels)" -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "❌ Failed to get payment details" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
}

# Test 4: Approve Payment (Level 1)
Write-Host "4. Testing Payment Approval (Level 1)..." -ForegroundColor Yellow
$approvalPayload = @{
    comments = "Approved by Checker - Budget verified"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/$paymentId/approve" -Method Post -Body $approvalPayload -Headers $headers
    Write-Host "✅ Payment approved at level 1" -ForegroundColor Green
    Write-Host $response.message -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "❌ Failed to approve payment" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
}

# Test 5: Get Approval History
Write-Host "5. Testing Get Approval History..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/$paymentId/approval-history" -Method Get -Headers $headers
    Write-Host "✅ Retrieved approval history" -ForegroundColor Green
    Write-Host "Approval Records: $($response.data.Count)" -ForegroundColor White
    if ($response.data.Count -gt 0) {
        $response.data | ForEach-Object {
            Write-Host "  Level $($_.approvallevel): $($_.action) by $($_.approvername) - $($_.comments)" -ForegroundColor Gray
        }
    }
    Write-Host ""
} catch {
    Write-Host "❌ Failed to get approval history" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
}

# Test 6: Initiate another payment for rejection test
Write-Host "6. Testing Payment Rejection..." -ForegroundColor Yellow
$rejectPayload = @{
    customerId = "22222222-2222-2222-2222-222222222222"
    accountId = "66666666-6666-6666-6666-666666666666"
    paymentType = "SUPPLIER"
    amount = 2000000
    currency = "KES"
    beneficiaryName = "XYZ Services Ltd"
    beneficiaryAccount = "9876543210"
    beneficiaryBank = "Equity Bank"
    purpose = "Payment for consulting services"
    reference = "INV-2026-002"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/initiate" -Method Post -Body $rejectPayload -Headers $headers
    $rejectPaymentId = $response.data.paymentId
    Write-Host "Payment initiated for rejection test: $rejectPaymentId" -ForegroundColor White
    
    # Now reject it
    $rejectionPayload = @{
        reason = "Insufficient documentation provided"
    } | ConvertTo-Json
    
    $response = Invoke-RestMethod -Uri "$baseUrl/$rejectPaymentId/reject" -Method Post -Body $rejectionPayload -Headers $headers
    Write-Host "✅ Payment rejected successfully" -ForegroundColor Green
    Write-Host $response.message -ForegroundColor White
    Write-Host ""
} catch {
    Write-Host "❌ Failed to reject payment" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
}

Write-Host "=== All Tests Completed ===" -ForegroundColor Cyan
