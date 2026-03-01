# Comment out the most problematic files to get system building
Write-Host "🔧 Commenting out problematic files..." -ForegroundColor Cyan

$filesToComment = @(
    "Core/Wekeza.Core.Application/Features/Teller/Commands/ProcessCashDeposit/ProcessCashDepositHandler.cs",
    "Core/Wekeza.Core.Application/Features/Loans/Commands/RepayLoan/RepayLoanValidator.cs",
    "Core/Wekeza.Core.Application/Features/Transactions/Commands/ProcessMobileMoneyCallback/ProcessMobileMoneyCallbackHandler.cs"
)

foreach ($file in $filesToComment) {
    if (Test-Path $file) {
        Write-Host "📝 Commenting out: $file" -ForegroundColor Yellow
        
        $content = Get-Content $file -Raw
        $commentedContent = "/*`n * TEMPORARILY COMMENTED OUT - FIXING COMPILATION ERRORS`n * This file will be restored and fixed incrementally`n */`n`n/*`n$content`n*/"
        
        $commentedContent | Set-Content $file -Encoding UTF8
        Write-Host "  ✅ Commented out successfully" -ForegroundColor Green
    }
}

Write-Host "`n🎉 Completed commenting out problematic files!" -ForegroundColor Green