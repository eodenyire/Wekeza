# WekezaERMS Controls API - Quick Reference

## Base URL
```
http://localhost:5252
```

## Authentication
All endpoints require JWT authentication:
```
Authorization: Bearer {your_jwt_token}
```

### Get Token
```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}

Response:
{
  "token": "eyJhbGc...",
  "user": { ... }
}
```

---

## Controls API Endpoints

### 1. Create Control
Create a new control for a risk.

**Endpoint:** `POST /api/risks/{riskId}/controls`  
**Auth:** RiskOfficer, RiskManager, Administrator  
**Request:**
```json
{
  "controlName": "Multi-Factor Authentication",
  "description": "Require MFA for all administrative access",
  "controlType": "Preventive",
  "ownerId": "00000000-0000-0000-0000-000000000001",
  "testingFrequency": "Quarterly"
}
```
**Response:** `201 Created`
```json
{
  "id": "afd886de-a87b-4888-bf3b-f076ff1e4419",
  "riskId": "273e5f4e-c289-46c1-b04d-d054a20e741c",
  "controlName": "Multi-Factor Authentication",
  "description": "Require MFA for all administrative access",
  "controlType": "Preventive",
  "effectiveness": null,
  "lastTestedDate": null,
  "nextTestDate": null,
  "ownerId": "00000000-0000-0000-0000-000000000001",
  "testingFrequency": "Quarterly",
  "testingEvidence": "",
  "createdAt": "2026-01-28T10:27:35.1167261Z"
}
```

---

### 2. List Controls for Risk
Get all controls associated with a risk.

**Endpoint:** `GET /api/risks/{riskId}/controls`  
**Auth:** RiskViewer, RiskOfficer, RiskManager, Auditor, Executive, Administrator  
**Response:** `200 OK`
```json
[
  {
    "id": "afd886de-a87b-4888-bf3b-f076ff1e4419",
    "riskId": "273e5f4e-c289-46c1-b04d-d054a20e741c",
    "controlName": "Multi-Factor Authentication",
    ...
  },
  {
    "id": "8631abee-2f4c-4f2c-93ac-7653f2776247",
    "riskId": "273e5f4e-c289-46c1-b04d-d054a20e741c",
    "controlName": "Access Logging",
    ...
  }
]
```

---

### 3. Get Control by ID
Retrieve a specific control.

**Endpoint:** `GET /api/controls/{id}`  
**Auth:** RiskViewer, RiskOfficer, RiskManager, Auditor, Executive, Administrator  
**Response:** `200 OK` or `404 Not Found`
```json
{
  "id": "afd886de-a87b-4888-bf3b-f076ff1e4419",
  "riskId": "273e5f4e-c289-46c1-b04d-d054a20e741c",
  "controlName": "Multi-Factor Authentication",
  "description": "Require MFA for all administrative access",
  "controlType": "Preventive",
  "effectiveness": 3,
  "lastTestedDate": "2026-01-28T10:27:35.305706Z",
  "nextTestDate": "2026-04-28T10:27:35.3059219Z",
  "ownerId": "00000000-0000-0000-0000-000000000001",
  "testingFrequency": "Quarterly",
  "testingEvidence": "All 100 accounts tested - MFA enforced",
  "createdAt": "2026-01-28T10:27:35.1167261Z"
}
```

---

### 4. Update Control
Update an existing control.

**Endpoint:** `PUT /api/controls/{id}`  
**Auth:** RiskOfficer, RiskManager, Administrator  
**Request:**
```json
{
  "controlName": "Enhanced Multi-Factor Authentication",
  "description": "Require MFA with biometric verification",
  "controlType": "Preventive",
  "ownerId": "00000000-0000-0000-0000-000000000001",
  "testingFrequency": "Monthly"
}
```
**Response:** `200 OK` or `404 Not Found`

---

### 5. Delete Control
Delete a control.

**Endpoint:** `DELETE /api/controls/{id}`  
**Auth:** RiskManager, Administrator  
**Response:** `204 No Content` or `404 Not Found`

---

### 6. Update Control Effectiveness
Update the effectiveness rating of a control.

**Endpoint:** `PUT /api/controls/{id}/effectiveness`  
**Auth:** RiskOfficer, RiskManager, Administrator  
**Request:**
```json
{
  "effectiveness": 3,
  "testingEvidence": "Tested on 100 user accounts - all successfully enforcing MFA"
}
```
**Response:** `200 OK` or `404 Not Found`

**Effectiveness Levels:**
- `1` = Ineffective
- `2` = PartiallyEffective
- `3` = Effective
- `4` = HighlyEffective

---

### 7. Record Control Test
Record the results of a control test.

**Endpoint:** `POST /api/controls/{id}/test`  
**Auth:** RiskOfficer, RiskManager, Administrator  
**Request:**
```json
{
  "effectiveness": 4,
  "testingEvidence": "Annual audit completed - control is highly effective",
  "testDate": "2026-01-28T10:00:00Z"
}
```
**Response:** `200 OK` or `404 Not Found`

**Note:** This automatically updates `lastTestedDate` and calculates `nextTestDate` based on `testingFrequency`.

---

## Reference Data

### Control Types
- `Preventive` - Prevents risk from occurring
- `Detective` - Detects when risk occurs
- `Corrective` - Corrects issues after occurrence

### Testing Frequency
- `Monthly` - Test every month
- `Quarterly` - Test every 3 months
- `Annually` - Test every year

### Effectiveness Levels
- `1` - Ineffective (not working)
- `2` - PartiallyEffective (working with gaps)
- `3` - Effective (working as intended)
- `4` - HighlyEffective (exceeds expectations)

---

## cURL Examples

### Complete Workflow Example

```bash
# 1. Login
TOKEN=$(curl -s -X POST http://localhost:5252/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}' | jq -r '.token')

# 2. Create a Risk (if needed)
RISK_ID=$(curl -s -X POST http://localhost:5252/api/risks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "title": "Unauthorized Access Risk",
    "description": "Risk of unauthorized access to systems",
    "category": 1,
    "inherentLikelihood": 3,
    "inherentImpact": 4,
    "ownerId": "00000000-0000-0000-0000-000000000001",
    "department": "IT Security",
    "treatmentStrategy": 2,
    "riskAppetite": 10
  }' | jq -r '.id')

# 3. Create Control
CONTROL_ID=$(curl -s -X POST "http://localhost:5252/api/risks/$RISK_ID/controls" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "controlName": "Multi-Factor Authentication",
    "description": "Require MFA for all administrative access",
    "controlType": "Preventive",
    "ownerId": "00000000-0000-0000-0000-000000000001",
    "testingFrequency": "Quarterly"
  }' | jq -r '.id')

# 4. Get Control
curl -s -X GET "http://localhost:5252/api/controls/$CONTROL_ID" \
  -H "Authorization: Bearer $TOKEN" | jq '.'

# 5. Update Control Effectiveness
curl -s -X PUT "http://localhost:5252/api/controls/$CONTROL_ID/effectiveness" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "effectiveness": 3,
    "testingEvidence": "Tested successfully on 100 accounts"
  }' | jq '.'

# 6. List All Controls for Risk
curl -s -X GET "http://localhost:5252/api/risks/$RISK_ID/controls" \
  -H "Authorization: Bearer $TOKEN" | jq '.'

# 7. Record Test Result
curl -s -X POST "http://localhost:5252/api/controls/$CONTROL_ID/test" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "effectiveness": 4,
    "testingEvidence": "Q1 audit completed - highly effective",
    "testDate": "'$(date -u +%Y-%m-%dT%H:%M:%SZ)'"
  }' | jq '.'
```

---

## Error Responses

### 400 Bad Request
```json
{
  "errors": [
    {
      "propertyName": "ControlName",
      "errorMessage": "Control name is required"
    }
  ]
}
```

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 403 Forbidden
```json
{
  "message": "Forbidden - Insufficient permissions"
}
```

### 404 Not Found
```json
{
  "message": "Control with ID {id} not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred while processing your request"
}
```

---

## Validation Rules

### Create/Update Control
- `controlName`: Required, max 200 characters
- `description`: Required, max 2000 characters
- `controlType`: Must be "Preventive", "Detective", or "Corrective"
- `testingFrequency`: Must be "Monthly", "Quarterly", or "Annually"
- `ownerId`: Required, valid GUID

### Update Effectiveness
- `effectiveness`: Required, must be 1-4
- `testingEvidence`: Required, max 2000 characters

### Record Test
- `effectiveness`: Required, must be 1-4
- `testingEvidence`: Required, max 2000 characters
- `testDate`: Cannot be in the future

---

## Rate Limiting
No rate limiting currently implemented. Consider implementing for production use.

## Versioning
Current version: v1  
API versioning not yet implemented.

## Support
For issues or questions, contact: eodenyire@github.com

---

**Last Updated:** January 28, 2026  
**Version:** 1.0
