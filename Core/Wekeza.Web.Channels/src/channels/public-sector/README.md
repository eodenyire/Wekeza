# Public Sector Portal

A comprehensive banking portal for Wekeza Bank to serve government entities (National and County Governments of Kenya).

## Overview

The Public Sector Portal provides four core modules:
1. **Securities Trading** - Trade government securities (T-Bills, Bonds, Stocks)
2. **Government Lending** - Manage government loan applications and portfolio
3. **Government Banking Services** - Handle accounts, payments, and revenues
4. **Grants & Philanthropy** - Manage grant programs and applications

## Features

### Securities Trading Module
- Treasury Bills trading (91-day, 182-day, 364-day)
- Government Bonds trading with accrued interest calculation
- NSE-listed stocks with real-time price updates (30-second polling)
- Consolidated portfolio view with performance metrics
- Maturity calendar and export functionality

### Government Lending Module
- Loan application management with filters and search
- Credit assessment and approval workflow
- Loan disbursement interface
- Portfolio tracking with exposure monitoring
- Repayment schedule management

### Government Banking Services Module
- Government account management
- Bulk payment processing with file upload (CSV/Excel)
- Revenue collection tracking and reconciliation
- Financial report generation and export

### Grants & Philanthropy Module
- Grant program listings with eligibility criteria
- Application submission and tracking
- Two-signatory approval workflow
- Impact reporting and compliance monitoring

### Dashboard & Analytics
- Key metrics cards (Securities, Loans, Banking, Grants)
- Interactive charts (Pie, Bar, Line, Area)
- Risk & compliance metrics
- Recent activities feed
- Export functionality

## Architecture

### Directory Structure

```
src/channels/public-sector/
├── components/          # Reusable UI components
│   ├── SecurityCard.tsx
│   ├── LoanCard.tsx
│   ├── TransactionTable.tsx
│   ├── GrantCard.tsx
│   ├── ApprovalWorkflow.tsx
│   ├── ErrorBoundary.tsx
│   ├── LoadingSpinner.tsx
│   ├── Toast.tsx
│   └── ProtectedRoute.tsx
├── pages/              # Page components
│   ├── Dashboard.tsx
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
│       └── Applications.tsx
├── types/              # TypeScript type definitions
│   └── index.ts
├── utils/              # Utility functions
│   ├── auth.ts
│   ├── compliance.ts
│   ├── errorHandler.ts
│   └── export.ts
├── hooks/              # Custom React hooks
│   └── usePublicSectorAuth.ts
├── Layout.tsx          # Main layout component
├── Login.tsx           # Authentication page
└── PublicSectorPortal.tsx  # Main routing component
```

### Technology Stack

- **Frontend Framework**: React 18 with TypeScript
- **Routing**: React Router v6
- **State Management**: React Hooks (useState, useEffect, useContext)
- **Charts**: Recharts
- **Icons**: Lucide React
- **Styling**: Tailwind CSS
- **Form Validation**: React Hook Form + Zod (planned)

## User Roles

1. **Treasury Officer** - Securities trading and portfolio management
2. **Credit Officer** - Loan applications and approvals
3. **Government Finance Officer** - Account and payment management
4. **CSR Manager** - Grant programs and approvals
5. **Compliance Officer** - All modules (read-only), audit logs
6. **Senior Management** - Dashboard and analytics

## Compliance & Regulations

### CBK Regulations
- Minimum T-Bill investment: KES 100,000
- Minimum Bond investment: KES 50,000
- Maximum single transaction: KES 1 Billion
- Trading hours: 9:00 AM - 3:00 PM EAT

### PFMA Requirements
- Maximum county exposure: 10% of bank capital
- Maximum national government exposure: 25% of bank capital
- Minimum loan amount: KES 10 Million
- Maximum loan tenor: 30 years
- Maximum grant amount: KES 5 Million
- Required approvals: 2 signatories

### AML/KYC
- Government entity validation
- Contact person verification
- Email and phone validation
- Comprehensive audit trail

## API Integration

### Base URL
```
/api/public-sector
```

### Endpoints

**Securities**
- `GET /securities/treasury-bills` - List T-Bills
- `POST /securities/treasury-bills/order` - Place T-Bill order
- `GET /securities/bonds` - List bonds
- `POST /securities/bonds/order` - Place bond order
- `GET /securities/stocks` - List stocks
- `POST /securities/stocks/order` - Place stock order
- `GET /securities/portfolio` - View portfolio

**Lending**
- `GET /loans/applications` - List applications
- `GET /loans/applications/:id` - Get application details
- `POST /loans/applications/:id/approve` - Approve loan
- `POST /loans/applications/:id/reject` - Reject loan
- `POST /loans/:id/disburse` - Disburse loan
- `GET /loans/portfolio` - View loan portfolio
- `GET /loans/:id/schedule` - View repayment schedule

**Banking**
- `GET /accounts` - List accounts
- `GET /accounts/:id/transactions` - Account transactions
- `POST /payments/bulk` - Process bulk payments
- `GET /revenues` - View revenues
- `POST /revenues/reconcile` - Reconcile revenues
- `GET /reports` - Generate reports

**Grants**
- `GET /grants/programs` - List programs
- `POST /grants/applications` - Submit application
- `GET /grants/applications` - List applications
- `POST /grants/applications/:id/approve` - Approve grant
- `POST /grants/:id/disburse` - Disburse grant

**Dashboard**
- `GET /dashboard/metrics` - Get dashboard metrics

## Development

### Prerequisites
- Node.js 18+
- npm or yarn

### Installation
```bash
cd Wekeza.Web.Channels
npm install
```

### Running Development Server
```bash
npm run dev
```

### Building for Production
```bash
npm run build
```

## Testing

### Unit Tests
```bash
npm run test
```

### E2E Tests
```bash
npm run test:e2e
```

## Security

- JWT-based authentication with role claims
- Role-based access control (RBAC)
- Multi-factor authentication for sensitive operations
- Comprehensive audit logging
- Data encryption in transit (HTTPS)
- Input validation and sanitization

## Performance

- Code splitting by module (lazy loading)
- Memoization of expensive computations
- Virtual scrolling for large lists
- Debounced search inputs
- Optimistic UI updates
- Real-time data polling (configurable intervals)

## Accessibility

- ARIA labels on interactive elements
- Keyboard navigation support
- Screen reader compatibility
- Focus indicators
- Semantic HTML
- Color contrast compliance

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Contributing

1. Follow the established code structure
2. Use TypeScript for type safety
3. Write meaningful commit messages
4. Add comments for complex logic
5. Test thoroughly before committing

## License

Proprietary - Wekeza Bank

## Support

For technical support, contact the development team.
