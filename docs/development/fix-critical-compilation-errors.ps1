# PowerShell script to fix critical compilation errors in Wekeza Core Banking System

Write-Host "Starting comprehensive compilation error fixes..." -ForegroundColor Green

# 1. Add missing enum values to CommonEnums.cs
Write-Host "Adding missing enum values..." -ForegroundColor Yellow

$commonEnumsPath = "Core/Wekeza.Core.Domain/Enums/CommonEnums.cs"
$commonEnumsContent = Get-Content $commonEnumsPath -Raw

# Add missing IntegrationStatus values
if ($commonEnumsContent -notmatch "Maintenance = 5") {
    $commonEnumsContent = $commonEnumsContent -replace "Error = 4", "Error = 4,`n    Maintenance = 5"
}

# Add missing GatewayStatus values
if ($commonEnumsContent -notmatch "Active = 1") {
    $commonEnumsContent = $commonEnumsContent -replace "public enum GatewayStatus`n{`n    Online = 1", "public enum GatewayStatus`n{`n    Active = 1,`n    Inactive = 2,`n    Online = 3"
}

# Add missing AuthenticationType values
if ($commonEnumsContent -notmatch "ApiKey = 3") {
    $commonEnumsContent = $commonEnumsContent -replace "ApiKey = 3", "ApiKey = 3,`n    APIKey = 4"
}

# Add missing WorkflowStatus values
if ($commonEnumsContent -notmatch "WorkflowStatus") {
    $workflowStatusEnum = @"

public enum WorkflowStatus
{
    Draft = 1,
    Initiated = 2,
    InProgress = 3,
    Pending = 4,
    Approved = 5,
    Rejected = 6,
    Completed = 7,
    Cancelled = 8,
    Suspended = 9,
    Active = 10,
    Validated = 11
}

public enum WorkflowCommentType
{
    General = 1,
    Approval = 2,
    Rejection = 3,
    Escalation = 4,
    Cancellation = 5,
    Information = 6
}

public enum ApprovalStepStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Skipped = 4,
    Cancelled = 5
}
"@
    $commonEnumsContent += $workflowStatusEnum
}

# Add missing DepositStatus values
if ($commonEnumsContent -notmatch "DepositStatus") {
    $depositStatusEnum = @"

public enum DepositStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Closed = 4,
    Matured = 5,
    Cancelled = 6
}

public enum AccountStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Closed = 4,
    Frozen = 5,
    Dormant = 6
}

public enum KYCStatus
{
    Pending = 1,
    InProgress = 2,
    Verified = 3,
    Rejected = 4,
    Expired = 5
}

public enum ProductType
{
    SavingsAccount = 1,
    CurrentAccount = 2,
    FixedDeposit = 3,
    RecurringDeposit = 4,
    Loan = 5,
    CreditCard = 6,
    DebitCard = 7,
    MortgageLoan = 8
}

public enum JournalType
{
    General = 1,
    Adjustment = 2,
    Closing = 3,
    Opening = 4,
    Transfer = 5
}
"@
    $commonEnumsContent += $depositStatusEnum
}

Set-Content $commonEnumsPath $commonEnumsContent -Encoding UTF8

Write-Host "Updated CommonEnums.cs with missing enum values" -ForegroundColor Green

# 2. Remove duplicate enum definitions from aggregate files
Write-Host "Removing duplicate enum definitions from aggregate files..." -ForegroundColor Yellow

$aggregateFiles = Get-ChildItem "Core/Wekeza.Core.Domain/Aggregates/*.cs" -Recurse

foreach ($file in $aggregateFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # Remove duplicate enum definitions at the end of files
    $content = $content -replace "// Enumerations[\s\S]*?public enum.*?{[\s\S]*?}", ""
    $content = $content -replace "public enum AMLAlertType[\s\S]*?}", ""
    $content = $content -replace "public enum AMLResolution[\s\S]*?}", ""
    $content = $content -replace "public enum ScreeningResult[\s\S]*?}", ""
    $content = $content -replace "public enum AlertSeverity[\s\S]*?}", ""
    $content = $content -replace "public enum MonitoringDecision[\s\S]*?}", ""
    $content = $content -replace "public enum MfaMethod[\s\S]*?}", ""
    $content = $content -replace "public enum UserStatus[\s\S]*?}", ""
    $content = $content -replace "public enum AccountStatus[\s\S]*?}", ""
    $content = $content -replace "public enum CreditRiskGrade[\s\S]*?}", ""
    $content = $content -replace "public enum GuaranteeType[\s\S]*?}", ""
    $content = $content -replace "public enum MoneyMarketDealType[\s\S]*?}", ""
    $content = $content -replace "public enum RiskLevel[\s\S]*?}", ""
    
    Set-Content $file.FullName $content -Encoding UTF8
}

Write-Host "Removed duplicate enum definitions from aggregate files" -ForegroundColor Green

# 3. Add missing using statements to files that need enums
Write-Host "Adding missing using statements..." -ForegroundColor Yellow

$filesToUpdate = @(
    "Core/Wekeza.Core.Domain/Aggregates/AMLCase.cs",
    "Core/Wekeza.Core.Domain/Aggregates/TransactionMonitoring.cs",
    "Core/Wekeza.Core.Domain/Aggregates/User.cs",
    "Core/Wekeza.Core.Domain/Aggregates/MoneyMarketDeal.cs",
    "Core/Wekeza.Core.Domain/Aggregates/Loan.cs",
    "Core/Wekeza.Core.Domain/Aggregates/BankGuarantee.cs",
    "Core/Wekeza.Core.Domain/Aggregates/AuditLog.cs",
    "Core/Wekeza.Core.Domain/Aggregates/ApprovalWorkflow.cs",
    "Core/Wekeza.Core.Domain/Aggregates/APIGateway.cs",
    "Core/Wekeza.Core.Domain/Aggregates/Integration.cs",
    "Core/Wekeza.Core.Domain/Services/PaymentProcessingService.cs",
    "Core/Wekeza.Core.Domain/Services/CreditScoringService.cs"
)

foreach ($filePath in $filesToUpdate) {
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Add using statement if not present
        if ($content -notmatch "using Wekeza\.Core\.Domain\.Enums;") {
            $content = $content -replace "(using Wekeza\.Core\.Domain\.Common;)", "`$1`nusing Wekeza.Core.Domain.Enums;"
            Set-Content $filePath $content -Encoding UTF8
            Write-Host "Added using statement to $filePath" -ForegroundColor Cyan
        }
    }
}

Write-Host "Compilation error fixes completed!" -ForegroundColor Green
Write-Host "Run 'dotnet build' to check remaining errors." -ForegroundColor Yellow