# PowerShell script to fix critical compilation errors

Write-Host "Fixing AggregateRoot inheritance issues..."

# Get all C# files that contain AggregateRoot<
$files = Get-ChildItem -Path "Core" -Recurse -Filter "*.cs" | Where-Object {
    (Get-Content $_.FullName -Raw) -match "AggregateRoot<"
}

foreach ($file in $files) {
    Write-Host "Processing: $($file.FullName)"
    
    $content = Get-Content $file.FullName -Raw
    
    # Replace AggregateRoot<Guid> with AggregateRoot
    $newContent = $content -replace "AggregateRoot<Guid>", "AggregateRoot"
    
    if ($newContent -ne $content) {
        Set-Content -Path $file.FullName -Value $newContent -NoNewline
        Write-Host "Fixed AggregateRoot inheritance in: $($file.FullName)"
    }
}

Write-Host "AggregateRoot fixes completed!"