# 🧪 Wekeza Banking System - Complete Testing Guide

## Overview

This guide provides comprehensive testing scenarios for all Wekeza Banking channels and API endpoints.

## Prerequisites

✅ Backend API running on http://localhost:5000  
✅ All web channels installed and running  
✅ PostgreSQL database running  
✅ Browser with DevTools (Chrome/Edge recommended)

## Testing Strategy

```
1. API Testing (Direct) → 2. UI Testing (Channels) → 3. End-to-End Testing
```

---

## Part 1: API Testing (Backend)

### 1.1 Test API Health

```powershell
# Test API is alive
Invoke-RestMethod -Uri "http://localhost:5000/" -Method Get

# Expected Response:
# service: "Wekeza Core Banking System"
# status: "Running"
```

### 1.2 Test Authentication

```powershell
# Login request
$loginBody = @{
    username = "admin"
    password = "test123"
} | ConvertTo-Json

$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" `
    -Method Post `
    -Body $loginBody `
    -ContentType "application/json"

# Save token for subsequent requests
$token = $loginResponse.token
Write-Host "Token: $token"

# Test get current user
$headers = @{
    Authorization = "Bearer $token"
}

Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/me" `
    -Method Get `
    -Headers $headers
```

### 1.3 Test Customer Portal Endpoints

```powershell
# Get customer accounts
Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/accounts" `
    -Method Get `
    -Headers $headers

# Get customer profile
Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/profile" `
    -Method Get `
    -Headers $headers

# Get customer cards
Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/cards" `
    -Method Get `
    -Headers $headers

# Get customer loans
Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/loans" `
    -Method Get `
    -Headers $headers
```

### 1.4 Test Product Catalog

```powershell
# Get all products
Invoke-RestMethod -Uri "http://localhost:5000/api/products/catalog" `
    -Method Get `
    -Headers $headers

# Get deposit products
Invoke-RestMethod -Uri "http://localhost:5000/api/products/deposits" `
    -Method Get `
    -Headers $headers

# Get loan products
Invoke-RestMethod -Uri "http://localhost:5000/api/products/loans" `
    -Method Get `
    -Headers $headers
```

### 1.5 Test Dashboard Analytics

```powershell
# Get transaction trends
Invoke-RestMethod -Uri "http://localhost:5000/api/dashboard/transactions/trends?period=daily&days=30" `
    -Method Get `
    -Headers $headers

# Get account statistics
Invoke-RestMethod -Uri "http://localhost:5000/api/dashboard/accounts/statistics" `
    -Method Get `
    -Headers $headers

# Get loan portfolio stats
Invoke-RestMethod -Uri "http://localhost:5000/api/dashboard/loans/portfolio" `
    -Method Get `
    -Headers $headers
```

---

## Part 2: UI Testing (Web Channels)

### 2.1 Public Website Testing

**URL:** http://localhost:3000

#### Test Scenarios:

**✅ Home Page**
1. Open http://localhost:3000
2. Verify hero section loads
3. Check navigation menu works
4. Click "Open Account" button
5. Verify responsive design (resize browser)

**✅ Products Page**
1. Navigate to /products
2. Verify product categories display
3. Check product details
4. Test product filters

**✅ About Page**
1. Navigate to /about
2. Verify company information displays
3. Check team section
4. Verify mission/vision content

**✅ Contact Page**
1. Navigate to /contact
2. Fill contact form:
   - Name: Test User
   - Email: test@example.com
   - Message: Test message
3. Submit form
4. Verify success message

**✅ Account Opening**
1. Navigate to /open-account
2. Step 1 - Basic Info:
   - First Name: John
   - Last Name: Doe
   - Email: john.doe@example.com
   - Phone: +254712345678
   - ID Number: 12345678
   - Date of Birth: 1990-01-01
3. Click "Next"
4. Step 2 - Documents:
   - Upload ID document
   - Upload proof of address
5. Click "Next"
6. Step 3 - Account Setup:
   - Select product
   - Set initial deposit
   - Create PIN
7. Submit application
8. Verify success message with account details

### 2.2 Personal Banking Testing

**URL:** http://localhost:3001

#### Test Login:

```
Username: admin
Password: test123
```

#### Test Scenarios:

**✅ Dashboard**
1. Login successfully
2. Verify dashboard loads
3. Check account summary cards
4. Verify recent transactions list
5. Check quick actions buttons

**✅ Accounts Page**
1. Navigate to /accounts
2. Verify account list displays
3. Click on an account
4. Check account details
5. View transaction history
6. Download statement

**✅ Transfer Funds**
1. Navigate to /transfer
2. Fill transfer form:
   - From Account: Select account
   - To Account: Enter account number
   - Amount: 1000
   - Currency: KES
   - Narration: Test transfer
3. Click "Transfer"
4. Verify confirmation dialog
5. Confirm transfer
6. Check success message
7. Verify transaction appears in history

**✅ Pay Bills**
1. Navigate to /payments
2. Select "Pay Bill"
3. Fill form:
   - Biller: KPLC
   - Account Number: 123456
   - Amount: 500
4. Submit payment
5. Verify success

**✅ Buy Airtime**
1. Navigate to /payments
2. Select "Buy Airtime"
3. Fill form:
   - Phone Number: +254712345678
   - Amount: 100
   - Provider: Safaricom
4. Submit
5. Verify success

**✅ Cards Management**
1. Navigate to /cards
2. View existing cards
3. Click "Request New Card"
4. Fill form:
   - Card Type: Debit
   - Delivery Address: Test Address
5. Submit request
6. Click "Request Virtual Card"
7. Verify virtual card details displayed
8. Test "Block Card" functionality

**✅ Loans**
1. Navigate to /loans
2. View existing loans
3. Click "Apply for Loan"
4. Fill application:
   - Product: Personal Loan
   - Amount: 50000
   - Term: 12 months
   - Purpose: Business
5. Submit application
6. Verify application status
7. Test loan repayment

**✅ Profile Management**
1. Navigate to /profile
2. Update profile information
3. Change password
4. Update contact details
5. Save changes
6. Verify success

### 2.3 Corporate Banking Testing

**URL:** http://localhost:3002

#### Test Login:

```
Username: corporate_admin
Password: test123
```

#### Test Scenarios:

**✅ Corporate Dashboard**
1. Login successfully
2. Verify corporate dashboard
3. Check multiple accounts summary
4. View pending approvals count
5. Check recent corporate transactions

**✅ Bulk Payments**
1. Navigate to /bulk-payments
2. Download payment template
3. Upload payment file (CSV/Excel)
4. Review payment batch
5. Submit for approval
6. Verify batch status

**✅ Trade Finance**
1. Navigate to /trade-finance
2. Click "New Letter of Credit"
3. Fill LC details:
   - Beneficiary: Test Company
   - Amount: 100000
   - Currency: USD
   - Expiry Date: Future date
4. Submit LC request
5. View LC status
6. Test bank guarantee creation

**✅ Treasury Operations**
1. Navigate to /treasury
2. Create FX Deal:
   - From Currency: KES
   - To Currency: USD
   - Amount: 100000
   - Rate: Market rate
3. Submit deal
4. View deal confirmation
5. Test money market operations

**✅ Approvals Workflow**
1. Navigate to /approvals
2. View pending approvals list
3. Click on an approval
4. Review details
5. Approve or reject
6. Add comments
7. Submit decision
8. Verify workflow updated

**✅ Reports**
1. Navigate to /reports
2. Select report type
3. Set date range
4. Generate report
5. View report
6. Download report (PDF/Excel)
7. Schedule recurring report

### 2.4 SME Banking Testing

**URL:** http://localhost:3003

#### Test Login:

```
Username: sme_user
Password: test123
```

#### Test Scenarios:

**✅ SME Dashboard**
1. Login successfully
2. Verify business dashboard
3. Check business accounts
4. View cash flow chart
5. Check business metrics

**✅ Business Accounts**
1. Navigate to /accounts
2. View business accounts
3. Check account balances
4. View transaction history
5. Download statements

**✅ Business Loans**
1. Navigate to /loans
2. View existing business loans
3. Apply for working capital loan:
   - Amount: 500000
   - Term: 24 months
   - Purpose: Working capital
4. Submit application
5. Track application status

**✅ Payroll Management**
1. Navigate to /payroll
2. Upload employee list
3. Set payment amounts
4. Review payroll batch
5. Submit for processing
6. Verify payment status

**✅ Business Analytics**
1. Navigate to /analytics
2. View revenue trends
3. Check expense breakdown
4. View profit/loss chart
5. Export analytics report

---

## Part 3: End-to-End Testing

### E2E Scenario 1: New Customer Onboarding

1. **Public Website** - Open account
2. **Receive** - Account credentials via email
3. **Personal Banking** - First login
4. **Change** - Password on first login
5. **View** - Account dashboard
6. **Make** - First transaction

### E2E Scenario 2: Loan Application Flow

1. **Personal Banking** - Apply for loan
2. **System** - Credit scoring
3. **Workflow** - Approval routing
4. **Corporate** - Loan officer reviews
5. **Approval** - Manager approves
6. **Disbursement** - Funds transferred
7. **Personal Banking** - View loan details
8. **Repayment** - Make first payment

### E2E Scenario 3: Corporate Payment Flow

1. **Corporate Banking** - Upload bulk payments
2. **Maker** - Submits batch
3. **Checker** - Reviews batch
4. **Approver** - Approves batch
5. **System** - Processes payments
6. **Recipients** - Receive funds
7. **Reports** - Payment confirmation

---

## Part 4: Browser Testing

### Test in Multiple Browsers

- ✅ Chrome/Edge (Chromium)
- ✅ Firefox
- ✅ Safari (if on Mac)

### Test Responsive Design

1. Desktop (1920x1080)
2. Tablet (768x1024)
3. Mobile (375x667)

### Browser DevTools Testing

**Open DevTools (F12):**

1. **Console Tab**
   - Check for JavaScript errors
   - Verify API calls
   - Check console logs

2. **Network Tab**
   - Monitor API requests
   - Check response times
   - Verify status codes
   - Inspect request/response data

3. **Application Tab**
   - Check localStorage
   - Verify token storage
   - Check session data

---

## Part 5: Performance Testing

### Load Testing

```powershell
# Test API performance
Measure-Command {
    1..100 | ForEach-Object {
        Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get
    }
}
```

### Response Time Testing

```powershell
# Measure API response time
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/accounts" -Headers $headers
$stopwatch.Stop()
Write-Host "Response time: $($stopwatch.ElapsedMilliseconds)ms"
```

---

## Part 6: Security Testing

### Test Authentication

```powershell
# Test without token (should fail)
try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/accounts" -Method Get
} catch {
    Write-Host "✅ Correctly rejected unauthorized request"
}

# Test with invalid token (should fail)
$badHeaders = @{
    Authorization = "Bearer invalid_token"
}
try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/accounts" -Method Get -Headers $badHeaders
} catch {
    Write-Host "✅ Correctly rejected invalid token"
}
```

### Test CORS

```javascript
// In browser console
fetch('http://localhost:5000/api')
  .then(r => r.json())
  .then(console.log)
  .catch(console.error);
```

---

## Part 7: Error Handling Testing

### Test Invalid Inputs

```powershell
# Test invalid transfer amount
$invalidTransfer = @{
    fromAccountId = "invalid-id"
    toAccountNumber = "123"
    amount = -100  # Negative amount
    currency = "INVALID"
    narration = ""
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/transactions/transfer" `
        -Method Post `
        -Body $invalidTransfer `
        -ContentType "application/json" `
        -Headers $headers
} catch {
    Write-Host "✅ Correctly rejected invalid input"
}
```

---

## Test Checklist

### Backend API
- [ ] API health check
- [ ] Authentication works
- [ ] All endpoints respond
- [ ] Error handling works
- [ ] CORS configured
- [ ] Token validation works

### Public Website
- [ ] Home page loads
- [ ] Navigation works
- [ ] Forms submit
- [ ] Account opening works
- [ ] Responsive design
- [ ] No console errors

### Personal Banking
- [ ] Login works
- [ ] Dashboard displays
- [ ] Accounts load
- [ ] Transfers work
- [ ] Payments work
- [ ] Cards management works
- [ ] Loans work
- [ ] Profile updates work

### Corporate Banking
- [ ] Login works
- [ ] Dashboard displays
- [ ] Bulk payments work
- [ ] Trade finance works
- [ ] Treasury works
- [ ] Approvals work
- [ ] Reports generate

### SME Banking
- [ ] Login works
- [ ] Dashboard displays
- [ ] Accounts work
- [ ] Loans work
- [ ] Payroll works
- [ ] Analytics display

---

## Troubleshooting

### Common Issues

**Issue: API returns 401**
- Solution: Check token is valid and not expired

**Issue: CORS error**
- Solution: Verify API CORS policy allows origin

**Issue: Network error**
- Solution: Check API is running on port 5000

**Issue: Data not loading**
- Solution: Check browser console for errors

---

## Next Steps

1. ✅ Complete all test scenarios
2. ✅ Document any bugs found
3. ✅ Create test data
4. ✅ Automate tests
5. ✅ Performance optimization
6. ✅ Security audit
7. ✅ User acceptance testing

---

**Happy Testing! 🧪**
