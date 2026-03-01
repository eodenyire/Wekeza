#!/usr/bin/env pwsh

Write-Host "🔧 Fixing Critical Compilation Errors Systematically..." -ForegroundColor Yellow

# Fix InterestRate constructor calls (2 parameters -> 1 parameter)
Write-Host "1. Fixing InterestRate constructor calls..." -ForegroundColor Green

$files = @(
    "Core/Wekeza.Core.Application/Features/Deposits/Commands/ProcessInterestAccrual/ProcessInterestAccrualHandler.cs",
    "Core/Wekeza.Core.Application/Features/Deposits/Commands/OpenRecurringDeposit/OpenRecurringDepositHandler.cs",
    "Core/Wekeza.Core.Application/Features/Deposits/Commands/BookFixedDeposit/BookFixedDepositHandler.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  Fixing $file" -ForegroundColor Cyan
        $content = Get-Content $file -Raw
        # Fix InterestRate constructor - remove second parameter
        $content = $content -replace 'new InterestRate\(([^,]+),\s*[^)]+\)', 'new InterestRate($1)'
        Set-Content $file $content -NoNewline
    }
}

# Fix NotFoundException constructor calls (missing key parameter)
Write-Host "2. Fixing NotFoundException constructor calls..." -ForegroundColor Green

$files = @(
    "Core/Wekeza.Core.Application/Features/Workflows/Commands/ApproveWorkflow/ApproveWorkflowHandler.cs",
    "Core/Wekeza.Core.Application/Features/CIF/Commands/UpdateKYCStatus/UpdateKYCStatusHandler.cs",
    "Core/Wekeza.Core.Application/Features/CIF/Queries/GetCustomer360View/GetCustomer360ViewHandler.cs",
    "Core/Wekeza.Core.Application/Features/Products/Commands/ActivateProduct/ActivateProductHandler.cs",
    "Core/Wekeza.Core.Application/Features/Products/Queries/GetProductDetails/GetProductDetailsHandler.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  Fixing $file" -ForegroundColor Cyan
        $content = Get-Content $file -Raw
        # Fix NotFoundException constructor - add key parameter
        $content = $content -replace 'new NotFoundException\("([^"]+)"\)', 'new NotFoundException("$1", "NotFound")'
        Set-Content $file $content -NoNewline
    }
}

# Fix Guid?? string issues (cannot use ?? operator with Guid? and string)
Write-Host "3. Fixing Guid?? string issues..." -ForegroundColor Green

$files = @(
    "Core/Wekeza.Core.Application/Features/Workflows/Commands/ApproveWorkflow/ApproveWorkflowHandler.cs",
    "Core/Wekeza.Core.Application/Features/CIF/Commands/CreateCorporateParty/CreateCorporatePartyHandler.cs",
    "Core/Wekeza.Core.Application/Features/GeneralLedger/Commands/CreateGLAccount/CreateGLAccountHandler.cs",
    "Core/Wekeza.Core.Application/Features/GeneralLedger/Commands/PostJournalEntry/PostJournalEntryHandler.cs",
    "Core/Wekeza.Core.Application/Features/Products/Commands/CreateProduct/CreateProductHandler.cs",
    "Core/Wekeza.Core.Application/Features/Teller/Commands/StartTellerSession/StartTellerSessionHandler.cs",
    "Core/Wekeza.Core.Application/Features/Loans/Commands/ApplyForLoan/ApplyForLoanHandler.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  Fixing $file" -ForegroundColor Cyan
        $content = Get-Content $file -Raw
        # Fix Guid?? string - use ToString() or conditional
        $content = $content -replace '(\w+Id)\s*\?\?\s*"([^"]*)"', '$1?.ToString() ?? "$2"'
        Set-Content $file $content -NoNewline
    }
}

# Fix missing transactionReference parameter in Account.Credit/Debit calls
Write-Host "4. Fixing missing transactionReference parameter..." -ForegroundColor Green

$files = @(
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/OpenAccount/OpenAccountHandler.cs",
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/Management/FreezeAccountHandler.cs",
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/Management/CloseAccountHandler.cs",
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/FreezeAccount/FreezeAccountHandler.cs",
    "Core/Wekeza.Core.Application/Features/FixedDeposits/BookFixedDepositHandler.cs",
    "Core/Wekeza.Core.Application/Features/Transactions/Commands/WithdrawFunds/WithdrawFundsHandler.cs",
    "Core/Wekeza.Core.Application/Features/Instruments/Cards/Commands/WithdrawFromAtm/WithdrawFromAtmHandler.cs",
    "Core/Wekeza.Core.Application/Features/Transactions/Commands/DepositFunds/DepositFundsHandler.cs",
    "Core/Wekeza.Core.Application/Features/Transactions/Commands/DepositCheque/DepositChequeHandler.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  Fixing $file" -ForegroundColor Cyan
        $content = Get-Content $file -Raw
        # Fix Credit/Debit calls - add transactionReference parameter
        $content = $content -replace '\.Credit\(([^,]+),\s*"([^"]+)"\)', '.Credit($1, Guid.NewGuid().ToString(), "$2")'
        $content = $content -replace '\.Debit\(([^,]+),\s*"([^"]+)"\)', '.Debit($1, Guid.NewGuid().ToString(), "$2")'
        Set-Content $file $content -NoNewline
    }
}

# Fix missing reason parameter in Account.Freeze/Close calls
Write-Host "5. Fixing missing reason parameter in Account.Freeze/Close..." -ForegroundColor Green

$files = @(
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/Management/FreezeAccountHandler.cs",
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/Management/CloseAccountHandler.cs",
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/FreezeAccount/FreezeAccountHandler.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  Fixing $file" -ForegroundColor Cyan
        $content = Get-Content $file -Raw
        # Fix Freeze calls - add reason parameter
        $content = $content -replace '\.Freeze\("([^"]+)"\)', '.Freeze("$1", _currentUserService.Username ?? "System")'
        # Fix Close calls - add reason parameter  
        $content = $content -replace '\.Close\("([^"]+)"\)', '.Close("$1", _currentUserService.Username ?? "System")'
        Set-Content $file $content -NoNewline
    }
}

Write-Host "✅ Critical compilation errors fixed!" -ForegroundColor Green
Write-Host "🔄 Testing compilation..." -ForegroundColor Yellow

# Test compilation
dotnet build Core/Wekeza.Core.Api/Wekeza.Core.Api.csproj --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Compilation successful!" -ForegroundColor Green
} else {
    Write-Host "❌ Still have compilation errors. Running detailed build..." -ForegroundColor Red
    dotnet build Core/Wekeza.Core.Api/Wekeza.Core.Api.csproj
}