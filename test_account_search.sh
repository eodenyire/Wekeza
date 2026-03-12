#!/bin/bash

# Get token
AUTH=$(curl -s -X POST http://localhost:8080/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}')

TOKEN=$(echo "$AUTH" | python3 -c "import sys, json; print(json.load(sys.stdin)['token'])")

echo "=== TESTING ACCOUNT SEARCH API ==="
echo ""

# Test search for specific accounts
for ACCOUNT in "ACC0000284592" "ACC1000000004" "ACC0008083435"; do
  echo "🔍 Searching for: $ACCOUNT"
  curl -s "http://localhost:8080/api/accounts/$ACCOUNT" \
    -H "Authorization: Bearer $TOKEN" | python3 -m json.tool 2>&1 | head -20
  echo ""
done

echo "=== TESTING ACCOUNT LIST API ==="
echo ""
curl -s "http://localhost:8080/api/accounts?pageNumber=1&pageSize=5" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool 2>&1 | head -30

