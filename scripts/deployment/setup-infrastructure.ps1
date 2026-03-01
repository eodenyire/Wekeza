#!/usr/bin/env pwsh

Write-Host "Setting up Wekeza Banking System Infrastructure..." -ForegroundColor Green

# Check if PostgreSQL is running
Write-Host "Checking PostgreSQL connection..." -ForegroundColor Yellow
try {
    # Use full path to psql since it's in Program Files
    $env:PATH += ";C:\Program Files\PostgreSQL\15\bin"
    $pgResult = & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -c "SELECT version();" 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "PostgreSQL is running" -ForegroundColor Green
    } else {
        Write-Host "PostgreSQL is not accessible. Trying to start service..." -ForegroundColor Yellow
        Start-Service -Name "postgresql*" -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 3
        $pgResult = & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -c "SELECT version();" 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "PostgreSQL is now running" -ForegroundColor Green
        } else {
            Write-Host "PostgreSQL connection failed. Please check your installation." -ForegroundColor Red
        }
    }
} catch {
    Write-Host "PostgreSQL is not accessible. Please ensure it's running." -ForegroundColor Red
}

# Check if Redis is running
Write-Host "Checking Redis connection..." -ForegroundColor Yellow
try {
    # Start Redis server if not running
    $redisProcess = Get-Process -Name "redis-server" -ErrorAction SilentlyContinue
    if (-not $redisProcess) {
        Write-Host "Starting Redis server..." -ForegroundColor Yellow
        Start-Process -FilePath "C:\Program Files\PostgreSQL\redis-server.exe" -WindowStyle Hidden -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
    }
    
    $redisResult = & "redis-cli" ping 2>$null
    if ($redisResult -eq "PONG") {
        Write-Host "Redis is running" -ForegroundColor Green
    } else {
        Write-Host "Redis is not responding. Please check your installation." -ForegroundColor Yellow
    }
} catch {
    Write-Host "Redis check failed. Please ensure it's installed and accessible." -ForegroundColor Yellow
}

# Create Wekeza database if it doesn't exist
Write-Host "Setting up Wekeza database..." -ForegroundColor Yellow
try {
    $dbExists = & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -lqt | Select-String "wekeza_banking"
    if (-not $dbExists) {
        Write-Host "Creating wekeza_banking database..." -ForegroundColor Yellow
        & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -c "CREATE DATABASE wekeza_banking;"
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Database created successfully" -ForegroundColor Green
        } else {
            Write-Host "Failed to create database" -ForegroundColor Red
        }
    } else {
        Write-Host "Database already exists" -ForegroundColor Green
    }
} catch {
    Write-Host "Database setup failed" -ForegroundColor Red
}

# Update connection strings in appsettings
Write-Host "Updating connection strings..." -ForegroundColor Yellow

$apiAppSettings = "Core/Wekeza.Core.Api/appsettings.json"
if (Test-Path $apiAppSettings) {
    $appSettings = Get-Content $apiAppSettings | ConvertFrom-Json
    
    # Update PostgreSQL connection string
    if (-not $appSettings.ConnectionStrings) {
        $appSettings | Add-Member -Type NoteProperty -Name "ConnectionStrings" -Value @{}
    }
    $appSettings.ConnectionStrings.DefaultConnection = "Host=localhost;Database=wekeza_banking;Username=postgres;Password=the_beast_pass"
    $appSettings.ConnectionStrings.Redis = "localhost:6379"
    
    $appSettings | ConvertTo-Json -Depth 10 | Set-Content $apiAppSettings
    Write-Host "Connection strings updated" -ForegroundColor Green
} else {
    Write-Host "appsettings.json not found" -ForegroundColor Red
}

# Build the Domain project (we know this works)
Write-Host "Building Domain project..." -ForegroundColor Yellow
dotnet build "Core/Wekeza.Core.Domain/Wekeza.Core.Domain.csproj" --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "Domain project built successfully" -ForegroundColor Green
} else {
    Write-Host "Domain project build failed" -ForegroundColor Red
}

Write-Host ""
Write-Host "Infrastructure setup completed!" -ForegroundColor Green
Write-Host ""
Write-Host "Database Connection Details:" -ForegroundColor Cyan
Write-Host "- Host: localhost" -ForegroundColor White
Write-Host "- Database: wekeza_banking" -ForegroundColor White
Write-Host "- Username: postgres" -ForegroundColor White
Write-Host "- Redis: localhost:6379" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Fix remaining Application layer compilation errors" -ForegroundColor White
Write-Host "2. Set up Entity Framework migrations" -ForegroundColor White
Write-Host "3. Run database migrations" -ForegroundColor White
Write-Host "4. Start the API server" -ForegroundColor White
Write-Host ""