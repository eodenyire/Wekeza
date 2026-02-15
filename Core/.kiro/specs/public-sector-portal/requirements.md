# Public Sector Portal - Requirements

## Overview

The Public Sector Portal is a specialized banking channel for Wekeza Bank to interact with government entities (National and County Governments of Kenya), facilitate government securities trading, provide government lending, and manage philanthropic initiatives.

## User Stories

### 1. Government Securities Trading

**As a** bank treasury officer  
**I want to** trade government securities (Treasury Bills, Bonds, Stocks)  
**So that** I can invest bank funds and manage liquidity

**Acceptance Criteria:**
- 1.1 User can view available Treasury Bills with maturity dates and rates
- 1.2 User can view available Government Bonds with coupon rates and tenors
- 1.3 User can view NSE-listed government stocks
- 1.4 User can place buy orders for T-Bills
- 1.5 User can place buy orders for Bonds
- 1.6 User can place buy/sell orders for stocks
- 1.7 User can view portfolio of government securities
- 1.8 User can track maturity dates and interest payments
- 1.9 System integrates with Central Bank of Kenya (CBK) for T-Bills/Bonds
- 1.10 System integrates with Nairobi Securities Exchange (NSE) for stocks

### 2. Government Lending

**As a** credit officer  
**I want to** provide loans to government entities  
**So that** governments can finance development projects

**Acceptance Criteria:**
- 2.1 User can view loan applications from National Government
- 2.2 User can view loan applications from County Governments
- 2.3 User can assess government creditworthiness
- 2.4 User can approve/reject government loan applications
- 2.5 User can disburse approved loans
- 2.6 User can track loan repayments from governments
- 2.7 User can view government loan portfolio
- 2.8 System tracks sovereign risk ratings
- 2.9 System enforces lending limits per government entity
- 2.10 System generates government loan reports

### 3. Government Banking Services

**As a** government finance officer  
**I want to** manage government accounts and transactions  
**So that** I can handle government finances efficiently

**Acceptance Criteria:**
- 3.1 User can view government account balances
- 3.2 User can process government payments (salaries, suppliers)
- 3.3 User can receive government revenues (taxes, fees)
- 3.4 User can generate government financial reports
- 3.5 User can reconcile government accounts
- 3.6 System supports multi-currency for international transactions
- 3.7 System provides audit trail for all government transactions
- 3.8 System integrates with IFMIS (Integrated Financial Management System)

### 4. Grants & Philanthropic Programs

**As a** CSR manager  
**I want to** manage grants and philanthropic initiatives  
**So that** the bank can support community development

**Acceptance Criteria:**
- 4.1 User can view available grant programs
- 4.2 User can submit grant applications
- 4.3 User can track grant application status
- 4.4 User can receive approved grants
- 4.5 User can report on grant utilization
- 4.6 User can view philanthropic initiatives
- 4.7 User can apply for philanthropic support
- 4.8 System tracks grant disbursements
- 4.9 System monitors grant compliance
- 4.10 System generates impact reports

### 5. Regulatory Compliance

**As a** compliance officer  
**I want to** ensure all government transactions comply with regulations  
**So that** the bank meets regulatory requirements

**Acceptance Criteria:**
- 5.1 System enforces CBK regulations for government securities
- 5.2 System enforces Public Finance Management Act requirements
- 5.3 System generates regulatory reports (CBK, Treasury)
- 5.4 System tracks AML/KYC for government entities
- 5.5 System maintains audit logs for all transactions
- 5.6 System supports regulatory inspections

### 6. Dashboard & Analytics

**As a** senior management  
**I want to** view analytics on government banking activities  
**So that** I can make informed strategic decisions

**Acceptance Criteria:**
- 6.1 Dashboard shows government securities portfolio value
- 6.2 Dashboard shows government loan portfolio
- 6.3 Dashboard shows government account balances
- 6.4 Dashboard shows grant disbursements
- 6.5 Dashboard shows revenue from government banking
- 6.6 Dashboard shows risk metrics for government exposure
- 6.7 User can generate custom reports
- 6.8 User can export data for analysis

## Functional Requirements

### FR1: Government Securities Trading Module

**FR1.1 Treasury Bills Trading**
- View primary market T-Bills (91-day, 182-day, 364-day)
- View secondary market T-Bills
- Place competitive bids
- Place non-competitive bids
- Track T-Bill portfolio
- Calculate yields and returns

**FR1.2 Government Bonds Trading**
- View available bonds (Infrastructure Bonds, IFB, etc.)
- View bond details (coupon rate, maturity, face value)
- Place bond orders
- Track bond portfolio
- Calculate accrued interest
- Manage bond redemptions

**FR1.3 Stock Trading**
- View NSE-listed stocks
- View real-time stock prices
- Place buy/sell orders
- Track stock portfolio
- View dividend payments
- Generate capital gains reports

### FR2: Government Lending Module

**FR2.1 Loan Origination**
- Receive loan applications from governments
- Assess creditworthiness (sovereign ratings, revenue streams)
- Perform due diligence
- Approve/reject applications
- Generate loan agreements
- Disburse loans

**FR2.2 Loan Servicing**
- Track loan repayments
- Calculate interest
- Manage loan schedules
- Handle prepayments
- Manage defaults
- Generate loan statements

**FR2.3 Portfolio Management**
- View all government loans
- Track exposure by government entity
- Monitor risk concentration
- Generate portfolio reports
- Track non-performing loans

### FR3: Government Banking Services

**FR3.1 Account Management**
- Open government accounts
- Manage account mandates
- Track account balances
- Generate account statements
- Handle account closures

**FR3.2 Payment Processing**
- Process salary payments
- Process supplier payments
- Process pension payments
- Handle bulk payments
- Integrate with IFMIS

**FR3.3 Revenue Collection**
- Receive tax payments
- Receive fee payments
- Receive license payments
- Reconcile collections
- Generate collection reports

### FR4: Grants & Philanthropy Module

**FR4.1 Grant Management**
- Define grant programs
- Receive grant applications
- Evaluate applications
- Approve/reject grants
- Disburse grants
- Track grant utilization
- Monitor compliance

**FR4.2 Philanthropic Initiatives**
- Define CSR programs
- Receive support applications
- Evaluate applications
- Approve support
- Disburse funds
- Track impact
- Generate impact reports

### FR5: Integration Requirements

**FR5.1 External Systems**
- Central Bank of Kenya (CBK) - T-Bills/Bonds trading
- Nairobi Securities Exchange (NSE) - Stock trading
- IFMIS - Government financial management
- KRA - Tax information
- CRB - Credit reference
- SWIFT - International payments

**FR5.2 Internal Systems**
- Core Banking System (Wekeza.Core.Api)
- Treasury Management System
- Risk Management System
- Compliance System
- Reporting System

## Non-Functional Requirements

### NFR1: Security
- Multi-factor authentication for all users
- Role-based access control
- Encryption for all data in transit and at rest
- Audit logging for all transactions
- Compliance with CBK security standards

### NFR2: Performance
- Transaction processing < 3 seconds
- Report generation < 10 seconds
- System availability 99.9%
- Support 1000+ concurrent users

### NFR3: Compliance
- CBK regulations compliance
- Public Finance Management Act compliance
- AML/KYC compliance
- Data protection compliance (Kenya Data Protection Act)

### NFR4: Usability
- Intuitive user interface
- Mobile responsive design
- Accessibility compliance (WCAG 2.1)
- Multi-language support (English, Swahili)

### NFR5: Scalability
- Support growth in government entities
- Support growth in transaction volumes
- Horizontal scaling capability
- Cloud-ready architecture

## User Roles

### 1. Treasury Officer
- Trade government securities
- Manage securities portfolio
- Generate trading reports

### 2. Credit Officer
- Review government loan applications
- Assess creditworthiness
- Approve/reject loans
- Monitor loan portfolio

### 3. Government Finance Officer
- Manage government accounts
- Process payments
- View reports
- Reconcile accounts

### 4. CSR Manager
- Manage grant programs
- Review grant applications
- Approve grants
- Track impact

### 5. Compliance Officer
- Monitor regulatory compliance
- Generate compliance reports
- Conduct audits
- Manage risk

### 6. Senior Management
- View dashboards
- Generate strategic reports
- Make policy decisions
- Monitor performance

## Business Rules

### BR1: Government Securities Trading
- Minimum T-Bill investment: KES 100,000
- Minimum Bond investment: KES 50,000
- Maximum single transaction: KES 1 Billion
- Trading hours: 9:00 AM - 3:00 PM EAT

### BR2: Government Lending
- Maximum exposure per county: 10% of bank capital
- Maximum exposure to national government: 25% of bank capital
- Minimum loan amount: KES 10 Million
- Maximum loan tenor: 30 years
- Interest rate: Based on sovereign risk + margin

### BR3: Grants & Philanthropy
- Maximum grant per application: KES 5 Million
- Grant approval requires 2 signatories
- Grant utilization reporting: Quarterly
- Philanthropic budget: 2% of annual profit

## Success Metrics

### KPIs
- Government securities portfolio value
- Government loan portfolio value
- Number of government accounts
- Transaction volume
- Revenue from government banking
- Grant disbursement amount
- System uptime
- User satisfaction score

### Targets (Year 1)
- Government securities portfolio: KES 5 Billion
- Government loans: KES 10 Billion
- Government accounts: 50+
- Grants disbursed: KES 100 Million
- System uptime: 99.9%

## Constraints

### Technical Constraints
- Must integrate with CBK systems
- Must integrate with NSE systems
- Must integrate with IFMIS
- Must use existing Wekeza.Core.Api backend

### Business Constraints
- Must comply with CBK regulations
- Must comply with Public Finance Management Act
- Must maintain capital adequacy ratios
- Must manage concentration risk

### Timeline Constraints
- Phase 1 (Securities Trading): 3 months
- Phase 2 (Government Lending): 3 months
- Phase 3 (Banking Services): 2 months
- Phase 4 (Grants & Philanthropy): 2 months

## Dependencies

### External Dependencies
- CBK API access
- NSE API access
- IFMIS integration
- SWIFT connectivity

### Internal Dependencies
- Wekeza.Core.Api (Backend)
- Treasury module
- Loan module
- Compliance module
- Reporting module

## Risks & Mitigation

### Risk 1: Regulatory Changes
- **Impact**: High
- **Probability**: Medium
- **Mitigation**: Regular regulatory monitoring, flexible architecture

### Risk 2: Integration Failures
- **Impact**: High
- **Probability**: Medium
- **Mitigation**: Robust error handling, fallback mechanisms, thorough testing

### Risk 3: Concentration Risk
- **Impact**: High
- **Probability**: Low
- **Mitigation**: Strict exposure limits, diversification, risk monitoring

### Risk 4: Cybersecurity Threats
- **Impact**: Critical
- **Probability**: Medium
- **Mitigation**: Multi-layer security, regular audits, incident response plan

## Glossary

- **T-Bill**: Treasury Bill - Short-term government security
- **Bond**: Long-term government debt security
- **CBK**: Central Bank of Kenya
- **NSE**: Nairobi Securities Exchange
- **IFMIS**: Integrated Financial Management Information System
- **KRA**: Kenya Revenue Authority
- **CRB**: Credit Reference Bureau
- **CSR**: Corporate Social Responsibility
- **AML**: Anti-Money Laundering
- **KYC**: Know Your Customer
- **WCAG**: Web Content Accessibility Guidelines

## Appendices

### Appendix A: API Endpoints Required

**Government Securities:**
- GET /api/public-sector/securities/treasury-bills
- POST /api/public-sector/securities/treasury-bills/order
- GET /api/public-sector/securities/bonds
- POST /api/public-sector/securities/bonds/order
- GET /api/public-sector/securities/stocks
- POST /api/public-sector/securities/stocks/order
- GET /api/public-sector/securities/portfolio

**Government Lending:**
- GET /api/public-sector/loans/applications
- POST /api/public-sector/loans/approve
- POST /api/public-sector/loans/disburse
- GET /api/public-sector/loans/portfolio
- GET /api/public-sector/loans/{id}/schedule

**Government Banking:**
- GET /api/public-sector/accounts
- POST /api/public-sector/payments/bulk
- GET /api/public-sector/revenues
- GET /api/public-sector/reports

**Grants & Philanthropy:**
- GET /api/public-sector/grants/programs
- POST /api/public-sector/grants/apply
- GET /api/public-sector/grants/applications
- POST /api/public-sector/grants/approve
- POST /api/public-sector/grants/disburse

### Appendix B: Regulatory References

- Central Bank of Kenya Act
- Banking Act
- Public Finance Management Act
- Kenya Data Protection Act
- Capital Markets Act
- Anti-Money Laundering Regulations

---

**Document Version**: 1.0  
**Date**: February 12, 2026  
**Status**: Draft  
**Owner**: Wekeza Bank - Public Sector Banking Division
