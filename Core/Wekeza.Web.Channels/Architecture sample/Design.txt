# Public Sector Portal - Design Document

## Overview

The Public Sector Portal is a specialized web channel for Wekeza Bank to serve government entities (National and County Governments of Kenya). It provides four core modules: Government Securities Trading, Government Lending, Government Banking Services, and Grants & Philanthropic Programs. The portal follows the existing Wekeza.Web.Channels architecture pattern, built with React/TypeScript and integrating with the Wekeza.Core.Api backend.

### Key Design Principles

1. **Separation of Concerns**: Clear separation between trading, lending, banking, and grants modules
2. **Role-Based Access**: Different user roles (Treasury Officer, Credit Officer, Government Finance Officer, CSR Manager, Compliance Officer, Senior Management) with appropriate permissions
3. **Integration-First**: Seamless integration with external systems (CBK, NSE, IFMIS) and internal Wekeza.Core.Api
4. **Audit & Compliance**: Comprehensive audit trails and regulatory compliance built into every transaction
5. **Scalability**: Modular architecture supporting future expansion and high transaction volumes

## Architecture

### High-Level Architecture

```mermaid
graph TB
    subgraph "Frontend - React/TypeScript"
        PSP[Public Sector Portal]
        ST[Securities Trading Module]
        GL[Government Lending Module]
        GB[Government Banking Module]
        GP[Grants & Philanthropy Module]
        DASH[Dashboard & Analytics]
    end
    
    subgraph "Backend - Wekeza.Core.Api"
        API[ASP.NET Core API]
        AUTH[Authentication Service]
        PS_SVC[Public Sector Service]
        SEC_SVC[Securities Service]
        LOAN_SVC[Loan Service]
        ACCT_SVC[Account Service]
        GRANT_SVC[Grant Service]
    end
    
    subgraph "External Systems"
        CBK[Central Bank of Kenya]
        NSE[Nairobi Securities Exchange]
        IFMIS[IFMIS System]
        CRB[Credit Reference Bureau]
    end
    
    PSP --> ST
    PSP --> GL
    PSP --> GB
    PSP --> GP
    PSP --> DASH
    
    ST --> API
    GL --> API
    GB --> API
    GP --> API
    DASH --> API
    
    API --> AUTH
    API --> PS_SVC
    API --> SEC_SVC
    API --> LOAN_SVC
    API --> ACCT_SVC
    API --> GRANT_SVC
    
    SEC_SVC --> CBK
    SEC_SVC --> NSE
    ACCT_SVC --> IFMIS
    LOAN_SVC --> CRB
```

### Frontend Architecture

The portal follows the established Wekeza.Web.Channels pattern:

```
Wekeza.Web.Channels/
└── src/
    └── channels/
        └── public-sector/
            ├── PublicSectorPortal.tsx       # Main routing component
            ├── Layout.tsx                    # Shared layout with navigation
            ├── Login.tsx                     # Authentication page
            ├── pages/
            │   ├── Dashboard.tsx             # Main dashboard
            │   ├── securities/
            │   │   ├── TreasuryBills.tsx
            │   │   ├── Bonds.tsx
            │   │   ├── Stocks.tsx
            │   │   └── Portfolio.tsx
            │   ├── lending/
            │   │   ├── Applications.tsx
            │   │   ├── LoanDetails.tsx
            │   │   ├── Disbursements.tsx
            │   │   └── Portfolio.tsx
            │   ├── banking/
            │   │   ├── Accounts.tsx
            │   │   ├── Payments.tsx
            │   │   ├── Revenues.tsx
            │   │   └── Reports.tsx
            │   └── grants/
            │       ├── Programs.tsx
            │       ├── Applications.tsx
            │       ├── Approvals.tsx
            │       └── Impact.tsx
            └── components/
                ├── SecurityCard.tsx
                ├── LoanCard.tsx
                ├── TransactionTable.tsx
                ├── GrantCard.tsx
                └── ApprovalWorkflow.tsx
```

### Backend API Structure

The backend exposes RESTful endpoints under `/api/public-sector`:

**Base URL**: `/api/public-sector`

**Securities Endpoints**:
- `GET /securities/treasury-bills` - List available T-Bills
- `POST /securities/treasury-bills/order` - Place T-Bill order
- `GET /securities/bonds` - List available bonds
- `POST /securities/bonds/order` - Place bond order
- `GET /securities/stocks` - List NSE stocks
- `POST /securities/stocks/order` - Place stock order
- `GET /securities/portfolio` - View securities portfolio

**Lending Endpoints**:
- `GET /loans/applications` - List loan applications
- `POST /loans/applications/{id}/approve` - Approve loan
- `POST /loans/applications/{id}/reject` - Reject loan
- `POST /loans/{id}/disburse` - Disburse loan
- `GET /loans/portfolio` - View loan portfolio
- `GET /loans/{id}/schedule` - View repayment schedule

**Banking Endpoints**:
- `GET /accounts` - List government accounts
- `GET /accounts/{id}/transactions` - Account transactions
- `POST /payments/bulk` - Process bulk payments
- `GET /revenues` - View revenue collections
- `POST /revenues/reconcile` - Reconcile revenues
- `GET /reports` - Generate reports

**Grants Endpoints**:
- `GET /grants/programs` - List grant programs
- `POST /grants/applications` - Submit grant application
- `GET /grants/applications` - List applications
- `POST /grants/applications/{id}/approve` - Approve grant
- `POST /grants/{id}/disburse` - Disburse grant
- `GET /grants/impact` - View impact reports

## Components and Interfaces

### 1. Authentication & Authorization

**User Roles**:
```typescript
enum UserRole {
  TreasuryOfficer = 'TREASURY_OFFICER',
  CreditOfficer = 'CREDIT_OFFICER',
  GovernmentFinanceOfficer = 'GOVERNMENT_FINANCE_OFFICER',
  CSRManager = 'CSR_MANAGER',
  ComplianceOfficer = 'COMPLIANCE_OFFICER',
  SeniorManagement = 'SENIOR_MANAGEMENT'
}
```

**Permission Matrix**:
- Treasury Officer: Securities trading, portfolio management
- Credit Officer: Loan applications, approvals, disbursements
- Government Finance Officer: Account management, payments, revenues
- CSR Manager: Grant programs, applications, approvals
- Compliance Officer: All read access, audit logs, reports
- Senior Management: Dashboard, analytics, all read access

**Authentication Flow**:
1. User enters credentials on `/public-sector/login`
2. POST to `/api/authentication/login` with credentials
3. Receive JWT token with role claims
4. Store token in localStorage
5. Redirect to dashboard based on role
6. Include token in Authorization header for all API calls

### 2. Securities Trading Module

**Components**:

**TreasuryBills.tsx**:
- Display available T-Bills (91-day, 182-day, 364-day)
- Show maturity dates, rates, minimum investment
- Order placement form (competitive/non-competitive bids)
- Integration with CBK API

**Bonds.tsx**:
- Display available government bonds
- Show coupon rates, maturity dates, face values
- Bond order placement form
- Calculate accrued interest

**Stocks.tsx**:
- Display NSE-listed government stocks
- Real-time price updates (polling every 30 seconds)
- Buy/sell order placement
- Order book display

**Portfolio.tsx**:
- Consolidated view of all securities
- Current valuations
- Maturity calendar
- Performance metrics (yield, returns)

**Data Models**:
```typescript
interface TreasuryBill {
  id: string;
  issueNumber: string;
  tenor: 91 | 182 | 364;
  issueDate: string;
  maturityDate: string;
  rate: number;
  minimumInvestment: number;
  availableAmount: number;
}

interface Bond {
  id: string;
  name: string;
  isin: string;
  couponRate: number;
  faceValue: number;
  issueDate: string;
  maturityDate: string;
  frequency: 'SEMI_ANNUAL' | 'ANNUAL';
  minimumInvestment: number;
}

interface Stock {
  id: string;
  symbol: string;
  name: string;
  currentPrice: number;
  change: number;
  changePercent: number;
  volume: number;
  marketCap: number;
}

interface SecurityOrder {
  securityId: string;
  securityType: 'TBILL' | 'BOND' | 'STOCK';
  orderType: 'BUY' | 'SELL';
  quantity: number;
  price?: number; // For stocks
  bidType?: 'COMPETITIVE' | 'NON_COMPETITIVE'; // For T-Bills
}

interface Portfolio {
  securities: PortfolioSecurity[];
  totalValue: number;
  unrealizedGain: number;
  yieldToMaturity: number;
}

interface PortfolioSecurity {
  securityId: string;
  securityType: 'TBILL' | 'BOND' | 'STOCK';
  name: string;
  quantity: number;
  purchasePrice: number;
  currentPrice: number;
  marketValue: number;
  unrealizedGain: number;
  maturityDate?: string;
}
```

### 3. Government Lending Module

**Components**:

**Applications.tsx**:
- List of loan applications (National/County governments)
- Filter by status (Pending, Under Review, Approved, Rejected)
- Application details view
- Creditworthiness assessment display

**LoanDetails.tsx**:
- Detailed loan information
- Government entity details
- Loan purpose and documentation
- Credit assessment results
- Approval/rejection workflow

**Disbursements.tsx**:
- List of approved loans pending disbursement
- Disbursement form with account details
- Disbursement confirmation
- Audit trail

**Portfolio.tsx**:
- All active government loans
- Exposure by government entity
- Repayment tracking
- Non-performing loans
- Risk metrics

**Data Models**:
```typescript
interface LoanApplication {
  id: string;
  applicationNumber: string;
  governmentEntity: GovernmentEntity;
  loanType: 'DEVELOPMENT' | 'WORKING_CAPITAL' | 'INFRASTRUCTURE';
  requestedAmount: number;
  tenor: number; // months
  purpose: string;
  status: 'PENDING' | 'UNDER_REVIEW' | 'APPROVED' | 'REJECTED' | 'DISBURSED';
  submittedDate: string;
  creditAssessment?: CreditAssessment;
}

interface GovernmentEntity {
  id: string;
  name: string;
  type: 'NATIONAL' | 'COUNTY';
  countyCode?: string;
  contactPerson: string;
  email: string;
  phone: string;
}

interface CreditAssessment {
  sovereignRating?: string;
  revenueStreams: RevenueStream[];
  existingDebt: number;
  debtServiceRatio: number;
  recommendation: 'APPROVE' | 'REJECT' | 'CONDITIONAL';
  comments: string;
  assessedBy: string;
  assessedDate: string;
}

interface RevenueStream {
  source: string;
  annualAmount: number;
  reliability: 'HIGH' | 'MEDIUM' | 'LOW';
}

interface Loan {
  id: string;
  loanNumber: string;
  governmentEntity: GovernmentEntity;
  principalAmount: number;
  interestRate: number;
  tenor: number;
  disbursementDate: string;
  maturityDate: string;
  outstandingBalance: number;
  status: 'ACTIVE' | 'CLOSED' | 'DEFAULT';
  repaymentSchedule: RepaymentSchedule[];
}

interface RepaymentSchedule {
  installmentNumber: number;
  dueDate: string;
  principalAmount: number;
  interestAmount: number;
  totalAmount: number;
  paidAmount: number;
  status: 'PENDING' | 'PAID' | 'OVERDUE';
}
```

### 4. Government Banking Services Module

**Components**:

**Accounts.tsx**:
- List of government accounts
- Account balances and details
- Account statements
- Transaction history

**Payments.tsx**:
- Bulk payment processing
- Payment file upload (CSV/Excel)
- Payment validation and preview
- Payment execution
- IFMIS integration

**Revenues.tsx**:
- Revenue collection tracking
- Tax payments, fees, licenses
- Reconciliation interface
- Collection reports

**Reports.tsx**:
- Financial reports generation
- Custom report builder
- Export functionality (PDF, Excel)
- Scheduled reports

**Data Models**:
```typescript
interface GovernmentAccount {
  id: string;
  accountNumber: string;
  accountName: string;
  governmentEntity: GovernmentEntity;
  accountType: 'CURRENT' | 'SAVINGS' | 'REVENUE_COLLECTION';
  balance: number;
  currency: string;
  status: 'ACTIVE' | 'DORMANT' | 'CLOSED';
}

interface BulkPayment {
  id: string;
  batchNumber: string;
  fromAccountId: string;
  paymentType: 'SALARY' | 'SUPPLIER' | 'PENSION' | 'OTHER';
  totalAmount: number;
  totalCount: number;
  uploadedDate: string;
  processedDate?: string;
  status: 'UPLOADED' | 'VALIDATED' | 'PROCESSING' | 'COMPLETED' | 'FAILED';
  payments: Payment[];
}

interface Payment {
  beneficiaryName: string;
  beneficiaryAccount: string;
  beneficiaryBank: string;
  amount: number;
  narration: string;
  status: 'PENDING' | 'SUCCESS' | 'FAILED';
  errorMessage?: string;
}

interface RevenueCollection {
  id: string;
  collectionDate: string;
  revenueType: 'TAX' | 'FEE' | 'LICENSE' | 'FINE' | 'OTHER';
  amount: number;
  payerName: string;
  payerReference: string;
  accountId: string;
  reconciled: boolean;
}
```

### 5. Grants & Philanthropy Module

**Components**:

**Programs.tsx**:
- List of available grant programs
- Program details and eligibility
- Application guidelines

**Applications.tsx**:
- Grant application form
- Application submission
- Application tracking
- Document upload

**Approvals.tsx**:
- Pending grant applications
- Application review interface
- Approval/rejection workflow
- Two-signatory approval process

**Impact.tsx**:
- Grant utilization reports
- Impact metrics and KPIs
- Beneficiary stories
- Compliance monitoring

**Data Models**:
```typescript
interface GrantProgram {
  id: string;
  name: string;
  description: string;
  category: 'EDUCATION' | 'HEALTH' | 'INFRASTRUCTURE' | 'ENVIRONMENT' | 'OTHER';
  maxAmount: number;
  eligibilityCriteria: string[];
  applicationDeadline: string;
  status: 'OPEN' | 'CLOSED';
}

interface GrantApplication {
  id: string;
  applicationNumber: string;
  programId: string;
  applicantName: string;
  applicantType: 'NGO' | 'COMMUNITY_GROUP' | 'INSTITUTION' | 'INDIVIDUAL';
  requestedAmount: number;
  projectTitle: string;
  projectDescription: string;
  expectedImpact: string;
  submittedDate: string;
  status: 'SUBMITTED' | 'UNDER_REVIEW' | 'APPROVED' | 'REJECTED' | 'DISBURSED';
  approvals: Approval[];
}

interface Approval {
  approverName: string;
  approverRole: string;
  decision: 'APPROVED' | 'REJECTED';
  comments: string;
  approvedDate: string;
}

interface Grant {
  id: string;
  grantNumber: string;
  applicationId: string;
  approvedAmount: number;
  disbursedAmount: number;
  disbursementDate: string;
  utilizationReports: UtilizationReport[];
  complianceStatus: 'COMPLIANT' | 'NON_COMPLIANT' | 'PENDING_REPORT';
}

interface UtilizationReport {
  reportingPeriod: string;
  amountUtilized: number;
  activities: string;
  outcomes: string;
  challenges: string;
  submittedDate: string;
}
```

### 6. Dashboard & Analytics

**Components**:

**Dashboard.tsx**:
- Key metrics cards (securities value, loan portfolio, account balances, grants)
- Charts and visualizations (Recharts library)
- Recent activities feed
- Quick actions based on role

**Metrics**:
- Government securities portfolio value
- Government loan portfolio value and NPL ratio
- Government account balances
- Grant disbursements
- Revenue from government banking
- Risk metrics (concentration, exposure limits)

**Visualizations**:
- Securities portfolio composition (pie chart)
- Loan portfolio by government entity (bar chart)
- Revenue trends (line chart)
- Grant impact metrics (area chart)

**Data Models**:
```typescript
interface DashboardMetrics {
  securitiesPortfolio: {
    totalValue: number;
    tbillsValue: number;
    bondsValue: number;
    stocksValue: number;
    yieldToMaturity: number;
  };
  loanPortfolio: {
    totalOutstanding: number;
    nationalGovernment: number;
    countyGovernments: number;
    nplRatio: number;
    exposureUtilization: number;
  };
  banking: {
    totalAccounts: number;
    totalBalance: number;
    monthlyTransactions: number;
    revenueCollected: number;
  };
  grants: {
    totalDisbursed: number;
    activeGrants: number;
    beneficiaries: number;
    complianceRate: number;
  };
}
```

## Data Models

### Core Entities

All data models follow TypeScript interfaces for type safety. Key entities include:

1. **Securities**: TreasuryBill, Bond, Stock, SecurityOrder, Portfolio
2. **Lending**: LoanApplication, Loan, RepaymentSchedule, CreditAssessment
3. **Banking**: GovernmentAccount, BulkPayment, Payment, RevenueCollection
4. **Grants**: GrantProgram, GrantApplication, Grant, UtilizationReport
5. **Common**: GovernmentEntity, User, AuditLog

### API Response Format

All API responses follow a standard format:

```typescript
interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: {
    code: string;
    message: string;
    details?: any;
  };
  metadata?: {
    timestamp: string;
    requestId: string;
  };
}
```

### Pagination

List endpoints support pagination:

```typescript
interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
```

## Error Handling

### Frontend Error Handling

**Error Boundaries**:
- Wrap each module in React Error Boundary
- Display user-friendly error messages
- Log errors to console in development
- Send errors to monitoring service in production

**API Error Handling**:
```typescript
async function handleApiCall<T>(apiCall: Promise<ApiResponse<T>>): Promise<T> {
  try {
    const response = await apiCall;
    if (response.success && response.data) {
      return response.data;
    }
    throw new Error(response.error?.message || 'Unknown error');
  } catch (error) {
    if (error.response?.status === 401) {
      // Redirect to login
      window.location.href = '/public-sector/login';
    }
    throw error;
  }
}
```

**User Feedback**:
- Toast notifications for success/error messages
- Loading states for async operations
- Validation errors displayed inline on forms
- Confirmation dialogs for critical actions

### Backend Error Handling

**Error Types**:
- `ValidationError`: Invalid input data (400)
- `AuthenticationError`: Invalid credentials (401)
- `AuthorizationError`: Insufficient permissions (403)
- `NotFoundError`: Resource not found (404)
- `BusinessRuleError`: Business logic violation (422)
- `IntegrationError`: External system failure (502)
- `ServerError`: Internal server error (500)

**Error Responses**:
```json
{
  "success": false,
  "error": {
    "code": "INSUFFICIENT_FUNDS",
    "message": "Account balance insufficient for transaction",
    "details": {
      "accountId": "ACC123",
      "requiredAmount": 100000,
      "availableBalance": 50000
    }
  },
  "metadata": {
    "timestamp": "2026-02-12T10:30:00Z",
    "requestId": "req_abc123"
  }
}
```

## Testing Strategy

### Unit Testing

**Frontend Unit Tests** (Vitest + React Testing Library):
- Component rendering tests
- User interaction tests
- Form validation tests
- Utility function tests
- Mock API responses

**Backend Unit Tests** (xUnit):
- Service layer tests
- Business logic tests
- Validation tests
- Data transformation tests

**Test Coverage Target**: 80% code coverage

### Integration Testing

**Frontend Integration Tests**:
- Multi-component workflows
- API integration tests with mock server
- Routing tests
- Authentication flow tests

**Backend Integration Tests**:
- API endpoint tests
- Database integration tests
- External system integration tests (mocked)

### End-to-End Testing

**E2E Test Scenarios** (Playwright/Cypress):
1. Complete securities trading flow (login → view T-Bills → place order → view portfolio)
2. Loan application approval flow (login → view application → assess → approve → disburse)
3. Bulk payment processing (login → upload file → validate → execute)
4. Grant application flow (login → apply → track status)

### Property-Based Testing

Property-based tests will be defined after completing the prework analysis of acceptance criteria. These tests will validate universal properties across all inputs using a property-based testing library.

## Security Considerations

### Authentication & Authorization

- JWT-based authentication with role claims
- Token expiration and refresh mechanism
- Multi-factor authentication for sensitive operations
- Role-based access control (RBAC) enforced on both frontend and backend

### Data Security

- All API calls over HTTPS
- Sensitive data encrypted at rest
- PII data masked in logs
- Secure storage of credentials (never in code)

### Audit Logging

Every transaction logged with:
- User ID and role
- Action performed
- Timestamp
- IP address
- Request/response data
- Success/failure status

### Compliance

- CBK security standards compliance
- Data protection (Kenya Data Protection Act)
- AML/KYC checks for all government entities
- Regular security audits

## Performance Considerations

### Frontend Performance

- Code splitting by module (lazy loading)
- Memoization of expensive computations
- Virtual scrolling for large lists
- Debounced search inputs
- Optimistic UI updates

### Backend Performance

- Database query optimization with indexes
- Caching frequently accessed data (Redis)
- Async processing for bulk operations
- Connection pooling for external systems
- Rate limiting to prevent abuse

### Performance Targets

- Page load time: < 2 seconds
- API response time: < 500ms (p95)
- Transaction processing: < 3 seconds
- Report generation: < 10 seconds
- System availability: 99.9%

## Deployment Architecture

### Frontend Deployment

- Build: `npm run build` (Vite)
- Output: Static files in `dist/`
- Hosting: CDN (Netlify/Vercel) or Azure Static Web Apps
- Environment variables: `VITE_API_URL`

### Backend Deployment

- Platform: Azure App Service or Kubernetes
- Database: SQL Server (Azure SQL)
- Cache: Redis (Azure Cache for Redis)
- File Storage: Azure Blob Storage
- Monitoring: Application Insights

### CI/CD Pipeline

1. Code commit triggers build
2. Run tests (unit + integration)
3. Build artifacts
4. Deploy to staging
5. Run E2E tests
6. Manual approval
7. Deploy to production
8. Health checks

## Integration Points

### External System Integrations

**Central Bank of Kenya (CBK)**:
- Protocol: REST API / SOAP
- Authentication: API Key + Certificate
- Operations: T-Bill/Bond listings, order placement, settlement
- Error Handling: Retry with exponential backoff, fallback to manual processing

**Nairobi Securities Exchange (NSE)**:
- Protocol: REST API
- Authentication: OAuth 2.0
- Operations: Stock listings, real-time prices, order placement
- Data Refresh: Polling every 30 seconds during trading hours

**IFMIS (Integrated Financial Management System)**:
- Protocol: REST API / File Transfer
- Authentication: API Key
- Operations: Payment file submission, status tracking, reconciliation
- Batch Processing: Scheduled jobs for bulk operations

**Credit Reference Bureau (CRB)**:
- Protocol: REST API
- Authentication: API Key
- Operations: Credit checks for government entities
- Caching: Cache results for 24 hours

### Internal System Integrations

**Wekeza.Core.Api**:
- All business logic resides in the backend API
- Frontend makes RESTful calls to backend
- WebSocket connection for real-time updates (optional)

**Treasury Management System**:
- Securities valuation
- Portfolio management
- Risk calculations

**Risk Management System**:
- Exposure monitoring
- Limit enforcement
- Risk reporting

**Compliance System**:
- AML/KYC checks
- Regulatory reporting
- Audit log storage

## Monitoring & Observability

### Application Monitoring

- Application Performance Monitoring (APM): Application Insights
- Error tracking: Sentry or similar
- User analytics: Google Analytics or Mixpanel
- Uptime monitoring: Pingdom or UptimeRobot

### Logging

- Structured logging (JSON format)
- Log levels: Debug, Info, Warning, Error, Critical
- Centralized log aggregation: Azure Log Analytics
- Log retention: 90 days

### Metrics

- Request rate and latency
- Error rate
- Database query performance
- External API call success rate
- User session duration
- Feature usage statistics

### Alerts

- API error rate > 5%
- Response time > 2 seconds (p95)
- Failed external integrations
- Security incidents
- System downtime

## Future Enhancements

### Phase 2 Features

1. **Mobile App**: Native iOS/Android apps for government officers
2. **Real-time Notifications**: WebSocket-based push notifications
3. **Advanced Analytics**: Machine learning for credit risk assessment
4. **Blockchain Integration**: Immutable audit trail using blockchain
5. **API Gateway**: Centralized API management and rate limiting

### Scalability Improvements

1. **Microservices**: Break down monolithic backend into microservices
2. **Event-Driven Architecture**: Use message queues for async processing
3. **Multi-Region Deployment**: Deploy to multiple Azure regions for redundancy
4. **Auto-Scaling**: Automatic scaling based on load

### User Experience Enhancements

1. **Dark Mode**: Theme toggle for user preference
2. **Customizable Dashboards**: Drag-and-drop dashboard widgets
3. **Advanced Search**: Full-text search across all modules
4. **Bulk Operations**: Batch approval/rejection of applications
5. **Export Templates**: Customizable export formats

---

**Document Version**: 1.0  
**Date**: February 12, 2026  
**Status**: Draft  
**Author**: Kiro AI Assistant

## Correctness Properties

A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.

### Securities Trading Properties

**Property 1: Order Submission Completeness**  
*For any* valid security order (T-Bill, Bond, or Stock), submitting the order should result in an API call containing all required fields (security ID, type, quantity, order type) and the order should be recorded in the system.  
**Validates: Requirements 1.4, 1.5, 1.6**

**Property 2: Interest Calculation Accuracy**  
*For any* security with a defined interest rate and maturity date, the calculated interest payment should equal the principal amount multiplied by the rate, prorated for the time period.  
**Validates: Requirements 1.8**

**Property 3: Portfolio Valuation Consistency**  
*For any* securities portfolio, the total portfolio value should equal the sum of (quantity × current price) for all securities in the portfolio.  
**Validates: Requirements 6.1**

### Government Lending Properties

**Property 4: Creditworthiness Calculation**  
*For any* government entity with revenue streams and existing debt, the debt service ratio should equal (annual debt payments / annual revenue), and this ratio should be used consistently in credit assessments.  
**Validates: Requirements 2.3**

**Property 5: Loan Status Transitions**  
*For any* loan application, status transitions should follow the valid sequence: PENDING → UNDER_REVIEW → (APPROVED | REJECTED) → DISBURSED, and no invalid transitions should be allowed.  
**Validates: Requirements 2.4, 2.5**

**Property 6: Repayment Schedule Accuracy**  
*For any* loan with principal amount, interest rate, and tenor, the generated repayment schedule should have installments that sum to the total loan amount plus interest, and each installment should have the correct principal and interest components.  
**Validates: Requirements 2.6**

**Property 7: Lending Limit Enforcement**  
*For any* loan application, if the total exposure to that government entity (existing loans + new loan) exceeds the defined limit (10% of bank capital for counties, 25% for national government), the application should be flagged or rejected.  
**Validates: Requirements 2.9**

### Government Banking Properties

**Property 8: Payment Processing Completeness**  
*For any* valid payment request, processing the payment should create a transaction record with all required fields (beneficiary, amount, narration, status) and update the source account balance by subtracting the payment amount.  
**Validates: Requirements 3.2**

**Property 9: Revenue Collection Recording**  
*For any* revenue collection, recording it should create a revenue record with all required fields and increase the destination account balance by the collection amount.  
**Validates: Requirements 3.3**

**Property 10: Account Reconciliation Balance**  
*For any* account, the reconciled balance should equal the opening balance plus all credits minus all debits for the reconciliation period.  
**Validates: Requirements 3.5**

**Property 11: Multi-Currency Consistency**  
*For any* transaction in a non-base currency, the system should store both the original currency amount and the converted base currency amount, and conversions should use consistent exchange rates within a transaction batch.  
**Validates: Requirements 3.6**

**Property 12: Audit Trail Completeness**  
*For any* transaction (payment, revenue, transfer, or other), an audit log entry should be created containing user ID, timestamp, action type, transaction details, and IP address.  
**Validates: Requirements 3.7, 5.5**

### Grants & Philanthropy Properties

**Property 13: Grant Application Submission**  
*For any* valid grant application, submitting it should create an application record with status SUBMITTED, all required fields populated, and a unique application number generated.  
**Validates: Requirements 4.2, 4.7**

**Property 14: Grant Disbursement Recording**  
*For any* approved grant, disbursing it should create a grant record with the disbursement amount, update the application status to DISBURSED, and create a transaction record.  
**Validates: Requirements 4.4, 4.8**

**Property 15: Grant Compliance Calculation**  
*For any* grant with utilization reports, if the total amount utilized (sum of all utilization reports) equals the disbursed amount and all reports are submitted on time, the compliance status should be COMPLIANT; otherwise, it should be NON_COMPLIANT or PENDING_REPORT.  
**Validates: Requirements 4.5, 4.9**

### Regulatory Compliance Properties

**Property 16: CBK Regulation Enforcement**  
*For any* securities transaction, if it violates CBK regulations (e.g., minimum investment amount, trading hours, maximum transaction size), the transaction should be rejected with a specific error code.  
**Validates: Requirements 5.1**

**Property 17: PFMA Requirement Enforcement**  
*For any* government transaction, if it violates Public Finance Management Act requirements (e.g., missing approvals, exceeds budget allocation), the transaction should be rejected or flagged for review.  
**Validates: Requirements 5.2**

**Property 18: AML/KYC Data Persistence**  
*For any* government entity, storing AML/KYC data should persist all required fields (entity name, type, verification status, documents) and retrieving it should return the same data.  
**Validates: Requirements 5.4**

### Dashboard & Analytics Properties

**Property 19: Loan Portfolio Aggregation**  
*For any* set of active loans, the total outstanding balance should equal the sum of outstanding balances for all loans, and the NPL ratio should equal (sum of non-performing loan balances / total outstanding balance).  
**Validates: Requirements 6.2**

**Property 20: Revenue Calculation Accuracy**  
*For any* set of transactions within a period, the total revenue should equal the sum of all fee income, interest income, and other revenue-generating transactions for that period.  
**Validates: Requirements 6.5**

**Property 21: Risk Exposure Calculation**  
*For any* portfolio, the exposure to each government entity should equal the sum of (outstanding loans + securities holdings + pending transactions) for that entity, and the exposure utilization percentage should equal (total exposure / exposure limit).  
**Validates: Requirements 6.6**

**Property 22: Data Export Completeness**  
*For any* data set being exported, the exported file should contain all records from the data set with all fields preserved, and re-importing the file should produce equivalent data.  
**Validates: Requirements 6.8**

### Edge Cases and Validation Properties

**Property 23: Empty Portfolio Handling**  
*For any* user with no securities, the portfolio view should display zero values for all metrics and not throw errors.  
**Edge Case for: Requirements 1.7**

**Property 24: Zero-Amount Transaction Rejection**  
*For any* payment or transfer request with amount ≤ 0, the system should reject the transaction with a validation error.  
**Edge Case for: Requirements 3.2**

**Property 25: Expired Grant Program Handling**  
*For any* grant program with application deadline in the past, attempting to submit an application should be rejected with an appropriate error message.  
**Edge Case for: Requirements 4.2**

### Integration Properties

**Property 26: External API Failure Handling**  
*For any* external API call (CBK, NSE, IFMIS) that fails or times out, the system should log the error, return a user-friendly error message, and not leave the transaction in an inconsistent state.  
**Validates: Requirements 1.9, 1.10, 3.8**

**Property 27: Retry Logic Consistency**  
*For any* failed external API call that is retryable, the system should retry with exponential backoff (1s, 2s, 4s) up to 3 times before marking the operation as failed.  
**Validates: Requirements 1.9, 1.10, 3.8**

---

These properties provide comprehensive coverage of the system's correctness requirements and will be implemented as property-based tests during the implementation phase. Each property is designed to be testable across a wide range of inputs, ensuring robust validation of the system's behavior.
