# Wekeza Bank - Web Channels Architecture

## Overview

Four modern web applications connecting to Wekeza.Core.Api backend:

1. **Personal Banking Portal** - Retail customer self-service
2. **Corporate Banking Portal** - Corporate treasury and operations
3. **SME Banking Portal** - Small business banking
4. **Public Website** - Marketing, onboarding, information

## Technology Stack

### Frontend
- **Framework**: React 18 with TypeScript
- **Build Tool**: Vite
- **Styling**: Tailwind CSS + shadcn/ui components
- **State Management**: Zustand + React Query
- **Forms**: React Hook Form + Zod validation
- **Routing**: React Router v6
- **HTTP Client**: Axios with interceptors
- **Charts**: Recharts
- **Icons**: Lucide React

### Backend Integration
- **API Base**: http://localhost:5000/api
- **Authentication**: JWT Bearer tokens
- **API Documentation**: Swagger at http://localhost:5000/swagger

## Project Structure

```
Wekeza.Web.Channels/
├── personal-banking/          # Personal Banking Portal
│   ├── src/
│   │   ├── components/        # Reusable UI components
│   │   ├── pages/            # Page components
│   │   ├── services/         # API service layer
│   │   ├── stores/           # State management
│   │   ├── hooks/            # Custom React hooks
│   │   ├── utils/            # Utility functions
│   │   └── types/            # TypeScript types
│   ├── public/
│   ├── package.json
│   └── vite.config.ts
│
├── corporate-banking/         # Corporate Banking Portal
│   └── (same structure)
│
├── sme-banking/              # SME Banking Portal
│   └── (same structure)
│
└── public-website/           # Public Marketing Site
    └── (same structure)
```

## Features by Portal

### 1. Personal Banking Portal

**Target Users**: Individual retail customers

**Key Features**:
- Dashboard with account summary
- Account management (view balances, transactions)
- Fund transfers (own accounts, other banks)
- Bill payments
- Airtime purchase
- Loan applications and management
- Card management (request, block, virtual cards)
- Statement downloads
- Profile management
- Digital channel enrollment (Mobile, Internet, USSD)

**API Endpoints Used**:
- `/api/authentication/login`
- `/api/customer-portal/*`
- `/api/accounts/*`
- `/api/transactions/*`
- `/api/loans/*`
- `/api/cards/*`

### 2. Corporate Banking Portal

**Target Users**: Corporate treasurers, finance managers

**Key Features**:
- Multi-account dashboard
- Bulk payments and payroll
- Trade finance (Letters of Credit, Bank Guarantees)
- Treasury operations (FX deals, money market)
- Cash management
- Multi-level approval workflows
- Detailed reporting and analytics
- User management (sub-users with permissions)
- API integration for ERP systems

**API Endpoints Used**:
- `/api/authentication/login`
- `/api/accounts/*`
- `/api/payments/*`
- `/api/tradefinance/*`
- `/api/treasury/*`
- `/api/workflows/*`
- `/api/reporting/*`

### 3. SME Banking Portal

**Target Users**: Small and medium business owners

**Key Features**:
- Business account dashboard
- Invoice management
- Payment collections
- Business loans
- Merchant services
- Simple accounting integration
- Employee salary payments
- Business analytics
- Tax payment services

**API Endpoints Used**:
- `/api/authentication/login`
- `/api/customer-portal/*`
- `/api/accounts/*`
- `/api/loans/*`
- `/api/payments/*`
- `/api/reporting/*`

### 4. Public Website

**Target Users**: Prospective customers, general public

**Key Features**:
- Homepage with bank information
- Product catalog (accounts, loans, cards)
- Branch locator
- Contact us
- About us / Corporate info
- News and updates
- Self-onboarding wizard
- Loan calculator
- Interest rate information
- Download mobile app links

**API Endpoints Used**:
- `/api/customer-portal/onboard/*`
- `/api/products/*` (read-only)
- Public information endpoints

## Shared Components Library

Create a shared component library for consistency:

```
shared-components/
├── Button
├── Input
├── Card
├── Table
├── Modal
├── Alert
├── Loader
├── Navigation
├── Footer
└── Layout
```

## Authentication Flow

1. User enters credentials on login page
2. Frontend calls `/api/authentication/login`
3. Backend returns JWT token + user info
4. Frontend stores token in localStorage/sessionStorage
5. All subsequent API calls include `Authorization: Bearer {token}`
6. Token expires after 1 hour (configurable)
7. Refresh token mechanism (optional)

## Security Considerations

- HTTPS only in production
- JWT token storage (httpOnly cookies preferred)
- CSRF protection
- XSS prevention (sanitize inputs)
- Content Security Policy headers
- Rate limiting on sensitive operations
- Session timeout
- Audit logging

## Deployment Strategy

### Development
- Personal Banking: http://localhost:3000
- Corporate Banking: http://localhost:3001
- SME Banking: http://localhost:3002
- Public Website: http://localhost:3003
- API: http://localhost:5000

### Production
- Personal Banking: https://personal.wekeza.com
- Corporate Banking: https://corporate.wekeza.com
- SME Banking: https://sme.wekeza.com
- Public Website: https://www.wekeza.com
- API: https://api.wekeza.com

## Next Steps

1. ✅ Create project structure
2. ✅ Set up shared components
3. ✅ Implement authentication
4. ✅ Build Personal Banking Portal (Priority 1)
5. Build Public Website (Priority 2)
6. Build SME Banking Portal (Priority 3)
7. Build Corporate Banking Portal (Priority 4)

---

**Ready to build?** Let's start with the Personal Banking Portal!
