# MVP5.0 - Wekeza Core Banking System Startup Script (PowerShell)
# This script starts all banking services for full end-to-end operational testing

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Wekeza Core Banking System MVP5.0" -ForegroundColor Cyan
Write-Host "  Full End-to-End Banking Platform" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Function to print colored messages
function Write-InfoMessage {
    param([string]$message)
    Write-Host "[INFO] $message" -ForegroundColor Blue
}

function Write-SuccessMessage {
    param([string]$message)
    Write-Host "[SUCCESS] $message" -ForegroundColor Green
}

function Write-WarningMessage {
    param([string]$message)
    Write-Host "[WARNING] $message" -ForegroundColor Yellow
}

function Write-ErrorMessage {
    param([string]$message)
    Write-Host "[ERROR] $message" -ForegroundColor Red
}

# Check if Docker is installed
if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-ErrorMessage "Docker is not installed. Please install Docker Desktop first."
    exit 1
}

# Check if Docker Compose is available
$dockerComposeCmd = $null
if (Get-Command docker-compose -ErrorAction SilentlyContinue) {
    $dockerComposeCmd = "docker-compose"
} elseif ((docker compose version 2>&1) -match "Docker Compose") {
    $dockerComposeCmd = "docker compose"
} else {
    Write-ErrorMessage "Docker Compose is not available. Please ensure Docker Desktop is properly installed."
    exit 1
}

Write-InfoMessage "Starting Wekeza Banking System MVP5.0..."
Write-Host ""

# Stop any existing containers
Write-InfoMessage "Stopping any existing MVP5.0 containers..."
& $dockerComposeCmd -f docker-compose.mvp5.yml down 2>$null

# Build all services
Write-InfoMessage "Building all banking services..."
& $dockerComposeCmd -f docker-compose.mvp5.yml build --no-cache

if ($LASTEXITCODE -ne 0) {
    Write-ErrorMessage "Build failed. Please check the error messages above."
    exit 1
}

# Start all services
Write-InfoMessage "Starting all services..."
& $dockerComposeCmd -f docker-compose.mvp5.yml up -d

if ($LASTEXITCODE -ne 0) {
    Write-ErrorMessage "Failed to start services. Please check the error messages above."
    exit 1
}

# Wait for services to be healthy
Write-InfoMessage "Waiting for services to become healthy..."
Start-Sleep -Seconds 10

# Check service status
Write-InfoMessage "Checking service status..."
Write-Host ""

& $dockerComposeCmd -f docker-compose.mvp5.yml ps

Write-Host ""
Write-SuccessMessage "MVP5.0 Banking System is now running!"
Write-Host ""

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Available Services:" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Database:" -ForegroundColor Green
Write-Host "  PostgreSQL      : localhost:5432"
Write-Host "  Database Name   : WekezaCoreDB"
Write-Host "  Username        : wekeza_app"
Write-Host ""
Write-Host "Banking APIs:" -ForegroundColor Green
Write-Host "  Minimal API     : http://localhost:8081"
Write-Host "  Database API    : http://localhost:8082/swagger"
Write-Host "  Enhanced API    : http://localhost:8083/swagger"
Write-Host "  Comprehensive   : http://localhost:8084/swagger"
Write-Host ""
Write-Host "Management Tools:" -ForegroundColor Green
Write-Host "  pgAdmin         : http://localhost:5050 (optional, use --profile tools)"
Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Quick Start Commands:" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "View logs:"
Write-Host "  docker-compose -f docker-compose.mvp5.yml logs -f"
Write-Host ""
Write-Host "Stop all services:"
Write-Host "  docker-compose -f docker-compose.mvp5.yml down"
Write-Host ""
Write-Host "Restart a service:"
Write-Host "  docker-compose -f docker-compose.mvp5.yml restart [service-name]"
Write-Host ""
Write-Host "Execute command in container:"
Write-Host "  docker-compose -f docker-compose.mvp5.yml exec [service-name] bash"
Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-SuccessMessage "System is operational! Open your browser and navigate to the APIs above."
Write-Host ""

# Optional: Open browser automatically
Write-Host "Opening Comprehensive API in browser..." -ForegroundColor Yellow
Start-Sleep -Seconds 3
Start-Process "http://localhost:8084/swagger"
