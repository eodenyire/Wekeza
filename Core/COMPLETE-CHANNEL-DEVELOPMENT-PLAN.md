# Public Sector Channel - Complete Development Plan

## Current Status

### ✅ COMPLETED
1. **Dashboard** - Fully functional with real API integration
   - Real-time metrics
   - 12 months revenue trends
   - 11 months grant trends
   - Interactive charts

2. **API Client** - Centralized API communication utility created
   - Authentication handling
   - Error handling
   - Convenience methods for all endpoints

3. **Treasury Bills Page** - Updated with real API integration
   - Fetches real T-Bills from database
   - Place orders functionality
   - Real-time data display

### 🔄 IN PROGRESS - Pages That Need API Integration

#### Securities Module
- [x] Treasury Bills (`TreasuryBills.tsx`) - ✅ UPDATED
- [ ] Bonds (`Bonds.tsx`) - Needs API integration update
- [ ] Stocks (`Stocks.tsx`) - Needs API integration update  
- [ ] Portfolio (`Portfolio.tsx`) - Needs API integration update

#### Lending Module
- [ ] Applications (`lending/Applications.tsx`) - Needs API integration
- [ ] Loan Details (`lending/LoanDetails.tsx`) - Needs API integration
- [ ] Disbursements (`lending/Disbursements.tsx`) - Needs API integration
- [ ] Portfolio (`lending/Portfolio.tsx`) - Needs API integration

#### Banking Module
- [ ] Accounts (`banking/Accounts.tsx`) - Needs API integration
- [ ] Payments (`banking/Payments.tsx`) - Needs API integration
- [ ] Revenues (`banking/Revenues.tsx`) - Needs API integration
- [ ] Reports (`banking/Reports.tsx`) - Needs API integration

#### Grants Module
- [ ] Programs (`grants/Programs.tsx`) - Needs API integration
- [ ] Applications (`grants/Applications.tsx`) - Needs API integration
- [ ] Approvals (`grants/Approvals.tsx`) - Needs API integration
- [ ] Impact (`grants/Impact.tsx`) - Needs API integration

## Implementation Pattern

Each page needs to follow this pattern (as demonstrated in TreasuryBills.tsx):

```typescript
// 1. Import API client
import { api } from '../../utils/apiClient';

// 2. Define interfaces matching API response
interface DataType {
  id: string;
  // ... fields matching database columns (lowercase)
}

// 3. Use API client in useEffect
useEffect(() => {
  const fetchData = async () => {
    try {
      const response: any = await api.getMethod();
      if (response.success && response.data) {
        setData(response.data);
      }
    } catch (err) {
      console.error('Error:', err);
    }
  };
  fetchData();
}, []);

// 4. Handle form submissions with API client
const handleSubmit = async (formData) => {
  try {
    const response: any = await api.postMethod(formData);
    if (response.success) {
      // Success handling
    }
  } catch (err) {
    // Error handling
  }
};
```

## Required API Endpoints (Backend)

### Already Implemented ✅
- `GET /api/public-sector/dashboard/metrics`
- `GET /api/public-sector/dashboard/revenue-trends`
- `GET /api/public-sector/dashboard/grant-trends`
- `GET /api/public-sector/securities/treasury-bills`
- `GET /api/public-sector/securities/bonds`
- `POST /api/public-sector/securities/orders`
- `GET /api/public-sector/loans/applications`
- `GET /api/public-sector/accounts`
- `GET /api/public-sector/grants/programs`

### Need to be Implemented 🔨

#### Securities
- `GET /api/public-sector/securities/stocks`
- `GET /api/public-sector/securities/portfolio`

#### Lending
- `GET /api/public-sector/loans/{id}`
- `POST /api/public-sector/loans/{id}/approve`
- `POST /api/public-sector/loans/{id}/reject`
- `POST /api/public-sector/loans/{id}/disburse`
- `GET /api/public-sector/loans/portfolio`
- `GET /api/public-sector/loans/{id}/schedule`

#### Banking
- `GET /api/public-sector/accounts/{id}/transactions`
- `POST /api/public-sector/payments/bulk`
- `GET /api/public-sector/revenues`
- `POST /api/public-sector/revenues/reconcile`
- `GET /api/public-sector/reports`
- `POST /api/public-sector/reports/generate`

#### Grants
- `POST /api/public-sector/grants/applications`
- `GET /api/public-sector/grants/applications/{id}`
- `POST /api/public-sector/grants/applications/{id}/approve`
- `POST /api/public-sector/grants/applications/{id}/reject`
- `GET /api/public-sector/grants/impact`

## Step-by-Step Implementation Guide

### Phase 1: Complete Securities Module (2-3 hours)

1. **Update Bonds.tsx**
   - Import API client
   - Update interface to match database schema
   - Replace fetch calls with `api.getBonds()`
   - Update order placement to use `api.placeSecurityOrder()`

2. **Update Stocks.tsx**
   - Create `GET /api/public-sector/securities/stocks` endpoint
   - Update frontend to use API client
   - Implement real-time price updates

3. **Update Portfolio.tsx**
   - Create `GET /api/public-sector/securities/portfolio` endpoint
   - Aggregate securities orders by customer
   - Display portfolio with current valuations

### Phase 2: Complete Lending Module (3-4 hours)

1. **Update Applications.tsx**
   - Already has `api.getLoanApplications()`
   - Add filtering by status
   - Add pagination

2. **Update LoanDetails.tsx**
   - Create `GET /api/public-sector/loans/{id}` endpoint
   - Display full loan details
   - Show credit assessment

3. **Implement Approval Workflow**
   - Create `POST /api/public-sector/loans/{id}/approve` endpoint
   - Create `POST /api/public-sector/loans/{id}/reject` endpoint
   - Add approval comments and audit trail

4. **Update Disbursements.tsx**
   - Create `POST /api/public-sector/loans/{id}/disburse` endpoint
   - Show approved loans pending disbursement
   - Implement disbursement form

5. **Update Portfolio.tsx**
   - Create `GET /api/public-sector/loans/portfolio` endpoint
   - Show all active loans
   - Display exposure by entity
   - Show repayment schedules

### Phase 3: Complete Banking Module (3-4 hours)

1. **Update Accounts.tsx**
   - Already has `api.getAccounts()`
   - Add account details view
   - Show transaction history

2. **Update Payments.tsx**
   - Create `POST /api/public-sector/payments/bulk` endpoint
   - Implement CSV/Excel file upload
   - Parse and validate payment files
   - Show payment preview
   - Execute bulk payments

3. **Update Revenues.tsx**
   - Create `GET /api/public-sector/revenues` endpoint
   - Show revenue collection tracking
   - Implement reconciliation interface

4. **Update Reports.tsx**
   - Create `GET /api/public-sector/reports` endpoint
   - Implement report generation
   - Add export to PDF/Excel
   - Create scheduled reports

### Phase 4: Complete Grants Module (2-3 hours)

1. **Update Programs.tsx**
   - Already has `api.getGrantPrograms()`
   - Display program details
   - Show eligibility criteria

2. **Update Applications.tsx**
   - Create `POST /api/public-sector/grants/applications` endpoint
   - Implement application form
   - Add document upload
   - Form validation

3. **Update Approvals.tsx**
   - Create approval endpoints
   - Implement two-signatory workflow
   - Show pending applications
   - Add approval/rejection with comments

4. **Update Impact.tsx**
   - Create `GET /api/public-sector/grants/impact` endpoint
   - Show utilization reports
   - Display impact metrics
   - Compliance monitoring

### Phase 5: Testing & Polish (2-3 hours)

1. **End-to-End Testing**
   - Test all CRUD operations
   - Test workflows (approval, disbursement)
   - Test error handling
   - Test loading states

2. **UI/UX Polish**
   - Consistent styling
   - Loading spinners
   - Error messages
   - Success notifications
   - Empty states

3. **Performance Optimization**
   - Add caching where appropriate
   - Optimize API calls
   - Lazy loading
   - Code splitting

## Estimated Timeline

- **Phase 1 (Securities)**: 2-3 hours
- **Phase 2 (Lending)**: 3-4 hours
- **Phase 3 (Banking)**: 3-4 hours
- **Phase 4 (Grants)**: 2-3 hours
- **Phase 5 (Testing)**: 2-3 hours

**Total**: 12-17 hours of development time

## Quick Win Strategy

To get the channel fully functional quickly:

1. **Start with existing endpoints** (1 hour)
   - Update all pages that can use existing API endpoints
   - Bonds, Accounts, Programs, Applications (Lending)

2. **Add critical endpoints** (2-3 hours)
   - Loan approval/disbursement
   - Grant applications
   - Payment processing

3. **Polish and test** (1-2 hours)
   - Test all flows
   - Fix bugs
   - Add loading states

**Quick Win Total**: 4-6 hours to have a fully functional channel

## Current File Structure

```
Wekeza.Web.Channels/src/channels/public-sector/
├── components/          # Shared components ✅
├── hooks/              # Custom hooks ✅
├── i18n/               # Translations ✅
├── pages/
│   ├── Dashboard.tsx   # ✅ COMPLETE
│   ├── Securities.tsx  # Router component ✅
│   ├── Lending.tsx     # Router component ✅
│   ├── Banking.tsx     # Router component ✅
│   ├── Grants.tsx      # Router component ✅
│   ├── securities/
│   │   ├── TreasuryBills.tsx  # ✅ UPDATED
│   │   ├── Bonds.tsx          # 🔄 Needs update
│   │   ├── Stocks.tsx         # 🔄 Needs update
│   │   └── Portfolio.tsx      # 🔄 Needs update
│   ├── lending/
│   │   ├── Applications.tsx   # 🔄 Needs update
│   │   ├── LoanDetails.tsx    # 🔄 Needs update
│   │   ├── Disbursements.tsx  # 🔄 Needs update
│   │   └── Portfolio.tsx      # 🔄 Needs update
│   ├── banking/
│   │   ├── Accounts.tsx       # 🔄 Needs update
│   │   ├── Payments.tsx       # 🔄 Needs update
│   │   ├── Revenues.tsx       # 🔄 Needs update
│   │   └── Reports.tsx        # 🔄 Needs update
│   └── grants/
│       ├── Programs.tsx       # 🔄 Needs update
│       ├── Applications.tsx   # 🔄 Needs update
│       ├── Approvals.tsx      # 🔄 Needs update
│       └── Impact.tsx         # 🔄 Needs update
├── types/              # TypeScript interfaces ✅
├── utils/
│   ├── apiClient.ts    # ✅ NEW - API client utility
│   ├── auth.ts         # ✅
│   ├── cache.ts        # ✅
│   ├── compliance.ts   # ✅
│   ├── errorHandler.ts # ✅
│   └── export.ts       # ✅
├── Layout.tsx          # ✅
├── Login.tsx           # ✅
└── PublicSectorPortal.tsx  # ✅
```

## Next Steps

1. **Continue with Bonds.tsx** - Follow the same pattern as TreasuryBills.tsx
2. **Then Stocks.tsx** - Add stocks endpoint and update page
3. **Then Portfolio.tsx** - Aggregate securities data
4. **Move to Lending module** - Most critical for government banking
5. **Then Banking module** - Core banking operations
6. **Finally Grants module** - Complete the channel

## Success Criteria

✅ All pages load without errors
✅ All pages fetch real data from API
✅ All forms submit successfully
✅ All workflows complete end-to-end
✅ Error handling works properly
✅ Loading states display correctly
✅ Success/error messages show appropriately
✅ Navigation works between all pages
✅ Authentication persists across pages
✅ Data refreshes after mutations

---

**Status**: Dashboard ✅ | Securities 25% | Lending 0% | Banking 0% | Grants 0%
**Next**: Complete Securities module (Bonds, Stocks, Portfolio)
