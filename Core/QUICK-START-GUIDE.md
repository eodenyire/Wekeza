# 🚀 Wekeza Public Sector Portal - Quick Start Guide

## Get Started in 5 Minutes

This guide will help you quickly access and test the Wekeza Public Sector Portal.

---

## ✅ Prerequisites

- ✅ Wekeza Core API running on http://localhost:5000
- ✅ PostgreSQL database running with `wekezacoredb`
- ✅ Sample data loaded (already done)

---

## 🔐 Step 1: Access the Portal

### Web Portal
1. Open your browser
2. Navigate to: **http://localhost:3000/public-sector/login**
3. Login credentials:
   - **Username**: `admin`
   - **Password**: `password123`

### API Documentation
- Swagger UI: **http://localhost:5000/swagger**
- Health Check: **http://localhost:5000/health**

---

## 🎯 Step 2: Explore Key Features

### A. Dashboard (First Page After Login)
**What you'll see**:
- Securities portfolio: KES 16.5B
- Loan portfolio: KES 65B
- Banking operations: KES 265B
- Grant disbursements: KES 12.8B
- 4 interactive charts with 12-month trends

**Try this**:
- Click on different chart segments
- View recent activities
- Check quick actions

---

### B. Payment Workflow (Maker-Checker-Approver)

#### Initiate a Payment
1. Go to **Banking → Payments**
2. Click **"Initiate Payment"**
3. Fill in the form:
   - Amount: KES 5,000,000
   - Beneficiary: ABC Suppliers Ltd
   - Account: 1234567890
   - Purpose: Office supplies
4. Click **"Submit for Approval"**

#### Approve a Payment
1. Go to **Banking → Pending Approvals**
2. You'll see your payment waiting
3. Click **"Approve"**
4. Add comment: "Budget verified"
5. Click **"Confirm Approval"**

#### View Approval History
1. Go to **Banking → Approval History**
2. See complete audit trail
3. View all approvers and comments

---

### C. Bulk Payments

#### Upload Payment File
1. Go to **Banking → Bulk Payments**
2. Click **"Upload File"**
3. Select file: `sample-bulk-payments.csv`
4. Choose account: Government Main Account
5. Click **"Upload"**

#### Validate Batch
1. After upload, click **"Validate"**
2. System checks:
   - Account numbers
   - Amounts
   - Duplicates
   - Balance availability
3. View validation results

#### Execute Batch
1. If validation passes, click **"Execute"**
2. Watch real-time progress
3. View success/failure summary

---

### D. Budget Control

#### View Budget Allocations
1. Go to **Banking → Budget**
2. See all department budgets
3. View utilization percentages
4. Check alert levels

#### Create Budget Commitment
1. Click **"Create Commitment"**
2. Select department
3. Enter amount: KES 5,000,000
4. Purpose: Infrastructure project
5. Click **"Create"**

#### View Budget Alerts
1. Go to **Budget → Alerts**
2. See departments approaching limits
3. Alert levels:
   - 🟢 NORMAL: > 20% available
   - 🟡 MEDIUM: 10-20% available
   - 🟠 HIGH: 0-10% available
   - 🔴 CRITICAL: 0% available

---

### E. Securities Trading

#### View Treasury Bills
1. Go to **Securities → Treasury Bills**
2. See available T-Bills:
   - 91-day
   - 182-day
   - 364-day
3. View rates and maturity dates

#### Place an Order
1. Click **"Place Order"** on any T-Bill
2. Enter amount: KES 1,000,000
3. Select bid type: Non-competitive
4. Click **"Submit Order"**

#### View Portfolio
1. Go to **Securities → Portfolio**
2. See all your securities
3. View current valuations
4. Check maturity calendar

---

### F. Government Lending

#### View Loan Applications
1. Go to **Lending → Applications**
2. See pending applications
3. Filter by status
4. View credit assessments

#### Approve a Loan
1. Click on an application
2. Review details
3. Click **"Approve"**
4. Add comments
5. Confirm approval

#### Disburse a Loan
1. Go to **Lending → Disbursements**
2. See approved loans
3. Click **"Disburse"**
4. Enter account details
5. Confirm disbursement

---

### G. Grants & Philanthropy

#### View Grant Programs
1. Go to **Grants → Programs**
2. See available programs
3. View eligibility criteria

#### Submit Application
1. Click **"Apply"** on a program
2. Fill in application form
3. Upload documents
4. Submit application

#### Approve Grant
1. Go to **Grants → Approvals**
2. See pending applications
3. Review and approve
4. Two signatories required

---

## 🧪 Step 3: Run Tests

### Test Payment Workflow
```powershell
./test-payment-workflow.ps1
```

**What it tests**:
- Payment initiation
- Pending approvals
- Payment approval
- Payment rejection
- Approval history

**Expected result**: ✅ All 6 tests pass

---

### Test All Features
```powershell
./test-all-features.ps1
```

**What it tests**:
- Budget control (4 tests)
- Bulk payments (4 tests)
- Payment workflow (2 tests)
- Dashboard (2 tests)

**Expected result**: ✅ All 12 tests pass

---

## 📊 Step 4: View Reports

### Dashboard Reports
1. Go to **Dashboard**
2. Click **"Export"** on any chart
3. Choose format: CSV, Excel, or PDF

### Financial Reports
1. Go to **Banking → Reports**
2. Select report type:
   - Department expenditure
   - Supplier payments
   - Budget performance
   - Revenue collection
3. Set date range
4. Click **"Generate"**
5. Download report

---

## 🔍 Step 5: Explore Advanced Features

### Audit Trail
1. Go to **Compliance → Audit Trail**
2. See all system activities
3. Filter by:
   - User
   - Action type
   - Date range
   - Entity type

### Budget Utilization
1. Go to **Banking → Budget → Utilization**
2. See department-wise spending
3. View charts and graphs
4. Export to Excel

### Loan Portfolio
1. Go to **Lending → Portfolio**
2. See all active loans
3. View exposure by entity
4. Check NPL ratio
5. View risk metrics

---

## 💡 Tips & Tricks

### Navigation
- Use the sidebar menu for quick access
- Breadcrumbs show your current location
- Back button returns to previous page

### Search
- Use search bar to find transactions
- Filter by date, amount, or status
- Sort by any column

### Keyboard Shortcuts
- `Ctrl + K`: Quick search
- `Ctrl + D`: Go to dashboard
- `Esc`: Close modals

### Mobile Access
- Portal is fully responsive
- Works on tablets and phones
- Touch-friendly interface

---

## 🆘 Troubleshooting

### Can't Login?
- Check username: `admin`
- Check password: `password123`
- Clear browser cache
- Try incognito mode

### API Not Responding?
- Check if API is running: http://localhost:5000/health
- Restart API: `dotnet run` in `Wekeza.Core.Api` folder
- Check database connection

### Data Not Showing?
- Verify database is running
- Check if sample data is loaded
- Run: `psql -h localhost -U postgres -d wekezacoredb -f seed-public-sector-data.sql`

### Payment Fails?
- Check account balance
- Verify budget availability
- Check approval limits
- Review error message

---

## 📚 Additional Resources

### Documentation
- **API Docs**: http://localhost:5000/swagger
- **Complete Status**: `COMPLETE-IMPLEMENTATION-STATUS.md`
- **Final Summary**: `FINAL-IMPLEMENTATION-SUMMARY.md`
- **Implementation Roadmap**: `IMPLEMENTATION-ROADMAP.md`

### Test Files
- **Payment Workflow Test**: `test-payment-workflow.ps1`
- **All Features Test**: `test-all-features.ps1`
- **Sample Bulk Payments**: `sample-bulk-payments.csv`

### Database Scripts
- **Schema Creation**: `create-workflow-schema.sql`
- **Sample Data**: `seed-public-sector-data.sql`
- **Comprehensive Data**: `seed-comprehensive-data.sql`

---

## 🎯 Next Steps

### For Developers
1. Review API documentation
2. Explore database schema
3. Run test scripts
4. Add custom features

### For Users
1. Complete user training
2. Test all workflows
3. Provide feedback
4. Request enhancements

### For Administrators
1. Configure user roles
2. Set approval limits
3. Define budget allocations
4. Monitor system health

---

## 📞 Support

### Getting Help
- Check documentation first
- Review error messages
- Check system logs
- Contact support team

### System Status
- API Health: http://localhost:5000/health
- Database Status: Check PostgreSQL service
- Application Logs: `Wekeza.Core.Api/logs/`

---

## 🎉 You're Ready!

You now have access to a **world-class government banking platform** with:

✅ **46+ API Endpoints** - All functional
✅ **21 Frontend Pages** - Complete UI
✅ **16 Database Tables** - Comprehensive data
✅ **12 Months History** - Real trends
✅ **6 User Roles** - Complete RBAC

**Start exploring and enjoy the power of modern government banking!**

---

**Built with ❤️ for Government Banking Excellence**

**Version**: 1.0.0
**Date**: February 15, 2026
**Status**: Production Ready ✅
