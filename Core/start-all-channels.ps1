#!/usr/bin/env pwsh
# 🚀 Start All Wekeza Banking Channels

Write-Host "🏦 Starting Wekeza Banking System..." -ForegroundColor Cyan
Write-Host ""

# Check if API is running
Write-Host "🔍 Checking if Wekeza Core API is running..." -ForegroundColor Yellow
try {
    $apiResponse = Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✅ API is running on port 5000" -ForegroundColor Green
    Write-Host "   Service: $($apiResponse.service)" -ForegroundColor White
    Write-Host "   Version: $($apiResponse.version)" -ForegroundColor White
} catch {
    Write-Host "❌ API is not running on port 5000" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please start the API first:" -ForegroundColor Yellow
    Write-Host "  cd Core" -ForegroundColor White
    Write-Host "  dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj" -ForegroundColor White
    Write-Host ""
    exit 1
}

Write-Host ""
Write-Host "🌐 Starting Web Channels..." -ForegroundColor Cyan
Write-Host ""

# Function to start a channel in a new window
function Start-Channel {
    param(
        [string]$Name,
        [string]$Path,
        [int]$Port
    )
    
    Write-Host "Starting $Name on port $Port..." -ForegroundColor Yellow
    
    # Check if node_modules exists
    $nodeModulesPath = Join-Path $Path "node_modules"
    if (-not (Test-Path $nodeModulesPath)) {
        Write-Host "  📦 Installing dependencies for $Name..." -ForegroundColor Cyan
        Push-Location $Path
        npm install
        Pop-Location
    }
    
    # Start in new PowerShell window
    $command = "cd '$Path'; npm run dev; Read-Host 'Press Enter to close'"
    Start-Process pwsh -ArgumentList "-NoExit", "-Command", $command
    
    Write-Host "✅ $Name started" -ForegroundColor Green
    Start-Sleep -Seconds 2
}

# Start each channel
$channelsPath = "Wekeza.Web.Channels"

if (Test-Path "$channelsPath/public-website/package.json") {
    Start-Channel -Name "Public Website" -Path "$channelsPath/public-website" -Port 3000
}

if (Test-Path "$channelsPath/personal-banking/package.json") {
    Start-Channel -Name "Personal Banking" -Path "$channelsPath/personal-banking" -Port 3001
}

if (Test-Path "$channelsPath/corporate-banking/package.json") {
    Start-Channel -Name "Corporate Banking" -Path "$channelsPath/corporate-banking" -Port 3002
}

if (Test-Path "$channelsPath/sme-banking/package.json") {
    Start-Channel -Name "SME Banking" -Path "$channelsPath/sme-banking" -Port 3003
}

Write-Host ""
Write-Host "🎉 All channels are starting!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Access URLs:" -ForegroundColor Cyan
Write-Host "  • API:              http://localhost:5000" -ForegroundColor White
Write-Host "  • Swagger:          http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  • Public Website:   http://localhost:3000" -ForegroundColor White
Write-Host "  • Personal Banking: http://localhost:3001" -ForegroundColor White
Write-Host "  • Corporate Banking: http://localhost:3002" -ForegroundColor White
Write-Host "  • SME Banking:      http://localhost:3003" -ForegroundColor White
Write-Host ""
Write-Host "💡 Tip: Each channel is running in a separate window" -ForegroundColor Yellow
Write-Host "    Close the windows to stop the channels" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press any key to exit this window..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
