#!/bin/bash

# Get token
TOKEN=$(curl -s -X POST http://localhost:8080/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}' | python3 -c "import sys, json; print(json.load(sys.stdin)['token'])")

# Fetch customers with token
echo "=== CUSTOMERS DATA ==="
curl -s "http://localhost:8080/api/customers?pageNumber=1&pageSize=3" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool | head -80

echo ""
echo "=== ACCOUNT DATA ==="
curl -s "http://localhost:8080/api/accounts?pageNumber=1&pageSize=3" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool | head -80

echo ""
echo "=== BRANCH DATA ==="
curl -s "http://localhost:8080/api/branches?pageNumber=1&pageSize=3" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool | head -40
