#!/usr/bin/env pwsh

Write-Host "Systematic fix for Application layer..." -ForegroundColor Green

# Step 1: Fix all files missing MediatR using statements
Write-Host "Step 1: Adding MediatR using statements..." -ForegroundColor Yellow

$files = Get-ChildItem -Path "Core/Wekeza.Core.Application" -Filter "*.cs" -Recurse
$mediatRTypes = @("IRequest", "IRequestHandler", "IPipelineBehavior")

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $needsMediatR = $false
    
    foreach ($type in $mediatRTypes) {
        if ($content -match $type -and $content -notmatch "using MediatR;") {
            $needsMediatR = $true
            break
        }
    }
    
    if ($needsMediatR) {
        $lines = Get-Content $file.FullName
        $newLines = @()
        $usingAdded = $false
        
        foreach ($line in $lines) {
            if ($line -match "^using " -and -not $usingAdded) {
                $newLines += "using MediatR;"
                $usingAdded = $true
            }
            $newLines += $line
        }
        
        if (-not $usingAdded) {
            $newLines = @("using MediatR;", "") + $newLines
        }
        
        Set-Content -Path $file.FullName -Value $newLines -Encoding UTF8
        Write-Host "  Added MediatR to: $($file.Name)" -ForegroundColor Cyan
    }
}

# Step 2: Fix files missing Domain using statements
Write-Host "Step 2: Adding Domain using statements..." -ForegroundColor Yellow

$domainTypes = @("Money", "Currency", "Account", "Customer", "Transaction")
$domainUsings = @(
    "using Wekeza.Core.Domain.Aggregates;",
    "using Wekeza.Core.Domain.ValueObjects;",
    "using Wekeza.Core.Domain.Interfaces;"
)

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $needsDomain = $false
    
    foreach ($type in $domainTypes) {
        if ($content -match $type -and $content -notmatch "using Wekeza\.Core\.Domain") {
            $needsDomain = $true
            break
        }
    }
    
    if ($needsDomain) {
        $lines = Get-Content $file.FullName
        $newLines = @()
        $usingSection = $true
        
        foreach ($line in $lines) {
            if ($line -match "^using " -and $usingSection) {
                $newLines += $line
            } elseif ($line.Trim() -eq "" -and $usingSection) {
                # Add domain usings before the empty line
                foreach ($using in $domainUsings) {
                    if ($content -notmatch [regex]::Escape($using)) {
                        $newLines += $using
                    }
                }
                $newLines += $line
                $usingSection = $false
            } else {
                $newLines += $line
                $usingSection = $false
            }
        }
        
        Set-Content -Path $file.FullName -Value $newLines -Encoding UTF8
        Write-Host "  Added Domain usings to: $($file.Name)" -ForegroundColor Cyan
    }
}

# Step 3: Create missing query/command classes
Write-Host "Step 3: Creating missing query/command classes..." -ForegroundColor Yellow

$missingClasses = @{
    "Core/Wekeza.Core.Application/Features/Transactions/Queries/GetStatement/GetStatementQuery.cs" = @"
using MediatR;

namespace Wekeza.Core.Application.Features.Transactions.Queries.GetStatement;

public record GetStatementQuery(
    string AccountNumber,
    DateTime FromDate,
    DateTime ToDate
) : IRequest<StatementDto>;

public record StatementDto
{
    public string AccountNumber { get; init; } = string.Empty;
    public string AccountName { get; init; } = string.Empty;
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public decimal OpeningBalance { get; init; }
    public decimal ClosingBalance { get; init; }
    public List<StatementEntryDto> Entries { get; init; } = new();
}

public record StatementEntryDto
{
    public DateTime Date { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Reference { get; init; } = string.Empty;
    public decimal DebitAmount { get; init; }
    public decimal CreditAmount { get; init; }
    public decimal Balance { get; init; }
}
"@
}

foreach ($class in $missingClasses.GetEnumerator()) {
    if (-not (Test-Path $class.Key)) {
        $directory = Split-Path $class.Key -Parent
        if (-not (Test-Path $directory)) {
            New-Item -Path $directory -ItemType Directory -Force | Out-Null
        }
        Set-Content -Path $class.Key -Value $class.Value -Encoding UTF8
        Write-Host "  Created: $($class.Key)" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Building Application project to check progress..." -ForegroundColor Yellow

$errorCount = (dotnet build "Core/Wekeza.Core.Application/Wekeza.Core.Application.csproj" --verbosity minimal | Select-String "error" | Measure-Object).Count

Write-Host "Remaining errors: $errorCount" -ForegroundColor $(if ($errorCount -lt 100) { "Green" } elseif ($errorCount -lt 150) { "Yellow" } else { "Red" })

if ($errorCount -lt 50) {
    Write-Host "Great progress! Moving to Infrastructure layer next..." -ForegroundColor Green
} elseif ($errorCount -lt 100) {
    Write-Host "Good progress! Continue with remaining fixes..." -ForegroundColor Yellow
} else {
    Write-Host "Still many errors. Need to continue systematic fixes..." -ForegroundColor Red
}

Write-Host ""