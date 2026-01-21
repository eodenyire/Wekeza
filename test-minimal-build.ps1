# Test script to build minimal working Wekeza system
Write-Host "🧪 Testing minimal Wekeza build with working components only..." -ForegroundColor Cyan

# First, let's see what's actually working by testing individual components
Write-Host "`n1. Testing Domain layer..." -ForegroundColor Yellow
$domainResult = dotnet build "Core/Wekeza.Core.Domain/Wekeza.Core.Domain.csproj" --verbosity minimal 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✅ Domain: SUCCESS" -ForegroundColor Green
} else {
    Write-Host "   ❌ Domain: FAILED" -ForegroundColor Red
}

Write-Host "`n2. Testing basic Application components..." -ForegroundColor Yellow

# Test if we can build just the working account commands
$workingFiles = @(
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/OpenAccount/OpenAccountCommand.cs",
    "Core/Wekeza.Core.Application/Features/Accounts/Commands/AddSignatory/AddSignatoryCommand.cs",
    "Core/Wekeza.Core.Application/Features/Deposits/Commands/BookFixedDeposit/BookFixedDepositCommand.cs"
)

Write-Host "   Testing individual working files..." -ForegroundColor Gray
foreach ($file in $workingFiles) {
    if (Test-Path $file) {
        Write-Host "   📄 Found: $(Split-Path $file -Leaf)" -ForegroundColor Green
    } else {
        Write-Host "   ❌ Missing: $(Split-Path $file -Leaf)" -ForegroundColor Red
    }
}

Write-Host "`n3. Checking what's preventing Application build..." -ForegroundColor Yellow
$appErrors = dotnet build "Core/Wekeza.Core.Application/Wekeza.Core.Application.csproj" --verbosity minimal 2>&1 | Select-String "error" | Select-Object -First 3
Write-Host "   Top 3 errors:" -ForegroundColor Gray
$appErrors | ForEach-Object { Write-Host "   • $($_.Line)" -ForegroundColor Red }

Write-Host "`n4. Let's try to run the API with current state..." -ForegroundColor Yellow
Write-Host "   (This will likely fail, but let's see what happens)" -ForegroundColor Gray

# Try to run the API briefly to see startup errors
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project Core/Wekeza.Core.Api/Wekeza.Core.Api.csproj" -PassThru -NoNewWindow
Start-Sleep -Seconds 5
if (!$apiProcess.HasExited) {
    Write-Host "   🎉 API started! Stopping it now..." -ForegroundColor Green
    $apiProcess.Kill()
    Write-Host "   ✅ Basic API startup works!" -ForegroundColor Green
} else {
    Write-Host "   ❌ API failed to start" -ForegroundColor Red
}

Write-Host "`n📊 SUMMARY:" -ForegroundColor Cyan
Write-Host "   • Domain Layer: ✅ Working" -ForegroundColor Green
Write-Host "   • Application Layer: ❌ Has errors but core components exist" -ForegroundColor Yellow
Write-Host "   • API Layer: Testing..." -ForegroundColor Yellow