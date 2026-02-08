#!/usr/bin/env pwsh

$ErrorActionPreference = "Stop"
$files = Get-ChildItem -Path "Core/Wekeza.Core.Application" -Filter "*.cs" -Recurse

$fixCount = 0

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Fix 1: Convert Guid? UserId to string with proper null handling
    $content = $content -replace '(\s+)_currentUserService\.UserId(\s*[,\)])', '$1_currentUserService.UserId?.ToString() ?? ""$2'
    $content = $content -replace '(\s+)request\.UserId(\s*[,\)])', '$1request.UserId?.ToString() ?? ""$2'
    
    # Fix 2: Convert new Currency(x) to Currency.FromCode(x)
    $content = $content -replace 'new Currency\(([^)]+)\)', 'Currency.FromCode($1)'
    
    # Fix 3: Convert double literals to decimal
    $content = $content -replace '(\d+\.\d+)m?([,\s\)])', '${1}m$2'
    
    # Fix 4: Handle Guid? to Guid conversions
    $content = $content -replace '(\s+)request\.(\w+)(\s+is Guid[^?])', '$1request.$2.Value$3'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        $fixCount++
        Write-Host "Fixed: $($file.FullName)"
    }
}

Write-Host "`nTotal files fixed: $fixCount"
