# PowerShell script to fix DomainException issues
$files = Get-ChildItem -Path "Core" -Recurse -Filter "*.cs" | Where-Object { $_.FullName -notlike "*\bin\*" -and $_.FullName -notlike "*\obj\*" }

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    # Add using statement if not present and file contains DomainException
    if ($content -match "throw new DomainException" -and $content -notmatch "using Wekeza\.Core\.Domain\.Exceptions;") {
        # Find the last using statement
        $lines = $content -split "`n"
        $lastUsingIndex = -1
        for ($i = 0; $i -lt $lines.Length; $i++) {
            if ($lines[$i] -match "^using ") {
                $lastUsingIndex = $i
            }
        }
        
        if ($lastUsingIndex -ge 0) {
            $lines = $lines[0..$lastUsingIndex] + "using Wekeza.Core.Domain.Exceptions;" + $lines[($lastUsingIndex + 1)..($lines.Length - 1)]
            $content = $lines -join "`n"
        }
    }
    
    # Replace DomainException with GenericDomainException
    $content = $content -replace "throw new DomainException\(", "throw new GenericDomainException("
    
    Set-Content $file.FullName $content -NoNewline
    Write-Host "Updated: $($file.FullName)"
}

Write-Host "Domain exception fixes completed."