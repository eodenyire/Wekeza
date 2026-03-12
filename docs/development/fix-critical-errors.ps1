# PowerShell script to fix critical compilation errors

Write-Host "Fixing critical compilation errors..."

# Fix all remaining AggregateRoot constructor issues
$files = Get-ChildItem -Path "Core/Wekeza.Core.Domain/Aggregates" -Filter "*.cs"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false
    
    # Fix private constructors without base call
    if ($content -match "private \w+\(\)\s*\{" -and $content -notmatch "private \w+\(\)\s*:\s*base\(") {
        $content = $content -replace "(private \w+\(\))\s*\{", '$1 : base(Guid.NewGuid()) {'
        $modified = $true
    }
    
    # Fix constructors with parameters that don't call base
    if ($content -match "public \w+\([^)]+\)\s*\{" -and $content -notmatch "public \w+\([^)]+\)\s*:\s*base\(") {
        # Look for constructors that have an 'id' parameter
        if ($content -match "public (\w+)\(([^)]*Guid id[^)]*)\)\s*\{") {
            $className = $matches[1]
            $parameters = $matches[2]
            $content = $content -replace "(public $className\($parameters\))\s*\{", '$1 : base(id) {'
            $modified = $true
        }
    }
    
    if ($modified) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "Fixed constructors in: $($file.Name)"
    }
}

# Fix specific type conversion issues
$filesToFix = @(
    "Core/Wekeza.Core.Domain/Aggregates/AMLCase.cs",
    "Core/Wekeza.Core.Domain/Aggregates/User.cs",
    "Core/Wekeza.Core.Domain/Aggregates/TransactionMonitoring.cs"
)

foreach ($file in $filesToFix) {
    if (Test-Path $file) {
        $content = Get-Content $file -Raw
        
        # Fix AMLAlertType conversion
        $content = $content -replace "AMLAlertType\.(\w+)", "Wekeza.Core.Domain.Enums.AMLAlertType.$1"
        
        # Fix other enum conversions
        $content = $content -replace "ScreeningResult\.(\w+)", "Wekeza.Core.Domain.Enums.ScreeningResult.$1"
        $content = $content -replace "AlertSeverity\.(\w+)", "Wekeza.Core.Domain.Enums.AlertSeverity.$1"
        $content = $content -replace "MonitoringDecision\.(\w+)", "Wekeza.Core.Domain.Enums.MonitoringDecision.$1"
        $content = $content -replace "MfaMethod\.(\w+)", "Wekeza.Core.Domain.Enums.MfaMethod.$1"
        $content = $content -replace "SecurityClearanceLevel\.(\w+)", "Wekeza.Core.Domain.Enums.SecurityClearanceLevel.$1"
        
        Set-Content $file $content -NoNewline
        Write-Host "Fixed type conversions in: $file"
    }
}

Write-Host "Critical error fixes completed."