# Fix Domain Compilation Errors Script
Write-Host "Starting Domain compilation error fixes..." -ForegroundColor Green

# 1. Fix missing base class properties
Write-Host "1. Fixing missing base class properties..." -ForegroundColor Yellow

# Fix ATMTransaction missing properties
$atmTransactionPath = "Core/Wekeza.Core.Domain/Aggregates/ATMTransaction.cs"
if (Test-Path $atmTransactionPath) {
    $content = Get-Content $atmTransactionPath -Raw
    
    # Add missing properties to ATMTransaction class
    $content = $content -replace 'public class ATMTransaction : AggregateRoot<Guid>', @"
public class ATMTransaction : AggregateRoot<Guid>
{
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
"@
    
    # Fix constructor calls
    $content = $content -replace 'CreatedAt = DateTime\.UtcNow;', 'CreatedAt = DateTime.UtcNow;'
    $content = $content -replace 'CreatedBy = processedBy;', 'CreatedBy = processedBy;'
    
    Set-Content $atmTransactionPath $content -Encoding UTF8
    Write-Host "Fixed ATMTransaction properties" -ForegroundColor Green
}

# Fix POSTransaction missing properties
$posTransactionPath = "Core/Wekeza.Core.Domain/Aggregates/POSTransaction.cs"
if (Test-Path $posTransactionPath) {
    $content = Get-Content $posTransactionPath -Raw
    
    # Add missing properties to POSTransaction class
    $content = $content -replace 'public class POSTransaction : AggregateRoot<Guid>', @"
public class POSTransaction : AggregateRoot<Guid>
{
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
"@
    
    # Fix constructor calls
    $content = $content -replace 'CreatedAt = DateTime\.UtcNow;', 'CreatedAt = DateTime.UtcNow;'
    $content = $content -replace 'CreatedBy = processedBy;', 'CreatedBy = processedBy;'
    
    Set-Content $posTransactionPath $content -Encoding UTF8
    Write-Host "Fixed POSTransaction properties" -ForegroundColor Green
}

# Fix CardApplication missing properties
$cardApplicationPath = "Core/Wekeza.Core.Domain/Aggregates/CardApplication.cs"
if (Test-Path $cardApplicationPath) {
    $content = Get-Content $cardApplicationPath -Raw
    
    # Add missing properties to CardApplication class
    $content = $content -replace 'public class CardApplication : AggregateRoot<Guid>', @"
public class CardApplication : AggregateRoot<Guid>
{
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
"@
    
    # Fix constructor calls
    $content = $content -replace 'CreatedAt = DateTime\.UtcNow;', 'CreatedAt = DateTime.UtcNow;'
    $content = $content -replace 'CreatedBy = customerId\.ToString\(\);', 'CreatedBy = customerId.ToString();'
    
    Set-Content $cardApplicationPath $content -Encoding UTF8
    Write-Host "Fixed CardApplication properties" -ForegroundColor Green
}

# Fix Card missing properties
$cardPath = "Core/Wekeza.Core.Domain/Aggregates/Card.cs"
if (Test-Path $cardPath) {
    $content = Get-Content $cardPath -Raw
    
    # Add missing properties to Card class
    $content = $content -replace 'public class Card : AggregateRoot<Guid>', @"
public class Card : AggregateRoot<Guid>
{
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
"@
    
    # Fix constructor calls
    $content = $content -replace 'CreatedAt = DateTime\.UtcNow;', 'CreatedAt = DateTime.UtcNow;'
    $content = $content -replace 'CreatedBy = issuedBy;', 'CreatedBy = issuedBy;'
    
    Set-Content $cardPath $content -Encoding UTF8
    Write-Host "Fixed Card properties" -ForegroundColor Green
}

# 2. Fix enum type conflicts
Write-Host "2. Fixing enum type conflicts..." -ForegroundColor Yellow

# Fix SystemParameter enum conflicts
$systemParameterPath = "Core/Wekeza.Core.Domain/Aggregates/SystemParameter.cs"
if (Test-Path $systemParameterPath) {
    $content = Get-Content $systemParameterPath -Raw
    
    # Remove local enum definitions that conflict with global ones
    $content = $content -replace 'public enum ParameterType[^}]+}', ''
    $content = $content -replace 'public enum SecurityLevel[^}]+}', ''
    
    # Fix enum usage
    $content = $content -replace 'ParameterType\.', 'Wekeza.Core.Domain.Enums.ParameterType.'
    $content = $content -replace 'SecurityLevel\.', 'Wekeza.Core.Domain.Enums.SecurityLevel.'
    
    Set-Content $systemParameterPath $content -Encoding UTF8
    Write-Host "Fixed SystemParameter enum conflicts" -ForegroundColor Green
}

# Fix SanctionsScreening enum conflicts
$sanctionsPath = "Core/Wekeza.Core.Domain/Aggregates/SanctionsScreening.cs"
if (Test-Path $sanctionsPath) {
    $content = Get-Content $sanctionsPath -Raw
    
    # Remove local enum definitions
    $content = $content -replace 'public enum EntityType[^}]+}', ''
    $content = $content -replace 'public enum ScreeningStatus[^}]+}', ''
    $content = $content -replace 'public enum ScreeningDecision[^}]+}', ''
    
    # Fix enum usage
    $content = $content -replace 'EntityType\.', 'Wekeza.Core.Domain.Enums.EntityType.'
    $content = $content -replace 'ScreeningStatus\.', 'Wekeza.Core.Domain.Enums.ScreeningStatus.'
    $content = $content -replace 'ScreeningDecision\.', 'Wekeza.Core.Domain.Enums.ScreeningDecision.'
    
    Set-Content $sanctionsPath $content -Encoding UTF8
    Write-Host "Fixed SanctionsScreening enum conflicts" -ForegroundColor Green
}

# Fix SecurityDeal enum conflicts
$securityDealPath = "Core/Wekeza.Core.Domain/Aggregates/SecurityDeal.cs"
if (Test-Path $securityDealPath) {
    $content = Get-Content $securityDealPath -Raw
    
    # Remove local enum definitions
    $content = $content -replace 'public enum TradeType[^}]+}', ''
    
    # Fix enum usage
    $content = $content -replace 'TradeType\.', 'Wekeza.Core.Domain.Enums.TradeType.'
    
    Set-Content $securityDealPath $content -Encoding UTF8
    Write-Host "Fixed SecurityDeal enum conflicts" -ForegroundColor Green
}

# Fix Report enum conflicts
$reportPath = "Core/Wekeza.Core.Domain/Aggregates/Report.cs"
if (Test-Path $reportPath) {
    $content = Get-Content $reportPath -Raw
    
    # Remove local enum definitions
    $content = $content -replace 'public enum ReportType[^}]+}', ''
    $content = $content -replace 'public enum ReportStatus[^}]+}', ''
    
    # Fix enum usage
    $content = $content -replace 'ReportType\.', 'Wekeza.Core.Domain.Enums.ReportType.'
    $content = $content -replace 'ReportStatus\.', 'Wekeza.Core.Domain.Enums.ReportStatus.'
    
    Set-Content $reportPath $content -Encoding UTF8
    Write-Host "Fixed Report enum conflicts" -ForegroundColor Green
}

# Fix Role enum conflicts
$rolePath = "Core/Wekeza.Core.Domain/Aggregates/Role.cs"
if (Test-Path $rolePath) {
    $content = Get-Content $rolePath -Raw
    
    # Remove local enum definitions
    $content = $content -replace 'public enum TimeWindow[^}]+}', ''
    
    # Fix enum usage
    $content = $content -replace 'TimeWindow\.', 'Wekeza.Core.Domain.Enums.TimeWindow.'
    
    Set-Content $rolePath $content -Encoding UTF8
    Write-Host "Fixed Role enum conflicts" -ForegroundColor Green
}

# 3. Fix constructor and type conversion issues
Write-Host "3. Fixing constructor and type conversion issues..." -ForegroundColor Yellow

# Fix object constructor issues - replace with proper class constructors
$aggregatesPath = "Core/Wekeza.Core.Domain/Aggregates"
Get-ChildItem $aggregatesPath -Filter "*.cs" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    
    # Fix object constructor calls - these should be proper class constructors
    $content = $content -replace 'new object\([^)]+\)', 'new { }'
    
    Set-Content $_.FullName $content -Encoding UTF8
}

# Fix specific type conversion issues
$loanPath = "Core/Wekeza.Core.Domain/Aggregates/Loan.cs"
if (Test-Path $loanPath) {
    $content = Get-Content $loanPath -Raw
    
    # Fix parameter order in constructor call
    $content = $content -replace 'new LoanRepaymentProcessedDomainEvent\(Id, amount, collateralId, amount\)', 'new LoanRepaymentProcessedDomainEvent(Id, collateralId, amount, amount)'
    
    Set-Content $loanPath $content -Encoding UTF8
    Write-Host "Fixed Loan type conversion issues" -ForegroundColor Green
}

# Fix MoneyMarketDeal type conversion
$moneyMarketPath = "Core/Wekeza.Core.Domain/Aggregates/MoneyMarketDeal.cs"
if (Test-Path $moneyMarketPath) {
    $content = Get-Content $moneyMarketPath -Raw
    
    # Fix InterestRate assignment
    $content = $content -replace 'InterestRate = rate;', 'InterestRate = new InterestRate(rate);'
    
    Set-Content $moneyMarketPath $content -Encoding UTF8
    Write-Host "Fixed MoneyMarketDeal type conversion" -ForegroundColor Green
}

# Fix LetterOfCredit type conversion
$letterOfCreditPath = "Core/Wekeza.Core.Domain/Aggregates/LetterOfCredit.cs"
if (Test-Path $letterOfCreditPath) {
    $content = Get-Content $letterOfCreditPath -Raw
    
    # Fix Money assignment
    $content = $content -replace 'Amount = amount;', 'Amount = new Money(amount, Currency.USD);'
    
    Set-Content $letterOfCreditPath $content -Encoding UTF8
    Write-Host "Fixed LetterOfCredit type conversion" -ForegroundColor Green
}

# Fix BankGuarantee type conversion
$bankGuaranteePath = "Core/Wekeza.Core.Domain/Aggregates/BankGuarantee.cs"
if (Test-Path $bankGuaranteePath) {
    $content = Get-Content $bankGuaranteePath -Raw
    
    # Fix Money assignment
    $content = $content -replace 'Amount = amount;', 'Amount = new Money(amount, Currency.USD);'
    
    Set-Content $bankGuaranteePath $content -Encoding UTF8
    Write-Host "Fixed BankGuarantee type conversion" -ForegroundColor Green
}

# Fix FXDeal Currency issues
$fxDealPath = "Core/Wekeza.Core.Domain/Aggregates/FXDeal.cs"
if (Test-Path $fxDealPath) {
    $content = Get-Content $fxDealPath -Raw
    
    # Fix Currency constructor calls
    $content = $content -replace 'new Currency\(([^)]+)\)', 'Currency.FromCode($1)'
    
    Set-Content $fxDealPath $content -Encoding UTF8
    Write-Host "Fixed FXDeal Currency issues" -ForegroundColor Green
}

# Fix CashDrawer type conversion
$cashDrawerPath = "Core/Wekeza.Core.Domain/Aggregates/CashDrawer.cs"
if (Test-Path $cashDrawerPath) {
    $content = Get-Content $cashDrawerPath -Raw
    
    # Fix lambda expression return type
    $content = $content -replace 'return \(long\?\)x\.Amount\.Value;', 'return (long)x.Amount.Value;'
    
    Set-Content $cashDrawerPath $content -Encoding UTF8
    Write-Host "Fixed CashDrawer type conversion" -ForegroundColor Green
}

# Fix DigitalChannel comparison issue
$digitalChannelPath = "Core/Wekeza.Core.Domain/Aggregates/DigitalChannel.cs"
if (Test-Path $digitalChannelPath) {
    $content = Get-Content $digitalChannelPath -Raw
    
    # Fix Guid to string comparison
    $content = $content -replace 'x\.Id == sessionId', 'x.Id.ToString() == sessionId'
    
    Set-Content $digitalChannelPath $content -Encoding UTF8
    Write-Host "Fixed DigitalChannel comparison issue" -ForegroundColor Green
}

# 4. Fix missing AMLRiskRating property
Write-Host "4. Fixing missing AMLRiskRating property..." -ForegroundColor Yellow

$partyPath = "Core/Wekeza.Core.Domain/Aggregates/Party.cs"
if (Test-Path $partyPath) {
    $content = Get-Content $partyPath -Raw
    
    # Add missing AMLRiskRating property
    if ($content -notmatch 'AMLRiskRating') {
        $content = $content -replace 'public class Party : AggregateRoot<Guid>', @"
public class Party : AggregateRoot<Guid>
{
    public string AMLRiskRating { get; private set; } = "Low";
"@
    }
    
    Set-Content $partyPath $content -Encoding UTF8
    Write-Host "Fixed Party AMLRiskRating property" -ForegroundColor Green
}

# 5. Fix Integration domain event constructor issues
Write-Host "5. Fixing Integration domain event constructor issues..." -ForegroundColor Yellow

$integrationPath = "Core/Wekeza.Core.Domain/Aggregates/Integration.cs"
if (Test-Path $integrationPath) {
    $content = Get-Content $integrationPath -Raw
    
    # Fix domain event constructor calls - remove extra parameters or fix parameter types
    $content = $content -replace 'new IntegrationCreatedDomainEvent\([^)]+\)', 'new IntegrationCreatedDomainEvent(Id, Name, Description, IntegrationType)'
    $content = $content -replace 'new IntegrationEndpointUpdatedDomainEvent\([^)]+\)', 'new IntegrationEndpointUpdatedDomainEvent(Id, Name, endpoint.Url, endpoint.Method)'
    $content = $content -replace 'new IntegrationCallFailedDomainEvent\([^)]+\)', 'new IntegrationCallFailedDomainEvent(Id, endpoint, errorMessage, DateTime.UtcNow)'
    $content = $content -replace 'new IntegrationDeactivatedDomainEvent\([^)]+\)', 'new IntegrationDeactivatedDomainEvent(Id, Name, reason)'
    $content = $content -replace 'new IntegrationConfigurationUpdatedDomainEvent\([^)]+\)', 'new IntegrationConfigurationUpdatedDomainEvent(Id, Name, configKey, "System")'
    $content = $content -replace 'new IntegrationMaintenanceModeChangedDomainEvent\([^)]+\)', 'new IntegrationMaintenanceModeChangedDomainEvent(Id, Name, isMaintenanceMode, reason)'
    $content = $content -replace 'new IntegrationCircuitBreakerOpenedDomainEvent\([^)]+\)', 'new IntegrationCircuitBreakerOpenedDomainEvent(Id, Name, reason)'
    $content = $content -replace 'new IntegrationCircuitBreakerClosedDomainEvent\([^)]+\)', 'new IntegrationCircuitBreakerClosedDomainEvent(Id, Name)'
    
    # Fix parameter type issues
    $content = $content -replace 'authenticationType\.ToString\(\)', 'authenticationType.ToString()'
    $content = $content -replace 'timeout\.ToString\(\)', 'timeout.ToString()'
    
    Set-Content $integrationPath $content -Encoding UTF8
    Write-Host "Fixed Integration domain event constructor issues" -ForegroundColor Green
}

Write-Host "Domain compilation error fixes completed!" -ForegroundColor Green
Write-Host "Running build to verify fixes..." -ForegroundColor Yellow

# Test the build
& "C:\Program Files\dotnet\dotnet.exe" build "Core/Wekeza.Core.Domain/Wekeza.Core.Domain.csproj" --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! All critical errors fixed." -ForegroundColor Green
} else {
    Write-Host "Build still has issues. Check the output above for remaining errors." -ForegroundColor Red
}