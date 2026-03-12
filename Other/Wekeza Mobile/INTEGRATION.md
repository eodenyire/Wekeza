# Wekeza Mobile - Backend Integration Guide

This guide explains how the Flutter mobile app integrates with the Wekeza Core Banking System backend.

## Architecture Overview

```
┌─────────────────────┐
│  Flutter Mobile App │
│   (Dart/Flutter)    │
└──────────┬──────────┘
           │ HTTP/REST
           │ JSON
           │ JWT Auth
           ▼
┌─────────────────────┐
│   Wekeza Core API   │
│   (.NET 8 / C#)     │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│   PostgreSQL DB     │
└─────────────────────┘
```

## API Endpoints Integration

### 1. Authentication (`/api/authentication`)

#### Login
```dart
POST /api/authentication/login
Content-Type: application/json

Request:
{
  "username": "string",
  "password": "string"
}

Response:
{
  "token": "eyJhbGc...",
  "userId": "guid",
  "username": "string",
  "email": "string",
  "roles": ["Customer", "Administrator"],
  "expiresAt": "2026-01-22T14:00:00Z"
}

Flutter Implementation:
lib/services/auth_service.dart -> login()
```

#### Get Current User
```dart
GET /api/authentication/me
Authorization: Bearer {token}

Response:
{
  "userId": "guid",
  "username": "string",
  "email": "string",
  "firstName": "string",
  "lastName": "string",
  "roles": ["Customer"]
}

Flutter Implementation:
lib/services/auth_service.dart -> getCurrentUser()
```

### 2. Accounts (`/api/accounts`)

#### Get User Accounts
```dart
GET /api/accounts/user/accounts
Authorization: Bearer {token}

Response:
[
  {
    "id": "guid",
    "accountNumber": "ACC001234567",
    "accountName": "John Doe",
    "accountType": "Savings",
    "currency": "KES",
    "availableBalance": 50000.00,
    "bookBalance": 52000.00,
    "status": "Active",
    "openedDate": "2026-01-01T00:00:00Z"
  }
]

Flutter Implementation:
lib/services/account_service.dart -> getUserAccounts()
```

#### Get Account Balance
```dart
GET /api/accounts/{accountNumber}/balance
Authorization: Bearer {token}

Response:
{
  "accountNumber": "ACC001234567",
  "availableBalance": 50000.00,
  "bookBalance": 52000.00,
  "clearedBalance": 50000.00,
  "unclearedBalance": 2000.00,
  "currency": "KES",
  "asOfDate": "2026-01-22T13:00:00Z"
}

Flutter Implementation:
lib/services/account_service.dart -> getAccountBalance()
```

### 3. Transactions (`/api/transactions`)

#### Transfer Funds
```dart
POST /api/transactions/transfer
Authorization: Bearer {token}
Content-Type: application/json

Request:
{
  "fromAccountId": "guid",
  "toAccountId": "guid",
  "amount": 10000.00,
  "currency": "KES",
  "description": "Payment for services",
  "type": "Internal"
}

Response:
{
  "success": true,
  "transactionId": "guid",
  "referenceNumber": "TXN20260122001",
  "status": "Completed",
  "timestamp": "2026-01-22T13:30:00Z"
}

Flutter Implementation:
lib/services/transaction_service.dart -> transferFunds()
```

#### Get Transaction History
```dart
GET /api/transactions/statement/{accountNumber}?pageNumber=1&pageSize=20
Authorization: Bearer {token}

Response:
{
  "transactions": [
    {
      "id": "guid",
      "transactionType": "Transfer",
      "referenceNumber": "TXN20260122001",
      "amount": 10000.00,
      "currency": "KES",
      "fromAccount": "ACC001234567",
      "toAccount": "ACC007654321",
      "description": "Payment",
      "status": "Completed",
      "transactionDate": "2026-01-22T13:30:00Z"
    }
  ],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 20
}

Flutter Implementation:
lib/services/transaction_service.dart -> getTransactionHistory()
```

### 4. Loans (`/api/loans`)

#### Get User Loans
```dart
GET /api/loans/user/loans
Authorization: Bearer {token}

Response:
[
  {
    "id": "guid",
    "loanNumber": "LOAN001234",
    "accountId": "guid",
    "customerId": "guid",
    "productType": "Personal Loan",
    "principalAmount": 500000.00,
    "outstandingBalance": 350000.00,
    "interestRate": 12.5,
    "tenorMonths": 24,
    "status": "Active",
    "applicationDate": "2025-12-01T00:00:00Z",
    "disbursementDate": "2025-12-15T00:00:00Z",
    "maturityDate": "2027-12-15T00:00:00Z",
    "monthlyRepayment": 23958.33
  }
]

Flutter Implementation:
lib/services/loan_service.dart -> getUserLoans()
```

#### Apply for Loan
```dart
POST /api/loans/apply
Authorization: Bearer {token}
Content-Type: application/json

Request:
{
  "accountId": "guid",
  "customerId": "guid",
  "amount": 500000.00,
  "tenorMonths": 24,
  "productType": "Personal Loan",
  "purpose": "Business expansion",
  "monthlyIncome": 150000.00
}

Response:
{
  "id": "guid",
  "loanNumber": "LOAN001235",
  "status": "Pending",
  "applicationDate": "2026-01-22T13:00:00Z"
}

Flutter Implementation:
lib/services/loan_service.dart -> applyForLoan()
```

### 5. Cards (`/api/cards`)

#### Get User Cards
```dart
GET /api/cards/user/cards
Authorization: Bearer {token}

Response:
[
  {
    "id": "guid",
    "cardNumber": "4532123456789012",
    "cardType": "Debit",
    "accountId": "guid",
    "cardholderName": "JOHN DOE",
    "expiryDate": "2028-12-31T00:00:00Z",
    "status": "Active",
    "issuedDate": "2026-01-01T00:00:00Z",
    "dailyWithdrawalLimit": 50000.00,
    "dailyPurchaseLimit": 100000.00,
    "isVirtual": false
  }
]

Flutter Implementation:
lib/services/card_service.dart -> getUserCards()
```

#### Issue New Card
```dart
POST /api/cards/issue
Authorization: Bearer {token}
Content-Type: application/json

Request:
{
  "accountId": "guid",
  "cardType": "Debit",
  "cardholderName": "JOHN DOE",
  "isVirtual": false,
  "dailyWithdrawalLimit": 50000.00,
  "dailyPurchaseLimit": 100000.00
}

Response:
{
  "id": "guid",
  "cardNumber": "4532123456789012",
  "cvv": "123",  // Only returned during issuance
  "expiryDate": "2028-12-31T00:00:00Z",
  "status": "Pending"
}

Flutter Implementation:
lib/services/card_service.dart -> issueCard()
```

## Authentication Flow

### JWT Token Handling

1. **Login**: User provides credentials
2. **Token Receipt**: Backend returns JWT token
3. **Token Storage**: Stored securely using Flutter Secure Storage
4. **Token Usage**: Included in Authorization header for all requests
5. **Token Refresh**: Automatic refresh before expiry
6. **Token Expiry**: Redirects to login

```dart
// Token storage
await _storageService.saveAuthToken(token);

// Token usage
headers['Authorization'] = 'Bearer $token';

// Token retrieval
final token = await _storageService.getAuthToken();
```

## Error Handling

### HTTP Status Codes

- **200-299**: Success
- **400**: Bad Request - Invalid input
- **401**: Unauthorized - Invalid/expired token
- **403**: Forbidden - Insufficient permissions
- **404**: Not Found - Resource doesn't exist
- **500**: Internal Server Error - Backend error

### Flutter Error Handling

```dart
try {
  final result = await _apiService.post('/endpoint', body: data);
  // Handle success
} on ApiException catch (e) {
  // Handle API errors
  print('API Error: ${e.message}');
} on SocketException {
  // Handle network errors
  print('No internet connection');
} catch (e) {
  // Handle unexpected errors
  print('Unexpected error: $e');
}
```

## Data Models

### Consistent JSON Parsing

All models implement `fromJson()` and `toJson()` methods:

```dart
class Account {
  factory Account.fromJson(Map<String, dynamic> json) {
    return Account(
      id: json['id'] ?? '',
      accountNumber: json['accountNumber'] ?? '',
      // ... other fields
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'accountNumber': accountNumber,
      // ... other fields
    };
  }
}
```

## Security Considerations

### 1. Token Security
- Tokens stored in encrypted storage
- Never logged or displayed
- Cleared on logout

### 2. Network Security
- HTTPS only in production
- Certificate pinning (recommended)
- Request timeout configured

### 3. Data Validation
- Client-side validation before API calls
- Server-side validation for security
- Sanitize user input

### 4. Error Messages
- Generic error messages to users
- Detailed errors logged for debugging
- No sensitive data in error messages

## Testing the Integration

### 1. Manual Testing

```dart
// Test login
await authService.login(
  username: 'testuser',
  password: 'testpass'
);

// Test account fetch
final accounts = await accountService.getUserAccounts();

// Test transfer
await transactionService.transferFunds(
  TransferRequest(
    fromAccountId: 'acc1',
    toAccountId: 'acc2',
    amount: 1000.00,
  )
);
```

### 2. API Testing Tools

Use Postman or curl to test backend endpoints:

```bash
# Test login
curl -X POST http://localhost:5000/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpass"}'

# Test with token
curl -X GET http://localhost:5000/api/accounts/user/accounts \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Troubleshooting

### Common Integration Issues

#### 1. CORS Errors
**Problem**: Browser/device blocks cross-origin requests
**Solution**: Configure CORS on backend

```csharp
// In Startup.cs or Program.cs
services.AddCors(options => {
    options.AddPolicy("AllowMobile", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

#### 2. SSL Certificate Issues
**Problem**: Device rejects self-signed certificates
**Solution**: 
- Use proper SSL certificates in production
- For dev, temporarily disable verification (NOT for production)

#### 3. Timeout Issues
**Problem**: Requests timeout before completion
**Solution**: Increase timeout in app_config.dart

```dart
static const Duration connectionTimeout = Duration(seconds: 60);
static const Duration receiveTimeout = Duration(seconds: 60);
```

#### 4. JSON Parsing Errors
**Problem**: Backend returns unexpected JSON structure
**Solution**: Add null checks and default values

```dart
accountNumber: json['accountNumber'] ?? '',
amount: (json['amount'] ?? 0.0).toDouble(),
```

## Performance Optimization

### 1. Caching
- Cache frequently accessed data
- Use local storage for offline support
- Implement cache expiry

### 2. Pagination
- Request only needed data
- Implement infinite scroll
- Default page size: 20-50 items

### 3. Lazy Loading
- Load images on demand
- Lazy load screens
- Defer heavy operations

## Monitoring and Logging

### Request Logging
```dart
// Log all requests (dev only)
print('REQUEST: $method $endpoint');
print('BODY: $body');
print('RESPONSE: ${response.statusCode}');
```

### Error Tracking
- Log errors to analytics service
- Track API failures
- Monitor user flows

## Backend Requirements

For full integration, the backend must support:

1. ✅ JWT authentication
2. ✅ RESTful API endpoints
3. ✅ JSON request/response format
4. ✅ CORS configuration
5. ✅ HTTPS in production
6. ✅ Proper error responses
7. ✅ Pagination support
8. ✅ Input validation
9. ✅ Rate limiting
10. ✅ API versioning

## API Documentation

For complete API documentation, refer to:
- Swagger UI: `http://localhost:5000/swagger`
- API docs: See backend README
- Postman collection: Available in repository

---

**For additional support, contact the backend team or refer to the Wekeza Core API documentation.**
