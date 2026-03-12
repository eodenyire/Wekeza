#!/usr/bin/env pwsh

Write-Host "Testing PostgreSQL connection with credentials..." -ForegroundColor Green

# Set environment variable for password to avoid interactive prompt
$env:PGPASSWORD = "the_beast_pass"

try {
    Write-Host "Testing connection to PostgreSQL..." -ForegroundColor Yellow
    $result = & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -h localhost -c "SELECT version();" 2>$null
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ PostgreSQL connection successful!" -ForegroundColor Green
        Write-Host "Version info: $($result[2])" -ForegroundColor Cyan
        
        # Check if wekeza_banking database exists
        Write-Host "Checking for wekeza_banking database..." -ForegroundColor Yellow
        $dbCheck = & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -h localhost -lqt | Select-String "wekeza_banking"
        
        if ($dbCheck) {
            Write-Host "✅ wekeza_banking database already exists" -ForegroundColor Green
        } else {
            Write-Host "Creating wekeza_banking database..." -ForegroundColor Yellow
            $createDb = & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -h localhost -c "CREATE DATABASE wekeza_banking;" 2>$null
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✅ wekeza_banking database created successfully!" -ForegroundColor Green
            } else {
                Write-Host "❌ Failed to create database" -ForegroundColor Red
            }
        }
        
        # Test connection to the wekeza_banking database
        Write-Host "Testing connection to wekeza_banking database..." -ForegroundColor Yellow
        $dbTest = & "C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -h localhost -d wekeza_banking -c "SELECT current_database();" 2>$null
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ Successfully connected to wekeza_banking database!" -ForegroundColor Green
        } else {
            Write-Host "❌ Failed to connect to wekeza_banking database" -ForegroundColor Red
        }
        
    } else {
        Write-Host "❌ PostgreSQL connection failed" -ForegroundColor Red
        Write-Host "Please check if PostgreSQL is running and credentials are correct" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "❌ Error testing PostgreSQL connection: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    # Clear the password environment variable for security
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
}

# Test Redis connection
Write-Host ""
Write-Host "Testing Redis connection..." -ForegroundColor Yellow
try {
    $redisResult = & "redis-cli" ping 2>$null
    if ($redisResult -eq "PONG") {
        Write-Host "✅ Redis connection successful!" -ForegroundColor Green
    } else {
        Write-Host "❌ Redis connection failed" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Redis test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Database Connection Summary:" -ForegroundColor Cyan
Write-Host "- PostgreSQL Host: localhost" -ForegroundColor White
Write-Host "- PostgreSQL Database: wekeza_banking" -ForegroundColor White
Write-Host "- PostgreSQL Username: postgres" -ForegroundColor White
Write-Host "- PostgreSQL Password: the_beast_pass" -ForegroundColor White
Write-Host "- Redis: localhost:6379" -ForegroundColor White
Write-Host ""