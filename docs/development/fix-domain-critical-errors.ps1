#!/usr/bin/env pwsh

Write-Host "Fixing critical Domain compilation errors..." -ForegroundColor Green

# Build and capture errors
$buildOutput = & "C:\Program Files\dotnet\dotnet.exe" build "Core/Wekeza.Core.Domain/Wekeza.Core.Domain.csproj" --verbosity minimal 2>&1

# Extract error lines
$errors = $buildOutput | Where-Object { $_ -match "error CS" }

Write-Host "Found $($errors.Count) errors to fix" -ForegroundColor Yellow

# Group errors by type for systematic fixing
$errorGroups = @{}
foreach ($error in $errors) {
    if ($error -match "error CS(\d+):") {
        $errorCode = $matches[1]
        if (-not $errorGroups.ContainsKey($errorCode)) {
            $errorGroups[$errorCode] = @()
        }
        $errorGroups[$errorCode] += $error
    }
}

Write-Host "Error breakdown:" -ForegroundColor Cyan
foreach ($code in $errorGroups.Keys | Sort-Object) {
    Write-Host "  CS$code`: $($errorGroups[$code].Count) errors" -ForegroundColor White
}

# Show first few errors of each type
foreach ($code in $errorGroups.Keys | Sort-Object) {
    Write-Host "`nCS$code errors (first 3):" -ForegroundColor Yellow
    $errorGroups[$code] | Select-Object -First 3 | ForEach-Object {
        Write-Host "  $_" -ForegroundColor Red
    }
}