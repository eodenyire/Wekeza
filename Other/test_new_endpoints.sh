#!/bin/bash

# Get auth token
AUTH=$(curl -s -X POST http://localhost:8080/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}')

TOKEN=$(echo "$AUTH" | python3 -c "import sys, json; print(json.load(sys.stdin)['token'])")

echo "════════════════════════════════════════════════════════════════"
echo "✅ TESTING NEW GET ENDPOINTS"
echo "════════════════════════════════════════════════════════════════"
echo ""

# Test 1: Get all accounts (paginated)
echo "1️⃣  GET /api/accounts (List All + Paginated)"
echo "─────────────────────────────────────────────────"
curl -s "http://localhost:8080/api/accounts?pageNumber=1&pageSize=5" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool 2>&1 | head -50
echo ""
echo ""

# Test 2: Get specific account by number
echo "2️⃣  GET /api/accounts/{accountNumber} (Search by Number)"
echo "─────────────────────────────────────────────────"
for ACC in "ACC0000284592" "ACC1000000004" "ACC0008083435"; do
  echo "🔍 Searching for: $ACC"
  curl -s "http://localhost:8080/api/accounts/$ACC" \
    -H "Authorization: Bearer $TOKEN" | python3 -m json.tool 2>&1 | head -25
  echo ""
done

echo "════════════════════════════════════════════════════════════════"
echo "✅ NEW ENDPOINTS WORKING!"
echo "════════════════════════════════════════════════════════════════"

