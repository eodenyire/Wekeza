# WekezaERMS Repository Push Script (PowerShell)
# This script automates the process of pushing WekezaERMS to a separate repository

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("subtree", "manual", "filter-repo")]
    [string]$Method = "manual"
)

# Configuration
$SourceRepo = "https://github.com/eodenyire/Wekeza.git"
$TargetRepo = "https://github.com/eodenyire/WekezaERMS.git"
$ErmsFolder = "WekezaERMS"
$WorkDir = "C:\Temp\erms-migration-$(Get-Date -Format 'yyyyMMddHHmmss')"

# Color functions
function Write-Status {
    param([string]$Message)
    Write-Host "[✓] $Message" -ForegroundColor Green
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "[✗] $Message" -ForegroundColor Red
}

function Write-Warning-Custom {
    param([string]$Message)
    Write-Host "[!] $Message" -ForegroundColor Yellow
}

function Write-Info {
    param([string]$Message)
    Write-Host "[i] $Message" -ForegroundColor Cyan
}

Write-Host "===============================================" -ForegroundColor Blue
Write-Host "WekezaERMS Repository Migration Script" -ForegroundColor Blue
Write-Host "===============================================" -ForegroundColor Blue
Write-Host ""

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow
Write-Host ""

# Check if git is installed
try {
    $gitVersion = git --version
    Write-Status "Git is installed: $gitVersion"
} catch {
    Write-Error-Custom "Git is not installed. Please install git first."
    exit 1
}

# Check if gh CLI is available
try {
    $ghVersion = gh --version 2>$null
    Write-Status "GitHub CLI (gh) is available"
    $GhAvailable = $true
} catch {
    Write-Warning-Custom "GitHub CLI (gh) is not available. Repository creation will be manual."
    $GhAvailable = $false
}

# If method not specified, ask user
if (-not $PSBoundParameters.ContainsKey('Method')) {
    Write-Host ""
    Write-Host "Select migration method:" -ForegroundColor Yellow
    Write-Host "1) Manual Copy (quick and simple) - RECOMMENDED for Windows"
    Write-Host "2) Git Subtree (preserves history)"
    Write-Host ""
    $methodChoice = Read-Host "Enter choice [1-2]"
    
    switch ($methodChoice) {
        "1" { $Method = "manual"; Write-Info "Using Manual Copy method" }
        "2" { $Method = "subtree"; Write-Info "Using Git Subtree method" }
        default { Write-Error-Custom "Invalid choice"; exit 1 }
    }
}

# Create working directory
Write-Host ""
Write-Info "Creating working directory: $WorkDir"
New-Item -ItemType Directory -Path $WorkDir -Force | Out-Null
Set-Location $WorkDir

# Method 1: Git Subtree
if ($Method -eq "subtree") {
    Write-Info "Cloning source repository..."
    git clone $SourceRepo wekeza-source
    Set-Location wekeza-source
    
    Write-Info "Extracting WekezaERMS with history using git subtree..."
    git subtree split --prefix=$ErmsFolder -b erms-only
    
    Write-Info "Creating standalone repository..."
    Set-Location ..
    New-Item -ItemType Directory -Path "erms-standalone" -Force | Out-Null
    Set-Location erms-standalone
    git init
    git pull ..\wekeza-source erms-only
    
    Write-Info "Adding remote for target repository..."
    git remote add origin $TargetRepo
    
    Write-Status "Repository prepared successfully!"
    Write-Host ""
    Write-Warning-Custom "To push to GitHub, run:"
    Write-Host "cd $WorkDir\erms-standalone"
    Write-Host "git push -u origin main"
}

# Method 2: Manual Copy
if ($Method -eq "manual") {
    Write-Info "Cloning source repository..."
    git clone $SourceRepo wekeza-source
    
    Write-Info "Initializing target repository..."
    New-Item -ItemType Directory -Path "erms-standalone" -Force | Out-Null
    Set-Location erms-standalone
    git init
    
    Write-Info "Copying WekezaERMS files..."
    $sourcePath = Join-Path $WorkDir "wekeza-source\$ErmsFolder\*"
    Copy-Item -Path $sourcePath -Destination . -Recurse -Force
    
    Write-Info "Adding files to git..."
    git add .
    git commit -m "Initial commit: WekezaERMS from Wekeza repository"
    
    Write-Info "Adding remote for target repository..."
    git remote add origin $TargetRepo
    
    Write-Status "Repository prepared successfully!"
    Write-Host ""
    Write-Warning-Custom "To push to GitHub, run:"
    Write-Host "cd $WorkDir\erms-standalone"
    Write-Host "git push -u origin main"
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Blue
Write-Host "Migration preparation complete!" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Blue
Write-Host ""
Write-Host "Working directory: $WorkDir\erms-standalone"
Write-Host ""
Write-Host "Next steps:"
Write-Host "1. Review the extracted files in $WorkDir\erms-standalone"
Write-Host "2. Ensure the target repository exists at $TargetRepo"
Write-Host "3. Run: cd $WorkDir\erms-standalone; git push -u origin main"
Write-Host ""
Write-Host "For more details, see: WekezaERMS\REPOSITORY-SETUP-GUIDE.md"
