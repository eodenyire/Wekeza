# Minimal Wekeza Core Banking System Startup
Write-Host "Starting Wekeza Core Banking System (Minimal Mode)" -ForegroundColor Green

# Check .NET
try {
    $dotnetVersion = dotnet --version
    Write-Host "OK .NET SDK: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "ERROR .NET 8 SDK not found" -ForegroundColor Red
    exit 1
}

# Start with the minimal API project that we know works
Write-Host "Starting Minimal Banking API..." -ForegroundColor Yellow

# Start the minimal API
dotnet run --project MinimalWekezaApi --configuration Release