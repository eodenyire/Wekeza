#!/bin/bash

echo "=== AUTHENTICATION ==="
AUTH_RESPONSE=$(curl -s -X POST http://localhost:8080/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}')

TOKEN=$(echo "$AUTH_RESPONSE" | python3 -c "import sys, json; print(json.load(sys.stdin)['token'])") 
ADMIN_ID=$(echo "$AUTH_RESPONSE" | python3 -c "import sys, json; print(json.load(sys.stdin)['user']['id'])")

echo "✅ Admin authenticated: $ADMIN_ID"
echo ""

# Test various endpoints that the dashboard would call
echo "=== APIs THAT SERVE REAL DATA ==="
echo ""

# 1. Get admin dashboard
echo "1️⃣ ADMIN DASHBOARD ENDPOINT"
echo "GET /api/admin-portal/dashboard"
curl -s "http://localhost:8080/api/admin-portal/dashboard" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool 2>&1 | head -40

echo ""
echo "---"
echo ""

# 2. Get customer dashboard  
echo "2️⃣ CUSTOMER PORTAL DASHBOARD ENDPOINT"
echo "GET /api/customer-portal/dashboard"
curl -s "http://localhost:8080/api/customer-portal/dashboard" \
  -H "Authorization: Bearer $TOKEN" | python3 -m json.tool 2>&1 | head -40

echo ""
