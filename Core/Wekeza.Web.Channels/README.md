# Wekeza Banking Web Channels

This folder contains all web-based customer-facing channels for Wekeza Bank.

## Channels

1. **Personal Banking** (`/personal`) - Retail customer portal
2. **Corporate Banking** (`/corporate`) - Business customer portal  
3. **SME Banking** (`/sme`) - Small & Medium Enterprise portal
4. **Public Website** (`/public`) - Marketing and information site

## Technology Stack

- **Frontend**: React 18 + TypeScript
- **Styling**: Tailwind CSS
- **State Management**: React Context + Hooks
- **API Client**: Axios
- **Routing**: React Router v6
- **Forms**: React Hook Form + Zod validation
- **Charts**: Recharts
- **Icons**: Lucide React

## Getting Started

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build
```

## API Configuration

All channels connect to the Wekeza Core API at `http://localhost:5000/api`

## Features by Channel

### Personal Banking
- Account dashboard
- Fund transfers
- Bill payments
- Loan applications
- Card management
- Statement downloads
- Profile management

### Corporate Banking
- Multi-user access
- Bulk payments
- Trade finance
- Treasury operations
- Advanced reporting
- Approval workflows

### SME Banking
- Business accounts
- Working capital loans
- Merchant services
- Payroll management
- Business analytics

### Public Website
- Product information
- Branch locator
- Online account opening
- Contact forms
- News and updates
