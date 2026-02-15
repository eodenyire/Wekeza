# Core Banking API Integration - Design

## 1. Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     Web Channels (React)                     │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐      │
│  │ Personal │ │   SME    │ │Corporate │ │  Public  │      │
│  │ Banking  │ │ Banking  │ │ Banking  │ │  Sector  │      │
│  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘      │
└───────┼────────────┼────────────┼────────────┼─────────────┘
        │            │            │            │
        └────────────┴────────────┴────────────┘
                     │ HTTPS/JSON
        ┌────────────▼────────────────────────────────┐
        │      Wekeza.Core.Api (ASP.NET Core)        │
        │  ┌──────────────────────────────────────┐  │
        │  │  Controllers (API Endpoints)         │  │
        │  └──────────────┬───────────────────────┘  │
        │  ┌──────────────▼───────────────────────┐  │
        │  │  Application Layer (MediatR)         │  │
        │  │  - Commands & Queries                │  │
        │  │  - Validators (FluentValidation)     │  │
        │  │  - Business Logic                    │  │
        │  └──────────────┬───────────────────────┘  │
        │  ┌──────────────▼───────────────────────┐  │
        │  │  Domain Layer                        │  │
        │  │  - Entities & Aggregates             │  │
        │  │  - Domain Services                   │  │
        │  │  - Business Rules                    │  │
        │  └──────────────┬───────────────────────┘  │
        │  ┌──────────────▼───────────────────────┐  │
        │  │  Infrastructure Layer                │  │
        │  │  - Repositories (EF Core)            │  │
        │  │  - DbContext                         │  │
        │  └──────────────┬───────────────────────┘  │
        └─────────────────┼──────────────────────────┘
                          │
        ┌─────────────────▼──────────────────────────┐
        │     Database (PostgreSQL/SQL Server)       │
        │  - Accounts, Transactions, Customers       │
        │  - Loans, Securities, Audit Logs           │
        └────────────────────────────────────────────┘
```

## 2. Database Schema Design

### 2.1 Core Tables

```sql
-- Customers
CREATE TABLE Customers (
    CustomerId UUID PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(20),
    DateOfBirth DATE,
    IdNumber VARCHAR(50) UNIQUE,
    CustomerType VARCHAR(20), -- INDIVIDUAL, SME, CORPORATE, GOVERNMENT
    Status VARCHAR(20), -- ACTIVE, DORMANT, CLOSED
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Accounts
CREATE TABLE Accounts (
    AccountId UUID PRIMARY KEY,
    AccountNumber VARCHAR(20) UNIQUE NOT NULL,
    CustomerId UUID REFERENCES Customers(CustomerId),
    AccountType VARCHAR(20), -- SAVINGS, CURRENT, LOAN, INVESTMENT
    Currency VARCHAR(3) DEFAULT 'KES',
    Balance DECIMAL(18,2) DEFAULT 0,
    AvailableBalance DECIMAL(18,2) DEFAULT 0,
    Status VARCHAR(20), -- ACTIVE, FROZEN, CLOSED
    OpenedDate DATE NOT NULL,
    ClosedDate DATE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Transactions
CREATE TABLE Transactions (
    TransactionId UUID PRIMARY KEY,
    AccountId UUID REFERENCES Accounts(AccountId),
    TransactionType VARCHAR(50), -- DEPOSIT, WITHDRAWAL, TRANSFER, FEE
    Amount DECIMAL(18,2) NOT NULL,
    Currency VARCHAR(3) DEFAULT 'KES',
    BalanceAfter DECIMAL(18,2),
    Description VARCHAR(500),
    Reference VARCHAR(100),
    Status VARCHAR(20), -- PENDING, POSTED, REVERSED
    ValueDate DATE NOT NULL,
    PostedDate TIMESTAMP,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Loans
CREATE TABLE Loans (
    LoanId UUID PRIMARY KEY,
    LoanNumber VARCHAR(20) UNIQUE NOT NULL,
    CustomerId UUID REFERENCES Customers(CustomerId),
    AccountId UUID REFERENCES Accounts(AccountId),
    LoanType VARCHAR(50), -- PERSONAL, MORTGAGE, BUSINESS, GOVERNMENT
    PrincipalAmount DECIMAL(18,2) NOT NULL,
    InterestRate DECIMAL(5,2) NOT NULL,
    TenorMonths INT NOT NULL,
    OutstandingBalance DECIMAL(18,2),
    Status VARCHAR(20), -- PENDING, APPROVED, DISBURSED, CLOSED, DEFAULT
    ApplicationDate DATE NOT NULL,
    ApprovalDate DATE,
    DisbursementDate DATE,
    MaturityDate DATE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Securities
CREATE TABLE Securities (
    SecurityId UUID PRIMARY KEY,
    SecurityType VARCHAR(20), -- TBILL, BOND, STOCK
    Symbol VARCHAR(20),
    Name VARCHAR(200),
    IssueDate DATE,
    MaturityDate DATE,
    CouponRate DECIMAL(5,2),
    FaceValue DECIMAL(18,2),
    CurrentPrice DECIMAL(18,2),
    Status VARCHAR(20), -- ACTIVE, MATURED, CANCELLED
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Security Orders
CREATE TABLE SecurityOrders (
    OrderId UUID PRIMARY KEY,
    CustomerId UUID REFERENCES Customers(CustomerId),
    SecurityId UUID REFERENCES Securities(SecurityId),
    OrderType VARCHAR(10), -- BUY, SELL
    Quantity INT NOT NULL,
    Price DECIMAL(18,2),
    TotalAmount DECIMAL(18,2),
    Status VARCHAR(20), -- PENDING, EXECUTED, CANCELLED
    OrderDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ExecutionDate TIMESTAMP
);

-- Audit Log
CREATE TABLE AuditLogs (
    AuditId UUID PRIMARY KEY,
    EntityType VARCHAR(100),
    EntityId UUID,
    Action VARCHAR(50), -- CREATE, UPDATE, DELETE
    UserId UUID,
    Username VARCHAR(100),
    Changes JSONB,
    IpAddress VARCHAR(50),
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### 2.2 Indexes

```sql
CREATE INDEX idx_accounts_customer ON Accounts(CustomerId);
CREATE INDEX idx_accounts_number ON Accounts(AccountNumber);
CREATE INDEX idx_transactions_account ON Transactions(AccountId);
CREATE INDEX idx_transactions_date ON Transactions(ValueDate);
CREATE INDEX idx_loans_customer ON Loans(CustomerId);
CREATE INDEX idx_loans_status ON Loans(Status);
CREATE INDEX idx_audit_entity ON AuditLogs(EntityType, EntityId);
CREATE INDEX idx_audit_timestamp ON AuditLogs(Timestamp);
```

## 3. API Endpoint Design

### 3.1 Public Sector Portal Endpoints

```
GET    /api/public-sector/dashboard/metrics
GET    /api/public-sector/securities/tbills
GET    /api/public-sector/securities/bonds
GET    /api/public-sector/securities/stocks
POST   /api/public-sector/securities/orders
GET    /api/public-sector/loans/applications
POST   /api/public-sector/loans/applications
GET    /api/public-sector/loans/{id}
POST   /api/public-sector/loans/{id}/approve
POST   /api/public-sector/loans/{id}/disburse
GET    /api/public-sector/accounts
GET    /api/public-sector/payments/bulk
POST   /api/public-sector/payments/bulk
GET    /api/public-sector/grants/applications
POST   /api/public-sector/grants/applications
POST   /api/public-sector/grants/{id}/approve
```

### 3.2 Personal Banking Endpoints

```
GET    /api/customer-portal/profile
GET    /api/customer-portal/accounts
GET    /api/customer-portal/accounts/{id}/balance
GET    /api/customer-portal/accounts/{id}/transactions
POST   /api/customer-portal/transactions/transfer
POST   /api/customer-portal/transactions/pay-bill
GET    /api/customer-portal/loans
POST   /api/customer-portal/loans/apply
GET    /api/customer-portal/cards
POST   /api/customer-portal/cards/request
```

### 3.3 Response Format

```json
{
  "success": true,
  "data": { /* actual data */ },
  "message": "Operation completed successfully",
  "metadata": {
    "timestamp": "2026-02-14T20:00:00Z",
    "requestId": "uuid"
  }
}
```

Error Response:
```json
{
  "success": false,
  "error": {
    "code": "INSUFFICIENT_FUNDS",
    "message": "Account balance is insufficient for this transaction",
    "details": {
      "accountBalance": 1000.00,
      "requestedAmount": 5000.00
    }
  },
  "metadata": {
    "timestamp": "2026-02-14T20:00:00Z",
    "requestId": "uuid"
  }
}
```

## 4. Implementation Strategy

### Phase 1: Database Setup (Priority: CRITICAL)
1. Create database schema with migrations
2. Seed initial data (test customers, accounts)
3. Configure connection strings
4. Implement health checks

### Phase 2: Core Repositories (Priority: CRITICAL)
1. Implement AccountRepository with balance queries
2. Implement TransactionRepository with posting logic
3. Implement CustomerRepository with KYC data
4. Implement LoanRepository with workflow states
5. Implement SecurityRepository with market data

### Phase 3: Public Sector Integration (Priority: HIGH)
1. Dashboard metrics endpoint (aggregate real data)
2. Securities trading endpoints (create real orders)
3. Loan application endpoints (store in database)
4. Bulk payment endpoints (validate accounts)
5. Grant management endpoints

### Phase 4: Personal Banking Integration (Priority: HIGH)
1. Account balance endpoint (query real balance)
2. Transaction history endpoint (query transactions)
3. Fund transfer endpoint (post real transactions)
4. Loan application endpoint (create loan records)
5. Card request endpoint (track in system)

### Phase 5: SME & Corporate Banking (Priority: MEDIUM)
1. Multi-account management
2. Payroll processing
3. Treasury operations
4. Trade finance
5. Group consolidation

### Phase 6: Testing & Optimization (Priority: HIGH)
1. Integration tests for all endpoints
2. Load testing for performance
3. Security testing for vulnerabilities
4. Database query optimization
5. Caching strategy implementation

## 5. Data Flow Examples

### 5.1 Fund Transfer Flow

```
1. User initiates transfer in web channel
   POST /api/customer-portal/transactions/transfer
   {
     "fromAccountId": "uuid",
     "toAccountNumber": "1234567890",
     "amount": 5000.00,
     "narration": "Payment"
   }

2. API validates request
   - Check authentication token
   - Validate account ownership
   - Check sufficient balance
   - Validate destination account exists

3. Application layer processes command
   - Begin database transaction
   - Lock source account
   - Debit source account
   - Credit destination account
   - Create transaction records
   - Update balances
   - Commit transaction

4. Return response
   {
     "success": true,
     "data": {
       "transactionId": "uuid",
       "reference": "TXN123456",
       "status": "POSTED"
     }
   }
```

### 5.2 Dashboard Metrics Aggregation

```sql
-- Securities Portfolio Value
SELECT 
    SUM(CASE WHEN s.SecurityType = 'TBILL' THEN so.TotalAmount ELSE 0 END) as TbillsValue,
    SUM(CASE WHEN s.SecurityType = 'BOND' THEN so.TotalAmount ELSE 0 END) as BondsValue,
    SUM(CASE WHEN s.SecurityType = 'STOCK' THEN so.TotalAmount ELSE 0 END) as StocksValue
FROM SecurityOrders so
JOIN Securities s ON so.SecurityId = s.SecurityId
WHERE so.Status = 'EXECUTED' AND so.OrderType = 'BUY';

-- Loan Portfolio
SELECT 
    SUM(OutstandingBalance) as TotalOutstanding,
    SUM(CASE WHEN LoanType = 'GOVERNMENT' THEN OutstandingBalance ELSE 0 END) as GovernmentLoans
FROM Loans
WHERE Status IN ('DISBURSED', 'ACTIVE');

-- Account Balances
SELECT 
    COUNT(*) as TotalAccounts,
    SUM(Balance) as TotalBalance
FROM Accounts
WHERE Status = 'ACTIVE';
```

## 6. Security Considerations

- All endpoints require JWT authentication
- Role-based authorization (Customer, Teller, Admin)
- Input validation using FluentValidation
- SQL injection prevention (parameterized queries)
- Rate limiting to prevent abuse
- Audit logging for compliance
- Encryption of sensitive data at rest

## 7. Performance Optimization

- Database connection pooling
- Query result caching (Redis)
- Async/await for I/O operations
- Pagination for large result sets
- Database indexing on frequently queried columns
- CDN for static assets
- API response compression

## 8. Monitoring & Observability

- Structured logging with Serilog
- Application Insights for telemetry
- Health check endpoints
- Performance metrics (response times)
- Error rate tracking
- Database query performance monitoring
