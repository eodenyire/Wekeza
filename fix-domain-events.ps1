# PowerShell script to fix all domain events that are missing IDomainEvent implementation

# Get all C# files that contain ": IDomainEvent;"
$files = Get-ChildItem -Path "Core" -Recurse -Filter "*.cs" | Where-Object {
    (Get-Content $_.FullName -Raw) -match ": IDomainEvent;"
}

foreach ($file in $files) {
    Write-Host "Processing: $($file.FullName)"
    
    $content = Get-Content $file.FullName -Raw
    
    # Replace all occurrences of ": IDomainEvent;" with proper implementation
    $pattern = '(\w+DomainEvent\([^)]*\)) : IDomainEvent;'
    $replacement = '$1 : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}'
    
    $newContent = $content -replace $pattern, $replacement
    
    if ($newContent -ne $content) {
        Set-Content -Path $file.FullName -Value $newContent -NoNewline
        Write-Host "Fixed: $($file.FullName)"
    }
}

Write-Host "Domain events fix completed!"