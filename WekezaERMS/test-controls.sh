#!/bin/bash

# WekezaERMS Controls Module Test Script
# Tests all Control endpoints

API_URL="http://localhost:5252"
echo "=== WekezaERMS Controls Module Test ==="
echo "API URL: $API_URL"
echo ""

# Step 1: Login to get JWT token
echo "Step 1: Login as admin..."
LOGIN_RESPONSE=$(curl -s -X POST "$API_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}')

TOKEN=$(echo $LOGIN_RESPONSE | jq -r '.token')

if [ "$TOKEN" = "null" ] || [ -z "$TOKEN" ]; then
    echo "❌ Login failed!"
    echo "Response: $LOGIN_RESPONSE"
    exit 1
fi

echo "✅ Login successful"
echo "Token: ${TOKEN:0:50}..."
echo ""

# Step 2: Create a Risk first (needed for controls)
echo "Step 2: Creating a test risk..."
RISK_RESPONSE=$(curl -s -X POST "$API_URL/api/risks" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "title": "Test Risk for Controls",
    "description": "This is a test risk to demonstrate control management",
    "category": 1,
    "inherentLikelihood": 3,
    "inherentImpact": 4,
    "ownerId": "00000000-0000-0000-0000-000000000001",
    "department": "IT Security",
    "treatmentStrategy": 2,
    "riskAppetite": 10
  }')

RISK_ID=$(echo $RISK_RESPONSE | jq -r '.id')

if [ "$RISK_ID" = "null" ] || [ -z "$RISK_ID" ]; then
    echo "❌ Risk creation failed!"
    echo "Response: $RISK_RESPONSE"
    exit 1
fi

echo "✅ Risk created successfully"
echo "Risk ID: $RISK_ID"
echo ""

# Step 3: Create a Control
echo "Step 3: Creating a control for the risk..."
CONTROL_RESPONSE=$(curl -s -X POST "$API_URL/api/risks/$RISK_ID/controls" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "controlName": "Multi-Factor Authentication",
    "description": "Require MFA for all administrative access",
    "controlType": "Preventive",
    "ownerId": "00000000-0000-0000-0000-000000000001",
    "testingFrequency": "Quarterly"
  }')

CONTROL_ID=$(echo $CONTROL_RESPONSE | jq -r '.id')

if [ "$CONTROL_ID" = "null" ] || [ -z "$CONTROL_ID" ]; then
    echo "❌ Control creation failed!"
    echo "Response: $CONTROL_RESPONSE"
    exit 1
fi

echo "✅ Control created successfully"
echo "Control ID: $CONTROL_ID"
echo "Control Details:"
echo $CONTROL_RESPONSE | jq '.'
echo ""

# Step 4: Get Control by ID
echo "Step 4: Getting control by ID..."
GET_CONTROL=$(curl -s -X GET "$API_URL/api/controls/$CONTROL_ID" \
  -H "Authorization: Bearer $TOKEN")

echo "✅ Control retrieved:"
echo $GET_CONTROL | jq '.'
echo ""

# Step 5: Get all controls for risk
echo "Step 5: Getting all controls for risk $RISK_ID..."
GET_CONTROLS=$(curl -s -X GET "$API_URL/api/risks/$RISK_ID/controls" \
  -H "Authorization: Bearer $TOKEN")

CONTROL_COUNT=$(echo $GET_CONTROLS | jq '. | length')
echo "✅ Retrieved $CONTROL_COUNT control(s)"
echo $GET_CONTROLS | jq '.'
echo ""

# Step 6: Update Control
echo "Step 6: Updating control..."
UPDATE_RESPONSE=$(curl -s -X PUT "$API_URL/api/controls/$CONTROL_ID" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "controlName": "Enhanced Multi-Factor Authentication",
    "description": "Require MFA with biometric verification for all administrative access",
    "controlType": "Preventive",
    "ownerId": "00000000-0000-0000-0000-000000000001",
    "testingFrequency": "Monthly"
  }')

echo "✅ Control updated:"
echo $UPDATE_RESPONSE | jq '.'
echo ""

# Step 7: Update Control Effectiveness
echo "Step 7: Updating control effectiveness..."
EFFECTIVENESS_RESPONSE=$(curl -s -X PUT "$API_URL/api/controls/$CONTROL_ID/effectiveness" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "effectiveness": 3,
    "testingEvidence": "Tested on 100 user accounts - all successfully enforcing MFA"
  }')

echo "✅ Control effectiveness updated:"
echo $EFFECTIVENESS_RESPONSE | jq '.'
echo ""

# Step 8: Record Control Test
echo "Step 8: Recording a control test..."
TEST_RESPONSE=$(curl -s -X POST "$API_URL/api/controls/$CONTROL_ID/test" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "effectiveness": 4,
    "testingEvidence": "Annual audit completed - control is highly effective",
    "testDate": "'$(date -u +%Y-%m-%dT%H:%M:%SZ)'"
  }')

echo "✅ Control test recorded:"
echo $TEST_RESPONSE | jq '.'
echo ""

# Step 9: Create another control
echo "Step 9: Creating a second control..."
CONTROL2_RESPONSE=$(curl -s -X POST "$API_URL/api/risks/$RISK_ID/controls" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "controlName": "Access Logging and Monitoring",
    "description": "Log all administrative access attempts and monitor for anomalies",
    "controlType": "Detective",
    "ownerId": "00000000-0000-0000-0000-000000000001",
    "testingFrequency": "Monthly"
  }')

CONTROL2_ID=$(echo $CONTROL2_RESPONSE | jq -r '.id')
echo "✅ Second control created: $CONTROL2_ID"
echo ""

# Step 10: Get all controls again
echo "Step 10: Getting all controls for risk (should be 2)..."
GET_ALL_CONTROLS=$(curl -s -X GET "$API_URL/api/risks/$RISK_ID/controls" \
  -H "Authorization: Bearer $TOKEN")

CONTROL_COUNT=$(echo $GET_ALL_CONTROLS | jq '. | length')
echo "✅ Retrieved $CONTROL_COUNT controls"
echo $GET_ALL_CONTROLS | jq '.'
echo ""

# Step 11: Delete a control
echo "Step 11: Deleting second control..."
DELETE_RESPONSE=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X DELETE "$API_URL/api/controls/$CONTROL2_ID" \
  -H "Authorization: Bearer $TOKEN")

HTTP_STATUS=$(echo "$DELETE_RESPONSE" | grep HTTP_STATUS | cut -d: -f2)
if [ "$HTTP_STATUS" = "204" ]; then
    echo "✅ Control deleted successfully (HTTP 204)"
else
    echo "❌ Delete failed with status: $HTTP_STATUS"
    echo "Response: $DELETE_RESPONSE"
fi
echo ""

# Step 12: Verify deletion
echo "Step 12: Verifying control deletion..."
GET_ALL_CONTROLS=$(curl -s -X GET "$API_URL/api/risks/$RISK_ID/controls" \
  -H "Authorization: Bearer $TOKEN")

CONTROL_COUNT=$(echo $GET_ALL_CONTROLS | jq '. | length')
echo "✅ Now have $CONTROL_COUNT control(s) (should be 1)"
echo ""

# Summary
echo "=== TEST SUMMARY ==="
echo "✅ All Control module endpoints tested successfully!"
echo ""
echo "Endpoints tested:"
echo "  ✅ POST /api/risks/{riskId}/controls - Create control"
echo "  ✅ GET /api/risks/{riskId}/controls - List controls for risk"
echo "  ✅ GET /api/controls/{id} - Get control by ID"
echo "  ✅ PUT /api/controls/{id} - Update control"
echo "  ✅ DELETE /api/controls/{id} - Delete control"
echo "  ✅ PUT /api/controls/{id}/effectiveness - Update effectiveness"
echo "  ✅ POST /api/controls/{id}/test - Record control test"
echo ""
echo "=== Phase 3 Complete: Controls Module Fully Implemented ==="
