# PowerShell script to add CorrelationId property to all command files
Write-Host "🔧 Adding CorrelationId property to command files..." -ForegroundColor Cyan

# Find all command files
$commandFiles = Get-ChildItem -Path "Core/Wekeza.Core.Application/Features" -Recurse -Filter "*Command.cs" | Where-Object { $_.Name -notlike "*Handler.cs" -and $_.Name -notlike "*Response.cs" }

foreach ($file in $commandFiles) {
    Write-Host "📝 Processing: $($file.FullName)" -ForegroundColor Yellow
    
    $content = Get-Content $file.FullName -Raw
    
    # Check if CorrelationId already exists
    if ($content -match "CorrelationId") {
        Write-Host "  ✅ Already has CorrelationId" -ForegroundColor Green
        continue
    }
    
    # Check if it implements ICommand
    if ($content -match "ICommand<") {
        # Find the class declaration and add CorrelationId as first property
        $pattern = '(public record \w+Command[^{]*\{)\s*(\r?\n)'
        if ($content -match $pattern) {
            $replacement = '$1$2    public Guid CorrelationId { get; init; } = Guid.NewGuid();$2'
            $newContent = $content -replace $pattern, $replacement
            
            $newContent | Set-Content $file.FullName -Encoding UTF8
            Write-Host "  ✅ Added CorrelationId" -ForegroundColor Green
        } else {
            Write-Host "  ⚠️  Could not find class pattern" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ⚠️  Does not implement ICommand" -ForegroundColor Yellow
    }
}

Write-Host "`n🎉 Completed adding CorrelationId properties!" -ForegroundColor Green