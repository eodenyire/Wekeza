# Core Banking API Integration - Tasks

## Phase 1: Database Foundation (CRITICAL - Start Here)

- [ ] 1.1 Configure database connection
  - Add connection string to appsettings.json
  - Configure DbContext in Infrastructure layer
  - Test database connectivity

- [ ] 1.2 Create Entity Framework migrations
  - Create initial migration for all entities
  - Apply migration to database
  - Verify schema creation

- [ ] 1.3 Seed initial test data
  - Create seed data for customers
  - Create seed data for accounts
  - Create seed data for securities
  - Create seed data for users/authentication

- [ ] 1.4 Implement database health check
  - Add health check endpoint
  - Verify database connectivity
  - Return database status

## Phase 2: Core Repositories & Services (CRITICAL)

- [ ] 2.1 Implement AccountRepository
  - GetById with balance
  - GetByCustomerId
  - UpdateBalance (atomic)
  - GetAccountByNumber

- [ ] 2.2 Implement TransactionRepository
  - Create transaction
  - GetByAccountId with pagination
  - GetByDateRange
  - Post transaction (update balance)

- [ ] 2.3 Implement CustomerRepository
  - GetById with accounts
  - GetByEmail
  - Create customer
  - Update customer

- [ ] 2.4 Implement LoanRepository
  - Create loan application
  - GetById with repayment schedule
  - GetByCustomerId
  - UpdateStatus (workflow)

- [ ] 2.5 Implement SecurityRepository
  - GetAllSecurities by type
  - GetById
  - GetMarketData
  - UpdatePrice

- [ ] 2.6 Implement SecurityOrderRepository
  - CreateOrder
  - GetByCustomerId
  - ExecuteOrder
  - GetPortfolio

## Phase 3: Authentication & Authorization (CRITICAL)

- [ ] 3.1 Implement user authentication
  - Create Users table
  - Hash passwords (BCrypt)
  - Validate credentials against database
  - Remove mock authentication

- [ ] 3.2 Implement role-based authorization
  - Create Roles table
  - Assign roles to users
  - Enforce role checks on endpoints
  - Add authorization policies

- [ ] 3.3 Implement token refresh
  - Add refresh token endpoint
  - Store refresh tokens
  - Validate and rotate tokens

## Phase 4: Public Sector Portal Integration (HIGH PRIORITY)

- [ ] 4.1 Implement dashboard metrics endpoint
  - Aggregate securities portfolio from database
  - Aggregate loan portfolio from database
  - Aggregate banking metrics from database
  - Aggregate grants metrics from database
  - Remove mock data

- [ ] 4.2 Implement securities endpoints
  - GET /securities/tbills (query database)
  - GET /securities/bonds (query database)
  - GET /securities/stocks (query database)
  - POST /securities/orders (create in database)
  - GET /securities/portfolio (query orders)

- [ ] 4.3 Implement loan endpoints
  - GET /loans/applications (query database)
  - POST /loans/applications (create in database)
  - GET /loans/{id} (query with details)
  - POST /loans/{id}/approve (update status)
  - POST /loans/{id}/disburse (create transactions)

- [ ] 4.4 Implement banking endpoints
  - GET /accounts (query database)
  - GET /accounts/{id}/transactions (query database)
  - POST /payments/bulk (validate and process)
  - GET /revenues (query database)

- [ ] 4.5 Implement grants endpoints
  - GET /grants/applications (query database)
  - POST /grants/applications (create in database)
  - POST /grants/{id}/approve (update status)
  - POST /grants/{id}/disburse (create transactions)

## Phase 5: Personal Banking Integration (HIGH PRIORITY)

- [ ] 5.1 Implement customer profile endpoint
  - GET /customer-portal/profile (query database)
  - Update profile (save to database)

- [ ] 5.2 Implement account endpoints
  - GET /accounts (query customer accounts)
  - GET /accounts/{id}/balance (query real balance)
  - GET /accounts/{id}/transactions (query with pagination)

- [ ] 5.3 Implement transfer endpoint
  - POST /transactions/transfer
  - Validate accounts and balance
  - Create debit/credit transactions atomically
  - Update account balances
  - Return transaction reference

- [ ] 5.4 Implement loan endpoints
  - GET /loans (query customer loans)
  - POST /loans/apply (create application)
  - POST /loans/repay (create payment transaction)

- [ ] 5.5 Implement card endpoints
  - GET /cards (query customer cards)
  - POST /cards/request (create request)
  - POST /cards/block (update status)

## Phase 6: SME Banking Integration (MEDIUM PRIORITY)

- [ ] 6.1 Implement business account management
  - Multi-signatory support
  - Account hierarchies
  - Delegation rules

- [ ] 6.2 Implement payroll processing
  - Bulk payment validation
  - Employee account verification
  - Batch processing

- [ ] 6.3 Implement invoice financing
  - Create loan facilities
  - Drawdown processing
  - Repayment tracking

## Phase 7: Corporate Banking Integration (MEDIUM PRIORITY)

- [ ] 7.1 Implement treasury operations
  - FX deal recording
  - Rate management
  - Position tracking

- [ ] 7.2 Implement trade finance
  - LC creation
  - Guarantee issuance
  - Document tracking

- [ ] 7.3 Implement group consolidation
  - Multi-entity queries
  - Consolidated reporting
  - Inter-company eliminations

## Phase 8: Testing & Quality Assurance (HIGH PRIORITY)

- [ ] 8.1 Write integration tests
  - Test all API endpoints with real database
  - Test authentication flows
  - Test authorization rules
  - Test error handling

- [ ] 8.2 Write unit tests
  - Test repository methods
  - Test business logic
  - Test validators
  - Test domain services

- [ ] 8.3 Perform load testing
  - Test concurrent users
  - Test bulk operations
  - Identify bottlenecks
  - Optimize queries

- [ ] 8.4 Security testing
  - Test authentication bypass attempts
  - Test SQL injection prevention
  - Test authorization enforcement
  - Test rate limiting

## Phase 9: Performance Optimization (MEDIUM PRIORITY)

- [ ] 9.1 Implement caching
  - Cache frequently accessed data
  - Implement cache invalidation
  - Use Redis for distributed cache

- [ ] 9.2 Optimize database queries
  - Add missing indexes
  - Optimize slow queries
  - Implement query result caching

- [ ] 9.3 Implement pagination
  - Add pagination to all list endpoints
  - Implement cursor-based pagination
  - Add sorting and filtering

## Phase 10: Monitoring & Observability (MEDIUM PRIORITY)

- [ ] 10.1 Implement structured logging
  - Log all API requests
  - Log database queries
  - Log errors with context

- [ ] 10.2 Implement metrics
  - Track response times
  - Track error rates
  - Track database performance

- [ ] 10.3 Implement alerting
  - Alert on high error rates
  - Alert on slow queries
  - Alert on database issues

## Priority Order for Immediate Production Readiness

1. **Phase 1** - Database Foundation (Without this, nothing works)
2. **Phase 2** - Core Repositories (Data access layer)
3. **Phase 3** - Authentication (Security first)
4. **Phase 4** - Public Sector Integration (Your current focus)
5. **Phase 5** - Personal Banking Integration (Most users)
6. **Phase 8** - Testing (Quality assurance)
7. **Phase 9** - Performance (User experience)
8. **Phase 6, 7, 10** - Can be done in parallel or after launch

## Estimated Timeline

- Phase 1-3: 3-5 days (Foundation)
- Phase 4-5: 5-7 days (Core integrations)
- Phase 6-7: 7-10 days (Advanced features)
- Phase 8-10: 5-7 days (Quality & optimization)

**Total: 20-29 days for complete production-ready system**

## Critical Path to Get Dashboard Working NOW

1. ✅ Fix frontend to use correct API URL (DONE)
2. [ ] Configure database connection (30 minutes)
3. [ ] Run EF migrations (15 minutes)
4. [ ] Seed test data (30 minutes)
5. [ ] Update dashboard endpoint to query database (1 hour)
6. [ ] Test end-to-end (30 minutes)

**Total: 3 hours to get dashboard working with real data**
