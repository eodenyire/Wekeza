# Reset Database with New Schema
Write-Host "Resetting database with enhanced schema..." -ForegroundColor Yellow

# Connect to PostgreSQL and drop/recreate the database
$env:PGPASSWORD = "the_beast_pass"
$psqlPath = "C:\Program Files\PostgreSQL\15\bin\psql.exe"

Write-Host "Dropping existing database..." -ForegroundColor Cyan
& $psqlPath -h localhost -U postgres -c "DROP DATABASE IF EXISTS wekeza_banking;"

Write-Host "Creating new database..." -ForegroundColor Cyan  
& $psqlPath -h localhost -U postgres -c "CREATE DATABASE wekeza_banking;"

Write-Host "Database reset complete!" -ForegroundColor Green
Write-Host "The application will recreate tables with the new schema on startup." -ForegroundColor Green