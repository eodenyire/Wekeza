# Public Sector Portal - Shared Components

This directory contains reusable components for the Public Sector Portal.

## Components

### 1. SecurityCard
Display security information (T-Bills, Bonds, Stocks) with buy/sell actions.

**Props:**
- `name`: Security name
- `type`: 'TBILL' | 'BOND' | 'STOCK'
- `currentPrice`: Current price
- `quantity?`: Quantity held
- `priceChange?`: Price change percentage
- `maturityDate?`: Maturity date
- `rate?`: Interest/coupon rate
- `onBuy?`: Buy action handler
- `onSell?`: Sell action handler
- `onView?`: View details handler

**Usage:**
```tsx
<SecurityCard
  name="91-Day T-Bill"
  type="TBILL"
  currentPrice={95000}
  rate={12.5}
  maturityDate="2026-06-15"
  onBuy={() => handleBuy()}
/>
```

### 2. LoanCard
Display loan information with status and repayment progress.

**Props:**
- `loanNumber`: Loan reference number
- `entityName`: Government entity name
- `entityType`: 'NATIONAL' | 'COUNTY'
- `principalAmount`: Loan principal
- `outstandingBalance`: Outstanding balance
- `interestRate`: Interest rate
- `status`: 'ACTIVE' | 'CLOSED' | 'DEFAULT'
- `maturityDate`: Maturity date
- `disbursementDate?`: Disbursement date
- `repaymentProgress?`: Progress percentage (0-100)
- `onView?`: View details handler
- `onDisburse?`: Disburse handler

**Usage:**
```tsx
<LoanCard
  loanNumber="LN-2026-001"
  entityName="Nairobi County"
  entityType="COUNTY"
  principalAmount={50000000}
  outstandingBalance={35000000}
  interestRate={8.5}
  status="ACTIVE"
  maturityDate="2031-12-31"
  onView={() => navigate(`/loans/${id}`)}
/>
```

### 3. TransactionTable
Paginated, sortable, and filterable transaction table.

**Props:**
- `transactions`: Array of transaction objects
- `onExport?`: Export handler
- `showFilters?`: Show filter controls (default: true)

**Transaction Object:**
```typescript
{
  id: string;
  date: string;
  type: string;
  description: string;
  amount: number;
  status: 'SUCCESS' | 'PENDING' | 'FAILED';
  reference?: string;
}
```

**Usage:**
```tsx
<TransactionTable
  transactions={transactions}
  onExport={() => exportToCSV()}
/>
```

### 4. GrantCard
Display grant program or application information.

**Props:**
- `programName`: Grant program name
- `category`: 'EDUCATION' | 'HEALTH' | 'INFRASTRUCTURE' | 'ENVIRONMENT' | 'OTHER'
- `description`: Program description
- `maxAmount`: Maximum grant amount
- `deadline?`: Application deadline
- `status`: 'OPEN' | 'CLOSED'
- `eligibilityCriteria?`: Array of criteria strings
- `applicantName?`: Applicant name (for applications)
- `requestedAmount?`: Requested amount (for applications)
- `applicationStatus?`: Application status
- `onApply?`: Apply handler
- `onView?`: View details handler

**Usage:**
```tsx
<GrantCard
  programName="Education Support Grant"
  category="EDUCATION"
  description="Supporting educational initiatives..."
  maxAmount={5000000}
  deadline="2026-12-31"
  status="OPEN"
  eligibilityCriteria={['NGO registered', 'Education focus']}
  onApply={() => handleApply()}
/>
```

### 5. ApprovalWorkflow
Display and manage multi-step approval workflows.

**Props:**
- `steps`: Array of approval steps
- `currentStep`: Current step index
- `requiresTwoSignatories?`: Requires two approvals (default: false)
- `onApprove?`: Approve handler
- `onReject?`: Reject handler
- `canApprove?`: User can approve (default: false)

**Approval Step Object:**
```typescript
{
  approverName: string;
  approverRole: string;
  decision?: 'APPROVED' | 'REJECTED' | 'PENDING';
  comments?: string;
  approvedDate?: string;
}
```

**Usage:**
```tsx
<ApprovalWorkflow
  steps={approvalSteps}
  currentStep={0}
  requiresTwoSignatories={true}
  canApprove={true}
  onApprove={(comments) => handleApprove(comments)}
  onReject={(comments) => handleReject(comments)}
/>
```

### 6. StatCard
Display key metrics with optional trends.

**Props:**
- `title`: Metric title
- `value`: Metric value (string or number)
- `subtitle?`: Additional info
- `icon`: Lucide icon component
- `iconColor?`: Icon color class (default: 'text-blue-600')
- `trend?`: Trend object with value and isPositive
- `onClick?`: Click handler

**Usage:**
```tsx
<StatCard
  title="Total Portfolio"
  value="KES 5,000,000,000"
  subtitle="Across all securities"
  icon={DollarSign}
  iconColor="text-green-600"
  trend={{ value: 12.5, isPositive: true }}
/>
```

### 7. LoadingSpinner
Display loading state with optional message.

**Props:**
- `size?`: 'sm' | 'md' | 'lg' (default: 'md')
- `message?`: Loading message

**Usage:**
```tsx
<LoadingSpinner size="lg" message="Loading portfolio..." />
```

### 8. EmptyState
Display empty state with icon and optional action.

**Props:**
- `icon`: Lucide icon component
- `title`: Empty state title
- `description`: Empty state description
- `actionLabel?`: Action button label
- `onAction?`: Action handler

**Usage:**
```tsx
<EmptyState
  icon={FileText}
  title="No applications found"
  description="There are currently no loan applications"
  actionLabel="Create Application"
  onAction={() => navigate('/apply')}
/>
```

### 9. ErrorAlert
Display error messages with optional dismiss.

**Props:**
- `title?`: Error title (default: 'Error')
- `message`: Error message
- `onDismiss?`: Dismiss handler

**Usage:**
```tsx
<ErrorAlert
  title="Failed to load data"
  message="Unable to fetch loan applications. Please try again."
  onDismiss={() => setError(null)}
/>
```

### 10. SuccessAlert
Display success messages with optional dismiss.

**Props:**
- `title?`: Success title (default: 'Success')
- `message`: Success message
- `onDismiss?`: Dismiss handler

**Usage:**
```tsx
<SuccessAlert
  message="Loan application approved successfully!"
  onDismiss={() => setSuccess(null)}
/>
```

### 11. Modal
Reusable modal dialog component.

**Props:**
- `isOpen`: Modal open state
- `onClose`: Close handler
- `title`: Modal title
- `children`: Modal content
- `size?`: 'sm' | 'md' | 'lg' | 'xl' (default: 'md')
- `showCloseButton?`: Show close button (default: true)

**Usage:**
```tsx
<Modal
  isOpen={showModal}
  onClose={() => setShowModal(false)}
  title="Confirm Action"
  size="lg"
>
  <p>Are you sure you want to proceed?</p>
  <div className="flex gap-3 mt-4">
    <button onClick={handleConfirm}>Confirm</button>
    <button onClick={() => setShowModal(false)}>Cancel</button>
  </div>
</Modal>
```

### 12. ProtectedRoute
Route protection based on authentication (already exists).

## Best Practices

1. **Import from index**: Always import components from the index file:
   ```tsx
   import { SecurityCard, LoanCard } from '../components';
   ```

2. **Type Safety**: All components are fully typed with TypeScript interfaces.

3. **Accessibility**: Components include proper ARIA labels and keyboard navigation.

4. **Responsive**: All components are mobile-responsive using Tailwind CSS.

5. **Consistent Styling**: Components follow the Wekeza design system with consistent colors, spacing, and typography.

## Styling

Components use Tailwind CSS utility classes. Key color schemes:
- Primary: Blue (blue-600)
- Success: Green (green-600)
- Warning: Yellow (yellow-600)
- Danger: Red (red-600)
- Info: Purple (purple-600)

## Icons

Components use Lucide React icons. Common icons:
- DollarSign: Financial amounts
- TrendingUp/Down: Trends and changes
- Building2: Organizations
- Calendar: Dates
- CheckCircle: Success/Approval
- XCircle: Rejection/Error
- AlertCircle: Warnings/Errors
