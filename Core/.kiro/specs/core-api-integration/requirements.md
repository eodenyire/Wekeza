# Core Banking API Integration - Requirements

## 1. Overview
Establish complete end-to-end integration between all web channels (Personal Banking, SME Banking, Corporate Banking, Public Sector Portal) and the Wekeza Core Banking API with real data persistence and business logic.

## 2. Problem Statement
Currently, the web channels are partially integrated with the core API:
- Authentication works but uses mock user validation
- Dashboard endpoints return mock/hardcoded data
- No real database persistence
- Business logic is not implemented in the core
- Each channel makes direct API calls without proper service layer

## 3. User Stories

### 3.1 As a System Administrator
- I want all API endpoints to connect to a real database so that data persists across sessions
- I want proper error handling and logging so that I can troubleshoot issues in production
- I want API documentation (Swagger) to be accurate and complete
- I want health checks to verify database connectivity and API status

### 3.2 As a Developer
- I want consistent API response formats across all endpoints
- I want proper authentication and authorization on all endpoints
- I want the API to validate all inputs and return meaningful error messages
- I want the API to follow RESTful conventions

### 3.3 As a Public Sector User
- I want the dashboard to show real government securities, loans, and banking data
- I want to place real securities orders that are processed by the core system
- I want loan applications to be stored and processed through approval workflows
- I want bulk payments to be validated and processed against real accounts

### 3.4 As a Personal Banking Customer
- I want to see my real account balances and transaction history
- I want to transfer funds between my accounts with proper validation
- I want to apply for loans and see real approval status
- I want to request cards and track their delivery status

### 3.5 As an SME Banking Customer
- I want to manage multiple business accounts with real data
- I want to process payroll and supplier payments
- I want to apply for business loans and lines of credit
- I want to see cash flow analytics based on real transactions

### 3.6 As a Corporate Banking Customer
- I want to manage treasury operations with real FX rates
- I want to process trade finance transactions (LCs, guarantees)
- I want to see consolidated group reporting across entities
- I want to manage multi-currency accounts

## 4. Acceptance Criteria

### 4.1 Database Integration
- [ ] PostgreSQL/SQL Server database is configured and connected
- [ ] Entity Framework migrations are created for all domain entities
- [ ] Database connection string is configurable via appsettings.json
- [ ] Database health check endpoint returns connection status
- [ ] All CRUD operations persist to database

### 4.2 Authentication & Authorization
- [ ] JWT tokens are validated on all protected endpoints
- [ ] User credentials are validated against database (not mocked)
- [ ] Role-based authorization is enforced (Customer, Teller, Admin, etc.)
- [ ] Token refresh mechanism is implemented
- [ ] Password hashing uses secure algorithm (BCrypt/PBKDF2)

### 4.3 Public Sector Portal Integration
- [ ] Dashboard metrics aggregate real data from accounts, loans, securities tables
- [ ] Securities trading endpoints create real orders in database
- [ ] Loan applications are stored with proper workflow state
- [ ] Bulk payments validate against real account balances
- [ ] Grant management tracks real disbursements and compliance

### 4.4 Personal Banking Integration
- [ ] Account balance reflects real transactions
- [ ] Fund transfers debit/credit actual accounts atomically
- [ ] Transaction history shows real posted transactions
- [ ] Loan applications create real loan records
- [ ] Card requests are tracked in card management system

### 4.5 SME Banking Integration
- [ ] Business accounts support multiple signatories
- [ ] Payroll processing validates employee accounts
- [ ] Invoice financing creates real loan facilities
- [ ] Cash flow analytics query real transaction data
- [ ] Multi-account sweeps execute real transfers

### 4.6 Corporate Banking Integration
- [ ] Treasury deals are recorded with real FX rates
- [ ] Trade finance LCs create real contingent liabilities
- [ ] Group consolidation queries real multi-entity data
- [ ] Liquidity management uses real account positions
- [ ] Derivatives are marked-to-market with real rates

### 4.7 API Quality Standards
- [ ] All endpoints return consistent ApiResponse<T> format
- [ ] Validation errors return 400 with detailed field errors
- [ ] Business rule violations return 422 with clear messages
- [ ] Server errors return 500 with correlation ID for tracing
- [ ] All endpoints are documented in Swagger with examples

### 4.8 Performance Requirements
- [ ] Dashboard loads in < 2 seconds with real data
- [ ] Account balance queries return in < 500ms
- [ ] Fund transfers complete in < 3 seconds
- [ ] Bulk payment processing handles 1000+ payments
- [ ] API supports 100+ concurrent users

### 4.9 Data Integrity
- [ ] All financial transactions use database transactions (ACID)
- [ ] Account balances are calculated atomically
- [ ] Concurrent updates use optimistic/pessimistic locking
- [ ] Audit trail records all state changes
- [ ] Data validation prevents invalid states

### 4.10 Testing
- [ ] Integration tests verify end-to-end flows
- [ ] API tests cover all endpoints with real database
- [ ] Load tests validate performance under load
- [ ] Security tests verify authorization rules
- [ ] Data migration tests ensure schema compatibility

## 5. Technical Requirements

### 5.1 Database Schema
- Accounts table with balance, currency, status
- Transactions table with debit/credit entries
- Customers table with KYC information
- Loans table with repayment schedules
- Securities table with positions and orders
- Audit log table for compliance

### 5.2 API Architecture
- Repository pattern for data access
- Service layer for business logic
- CQRS for read/write separation
- MediatR for command/query handling
- AutoMapper for DTO mapping

### 5.3 Configuration
- Database connection strings in appsettings.json
- JWT settings (secret, issuer, audience, expiry)
- CORS policy for web channels
- Rate limiting configuration
- Logging configuration (Serilog)

## 6. Out of Scope
- Payment gateway integration (Mpesa, card processing)
- External credit bureau integration
- Core banking system migration
- Mobile app development
- Blockchain/cryptocurrency features

## 7. Dependencies
- Wekeza.Core.Domain (domain entities)
- Wekeza.Core.Application (business logic)
- Wekeza.Core.Infrastructure (data access)
- Entity Framework Core
- PostgreSQL or SQL Server

## 8. Risks & Mitigation
- **Risk**: Database performance degrades with large datasets
  - **Mitigation**: Implement indexing, query optimization, caching
- **Risk**: Concurrent transactions cause data inconsistency
  - **Mitigation**: Use database transactions, row-level locking
- **Risk**: API breaking changes affect web channels
  - **Mitigation**: API versioning, backward compatibility
- **Risk**: Security vulnerabilities in authentication
  - **Mitigation**: Security audit, penetration testing

## 9. Success Metrics
- All 4 web channels load real data from API
- Zero mock data in production endpoints
- < 1% API error rate
- < 2 second average response time
- 100% test coverage on critical paths
