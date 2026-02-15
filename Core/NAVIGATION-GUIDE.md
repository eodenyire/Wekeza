# 🧭 Wekeza Public Sector Portal - Navigation Guide

## What You're Seeing Now

You're currently on the **Dashboard** page, which shows:
- ✅ Securities Portfolio: KES 16,543,380,000
- ✅ Loan Portfolio: KES 65,000,000,000
- ✅ Account Balance: KES 225,000,000,000
- ✅ Grants Disbursed: KES 12,800,000,000
- ✅ Two charts: Securities Portfolio Composition & Loan Portfolio by Entity

## How to Access Other Features

### 📍 Look at the LEFT SIDEBAR

You should see a navigation menu with these options:

1. **📊 Dashboard** (currently active - green background)
2. **📈 Securities** - Click to access Securities Trading
3. **💰 Lending** - Click to access Government Lending
4. **🏦 Banking** - Click to access Banking Services
5. **🎁 Grants** - Click to access Grants & Philanthropy

---

## 🎯 Step-by-Step: Access Each Module

### 1. Securities Trading Module

**Click on "Securities" in the sidebar**

You'll see sub-pages:
- **Treasury Bills** - Trade 91-day, 182-day, 364-day T-Bills
- **Bonds** - Trade government bonds
- **Stocks** - Trade NSE-listed stocks
- **Portfolio** - View your securities portfolio

**What to do**:
1. Click "Securities" in sidebar
2. You'll see the securities overview
3. Click on any sub-section to explore

---

### 2. Government Lending Module

**Click on "Lending" in the sidebar**

You'll see sub-pages:
- **Applications** - View loan applications from governments
- **Loan Details** - Review and approve loans
- **Disbursements** - Disburse approved loans
- **Portfolio** - View loan portfolio and repayments

**What to do**:
1. Click "Lending" in sidebar
2. Browse loan applications
3. Click on any loan to see details
4. Approve or reject loans

---

### 3. Banking Services Module ⭐ MOST IMPORTANT

**Click on "Banking" in the sidebar**

This is where the NEW features are:

#### A. Payment Workflow (Maker-Checker-Approver)
- **Initiate Payment** - Start a new payment
- **Pending Approvals** - Approve/reject payments
- **Approval History** - View audit trail

#### B. Bulk Payments
- **Upload File** - Upload CSV with multiple payments
- **Validate Batch** - Check for errors
- **Execute Batch** - Process all payments

#### C. Budget Control
- **Budget Allocations** - View department budgets
- **Create Commitment** - Reserve budget funds
- **Budget Alerts** - See departments approaching limits
- **Utilization Reports** - Track spending

#### D. Other Banking Features
- **Accounts** - View government accounts
- **Revenues** - Track revenue collections
- **Reports** - Generate financial reports

**What to do**:
1. Click "Banking" in sidebar
2. You'll see multiple tabs/sections
3. Explore each section

---

### 4. Grants & Philanthropy Module

**Click on "Grants" in the sidebar**

You'll see sub-pages:
- **Programs** - Available grant programs
- **Applications** - Submit and track applications
- **Approvals** - Approve grant applications
- **Impact** - View impact reports and metrics

**What to do**:
1. Click "Grants" in sidebar
2. Browse grant programs
3. Submit applications
4. Track impact

---

## 🚀 Quick Actions from Dashboard

On the dashboard, you should also see:

### Quick Action Buttons
Look for buttons like:
- "Initiate Payment"
- "View Pending Approvals"
- "Upload Bulk Payments"
- "View Budget"

These provide shortcuts to key features.

---

## 🔍 If You Don't See the Sidebar

### On Desktop:
- The sidebar should be visible on the left side
- It has a white background
- Navigation items are listed vertically

### On Mobile/Small Screen:
- Click the **☰ Menu** icon (three horizontal lines) in the top-left
- This will open the sidebar
- Click any navigation item
- Sidebar will close automatically

---

## 🎨 Visual Indicators

### Active Page
- The current page has a **green background** in the sidebar
- Text is **white** on the active item

### Hover Effect
- When you hover over a navigation item, it gets a **gray background**
- This shows it's clickable

---

## 📱 Testing the New Features

### Test Payment Workflow:
1. Click **"Banking"** in sidebar
2. Look for **"Payments"** or **"Payment Workflow"** section
3. Click **"Initiate Payment"**
4. Fill in the form
5. Submit for approval

### Test Bulk Payments:
1. Click **"Banking"** in sidebar
2. Look for **"Bulk Payments"** section
3. Click **"Upload File"**
4. Select `sample-bulk-payments.csv`
5. Upload and validate

### Test Budget Control:
1. Click **"Banking"** in sidebar
2. Look for **"Budget"** section
3. View allocations
4. Create a commitment
5. Check alerts

---

## 🐛 Troubleshooting

### "I don't see the sidebar"
**Solution**: 
- Refresh the page (F5)
- Check browser width (sidebar hides on very small screens)
- Click the menu icon (☰) in top-left

### "I only see Dashboard"
**Solution**:
- Look for navigation items in the left sidebar
- Make sure you're logged in as "admin"
- Check that the page has fully loaded

### "Navigation items are grayed out"
**Solution**:
- This means you don't have permission for that module
- Login as "admin" to access all modules
- Check your user role

### "Banking section doesn't show new features"
**Solution**:
- The Banking module might have sub-routes
- Look for tabs or sub-navigation within Banking
- Check the URL - it should be `/public-sector/banking/*`

---

## 🎯 Expected URLs

When you navigate, your browser URL should change:

- Dashboard: `http://localhost:3000/public-sector/dashboard`
- Securities: `http://localhost:3000/public-sector/securities`
- Lending: `http://localhost:3000/public-sector/lending`
- Banking: `http://localhost:3000/public-sector/banking`
- Grants: `http://localhost:3000/public-sector/grants`

---

## 💡 Pro Tips

1. **Use Keyboard Shortcuts**:
   - `Ctrl + K`: Quick search
   - `Ctrl + D`: Go to dashboard

2. **Breadcrumbs**:
   - Look at the top of the page
   - Shows your current location
   - Click to navigate back

3. **Quick Actions**:
   - Dashboard has quick action cards
   - Click on any metric card for details

4. **Export Data**:
   - Most pages have an "Export" button
   - Download reports as CSV, Excel, or PDF

---

## 🆘 Still Can't Find Features?

### Check if Frontend is Running:
```powershell
# In Wekeza.Web.Channels folder
npm run dev
```

### Check if API is Running:
```powershell
# In Wekeza.Core.Api folder
dotnet run
```

### Verify Login:
- Username: `admin`
- Password: `password123`
- Role should show as "Administrator" or "Senior Management"

---

## 📞 Need Help?

If you still can't access the features:

1. **Take a screenshot** of what you see
2. **Check the browser console** (F12) for errors
3. **Verify the URL** in the address bar
4. **Check if sidebar is visible** on the left

The features ARE there - they're just accessed through the sidebar navigation!

---

**Remember**: The dashboard is just the overview. Click on the sidebar items to access the full features! 🚀
