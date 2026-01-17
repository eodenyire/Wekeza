# Wekeza Bank - Master Control Script
# One script to rule them all!

param(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateSet('setup', 'start', 'stop', 'migrate', 'test', 'docker-up', 'docker-down', 'logs', 'clean', 'help')]
    [string]$Command,
    
    [Parameter(Mandatory=$false)]
    [string]$MigrationName = "Migration_$(Get-Date -Format 'yyyyMMddHHmmss')"
)

$ErrorActionPreference = "Stop"

function Write-Banner {
    Write-Host ""
    Write-Host "╔════════════════════════════════════════╗" -ForegroundColor Cyan
    Write-Host "║        WEKEZA BANK - CONTROL CLI       ║" -ForegroundColor Cyan
    Write-Host "╚════════════════════════════════════════╝" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Info {
    param([string]$Message)
    Write-Host "→ $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

Write-Banner

switch ($Command) {
    'setup' {
        Write-Info "Setting up Wekeza Bank for local development..."
        Write-Host ""
        
        # Check prerequisites
        Write-Info "Checking prerequisites..."
        
        $dotnetVersion = dotnet --version 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Success ".NET SDK found: $dotnetVersion"
        } else {
            Write-Error ".NET 8 SDK not found. Install from: https://dotnet.microsoft.com/download/dotnet/8.0"
            exit 1
        }
        
        $pgVersion = psql --version 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Success "PostgreSQL found: $pgVersion"
        } else {
            Write-Error "PostgreSQL not found. Install from: https://www.postgresql.org/download/windows/"
            exit 1
        }
        
        Write-Host ""
        Write-Info "Running database setup..."
        & .\scripts\setup-local-db.ps1
        
        Write-Host ""
        Write-Info "Running migrations..."
        & .\scripts\run-migrations.ps1 -MigrationName "InitialCreate"
        
        Write-Host ""
        Write-Success "Setup complete! Run './wekeza.ps1 start' to launch the application."
    }
    
    'start' {
        Write-Info "Starting Wekeza Bank..."
        & .\scripts\start-local.ps1
    }
    
    'stop' {
        Write-Info "Stopping Wekeza Bank..."
        Get-Process -Name "Wekeza.Core.Api" -ErrorAction SilentlyContinue | Stop-Process -Force
        Write-Success "Application stopped"
    }
    
    'migrate' {
        Write-Info "Creating and applying migration: $MigrationName"
        & .\scripts\run-migrations.ps1 -MigrationName $MigrationName
    }
    
    'test' {
        Write-Info "Running tests..."
        dotnet test
        if ($LASTEXITCODE -eq 0) {
            Write-Success "All tests passed!"
        } else {
            Write-Error "Some tests failed"
            exit 1
        }
    }
    
    'docker-up' {
        Write-Info "Starting Docker containers..."
        
        if (-not (Test-Path ".env")) {
            Write-Info "Creating .env file from template..."
            Copy-Item ".env.example" ".env"
            Write-Success ".env file created. Please update with your values."
        }
        
        docker-compose up -d
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Containers started successfully"
            Write-Host ""
            Write-Info "Running migrations..."
            Start-Sleep -Seconds 5
            docker-compose exec api dotnet ef database update
            
            Write-Host ""
            Write-Success "Wekeza Bank is running!"
            Write-Info "API: http://localhost:8080/swagger"
            Write-Info "Health: http://localhost:8080/health"
        } else {
            Write-Error "Failed to start containers"
            exit 1
        }
    }
    
    'docker-down' {
        Write-Info "Stopping Docker containers..."
        docker-compose down
        Write-Success "Containers stopped"
    }
    
    'logs' {
        Write-Info "Showing application logs..."
        docker-compose logs -f api
    }
    
    'clean' {
        Write-Info "Cleaning build artifacts..."
        
        Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force
        
        Write-Success "Clean complete"
    }
    
    'help' {
        Write-Host "Wekeza Bank Control CLI" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Usage: .\wekeza.ps1 <command> [options]" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Commands:" -ForegroundColor Yellow
        Write-Host "  setup              - Complete local development setup" -ForegroundColor White
        Write-Host "  start              - Start the application locally" -ForegroundColor White
        Write-Host "  stop               - Stop the running application" -ForegroundColor White
        Write-Host "  migrate [name]     - Create and apply database migration" -ForegroundColor White
        Write-Host "  test               - Run all tests" -ForegroundColor White
        Write-Host "  docker-up          - Start Docker containers" -ForegroundColor White
        Write-Host "  docker-down        - Stop Docker containers" -ForegroundColor White
        Write-Host "  logs               - Show application logs (Docker)" -ForegroundColor White
        Write-Host "  clean              - Clean build artifacts" -ForegroundColor White
        Write-Host "  help               - Show this help message" -ForegroundColor White
        Write-Host ""
        Write-Host "Examples:" -ForegroundColor Yellow
        Write-Host "  .\wekeza.ps1 setup" -ForegroundColor Gray
        Write-Host "  .\wekeza.ps1 start" -ForegroundColor Gray
        Write-Host "  .\wekeza.ps1 migrate -MigrationName 'AddCardSupport'" -ForegroundColor Gray
        Write-Host "  .\wekeza.ps1 docker-up" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Documentation:" -ForegroundColor Yellow
        Write-Host "  Quick Start:  QUICKSTART.md" -ForegroundColor Gray
        Write-Host "  Local Setup:  SETUP-LOCAL.md" -ForegroundColor Gray
        Write-Host "  Docker Setup: SETUP-DOCKER.md" -ForegroundColor Gray
        Write-Host "  Deployment:   DEPLOYMENT-GUIDE.md" -ForegroundColor Gray
        Write-Host ""
    }
}
