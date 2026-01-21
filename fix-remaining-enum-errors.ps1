# Fix Remaining Enum Errors Script
Write-Host "Fixing remaining enum errors..." -ForegroundColor Green

# 1. Add missing enum types to CommonEnums.cs
Write-Host "1. Adding missing enum types to CommonEnums.cs..." -ForegroundColor Yellow

$commonEnumsPath = "Core/Wekeza.Core.Domain/Enums/CommonEnums.cs"
if (Test-Path $commonEnumsPath) {
    $content = Get-Content $commonEnumsPath -Raw
    
    # Add missing enums if they don't exist
    if ($content -notmatch 'enum ParameterType') {
        $content += @"

    public enum ParameterType
    {
        String,
        Integer,
        Decimal,
        Boolean,
        DateTime,
        Json,
        Encrypted
    }
"@
    }
    
    if ($content -notmatch 'enum SecurityLevel') {
        $content += @"

    public enum SecurityLevel
    {
        Public,
        Internal,
        Confidential,
        Secret,
        TopSecret
    }
"@
    }
    
    if ($content -notmatch 'enum EntityType') {
        $content += @"

    public enum EntityType
    {
        Customer,
        Account,
        Transaction,
        User,
        Organization,
        Product
    }
"@
    }
    
    if ($content -notmatch 'enum ScreeningStatus') {
        $content += @"

    public enum ScreeningStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Cleared,
        Flagged
    }
"@
    }
    
    if ($content -notmatch 'enum ScreeningDecision') {
        $content += @"

    public enum ScreeningDecision
    {
        Approved,
        Rejected,
        RequiresReview,
        Escalated
    }
"@
    }
    
    if ($content -notmatch 'enum TradeType') {
        $content += @"

    public enum TradeType
    {
        Buy,
        Sell,
        Transfer,
        Exchange
    }
"@
    }
    
    if ($content -notmatch 'enum TimeWindow') {
        $content += @"

    public enum TimeWindow
    {
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        Yearly
    }
"@
    }
    
    Set-Content $commonEnumsPath $content -Encoding UTF8
    Write-Host "Added missing enum types to CommonEnums.cs" -ForegroundColor Green
}

# 2. Fix SystemParameter.cs to use proper enum references
Write-Host "2. Fixing SystemParameter.cs enum references..." -ForegroundColor Yellow

$systemParameterPath = "Core/Wekeza.Core.Domain/Aggregates/SystemParameter.cs"
if (Test-Path $systemParameterPath) {
    $content = Get-Content $systemParameterPath -Raw
    
    # Add using statement for enums
    if ($content -notmatch 'using Wekeza\.Core\.Domain\.Enums;') {
        $content = $content -replace 'using System;', @"
using System;
using Wekeza.Core.Domain.Enums;
"@
    }
    
    # Fix property declarations
    $content = $content -replace 'public ParameterType Type', 'public Wekeza.Core.Domain.Enums.ParameterType Type'
    $content = $content -replace 'public SecurityLevel ReadLevel', 'public Wekeza.Core.Domain.Enums.SecurityLevel ReadLevel'
    $content = $content -replace 'public SecurityLevel WriteLevel', 'public Wekeza.Core.Domain.Enums.SecurityLevel WriteLevel'
    
    # Fix method parameters
    $content = $content -replace 'ParameterType type', 'Wekeza.Core.Domain.Enums.ParameterType type'
    $content = $content -replace 'SecurityLevel readLevel', 'Wekeza.Core.Domain.Enums.SecurityLevel readLevel'
    $content = $content -replace 'SecurityLevel writeLevel', 'Wekeza.Core.Domain.Enums.SecurityLevel writeLevel'
    
    Set-Content $systemParameterPath $content -Encoding UTF8
    Write-Host "Fixed SystemParameter.cs enum references" -ForegroundColor Green
}

# 3. Fix repository interfaces
Write-Host "3. Fixing repository interface enum references..." -ForegroundColor Yellow

# Fix ISanctionsScreeningRepository
$sanctionsRepoPath = "Core/Wekeza.Core.Domain/Interfaces/ISanctionsScreeningRepository.cs"
if (Test-Path $sanctionsRepoPath) {
    $content = Get-Content $sanctionsRepoPath -Raw
    
    # Add using statement
    if ($content -notmatch 'using Wekeza\.Core\.Domain\.Enums;') {
        $content = $content -replace 'using System;', @"
using System;
using Wekeza.Core.Domain.Enums;
"@
    }
    
    # Fix enum references
    $content = $content -replace '\bEntityType\b', 'Wekeza.Core.Domain.Enums.EntityType'
    $content = $content -replace '\bScreeningStatus\b', 'Wekeza.Core.Domain.Enums.ScreeningStatus'
    
    Set-Content $sanctionsRepoPath $content -Encoding UTF8
    Write-Host "Fixed ISanctionsScreeningRepository enum references" -ForegroundColor Green
}

# Fix ISecurityDealRepository
$securityDealRepoPath = "Core/Wekeza.Core.Domain/Interfaces/ISecurityDealRepository.cs"
if (Test-Path $securityDealRepoPath) {
    $content = Get-Content $securityDealRepoPath -Raw
    
    # Add using statement
    if ($content -notmatch 'using Wekeza\.Core\.Domain\.Enums;') {
        $content = $content -replace 'using System;', @"
using System;
using Wekeza.Core.Domain.Enums;
"@
    }
    
    # Fix enum references
    $content = $content -replace '\bTradeType\b', 'Wekeza.Core.Domain.Enums.TradeType'
    
    Set-Content $securityDealRepoPath $content -Encoding UTF8
    Write-Host "Fixed ISecurityDealRepository enum references" -ForegroundColor Green
}

# Fix ISystemParameterRepository
$systemParamRepoPath = "Core/Wekeza.Core.Domain/Interfaces/ISystemParameterRepository.cs"
if (Test-Path $systemParamRepoPath) {
    $content = Get-Content $systemParamRepoPath -Raw
    
    # Add using statement
    if ($content -notmatch 'using Wekeza\.Core\.Domain\.Enums;') {
        $content = $content -replace 'using System;', @"
using System;
using Wekeza.Core.Domain.Enums;
"@
    }
    
    # Fix enum references
    $content = $content -replace '\bParameterType\b', 'Wekeza.Core.Domain.Enums.ParameterType'
    $content = $content -replace '\bSecurityLevel\b', 'Wekeza.Core.Domain.Enums.SecurityLevel'
    
    Set-Content $systemParamRepoPath $content -Encoding UTF8
    Write-Host "Fixed ISystemParameterRepository enum references" -ForegroundColor Green
}

# 4. Add missing ReportStatus enum values
Write-Host "4. Adding missing ReportStatus enum values..." -ForegroundColor Yellow

$reportingEnumsPath = "Core/Wekeza.Core.Domain/Enums/ReportingEnums.cs"
if (Test-Path $reportingEnumsPath) {
    $content = Get-Content $reportingEnumsPath -Raw
    
    # Add missing enum values if they don't exist
    if ($content -match 'enum ReportStatus' -and $content -notmatch 'Pending') {
        $content = $content -replace '(enum ReportStatus[^}]+)', '$1,
        Pending,
        Failed,
        Archived'
    }
    
    Set-Content $reportingEnumsPath $content -Encoding UTF8
    Write-Host "Added missing ReportStatus enum values" -ForegroundColor Green
}

Write-Host "Enum fixes completed!" -ForegroundColor Green
Write-Host "Running build to verify fixes..." -ForegroundColor Yellow

# Test the build
& "C:\Program Files\dotnet\dotnet.exe" build "Core/Wekeza.Core.Domain/Wekeza.Core.Domain.csproj" --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! All critical errors fixed." -ForegroundColor Green
} else {
    Write-Host "Build still has issues. Check the output above for remaining errors." -ForegroundColor Red
}