# Fix Syntax Errors Script
Write-Host "Fixing syntax errors in repository interfaces..." -ForegroundColor Green

# Fix ISanctionsScreeningRepository
$sanctionsRepoPath = "Core/Wekeza.Core.Domain/Interfaces/ISanctionsScreeningRepository.cs"
if (Test-Path $sanctionsRepoPath) {
    $content = Get-Content $sanctionsRepoPath -Raw
    
    # Fix the duplicate enum references
    $content = $content -replace 'Wekeza\.Core\.Domain\.Enums\.EntityType Wekeza\.Core\.Domain\.Enums\.EntityType', 'Wekeza.Core.Domain.Enums.EntityType entityType'
    $content = $content -replace 'Wekeza\.Core\.Domain\.Enums\.ScreeningStatus Wekeza\.Core\.Domain\.Enums\.ScreeningStatus', 'Wekeza.Core.Domain.Enums.ScreeningStatus status'
    
    Set-Content $sanctionsRepoPath $content -Encoding UTF8
    Write-Host "Fixed ISanctionsScreeningRepository syntax errors" -ForegroundColor Green
}

# Fix ISecurityDealRepository
$securityDealRepoPath = "Core/Wekeza.Core.Domain/Interfaces/ISecurityDealRepository.cs"
if (Test-Path $securityDealRepoPath) {
    $content = Get-Content $securityDealRepoPath -Raw
    
    # Fix the duplicate enum references
    $content = $content -replace 'Wekeza\.Core\.Domain\.Enums\.TradeType Wekeza\.Core\.Domain\.Enums\.TradeType', 'Wekeza.Core.Domain.Enums.TradeType tradeType'
    
    Set-Content $securityDealRepoPath $content -Encoding UTF8
    Write-Host "Fixed ISecurityDealRepository syntax errors" -ForegroundColor Green
}

# Fix ISystemParameterRepository
$systemParamRepoPath = "Core/Wekeza.Core.Domain/Interfaces/ISystemParameterRepository.cs"
if (Test-Path $systemParamRepoPath) {
    $content = Get-Content $systemParamRepoPath -Raw
    
    # Fix the duplicate enum references
    $content = $content -replace 'Wekeza\.Core\.Domain\.Enums\.ParameterType Wekeza\.Core\.Domain\.Enums\.ParameterType', 'Wekeza.Core.Domain.Enums.ParameterType parameterType'
    $content = $content -replace 'Wekeza\.Core\.Domain\.Enums\.SecurityLevel Wekeza\.Core\.Domain\.Enums\.SecurityLevel', 'Wekeza.Core.Domain.Enums.SecurityLevel securityLevel'
    
    Set-Content $systemParamRepoPath $content -Encoding UTF8
    Write-Host "Fixed ISystemParameterRepository syntax errors" -ForegroundColor Green
}

Write-Host "Syntax error fixes completed!" -ForegroundColor Green
Write-Host "Running build to verify fixes..." -ForegroundColor Yellow

# Test the build
& "C:\Program Files\dotnet\dotnet.exe" build "Core/Wekeza.Core.Domain/Wekeza.Core.Domain.csproj" --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! All critical errors fixed." -ForegroundColor Green
} else {
    Write-Host "Build still has issues. Check the output above for remaining errors." -ForegroundColor Red
}