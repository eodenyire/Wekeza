# WekezaERMS API Reference

## Base URL
```
https://api.wekeza.com/erms/v1
```

## Authentication
All API requests require authentication using JWT Bearer tokens from Wekeza Core authentication system.

```
Authorization: Bearer {your_jwt_token}
```

---

## Risk Management Endpoints

### Create Risk
Create a new risk entry in the risk register.

**Endpoint:** `POST /api/risks`

**Request Body:**
```json
{
  "riskCode": "RISK-2024-001",
  "title": "Credit Concentration Risk - Corporate Sector",
  "description": "High exposure to corporate lending sector may lead to significant losses",
  "category": "Credit",
  "inherentLikelihood": "Likely",
  "inherentImpact": "Major",
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "department": "Credit Risk Management",
  "treatmentStrategy": "Mitigate",
  "riskAppetite": 12
}
```

**Response:** `201 Created`
```json
{
  "id": "7b8e1234-5678-90ab-cdef-1234567890ab",
  "riskCode": "RISK-2024-001",
  "title": "Credit Concentration Risk - Corporate Sector",
  "status": "Identified",
  "inherentRiskScore": 16,
  "inherentRiskLevel": "VeryHigh",
  "createdAt": "2024-01-28T10:00:00Z"
}
```

---

### List All Risks
Retrieve all risks with optional filtering.

**Endpoint:** `GET /api/risks`

**Query Parameters:**
- `category` (optional): Filter by risk category
- `status` (optional): Filter by risk status
- `riskLevel` (optional): Filter by risk level
- `ownerId` (optional): Filter by risk owner
- `department` (optional): Filter by department
- `page` (default: 1): Page number
- `pageSize` (default: 20): Items per page

**Example:**
```
GET /api/risks?category=Credit&riskLevel=High&page=1&pageSize=20
```

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": "7b8e1234-5678-90ab-cdef-1234567890ab",
      "riskCode": "RISK-2024-001",
      "title": "Credit Concentration Risk - Corporate Sector",
      "category": "Credit",
      "status": "Active",
      "inherentRiskLevel": "VeryHigh",
      "residualRiskLevel": "High",
      "owner": "John Doe",
      "department": "Credit Risk Management"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalRecords": 95
  }
}
```

---

### Get Risk Details
Retrieve detailed information about a specific risk.

**Endpoint:** `GET /api/risks/{id}`

**Response:** `200 OK`
```json
{
  "id": "7b8e1234-5678-90ab-cdef-1234567890ab",
  "riskCode": "RISK-2024-001",
  "title": "Credit Concentration Risk - Corporate Sector",
  "description": "High exposure to corporate lending sector...",
  "category": "Credit",
  "status": "Active",
  "inherentLikelihood": "Likely",
  "inherentImpact": "Major",
  "inherentRiskScore": 16,
  "inherentRiskLevel": "VeryHigh",
  "residualLikelihood": "Possible",
  "residualImpact": "Major",
  "residualRiskScore": 12,
  "residualRiskLevel": "High",
  "treatmentStrategy": "Mitigate",
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "department": "Credit Risk Management",
  "identifiedDate": "2024-01-15T09:00:00Z",
  "lastAssessmentDate": "2024-01-20T14:30:00Z",
  "nextReviewDate": "2024-04-15T09:00:00Z",
  "riskAppetite": 12,
  "controls": [],
  "mitigationActions": [],
  "keyRiskIndicators": []
}
```

---

### Update Risk
Update an existing risk entry.

**Endpoint:** `PUT /api/risks/{id}`

**Request Body:**
```json
{
  "title": "Updated Risk Title",
  "description": "Updated description",
  "treatmentStrategy": "Mitigate"
}
```

**Response:** `200 OK`

---

### Assess Risk
Perform or update risk assessment.

**Endpoint:** `POST /api/risks/{id}/assess`

**Request Body:**
```json
{
  "inherentLikelihood": "Likely",
  "inherentImpact": "Major",
  "residualLikelihood": "Possible",
  "residualImpact": "Moderate",
  "assessmentNotes": "Controls are partially effective"
}
```

**Response:** `200 OK`
```json
{
  "inherentRiskScore": 16,
  "inherentRiskLevel": "VeryHigh",
  "residualRiskScore": 9,
  "residualRiskLevel": "Medium",
  "message": "Risk assessment updated successfully"
}
```

---

### Delete Risk
Delete a risk entry (soft delete - moves to archived).

**Endpoint:** `DELETE /api/risks/{id}`

**Response:** `204 No Content`

---

## Risk Controls Endpoints

### Add Control to Risk
Add a control mechanism to mitigate a risk.

**Endpoint:** `POST /api/risks/{riskId}/controls`

**Request Body:**
```json
{
  "controlName": "Credit Limit Monitoring",
  "description": "Automated monitoring of credit exposure limits",
  "controlType": "Preventive",
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "testingFrequency": "Monthly"
}
```

**Response:** `201 Created`
```json
{
  "id": "9c7d2345-6789-01bc-def0-234567890bcd",
  "controlName": "Credit Limit Monitoring",
  "effectiveness": null,
  "createdAt": "2024-01-28T10:30:00Z"
}
```

---

### Update Control Effectiveness
Update the effectiveness rating of a control after testing.

**Endpoint:** `PUT /api/controls/{id}/effectiveness`

**Request Body:**
```json
{
  "effectiveness": "Effective",
  "testingEvidence": "Tested 100 transactions, all within limits",
  "lastTestedDate": "2024-01-25T15:00:00Z"
}
```

**Response:** `200 OK`

---

## Mitigation Actions Endpoints

### Add Mitigation Action
Create an action plan to mitigate a risk.

**Endpoint:** `POST /api/risks/{riskId}/mitigations`

**Request Body:**
```json
{
  "actionTitle": "Implement Sector Diversification Policy",
  "description": "Develop and implement policy to limit sector concentration",
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "dueDate": "2024-06-30T23:59:59Z",
  "estimatedCost": 50000.00
}
```

**Response:** `201 Created`

---

### Update Mitigation Progress
Update the progress of a mitigation action.

**Endpoint:** `PUT /api/mitigations/{id}/progress`

**Request Body:**
```json
{
  "progressPercentage": 45,
  "notes": "Policy draft completed, under review",
  "actualCost": 25000.00
}
```

**Response:** `200 OK`

---

## Key Risk Indicators (KRI) Endpoints

### Create KRI
Define a Key Risk Indicator for monitoring.

**Endpoint:** `POST /api/risks/{riskId}/kris`

**Request Body:**
```json
{
  "name": "Corporate Lending Concentration Ratio",
  "description": "Percentage of total loans to corporate sector",
  "measurementUnit": "Percentage",
  "thresholdWarning": 60.0,
  "thresholdCritical": 75.0,
  "frequency": "Monthly",
  "dataSource": "Loan Management System",
  "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response:** `201 Created`

---

### Record KRI Measurement
Record a new measurement for a KRI.

**Endpoint:** `POST /api/kris/{id}/measurements`

**Request Body:**
```json
{
  "value": 68.5,
  "notes": "Slight increase from last month, monitoring closely"
}
```

**Response:** `201 Created`
```json
{
  "status": "Warning",
  "message": "KRI threshold warning exceeded"
}
```

---

### Get KRI Trend
Retrieve historical measurements for trend analysis.

**Endpoint:** `GET /api/kris/{id}/trend`

**Query Parameters:**
- `startDate` (optional): Start date for trend data
- `endDate` (optional): End date for trend data

**Response:** `200 OK`
```json
{
  "kriName": "Corporate Lending Concentration Ratio",
  "currentValue": 68.5,
  "status": "Warning",
  "measurements": [
    {
      "value": 65.2,
      "measuredDate": "2023-12-31T23:59:59Z",
      "status": "Normal"
    },
    {
      "value": 67.8,
      "measuredDate": "2024-01-31T23:59:59Z",
      "status": "Warning"
    }
  ]
}
```

---

## Dashboard & Reporting Endpoints

### Get Risk Dashboard
Retrieve risk dashboard summary data.

**Endpoint:** `GET /api/risks/dashboard`

**Response:** `200 OK`
```json
{
  "summary": {
    "totalRisks": 95,
    "activeRisks": 78,
    "criticalRisks": 5,
    "veryHighRisks": 12,
    "highRisks": 28,
    "mediumRisks": 33,
    "lowRisks": 17
  },
  "byCategory": {
    "Credit": 32,
    "Operational": 28,
    "Market": 15,
    "Liquidity": 8,
    "Strategic": 7,
    "Compliance": 5
  },
  "top10Risks": [],
  "overdueMitigations": 8,
  "kriAlertsCount": 3
}
```

---

### Get Risk Heat Map
Retrieve data for risk heat map visualization.

**Endpoint:** `GET /api/risks/heatmap`

**Response:** `200 OK`
```json
{
  "matrix": [
    {
      "likelihood": "AlmostCertain",
      "impact": "Catastrophic",
      "count": 2,
      "riskLevel": "Critical"
    }
  ]
}
```

---

### Generate Risk Report
Generate comprehensive risk report.

**Endpoint:** `POST /api/risks/reports/generate`

**Request Body:**
```json
{
  "reportType": "Executive",
  "reportPeriod": "Monthly",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-01-31T23:59:59Z",
  "includeControls": true,
  "includeMitigations": true,
  "includeKRIs": true,
  "format": "PDF"
}
```

**Response:** `200 OK`
```json
{
  "reportId": "RPT-2024-001",
  "status": "Generated",
  "downloadUrl": "https://api.wekeza.com/erms/v1/reports/RPT-2024-001/download"
}
```

---

## Integration Endpoints

### Sync with Wekeza Core
Trigger synchronization with Wekeza Core Banking System.

**Endpoint:** `POST /api/integration/sync`

**Request Body:**
```json
{
  "syncType": "Full",
  "modules": ["Credit", "AML", "Compliance"]
}
```

**Response:** `202 Accepted`
```json
{
  "syncJobId": "SYNC-2024-001",
  "status": "InProgress",
  "startedAt": "2024-01-28T11:00:00Z"
}
```

---

### Get Integration Status
Check the status of integration with Wekeza Core.

**Endpoint:** `GET /api/integration/status`

**Response:** `200 OK`
```json
{
  "status": "Connected",
  "lastSyncDate": "2024-01-28T10:00:00Z",
  "nextSyncDate": "2024-01-28T22:00:00Z",
  "syncedModules": ["Credit", "AML", "Compliance"],
  "errors": []
}
```

---

## Error Responses

All endpoints may return the following error responses:

### 400 Bad Request
```json
{
  "error": "ValidationError",
  "message": "Invalid input data",
  "details": [
    {
      "field": "riskAppetite",
      "message": "Risk appetite must be between 1 and 25"
    }
  ]
}
```

### 401 Unauthorized
```json
{
  "error": "Unauthorized",
  "message": "Invalid or expired authentication token"
}
```

### 403 Forbidden
```json
{
  "error": "Forbidden",
  "message": "Insufficient permissions to access this resource"
}
```

### 404 Not Found
```json
{
  "error": "NotFound",
  "message": "Risk with ID '7b8e1234-5678-90ab-cdef-1234567890ab' not found"
}
```

### 500 Internal Server Error
```json
{
  "error": "InternalServerError",
  "message": "An unexpected error occurred",
  "requestId": "req-123456"
}
```

---

## Rate Limiting

API requests are rate limited:
- **Standard Users**: 1000 requests per hour
- **Risk Officers**: 5000 requests per hour
- **System Integration**: 10000 requests per hour

Rate limit headers are included in all responses:
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 987
X-RateLimit-Reset: 1706443200
```

---

## Pagination

List endpoints support pagination with the following parameters:
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 20, max: 100)

Pagination metadata is included in the response:
```json
{
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalRecords": 95
  }
}
```

---

## Versioning

The API uses URL versioning. The current version is `v1`.

```
/api/v1/risks
```

Breaking changes will be introduced in new versions while maintaining backward compatibility for at least 12 months.

---

## Support

For API support:
- Email: api-support@wekeza.com
- Documentation: https://docs.wekeza.com/erms/api
- Status Page: https://status.wekeza.com
