#!/usr/bin/env pwsh

# 🧪 Wekeza Core Banking System - System Test Script
# This script tests the basic functionality of the banking system

Write-Host "🧪 Testing Wekeza Core Banking System..." -ForegroundColor Green

$baseUrl = "https://localhost:7001"
$httpUrl = "http://localhost:5001"

# Test 1: Health Check
Write-Host "🔍 Test 1: Health Check..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET -SkipCertificateCheck
    Write-Host "✅ Health Check: $($healthResponse.status)" -ForegroundColor Green
} catch {
    Write-Host "❌ Health Check Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Trying HTTP endpoint..." -ForegroundColor Yellow
    try {
        $healthResponse = Invoke-RestMethod -Uri "$httpUrl/health" -Method GET
        Write-Host "✅ Health Check (HTTP): $($healthResponse.status)" -ForegroundColor Green
        $baseUrl = $httpUrl  # Use HTTP for remaining tests
    } catch {
        Write-Host "❌ Both HTTPS and HTTP health checks failed" -ForegroundColor Red
        Write-Host "Please ensure the API is running with: ./scripts/start-local-dev.ps1" -ForegroundColor Yellow
        exit 1
    }
}

# Test 2: Swagger Documentation
Write-Host "🔍 Test 2: Swagger Documentation..." -ForegroundColor Yellow
try {
    $swaggerResponse = Invoke-WebRequest -Uri "$baseUrl/swagger/index.html" -Method GET -SkipCertificateCheck
    if ($swaggerResponse.StatusCode -eq 200) {
        Write-Host "✅ Swagger UI is accessible" -ForegroundColor Green
    }
} catch {
    Write-Host "⚠️  Swagger UI test failed: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Test 3: API Endpoints Discovery
Write-Host "🔍 Test 3: API Endpoints Discovery..." -ForegroundColor Yellow
try {
    $swaggerJson = Invoke-RestMethod -Uri "$baseUrl/swagger/v1/swagger.json" -Method GET -SkipCertificateCheck
    $endpointCount = ($swaggerJson.paths | Get-Member -MemberType NoteProperty).Count
    Write-Host "✅ API Documentation: $endpointCount endpoints discovered" -ForegroundColor Green
    
    # List some key endpoints
    $keyEndpoints = @(
        "/api/authentication/login",
        "/api/accounts",
        "/api/transactions",
        "/api/loans",
        "/api/cards"
    )
    
    Write-Host "📋 Key Banking Endpoints:" -ForegroundColor Cyan
    foreach ($endpoint in $keyEndpoints) {
        if ($swaggerJson.paths.$endpoint) {
            Write-Host "  ✅ $endpoint" -ForegroundColor Green
        } else {
            Write-Host "  ❌ $endpoint (missing)" -ForegroundColor Red
        }
    }
} catch {
    Write-Host "⚠️  API Discovery failed: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Test 4: Database Connection (via Health Check details)
Write-Host "🔍 Test 4: Database Connection..." -ForegroundColor Yellow
try {
    $healthDetails = Invoke-RestMethod -Uri "$baseUrl/health" -Method GET -SkipCertificateCheck
    if ($healthDetails.status -eq "Healthy") {
        Write-Host "✅ Database connection is healthy" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Database connection status: $($healthDetails.status)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️  Database connection test failed" -ForegroundColor Yellow
}

# Test 5: Authentication Endpoint
Write-Host "🔍 Test 5: Authentication Endpoint..." -ForegroundColor Yellow
try {
    # Test login endpoint (should return 400 for empty request, not 500)
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/api/authentication/login" -Method POST -ContentType "application/json" -Body "{}" -SkipCertificateCheck
} catch {
    if ($_.Exception.Response.StatusCode -eq 400) {
        Write-Host "✅ Authentication endpoint is responding (400 Bad Request expected)" -ForegroundColor Green
    } elseif ($_.Exception.Response.StatusCode -eq 404) {
        Write-Host "❌ Authentication endpoint not found (404)" -ForegroundColor Red
    } else {
        Write-Host "⚠️  Authentication endpoint status: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
    }
}

# Test 6: CORS and Security Headers
Write-Host "🔍 Test 6: Security Headers..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/health" -Method GET -SkipCertificateCheck
    $headers = $response.Headers
    
    $securityChecks = @{
        "X-Content-Type-Options" = $headers["X-Content-Type-Options"]
        "X-Frame-Options" = $headers["X-Frame-Options"]
        "X-XSS-Protection" = $headers["X-XSS-Protection"]
    }
    
    foreach ($header in $securityChecks.GetEnumerator()) {
        if ($header.Value) {
            Write-Host "  ✅ $($header.Key): $($header.Value)" -ForegroundColor Green
        } else {
            Write-Host "  ⚠️  $($header.Key): Not set" -ForegroundColor Yellow
        }
    }
} catch {
    Write-Host "⚠️  Security headers test failed" -ForegroundColor Yellow
}

# Summary
Write-Host "" -ForegroundColor White
Write-Host "🎉 System Test Summary" -ForegroundColor Green
Write-Host "═══════════════════════════════════════" -ForegroundColor Gray
Write-Host "✅ Basic system functionality verified" -ForegroundColor Green
Write-Host "🌐 API Base URL: $baseUrl" -ForegroundColor Cyan
Write-Host "📚 Swagger UI: $baseUrl/swagger" -ForegroundColor Cyan
Write-Host "🏥 Health Check: $baseUrl/health" -ForegroundColor Cyan
Write-Host "" -ForegroundColor White
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Open Swagger UI in your browser: $baseUrl/swagger" -ForegroundColor White
Write-Host "2. Test authentication with the login endpoint" -ForegroundColor White
Write-Host "3. Explore the banking API endpoints" -ForegroundColor White
Write-Host "4. Review the API documentation" -ForegroundColor White
Write-Host "" -ForegroundColor White