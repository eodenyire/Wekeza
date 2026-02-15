#!/usr/bin/env pwsh
# Verify Node.js Installation

Write-Host "🔍 Verifying Node.js Installation..." -ForegroundColor Cyan
Write-Host ""

# Check Node.js
Write-Host "Checking Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Node.js is installed: $nodeVersion" -ForegroundColor Green
    } else {
        throw "Node.js not found"
    }
} catch {
    Write-Host "❌ Node.js is not found in PATH" -ForegroundColor Red
    Write-Host ""
    Write-Host "Solutions:" -ForegroundColor Yellow
    Write-Host "1. Close ALL PowerShell windows and open a NEW one" -ForegroundColor White
    Write-Host "2. If that doesn't work, restart your computer" -ForegroundColor White
    Write-Host "3. Verify Node.js is installed in: C:\Program Files\nodejs\" -ForegroundColor White
    Write-Host ""
    Write-Host "To check if Node.js is installed:" -ForegroundColor Cyan
    Write-Host "  dir 'C:\Program Files\nodejs\'" -ForegroundColor White
    Write-Host ""
    exit 1
}

Write-Host ""

# Check npm
Write-Host "Checking npm..." -ForegroundColor Yellow
try {
    $npmVersion = npm --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ npm is installed: $npmVersion" -ForegroundColor Green
    } else {
        throw "npm not found"
    }
} catch {
    Write-Host "❌ npm is not found in PATH" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "✅ Node.js is ready!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Install Personal Banking:" -ForegroundColor White
Write-Host "   cd Wekeza.Web.Channels\personal-banking" -ForegroundColor Gray
Write-Host "   npm install" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Start Personal Banking:" -ForegroundColor White
Write-Host "   npm run dev" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Open browser:" -ForegroundColor White
Write-Host "   http://localhost:3001" -ForegroundColor Gray
Write-Host ""
Write-Host "Or use the automated script:" -ForegroundColor Yellow
Write-Host "   .\start-all-channels.ps1" -ForegroundColor Gray
Write-Host ""
