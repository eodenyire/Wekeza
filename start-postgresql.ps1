#!/usr/bin/env pwsh

Write-Host "Starting PostgreSQL Service..." -ForegroundColor Green

# Try to start PostgreSQL service
try {
    Write-Host "Checking for PostgreSQL services..." -ForegroundColor Yellow
    $pgServices = Get-Service | Where-Object {$_.Name -like "*postgresql*"}
    
    if ($pgServices) {
        foreach ($service in $pgServices) {
            Write-Host "Found service: $($service.Name) - Status: $($service.Status)" -ForegroundColor Cyan
            
            if ($service.Status -ne "Running") {
                Write-Host "Starting service: $($service.Name)..." -ForegroundColor Yellow
                Start-Service -Name $service.Name
                Write-Host "Service $($service.Name) started successfully" -ForegroundColor Green
            } else {
                Write-Host "Service $($service.Name) is already running" -ForegroundColor Green
            }
        }
    } else {
        Write-Host "No PostgreSQL services found. Trying alternative methods..." -ForegroundColor Yellow
        
        # Try to start PostgreSQL directly
        $pgPath = "C:\Program Files\PostgreSQL\15\bin\pg_ctl.exe"
        $dataDir = "C:\Program Files\PostgreSQL\15\data"
        
        if (Test-Path $pgPath) {
            Write-Host "Starting PostgreSQL directly..." -ForegroundColor Yellow
            & $pgPath -D $dataDir -l "C:\Program Files\PostgreSQL\15\data\logfile" start
        } else {
            Write-Host "PostgreSQL executable not found at expected location" -ForegroundColor Red
        }
    }
    
    # Test connection
    Start-Sleep -Seconds 3
    Write-Host "Testing PostgreSQL connection..." -ForegroundColor Yellow
    $testResult = & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -c "SELECT version();" 2>$null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "PostgreSQL is now running and accessible!" -ForegroundColor Green
        Write-Host "Connection test successful" -ForegroundColor Green
    } else {
        Write-Host "PostgreSQL may be running but connection test failed" -ForegroundColor Yellow
        Write-Host "This might be due to authentication settings" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "Error starting PostgreSQL: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Manual steps if automatic start failed:" -ForegroundColor Cyan
Write-Host "1. Open Services (services.msc)" -ForegroundColor White
Write-Host "2. Look for 'postgresql-x64-15' or similar" -ForegroundColor White
Write-Host "3. Right-click and select 'Start'" -ForegroundColor White
Write-Host "4. Or run: net start postgresql-x64-15" -ForegroundColor White
Write-Host ""