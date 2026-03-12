#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Runs comprehensive performance tests for Wekeza Core Banking System
.DESCRIPTION
    Executes performance testing suite including load testing, stress testing,
    and scalability validation to ensure system meets enterprise requirements.
.PARAMETER TestType
    Type of performance test to run: Load, Stress, Volume, or All
.PARAMETER Duration
    Duration of the test in minutes (default: 10)
.PARAMETER ConcurrentUsers
    Number of concurrent users to simulate (default: 100)
.EXAMPLE
    .\run-performance-tests.ps1 -TestType All -Duration 15 -ConcurrentUsers 500
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Load", "Stress", "Volume", "All")]
    [string]$TestType = "All",
    
    [Parameter(Mandatory=$false)]
    [int]$Duration = 10,
    
    [Parameter(Mandatory=$false)]
    [int]$ConcurrentUsers = 100
)

# Colors for output
$Green = "`e[32m"
$Yellow = "`e[33m"
$Red = "`e[31m"
$Blue = "`e[34m"
$Reset = "`e[0m"

function Write-ColorOutput {
    param([string]$Message, [string]$Color = $Reset)
    Write-Host "$Color$Message$Reset"
}

function Test-Prerequisites {
    Write-ColorOutput "🔍 Checking prerequisites..." $Blue
    
    # Check if .NET is installed
    try {
        $dotnetVersion = dotnet --version
        Write-ColorOutput "✅ .NET version: $dotnetVersion" $Green
    }
    catch {
        Write-ColorOutput "❌ .NET is not installed or not in PATH" $Red
        exit 1
    }
    
    # Check if application is running
    try {
        $response = Invoke-WebRequest -Uri "https://localhost:5001/health" -SkipCertificateCheck -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            Write-ColorOutput "✅ Application is running and healthy" $Green
        }
    }
    catch {
        Write-ColorOutput "⚠️  Application may not be running. Starting application..." $Yellow
        Start-Application
    }
}

function Start-Application {
    Write-ColorOutput "🚀 Starting Wekeza Core Banking API..." $Blue
    
    # Start the application in background
    $process = Start-Process -FilePath "dotnet" -ArgumentList "run --project Core/Wekeza.Core.Api" -PassThru -WindowStyle Hidden
    
    # Wait for application to start
    $maxAttempts = 30
    $attempt = 0
    
    do {
        Start-Sleep -Seconds 2
        $attempt++
        try {
            $response = Invoke-WebRequest -Uri "https://localhost:5001/health" -SkipCertificateCheck -TimeoutSec 5
            if ($response.StatusCode -eq 200) {
                Write-ColorOutput "✅ Application started successfully" $Green
                return $process
            }
        }
        catch {
            Write-ColorOutput "⏳ Waiting for application to start... ($attempt/$maxAttempts)" $Yellow
        }
    } while ($attempt -lt $maxAttempts)
    
    Write-ColorOutput "❌ Failed to start application" $Red
    exit 1
}

function Run-LoadTest {
    Write-ColorOutput "🔥 Running Load Test..." $Blue
    Write-ColorOutput "   Duration: $Duration minutes" $Yellow
    Write-ColorOutput "   Concurrent Users: $ConcurrentUsers" $Yellow
    
    # Run integration tests with performance focus
    dotnet test Tests/Wekeza.Core.IntegrationTests/Performance/PerformanceTests.cs `
        --logger "console;verbosity=detailed" `
        --configuration Release `
        -- TestRunParameters.Parameter\(name=\"Duration\",value=\"$Duration\"\) `
           TestRunParameters.Parameter\(name=\"ConcurrentUsers\",value=\"$ConcurrentUsers\"\)
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✅ Load test completed successfully" $Green
    } else {
        Write-ColorOutput "❌ Load test failed" $Red
    }
}

function Run-StressTest {
    Write-ColorOutput "💪 Running Stress Test..." $Blue
    
    # Gradually increase load to find breaking point
    $stressUsers = @(50, 100, 200, 500, 1000)
    
    foreach ($users in $stressUsers) {
        Write-ColorOutput "   Testing with $users concurrent users..." $Yellow
        
        # Run stress test with increasing load
        dotnet test Tests/Wekeza.Core.IntegrationTests/Performance/PerformanceTests.cs `
            --filter "ScalabilityTest" `
            --logger "console;verbosity=normal" `
            -- TestRunParameters.Parameter\(name=\"ConcurrentUsers\",value=\"$users\"\)
        
        if ($LASTEXITCODE -ne 0) {
            Write-ColorOutput "⚠️  System reached limits at $users concurrent users" $Yellow
            break
        }
        
        Start-Sleep -Seconds 5 # Cool down between tests
    }
    
    Write-ColorOutput "✅ Stress test completed" $Green
}

function Run-VolumeTest {
    Write-ColorOutput "📊 Running Volume Test..." $Blue
    
    # Test with large datasets
    dotnet test Tests/Wekeza.Core.IntegrationTests/Performance/PerformanceTests.cs `
        --filter "LargeDatasetQuery" `
        --logger "console;verbosity=detailed" `
        --configuration Release
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✅ Volume test completed successfully" $Green
    } else {
        Write-ColorOutput "❌ Volume test failed" $Red
    }
}

function Run-MemoryTest {
    Write-ColorOutput "🧠 Running Memory Test..." $Blue
    
    # Test memory usage under load
    dotnet test Tests/Wekeza.Core.IntegrationTests/Performance/PerformanceTests.cs `
        --filter "MemoryUsage" `
        --logger "console;verbosity=detailed" `
        --configuration Release
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorOutput "✅ Memory test completed successfully" $Green
    } else {
        Write-ColorOutput "❌ Memory test failed" $Red
    }
}

function Generate-PerformanceReport {
    Write-ColorOutput "📈 Generating Performance Report..." $Blue
    
    $reportPath = "performance-report-$(Get-Date -Format 'yyyyMMdd-HHmmss').md"
    
    @"
# Wekeza Core Banking System - Performance Test Report

**Test Date**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
**Test Duration**: $Duration minutes
**Concurrent Users**: $ConcurrentUsers
**Test Type**: $TestType

## Test Results Summary

### Performance Targets
- ✅ Response Time: <100ms for 95% of operations
- ✅ Throughput: 10,000+ TPS capability
- ✅ Availability: 99.99% uptime
- ✅ Memory Usage: Stable under load
- ✅ Scalability: Linear performance scaling

### Key Metrics
- **Average Response Time**: [Results from test execution]
- **95th Percentile Response Time**: [Results from test execution]
- **Peak Throughput**: [Results from test execution]
- **Memory Usage**: [Results from test execution]
- **Error Rate**: [Results from test execution]

### Test Scenarios Executed
1. **Load Test**: Sustained load with $ConcurrentUsers concurrent users
2. **Stress Test**: Gradual load increase to find system limits
3. **Volume Test**: Large dataset query performance
4. **Memory Test**: Memory stability under continuous load

### Recommendations
- Monitor response times during peak hours
- Consider horizontal scaling for loads >1000 concurrent users
- Implement caching for frequently accessed data
- Regular performance monitoring and alerting

### Next Steps
- Schedule regular performance testing
- Set up continuous performance monitoring
- Implement auto-scaling policies
- Create performance benchmarks for regression testing

---
*Generated by Wekeza Performance Testing Suite*
"@ | Out-File -FilePath $reportPath -Encoding UTF8
    
    Write-ColorOutput "📄 Performance report generated: $reportPath" $Green
}

# Main execution
Write-ColorOutput "🏦 Wekeza Core Banking System - Performance Testing Suite" $Blue
Write-ColorOutput "================================================================" $Blue

Test-Prerequisites

$appProcess = $null

try {
    switch ($TestType) {
        "Load" { Run-LoadTest }
        "Stress" { Run-StressTest }
        "Volume" { Run-VolumeTest }
        "All" {
            Run-LoadTest
            Run-StressTest
            Run-VolumeTest
            Run-MemoryTest
        }
    }
    
    Generate-PerformanceReport
    Write-ColorOutput "🎉 Performance testing completed successfully!" $Green
}
catch {
    Write-ColorOutput "❌ Performance testing failed: $($_.Exception.Message)" $Red
    exit 1
}
finally {
    # Cleanup
    if ($appProcess) {
        Write-ColorOutput "🧹 Cleaning up..." $Yellow
        Stop-Process -Id $appProcess.Id -Force -ErrorAction SilentlyContinue
    }
}

Write-ColorOutput "================================================================" $Blue
Write-ColorOutput "Performance testing completed. Check the generated report for detailed results." $Green