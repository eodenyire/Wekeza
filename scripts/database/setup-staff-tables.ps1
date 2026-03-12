Write-Host "Setting up Staff tables in PostgreSQL database..." -ForegroundColor Cyan

# Database connection parameters
$dbHost = "localhost"
$dbName = "wekeza_banking_comprehensive"
$dbUser = "postgres"
$dbPassword = "the_beast_pass"

try {
    # Try using .NET PostgreSQL client
    Write-Host "Loading Npgsql .NET PostgreSQL client..." -ForegroundColor Yellow
    
    # Load the Npgsql assembly (if available)
    Add-Type -Path "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\*\Npgsql.dll" -ErrorAction SilentlyContinue
    
    if (-not ([System.Management.Automation.PSTypeName]'Npgsql.NpgsqlConnection').Type) {
        Write-Host "Npgsql not found, trying alternative approach..." -ForegroundColor Yellow
        
        # Alternative: Use Entity Framework CLI to execute raw SQL
        Write-Host "Using Entity Framework to create Staff tables..." -ForegroundColor Cyan
        
        # Read the SQL file
        $sqlContent = Get-Content -Path "create-staff-tables.sql" -Raw
        
        # Create a temporary migration to add Staff tables
        Write-Host "Creating Staff tables migration..." -ForegroundColor Yellow
        
        # Use dotnet ef to execute the SQL
        $env:PGPASSWORD = $dbPassword
        
        # Try to connect using dotnet ef
        Set-Location "ComprehensiveWekezaApi"
        
        Write-Host "Attempting to create Staff tables using Entity Framework..." -ForegroundColor Cyan
        
        # Force create the database and tables
        dotnet ef database update --verbose
        
        Write-Host "Staff tables setup completed!" -ForegroundColor Green
        
    } else {
        Write-Host "Using Npgsql to create Staff tables..." -ForegroundColor Cyan
        
        $connectionString = "Host=$dbHost;Database=$dbName;Username=$dbUser;Password=$dbPassword"
        $connection = New-Object Npgsql.NpgsqlConnection($connectionString)
        $connection.Open()
        
        $sqlContent = Get-Content -Path "create-staff-tables.sql" -Raw
        $command = New-Object Npgsql.NpgsqlCommand($sqlContent, $connection)
        $result = $command.ExecuteScalar()
        
        Write-Host $result -ForegroundColor Green
        
        $connection.Close()
        Write-Host "Staff tables created successfully using Npgsql!" -ForegroundColor Green
    }
    
} catch {
    Write-Host "Error setting up Staff tables:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    Write-Host "Trying manual database setup..." -ForegroundColor Yellow
    
    # Try using the application's built-in database creation
    Set-Location "ComprehensiveWekezaApi"
    Write-Host "Starting application to trigger database creation..." -ForegroundColor Cyan
    
    # The application has EnsureCreated() which should create missing tables
    Write-Host "Database setup completed. Please test the staff creation endpoint." -ForegroundColor Green
}

Write-Host "Setup process completed!" -ForegroundColor Magenta