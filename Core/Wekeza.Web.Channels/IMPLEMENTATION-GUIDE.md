# Wekeza Web Channels - Implementation Guide

## Overview

This document provides a complete guide to implementing all four web channels for Wekeza Bank.

## Project Structure

```
Wekeza.Web.Channels/
├── src/
│   ├── channels/
│   │   ├── public/          # Public website
│   │   │   ├── pages/
│   │   │   │   ├── HomePage.tsx
│   │   │   │   ├── ProductsPage.tsx
│   │   │   │   ├── AboutPage.tsx
│   │   │   │   ├── ContactPage.tsx
│   │   │   │   └── OpenAccountPage.tsx
│   │   │   └── PublicWebsite.tsx
│   │   ├── personal/        # Personal banking portal
│   │   │   ├── pages/
│   │   │   │   ├── Dashboard.tsx
│   │   │   │   ├── Accounts.tsx
│   │   │   │   ├── Transfer.tsx
│   │   │   │   ├── Payments.tsx
│   │   │   │   ├── Cards.tsx
│   │   │   │   ├── Loans.tsx
│   │   │   │   └── Profile.tsx
│   │   │   ├── components/
│   │   │   │   ├── AccountCard.tsx
│   │   │   │   ├── TransactionList.tsx
│   │   │   │   └── QuickActions.tsx
│   │   │   ├── Login.tsx
│   │   │   └── PersonalBanking.tsx
│   │   ├── corporate/       # Corporate banking portal
│   │   │   ├── pages/
│   │   │   │   ├── Dashboard.tsx
│   │   │   │   ├── Accounts.tsx
│   │   │   │   ├── BulkPayments.tsx
│   │   │   │   ├── TradeFinance.tsx
│   │   │   │   ├── Treasury.tsx
│   │   │   │   ├── Approvals.tsx
│   │   │   │   └── Reports.tsx
│   │   │   ├── Login.tsx
│   │   │   └── CorporateBanking.tsx
│   │   └── sme/             # SME banking portal
│   │       ├── pages/
│   │       │   ├── Dashboard.tsx
│   │       │   ├── Accounts.tsx
│   │       │   ├── Loans.tsx
│   │       │   ├── Payroll.tsx
│   │       │   └── Analytics.tsx
│   │       ├── Login.tsx
│   │       └── SMEBanking.tsx
│   ├── components/          # Shared components
│   │   ├── ui/
│   │   │   ├── Button.tsx
│   │   │   ├── Card.tsx
│   │   │   ├── Input.tsx
│   │   │   ├── Modal.tsx
│   │   │   └── Table.tsx
│   │   └── layout/
│   │       ├── Header.tsx
│   │       ├── Sidebar.tsx
│   │       └── Footer.tsx
│   ├── contexts/
│   │   └── AuthContext.tsx
│   ├── lib/
│   │   ├── api-client.ts
│   │   └── utils.ts
│   ├── App.tsx
│   ├── main.tsx
│   └── index.css
├── index.html
├── package.json
├── vite.config.ts
├── tailwind.config.js
└── tsconfig.json
```

## Installation & Setup

### 1. Install Dependencies

```bash
cd Wekeza.Web.Channels
npm install
```

### 2. Environment Configuration

Create `.env` file:

```env
VITE_API_URL=http://localhost:5000/api
```

### 3. Start Development Server

```bash
npm run dev
```

The application will be available at `http://localhost:3000`

## Channel Features

### 1. Public Website (`/`)

**Purpose**: Marketing, information, and customer acquisition

**Key Pages**:
- **Home**: Hero section, features, call-to-action
- **Products**: Product catalog (Personal, Corporate, SME, Loans)
- **About**: Company information, mission, values
- **Contact**: Contact form, branch locator
- **Open Account**: Self-onboarding flow (3 steps)

**Features**:
- Responsive design
- Product showcase
- Online account opening
- Branch locator
- Contact forms

### 2. Personal Banking Portal (`/personal`)

**Purpose**: Retail customer self-service

**Key Features**:
- **Dashboard**: Account summary, recent transactions, quick actions
- **Accounts**: View all accounts, balances, statements
- **Transfer**: Internal & external transfers
- **Payments**: Bill payments, airtime purchase
- **Cards**: View cards, request new, block/unblock, virtual cards
- **Loans**: Apply for loans, view loan details, make repayments
- **Profile**: Update personal information, change password

**API Endpoints Used**:
- `GET /api/customer-portal/accounts`
- `GET /api/customer-portal/accounts/{id}/transactions`
- `POST /api/customer-portal/transactions/transfer`
- `POST /api/customer-portal/transactions/pay-bill`
- `GET /api/customer-portal/cards`
- `POST /api/customer-portal/cards/request-virtual`
- `GET /api/customer-portal/loans`
- `POST /api/customer-portal/loans/apply`

### 3. Corporate Banking Portal (`/corporate`)

**Purpose**: Business customer operations

**Key Features**:
- **Dashboard**: Corporate account overview, pending approvals
- **Accounts**: Multiple account management
- **Bulk Payments**: Upload payment files, batch processing
- **Trade Finance**: Letters of credit, bank guarantees
- **Treasury**: FX deals, money market operations
- **Approvals**: Maker-checker workflow
- **Reports**: Custom reports, regulatory reports
- **User Management**: Multi-user access control

**API Endpoints Used**:
- `GET /api/accounts`
- `POST /api/payments/bulk`
- `GET /api/tradefinance/letters-of-credit`
- `POST /api/treasury/fx-deals`
- `GET /api/workflows/pending-approvals`
- `POST /api/workflows/approve`
- `GET /api/reporting/custom`

### 4. SME Banking Portal (`/sme`)

**Purpose**: Small & Medium Enterprise banking

**Key Features**:
- **Dashboard**: Business metrics, cash flow
- **Accounts**: Business accounts, transaction history
- **Loans**: Working capital loans, equipment financing
- **Payroll**: Employee payment management
- **Merchant Services**: POS transactions, settlements
- **Analytics**: Business insights, spending patterns
- **Invoicing**: Invoice management

**API Endpoints Used**:
- `GET /api/customer-portal/accounts`
- `POST /api/loans/apply`
- `POST /api/payments/payroll`
- `GET /api/dashboard/transactions/trends`
- `GET /api/dashboard/accounts/statistics`

## Authentication Flow

### Login Process

1. User enters credentials on login page
2. Call `POST /api/authentication/login`
3. Receive JWT token and user info
4. Store token in localStorage
5. Redirect to appropriate dashboard based on role

### Token Management

```typescript
// Store token
localStorage.setItem('auth_token', token);
localStorage.setItem('user_info', JSON.stringify(userInfo));

// Retrieve token
const token = localStorage.getItem('auth_token');

// Clear on logout
localStorage.removeItem('auth_token');
localStorage.removeItem('user_info');
```

### Protected Routes

```typescript
function ProtectedRoute({ children }: { children: ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) return <div>Loading...</div>;
  if (!isAuthenticated) return <Navigate to="/login" />;

  return <>{children}</>;
}
```

## Key Components to Implement

### 1. Account Card Component

```typescript
interface AccountCardProps {
  accountNumber: string;
  accountType: string;
  balance: number;
  currency: string;
}

function AccountCard({ accountNumber, accountType, balance, currency }: AccountCardProps) {
  return (
    <div className="card">
      <div className="flex justify-between items-start">
        <div>
          <p className="text-sm text-gray-600">{accountType}</p>
          <p className="text-lg font-semibold">{accountNumber}</p>
        </div>
        <div className="text-right">
          <p className="text-sm text-gray-600">Available Balance</p>
          <p className="text-2xl font-bold text-wekeza-blue">
            {currency} {balance.toLocaleString()}
          </p>
        </div>
      </div>
    </div>
  );
}
```

### 2. Transaction List Component

```typescript
interface Transaction {
  id: string;
  date: string;
  description: string;
  amount: number;
  type: 'credit' | 'debit';
}

function TransactionList({ transactions }: { transactions: Transaction[] }) {
  return (
    <div className="card">
      <h3 className="text-lg font-semibold mb-4">Recent Transactions</h3>
      <div className="space-y-2">
        {transactions.map((txn) => (
          <div key={txn.id} className="flex justify-between items-center py-2 border-b">
            <div>
              <p className="font-medium">{txn.description}</p>
              <p className="text-sm text-gray-600">{txn.date}</p>
            </div>
            <p className={`font-semibold ${txn.type === 'credit' ? 'text-green-600' : 'text-red-600'}`}>
              {txn.type === 'credit' ? '+' : '-'}{txn.amount.toLocaleString()}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
}
```

### 3. Transfer Form Component

```typescript
function TransferForm() {
  const [formData, setFormData] = useState({
    fromAccountId: '',
    toAccountNumber: '',
    amount: '',
    narration: '',
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await apiClient.transferFunds({
        ...formData,
        amount: parseFloat(formData.amount),
        currency: 'KES',
      });
      alert('Transfer successful!');
    } catch (error) {
      alert('Transfer failed');
    }
  };

  return (
    <form onSubmit={handleSubmit} className="card space-y-4">
      <h3 className="text-lg font-semibold">Transfer Funds</h3>
      
      <div>
        <label className="block text-sm font-medium mb-1">From Account</label>
        <select
          className="input"
          value={formData.fromAccountId}
          onChange={(e) => setFormData({ ...formData, fromAccountId: e.target.value })}
          required
        >
          <option value="">Select account</option>
          {/* Map accounts here */}
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium mb-1">To Account Number</label>
        <input
          type="text"
          className="input"
          value={formData.toAccountNumber}
          onChange={(e) => setFormData({ ...formData, toAccountNumber: e.target.value })}
          required
        />
      </div>

      <div>
        <label className="block text-sm font-medium mb-1">Amount</label>
        <input
          type="number"
          className="input"
          value={formData.amount}
          onChange={(e) => setFormData({ ...formData, amount: e.target.value })}
          required
        />
      </div>

      <div>
        <label className="block text-sm font-medium mb-1">Narration</label>
        <input
          type="text"
          className="input"
          value={formData.narration}
          onChange={(e) => setFormData({ ...formData, narration: e.target.value })}
          required
        />
      </div>

      <button type="submit" className="btn btn-primary w-full">
        Transfer
      </button>
    </form>
  );
}
```

## Testing the Channels

### 1. Test Login

```
Username: admin
Password: (any password)
```

### 2. Test API Connectivity

Open browser console and run:

```javascript
fetch('http://localhost:5000/api')
  .then(r => r.json())
  .then(console.log);
```

### 3. Test Authentication

```javascript
fetch('http://localhost:5000/api/authentication/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ username: 'admin', password: 'test' })
})
  .then(r => r.json())
  .then(console.log);
```

## Next Steps

1. **Complete all page implementations** for each channel
2. **Add form validation** using React Hook Form + Zod
3. **Implement error handling** and loading states
4. **Add charts and visualizations** using Recharts
5. **Implement real-time updates** using polling or WebSockets
6. **Add unit tests** using Vitest
7. **Optimize performance** with React.memo and useMemo
8. **Add accessibility** features (ARIA labels, keyboard navigation)
9. **Implement PWA** features for mobile
10. **Deploy** to production

## Deployment

### Build for Production

```bash
npm run build
```

### Deploy to Netlify/Vercel

```bash
# Netlify
netlify deploy --prod

# Vercel
vercel --prod
```

### Environment Variables

Set in deployment platform:
- `VITE_API_URL`: Production API URL

## Support

For issues or questions, refer to the main Wekeza documentation or contact the development team.
