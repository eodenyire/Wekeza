#!/usr/bin/env pwsh

Write-Host "Fixing Wekeza Application Layer compilation errors..." -ForegroundColor Green

# Common using statements that are missing
$commonUsings = @(
    "using MediatR;",
    "using Wekeza.Core.Domain.Aggregates;",
    "using Wekeza.Core.Domain.ValueObjects;",
    "using Wekeza.Core.Domain.Interfaces;",
    "using Wekeza.Core.Domain.Enums;",
    "using Wekeza.Core.Application.Common.Exceptions;"
)

# Find all .cs files in Application layer
$csFiles = Get-ChildItem -Path "Core/Wekeza.Core.Application" -Filter "*.cs" -Recurse

Write-Host "Found $($csFiles.Count) C# files to process..." -ForegroundColor Yellow

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Skip if file already has namespace declaration
    if ($content -match "namespace\s+") {
        continue
    }
    
    # Add namespace if missing
    $relativePath = $file.FullName.Replace((Get-Location).Path, "").Replace("\", "/").Replace("Core/Wekeza.Core.Application/", "")
    $namespaceParts = $relativePath.Split("/")[0..($relativePath.Split("/").Length - 2)]
    $namespace = "Wekeza.Core.Application"
    if ($namespaceParts.Length -gt 0) {
        $namespace += "." + ($namespaceParts -join ".")
    }
    
    # Add using statements and namespace
    $newContent = ""
    
    # Add common using statements if not present
    foreach ($using in $commonUsings) {
        if ($content -notmatch [regex]::Escape($using)) {
            $newContent += "$using`n"
        }
    }
    
    $newContent += "`nnamespace $namespace;`n`n"
    
    # Remove any existing class/record/interface declarations and add them back
    $content = $content -replace "^(public\s+(class|record|interface)\s+)", "`$1"
    
    $newContent += $content
    
    # Write back to file
    Set-Content -Path $file.FullName -Value $newContent -Encoding UTF8
    Write-Host "Updated: $($file.Name)" -ForegroundColor Cyan
}

Write-Host "Application layer files updated!" -ForegroundColor Green