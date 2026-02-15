# 🎯 Quick Access Guide - Public Sector Portal

## 🚀 System is Running!

Both the backend API and frontend web channels are now operational.

---

## 🌐 Access the Public Sector Portal

### Direct URL
```
http://localhost:3000/public-sector
```

### Step-by-Step Access
1. Open your web browser
2. Navigate to: `http://localhost:3000/public-sector`
3. You'll see the Public Sector Portal login page
4. Use one of the test credentials below

---

## 🔑 Test Credentials

### Option 1: Treasury Officer (Securities Trading)
```
Username: treasury.officer@wekeza.bank
Password: Treasury@123

Access to:
- Securities Trading (T-Bills, Bonds, Stocks)
- Portfolio Management
- Dashboard
```

### Option 2: Credit Officer (Government Lending)
```
Username: credit.officer@wekeza.bank
Password: Credit@123

Access to:
- Loan Applications
- Credit Assessment
- Loan Disbursements
- Loan Portfolio
- Dashboard
```

### Option 3: Finance Officer (Government Banking)
```
Username: finance.officer@wekeza.bank
Password: Finance@123

Access to:
- Account Management
- Bulk Payments
- Revenue Collection
- Financial Reports
- Dashboard
```

### Option 4: CSR Manager (Grants & Philanthropy)
```
Username: csr.manager@wekeza.bank
Password: CSR@123

Access to:
- Grant Programs
- Grant Applications
- Approvals (Two-signatory)
- Impact Tracking
- Dashboard
```

### Option 5: Senior Management (Full Access)
```
Username: senior.manager@wekeza.bank
Password: Manager@123

Access to:
- Dashboard & Analytics
- All Modules (Overview)
- Reports
```

---

## 🎨 Features to Test

### 1. Securities Trading
1. Login as Treasury Officer
2. Navigate to **Securities** > **T-Bills**
3. View available T-Bills (91-day, 182-day, 364-day)
4. Place an order (competitive or non-competitive bid)
5. Go to **Portfolio** to see your holdings
6. Check **Bonds** and **Stocks** sections

### 2. Government Lending
1. Login as Credit Officer
2. Navigate to **Lending** > **Applications**
3. View pending loan applications
4. Click on an application to see details
5. Review creditworthiness assessment
6. Approve or reject the application
7. Go to **Disbursements** to process approved loans
8. Check **Portfolio** for active loans

### 3. Government Banking
1. Login as Finance Officer
2. Navigate to **Banking** > **Accounts**
3. View government account balances
4. Go to **Payments** > Upload bulk payment file (CSV/Excel)
5. Preview and process payments
6. Check **Revenues** for collection tracking
7. Generate **Reports**

### 4. Grants & Philanthropy
1. Login as CSR Manager
2. Navigate to **Grants** > **Programs**
3. View available grant programs
4. Go to **Applications** to see submissions
5. Review application details
6. Go to **Approvals** for two-signatory workflow
7. Check **Impact** for utilization and metrics

### 5. Dashboard & Analytics
1. Login with any user
2. View the **Dashboard**
3. See key metrics cards
4. Explore charts:
   - Securities portfolio composition
   - Loan portfolio by entity
   - Revenue trends
   - Grant impact metrics
5. Check recent activities
6. Use quick actions

---

## ⌨️ Keyboard Shortcuts

The portal supports keyboard navigation:

- `Alt + D` - Go to Dashboard
- `Alt + S` - Go to Securities
- `Alt + L` - Go to Lending
- `Alt + B` - Go to Banking
- `Alt + G` - Go to Grants
- `Alt + H` - Show keyboard shortcuts help
- `Alt + Q` - Logout
- `Esc` - Close modals/dialogs

---

## 🌍 Language Switching

The portal supports English and Swahili:

1. Look for the language switcher in the header (🇬🇧 🇰🇪)
2. Click to switch between English and Swahili
3. All UI text will be translated
4. Language preference is saved

---

## 🔍 API Documentation

### Swagger UI
```
http://localhost:5000/swagger
```

Features:
- Interactive API documentation
- Test API endpoints directly
- View request/response schemas
- Authentication testing

### API Base URL
```
http://localhost:5000/api
```

---

## 📊 What You Can Do

### Securities Trading
- ✅ View available T-Bills, Bonds, and Stocks
- ✅ Place buy/sell orders
- ✅ Track portfolio performance
- ✅ View maturity calendar
- ✅ Calculate yields and returns

### Government Lending
- ✅ Submit loan applications
- ✅ Review creditworthiness
- ✅ Approve/reject applications
- ✅ Disburse loans
- ✅ Track repayment schedules
- ✅ Monitor portfolio risk

### Government Banking
- ✅ Manage government accounts
- ✅ Process bulk payments
- ✅ Track revenue collections
- ✅ Reconcile accounts
- ✅ Generate financial reports
- ✅ IFMIS integration status

### Grants & Philanthropy
- ✅ Browse grant programs
- ✅ Submit applications
- ✅ Two-signatory approval workflow
- ✅ Track grant utilization
- ✅ Monitor compliance
- ✅ View impact metrics

### Dashboard
- ✅ Real-time metrics
- ✅ Interactive charts
- ✅ Recent activities
- ✅ Quick actions
- ✅ Export data (CSV, Excel, PDF)

---

## 🧪 Testing Scenarios

### Scenario 1: Complete Securities Trade
1. Login as Treasury Officer
2. Navigate to Securities > T-Bills
3. Select a 91-day T-Bill
4. Place a non-competitive bid for KES 100,000
5. Submit the order
6. View confirmation
7. Check Portfolio to see the order

### Scenario 2: Loan Approval Workflow
1. Login as Credit Officer
2. Navigate to Lending > Applications
3. Select a pending application
4. Review entity details and credit score
5. Add approval comments
6. Approve the application
7. Go to Disbursements
8. Process the disbursement

### Scenario 3: Bulk Payment Processing
1. Login as Finance Officer
2. Navigate to Banking > Payments
3. Upload a CSV file with payment details
4. Review the payment preview
5. Validate all payments
6. Execute the bulk payment
7. Check payment status

### Scenario 4: Grant Application Review
1. Login as CSR Manager
2. Navigate to Grants > Applications
3. Select a pending application
4. Review project details and budget
5. Go to Approvals
6. First signatory approves
7. Second signatory approves
8. Check Impact for utilization

---

## 🐛 Troubleshooting

### Cannot Access Portal
- Verify web channels are running: http://localhost:3000
- Check browser console for errors
- Try refreshing the page

### Login Fails
- Verify API is running: http://localhost:5000
- Check credentials are correct
- Look at network tab in browser dev tools
- Check API logs for authentication errors

### Features Not Loading
- Check API is responding: http://localhost:5000/api
- Verify network connectivity
- Check browser console for errors
- Review API logs

### Keyboard Shortcuts Not Working
- Ensure you're not in an input field
- Try clicking outside any input first
- Press Alt+H to see shortcuts help

---

## 📱 Browser Compatibility

Tested and working on:
- ✅ Chrome/Edge (Recommended)
- ✅ Firefox
- ✅ Safari
- ✅ Opera

---

## 🎉 You're All Set!

The Public Sector Portal is ready for testing and interaction.

**Quick Start:**
1. Open: http://localhost:3000/public-sector
2. Login: treasury.officer@wekeza.bank / Treasury@123
3. Explore: Navigate through Securities, Lending, Banking, Grants
4. Test: Try placing orders, approving loans, processing payments

**Need Help?**
- API Docs: http://localhost:5000/swagger
- Keyboard Shortcuts: Press Alt+H in the portal
- System Status: Check SYSTEM-RUNNING-STATUS.md

---

**Happy Testing!** 🚀

The Wekeza Public Sector Portal is now live and ready for comprehensive testing and interaction with the core banking system.
