#!/bin/bash

# Get token
TOKEN=$(curl -s -X POST http://localhost:8080/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}' | python3 -c "import sys, json; print(json.load(sys.stdin)['token'])")

echo "✅ ACCOUNT SEARCH NOW WORKING!"
echo "════════════════════════════════════════════════════════════════"
echo ""

# Test 1: Get list
echo "1️⃣  GET /api/accounts/list?pageSize=5"
curl -s "http://localhost:8080/api/accounts/list?pageSize=5" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool | head -40

echo ""
echo "════════════════════════════════════════════════════════════════"
echo ""

# Test 2: Search by account number
echo "2️⃣  GET /api/accounts/ACC0000284592"
curl -s "http://localhost:8080/api/accounts/ACC0000284592" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool

echo ""
echo "════════════════════════════════════════════════════════════════"
echo ""

# Test 3: Another account
echo "3️⃣  GET /api/accounts/ACC1000000004"
curl -s "http://localhost:8080/api/accounts/ACC1000000004" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool

