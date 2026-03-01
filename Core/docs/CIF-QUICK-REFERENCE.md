# CIF Module - Quick Reference Guide

## 🚀 Quick Start

### 1. Run Database Migration
```powershell
cd Core/Wekeza.Core.Infrastructure
dotnet ef database update --startup-project ../Wekeza.Core.Api
```

### 2. Start Application
```powershell
cd Core/Wekeza.Core.Api
dotnet run
```

### 3. Access Swagger UI
```
https://localhost:5001/swagger
```

---

## 📋 API Endpoints Cheat Sheet

### Create Individual Party
```http
POST /api/cif/individual
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1990-01-15",
  "gender": "Male",
  "nationality": "Kenyan",
  "primaryEmail": "john.doe@example.com",
  "primaryPhone": "254712345678",
  "primaryAddress": {
    "addressLine1": "123 Main Street",
    "city": "Nairobi",
    "country": "Kenya",
    "postalCode": "00100"
  },
  "primaryIdentification": {
    "documentType": "NationalID",
    "documentNumber": "12345678",
    "issuingCountry": "Kenya",
    "issueDate": "2020-01-01",
    "expiryDate": "2030-01-01"
  }
}
```

**Response**: `CIF20260117001`

---

### Create Corporate Party
```http
POST /api/cif/corporate
Authorization: Bearer {token}
Content-Type: application/json

{
  "companyName": "Acme Corporation Ltd",
  "registrationNumber": "PVT-2020-12345",
  "incorporationDate": "2020-01-15",
  "companyType": "LLC",
  "industry": "Technology",
  "primaryEmail": "info@acme.co.ke",
  "primaryPhone": "254712345678",
  "registeredAddress": {
    "addressLine1": "456 Business Park",
    "city": "Nairobi",
    "country": "Kenya",
    "postalCode": "00100"
  },
  "directors": [
    {
      "firstName": "Jane",
      "lastName": "Smith",
      "identificationNumber": "87654321",
      "nationality": "Kenyan",
      "role": "Managing Director",
      "shareholdingPercentage": 60
    }
  ]
}
```

**Response**: `CIF20260117002`

---

### Get Customer 360° View
```http
GET /api/cif/{partyNumber}/360-view
Authorization: Bearer {token}
```

**Example**: `GET /api/cif/CIF20260117001/360-view`

**Response**:
```json
{
  "partyNumber": "CIF20260117001",
  "fullName": "John Doe",
  "partyType": "Individual",
  "status": "Active",
  "kycStatus": "Completed",
  "riskRating": "Low",
  "accounts": [...],
  "loans": [...],
  "cards": [...],
  "recentTransactions": [...]
}
```

---

### Perform AML Screening
```http
POST /api/cif/aml-screening
Authorization: Bearer {token}
Content-Type: application/json

{
  "partyNumber": "CIF20260117001",
  "checkSanctions": true,
  "checkPEP": true,
  "checkAdverseMedia": true
}
```

**Response**:
```json
{
  "partyNumber": "CIF20260117001",
  "screeningDate": "2026-01-17T10:00:00Z",
  "overallRisk": "Low",
  "sanctionsMatch": false,
  "pepMatch": false,
  "adverseMediaMatch": false,
  "recommendation": "Approve"
}
```

---

### Update KYC Status
```http
PUT /api/cif/kyc-status
Authorization: Bearer {token}
Content-Type: application/json

{
  "partyNumber": "CIF20260117001",
  "newStatus": 2,
  "remarks": "KYC documents verified",
  "expiryDate": "2027-01-17"
}
```

**KYC Status Values**:
- 0 = Pending
- 1 = InProgress
- 2 = Completed
- 3 = Expired
- 4 = Rejected
- 5 = UnderReview

---

### Search Parties
```http
GET /api/cif/search?name=John
Authorization: Bearer {token}
```

**Response**:
```json
[
  {
    "partyNumber": "CIF20260117001",
    "fullName": "John Doe",
    "partyType": "Individual",
    "email": "john.doe@example.com",
    "phone": "254712345678",
    "status": "Active",
    "kycStatus": "Completed",
    "riskRating": "Low"
  }
]
```

---

### Get Pending KYC
```http
GET /api/cif/pending-kyc
Authorization: Bearer {token}
```

**Response**:
```json
[
  {
    "partyNumber": "CIF20260117003",
    "fullName": "Jane Smith",
    "partyType": "Individual",
    "kycStatus": "Pending",
    "createdDate": "2026-01-15T10:00:00Z",
    "daysPending": 2
  }
]
```

---

### Get High-Risk Parties
```http
GET /api/cif/high-risk
Authorization: Bearer {token}
```

**Response**:
```json
[
  {
    "partyNumber": "CIF20260117004",
    "fullName": "Risk Corp Ltd",
    "partyType": "Corporate",
    "riskRating": "High",
    "isPEP": false,
    "isSanctioned": false,
    "kycStatus": "Completed",
    "riskFlags": ["High Risk", "Large Transactions"]
  }
]
```

---

## 🔐 Authorization Roles

| Endpoint | Teller | RiskOfficer | Administrator |
|----------|--------|-------------|---------------|
| Create Individual | ✅ | ✅ | ✅ |
| Create Corporate | ❌ | ✅ | ✅ |
| Get 360° View | ✅ | ✅ | ✅ |
| AML Screening | ❌ | ✅ | ✅ |
| Update KYC | ❌ | ✅ | ✅ |
| Search | ✅ | ✅ | ✅ |
| Pending KYC | ❌ | ✅ | ✅ |
| High-Risk | ❌ | ✅ | ✅ |

---

## 📊 Enums Reference

### PartyType
```csharp
Individual = 0
Corporate = 1
Government = 2
FinancialInstitution = 3
Trust = 4
Partnership = 5
Cooperative = 6
NGO = 7
```

### PartyStatus
```csharp
Active = 0
Inactive = 1
Blocked = 2
Suspended = 3
Deceased = 4
Merged = 5
Closed = 6
```

### KYCStatus
```csharp
Pending = 0
InProgress = 1
Completed = 2
Expired = 3
Rejected = 4
UnderReview = 5
```

### RiskRating
```csharp
Low = 0
Medium = 1
High = 2
VeryHigh = 3
Prohibited = 4
```

### CustomerSegment
```csharp
Retail = 0
Affluent = 1
SME = 2
Corporate = 3
Government = 4
FinancialInstitution = 5
```

---

## 🔍 Common Queries

### Find Party by Email
```csharp
var party = await _partyRepository.GetByEmailAsync("john.doe@example.com");
```

### Find Party by Phone
```csharp
var party = await _partyRepository.GetByPhoneAsync("254712345678");
```

### Get All High-Risk Parties
```csharp
var parties = await _partyRepository.GetHighRiskPartiesAsync();
```

### Get Pending KYC
```csharp
var parties = await _partyRepository.GetPendingKYCAsync();
```

### Search by Name
```csharp
var parties = await _partyRepository.SearchByNameAsync("John");
```

---

## 🛠️ Validation Rules

### Individual Party
- ✅ First name required (max 100 chars)
- ✅ Last name required (max 100 chars)
- ✅ Date of birth required (must be 18+ years old)
- ✅ Email required (valid format)
- ✅ Phone required (format: 254XXXXXXXXX)
- ✅ Address required
- ✅ Identification required

### Corporate Party
- ✅ Company name required (max 200 chars)
- ✅ Registration number required (max 50 chars)
- ✅ Incorporation date required (not in future)
- ✅ Company type required (LLC, PLC, Partnership, etc.)
- ✅ Industry required (max 100 chars)
- ✅ Email required (valid format)
- ✅ Phone required (format: 254XXXXXXXXX)
- ✅ Registered address required
- ✅ At least one director required

---

## 🚨 Error Handling

### Common Errors

**400 Bad Request**
```json
{
  "type": "ValidationError",
  "title": "One or more validation errors occurred",
  "errors": {
    "PrimaryEmail": ["Invalid email format"]
  }
}
```

**404 Not Found**
```json
{
  "type": "NotFound",
  "title": "Party with number CIF20260117001 not found"
}
```

**409 Conflict**
```json
{
  "type": "Conflict",
  "title": "A party with email john.doe@example.com already exists"
}
```

**401 Unauthorized**
```json
{
  "type": "Unauthorized",
  "title": "Authentication required"
}
```

**403 Forbidden**
```json
{
  "type": "Forbidden",
  "title": "Insufficient permissions"
}
```

---

## 💾 Database Queries

### Check Party Exists
```sql
SELECT * FROM "Parties" WHERE "PartyNumber" = 'CIF20260117001';
```

### Get All Pending KYC
```sql
SELECT * FROM "Parties" 
WHERE "KYCStatus" IN (0, 1)
ORDER BY "CreatedDate";
```

### Get High-Risk Parties
```sql
SELECT * FROM "Parties" 
WHERE "RiskRating" IN (2, 3)
OR "IsPEP" = true
OR "IsSanctioned" = true;
```

### Count by Segment
```sql
SELECT "Segment", COUNT(*) 
FROM "Parties" 
GROUP BY "Segment";
```

---

## 🧪 Testing Examples

### Using cURL

**Create Individual**
```bash
curl -X POST https://localhost:5001/api/cif/individual \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "dateOfBirth": "1990-01-15",
    "gender": "Male",
    "nationality": "Kenyan",
    "primaryEmail": "john.doe@example.com",
    "primaryPhone": "254712345678"
  }'
```

**Get 360° View**
```bash
curl -X GET https://localhost:5001/api/cif/CIF20260117001/360-view \
  -H "Authorization: Bearer {token}"
```

### Using PowerShell

**Create Individual**
```powershell
$body = @{
    firstName = "John"
    lastName = "Doe"
    dateOfBirth = "1990-01-15"
    gender = "Male"
    nationality = "Kenyan"
    primaryEmail = "john.doe@example.com"
    primaryPhone = "254712345678"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/cif/individual" `
    -Method Post `
    -Headers @{ Authorization = "Bearer $token" } `
    -Body $body `
    -ContentType "application/json"
```

---

## 📚 Additional Resources

- **Full Documentation**: `WEEK1-CIF-COMPLETE.md`
- **Implementation Summary**: `CIF-MODULE-SUMMARY.md`
- **Completion Report**: `WEEK1-COMPLETION-REPORT.md`
- **Enterprise Roadmap**: `ENTERPRISE-ROADMAP.md`
- **Swagger UI**: `https://localhost:5001/swagger`

---

## 🆘 Troubleshooting

### Issue: Migration fails
**Solution**: Ensure PostgreSQL is running and connection string is correct

### Issue: Duplicate party error
**Solution**: Check if email/phone/ID already exists in database

### Issue: Validation error
**Solution**: Check request body matches validation rules

### Issue: 401 Unauthorized
**Solution**: Ensure valid JWT token is provided

### Issue: 403 Forbidden
**Solution**: Check user has required role for the operation

---

**Quick Reference Version**: 1.0  
**Last Updated**: January 17, 2026  
**Module**: CIF / Party Management
