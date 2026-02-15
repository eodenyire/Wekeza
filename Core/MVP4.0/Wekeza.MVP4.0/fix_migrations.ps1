# PowerShell script to fix migration history
$connectionString = "Host=localhost;Database=wekeza_mvp4;Username=postgres;Password=the_beast_pass"

# Read the SQL file
$sqlContent = Get-Content -Path "fix_migration_history.sql" -Raw

# Use dotnet to execute the SQL
Add-Type -AssemblyName "System.Data"
Add-Type -Path "C:\Users\Emmanuel Odenyire\.nuget\packages\npgsql\8.0.5\lib\net8.0\Npgsql.dll"

try {
    $connection = New-Object Npgsql.NpgsqlConnection($connectionString)
    $connection.Open()
    
    $command = New-Object Npgsql.NpgsqlCommand($sqlContent, $connection)
    $result = $command.ExecuteNonQuery()
    
    Write-Host "Migration history fixed. Rows affected: $result"
    
    $connection.Close()
} catch {
    Write-Host "Error: $($_.Exception.Message)"
}