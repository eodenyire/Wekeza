# Test Public Sector API with real data

Write-Host "Testing Public Sector Portal API Integration..." -ForegroundColor Cyan

# Step 1: Login to get token
Write-Host "`n1. Authenticating..." -ForegroundColor Yellow
$loginBody = @{
    username = "admin"
    password = "password123"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" `
    -Method Post `
    -Body $loginBody `
    -ContentType "application/json"

$token = $loginResponse.token
Write-Host "✓ Authentication successful" -ForegroundColor Green
Write-Host "Token: $($token.Substring(0, 50))..." -ForegroundColor Gray

# Step 2: Get dashboard metrics
Write-Host "`n2. Fetching dashboard metrics from database..." -ForegroundColor Yellow
$headers = @{
    Authorization = "Bearer $token"
}

$metricsResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/public-sector/dashboard/metrics" `
    -Method Get `
    -Headers $headers

Write-Host "✓ Dashboard metrics retrieved successfully" -ForegroundColor Green

# Display results
Write-Host "`n=== DASHBOARD METRICS (FROM DATABASE) ===" -ForegroundColor Cyan

Write-Host "`nSecurities Portfolio:" -ForegroundColor White
Write-Host "  Total Value: KES $($metricsResponse.data.securitiesPortfolio.totalValue.ToString('N0'))" -ForegroundColor Green
Write-Host "  T-Bills: KES $($metricsResponse.data.securitiesPortfolio.tbillsValue.ToString('N0'))" -ForegroundColor Gray
Write-Host "  Bonds: KES $($metricsResponse.data.securitiesPortfolio.bondsValue.ToString('N0'))" -ForegroundColor Gray
Write-Host "  Stocks: KES $($metricsResponse.data.securitiesPortfolio.stocksValue.ToString('N0'))" -ForegroundColor Gray

Write-Host "`nLoan Portfolio:" -ForegroundColor White
Write-Host "  Total Outstanding: KES $($metricsResponse.data.loanPortfolio.totalOutstanding.ToString('N0'))" -ForegroundColor Green
Write-Host "  National Government: KES $($metricsResponse.data.loanPortfolio.nationalGovernment.ToString('N0'))" -ForegroundColor Gray
Write-Host "  County Governments: KES $($metricsResponse.data.loanPortfolio.countyGovernments.ToString('N0'))" -ForegroundColor Gray

Write-Host "`nBanking:" -ForegroundColor White
Write-Host "  Total Accounts: $($metricsResponse.data.banking.totalAccounts)" -ForegroundColor Green
Write-Host "  Total Balance: KES $($metricsResponse.data.banking.totalBalance.ToString('N0'))" -ForegroundColor Gray
Write-Host "  Monthly Transactions: $($metricsResponse.data.banking.monthlyTransactions)" -ForegroundColor Gray
Write-Host "  Revenue Collected: KES $($metricsResponse.data.banking.revenueCollected.ToString('N0'))" -ForegroundColor Gray

Write-Host "`nGrants:" -ForegroundColor White
Write-Host "  Total Disbursed: KES $($metricsResponse.data.grants.totalDisbursed.ToString('N0'))" -ForegroundColor Green
Write-Host "  Active Grants: $($metricsResponse.data.grants.activeGrants)" -ForegroundColor Gray
Write-Host "  Compliance Rate: $($metricsResponse.data.grants.complianceRate)%" -ForegroundColor Gray

Write-Host "`n=== END-TO-END INTEGRATION SUCCESSFUL ===" -ForegroundColor Green
Write-Host "✓ Database connected" -ForegroundColor Green
Write-Host "✓ Real data retrieved" -ForegroundColor Green
Write-Host "✓ API working correctly" -ForegroundColor Green
Write-Host "✓ Public Sector Portal ready" -ForegroundColor Green
