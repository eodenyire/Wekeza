# PowerShell script to fix remaining compilation errors

Write-Host "Fixing remaining compilation errors..."

# 1. Fix AggregateRoot constructor issues - add Guid.NewGuid() to constructors missing id parameter
$files = @(
    "Core/Wekeza.Core.Domain/Aggregates/AMLCase.cs",
    "Core/Wekeza.Core.Domain/Aggregates/WorkflowDefinition.cs",
    "Core/Wekeza.Core.Domain/Aggregates/WebhookSubscription.cs",
    "Core/Wekeza.Core.Domain/Aggregates/User.cs",
    "Core/Wekeza.Core.Domain/Aggregates/TransactionMonitoring.cs",
    "Core/Wekeza.Core.Domain/Aggregates/TermDeposit.cs",
    "Core/Wekeza.Core.Domain/Aggregates/TaskAssignment.cs"
)

foreach ($file in $files) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        
        # Fix constructors that call base() without id parameter
        $content = $content -replace ": base\(\)", ": base(Guid.NewGuid())"
        
        Set-Content $file $content -NoNewline
        Write-Host "Fixed AggregateRoot constructor in: $file"
    }
}

# 2. Fix type conversion issues - replace aggregate types with enum types
$typeConversions = @{
    "Wekeza.Core.Domain.Aggregates.AMLAlertType" = "Wekeza.Core.Domain.Enums.AMLAlertType"
    "Wekeza.Core.Domain.Aggregates.ScreeningResult" = "Wekeza.Core.Domain.Enums.ScreeningResult"
    "Wekeza.Core.Domain.Aggregates.AlertSeverity" = "Wekeza.Core.Domain.Enums.AlertSeverity"
    "Wekeza.Core.Domain.Aggregates.MonitoringDecision" = "Wekeza.Core.Domain.Enums.MonitoringDecision"
    "Wekeza.Core.Domain.Aggregates.MfaMethod" = "Wekeza.Core.Domain.Enums.MfaMethod"
    "Wekeza.Core.Domain.Aggregates.SecurityClearanceLevel" = "Wekeza.Core.Domain.Enums.SecurityClearanceLevel"
}

$allFiles = Get-ChildItem -Path "Core" -Recurse -Filter "*.cs" | Where-Object { $_.FullName -notlike "*\bin\*" -and $_.FullName -notlike "*\obj\*" }

foreach ($file in $allFiles) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    foreach ($oldType in $typeConversions.Keys) {
        $newType = $typeConversions[$oldType]
        if ($content -match [regex]::Escape($oldType)) {
            $content = $content -replace [regex]::Escape($oldType), $newType
            $modified = $true
        }
    }
    
    if ($modified) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "Fixed type conversions in: $($file.FullName)"
    }
}

Write-Host "Remaining error fixes completed."