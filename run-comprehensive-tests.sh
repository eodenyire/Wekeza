#!/bin/bash

# Comprehensive Integration Test Suite Execution Script
# Tests all 4 operational APIs in the Wekeza Core Banking System

set -e

echo "=========================================================================="
echo "  WEKEZA CORE BANKING SYSTEM - COMPREHENSIVE INTEGRATION TEST SUITE"
echo "=========================================================================="
echo ""
echo "Testing 4 Operational APIs:"
echo "  1. MinimalWekezaApi (Port 8081)"
echo "  2. DatabaseWekezaApi (Port 8082)"
echo "  3. EnhancedWekezaApi (Port 8083)"
echo "  4. ComprehensiveWekezaApi (Port 8084)"
echo ""
echo "=========================================================================="
echo ""

# Navigate to test directory
cd "$(dirname "$0")/Tests/Wekeza.AllApis.IntegrationTests"

echo "[1/4] Restoring test project dependencies..."
dotnet restore --verbosity quiet

echo "[2/4] Building test project..."
dotnet build --configuration Release --no-restore --verbosity quiet

echo "[3/4] Running comprehensive integration tests..."
echo ""

# Run tests with detailed output
dotnet test --configuration Release --no-build --logger "console;verbosity=detailed"

# Capture exit code
TEST_EXIT_CODE=$?

echo ""
echo "=========================================================================="
echo "  TEST EXECUTION COMPLETE"
echo "=========================================================================="
echo ""

if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo "✅ ALL TESTS PASSED"
    echo ""
    echo "Test Coverage:"
    echo "  ✅ Database Connectivity Tests"
    echo "  ✅ CRUD Operations Tests"
    echo "  ✅ Complete Banking Workflow Tests"
    echo "  ✅ Concurrent Operations Tests"
    echo "  ✅ Entity Accessibility Tests"
    echo "  ✅ Performance Benchmark Tests"
    echo "  ✅ API Build Status Tests"
    echo ""
    echo "Core Banking System Status: FULLY OPERATIONAL"
    echo "All 4 APIs: VERIFIED WORKING"
else
    echo "❌ SOME TESTS FAILED"
    echo "Please review the test output above for details."
    exit 1
fi

echo ""
echo "=========================================================================="
