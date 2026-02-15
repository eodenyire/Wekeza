# 🚀 Wekeza Banking System - Running Status

## ✅ System Status: FULLY OPERATIONAL

---

## 🟢 Running Services

### 1. Wekeza Core API (Backend)
- **Status**: ✅ Running
- **Port**: 5000
- **Process ID**: 2
- **URL**: http://localhost:5000
- **Swagger Documentation**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

### 2. Wekeza Web Channels (Frontend)
- **Status**: ✅ Running
- **Port**: 3000
- **Process ID**: 4
- **URL**: http://localhost:3000
- **Build Tool**: Vite v5.4.21

---

## 🌐 Available Channels

Access the following banking channels at http://localhost:3000:

### 1. Public Sector Portal ✨ NEW
- **Route**: `/public-sector`
- **URL**: http://localhost:3000/public-sector
- **Features**:
  - Securities Trading (T-Bills, Bonds, Stocks)
  - Government Lending
  - Government Banking Services
  - Grants & Philanthropy
  - Dashboard & Analytics
  - Accessibility Features (Keyboard navigation)
  - Multi-language Support (English & Swahili)

### 2. Personal Banking
- **Route**: `/personal-banking`
- **URL**: http://localhost:3000/personal-banking

### 3. Corporate Banking
- **Route**: `/corporate-banking`
- **URL**: http://localhost:3000/corporate-banking

### 4. SME Banking
- **Route**: `/sme-banking`
- **URL**: http://localhost:3000/sme-banking

### 5. Public Website
- **Route**: `/`
- **URL**: http://localhost:3000

---

## 🔑 Test Credentials

### Public Sector Portal Users

**Treasury Officer:**
- Username: `treasury.officer@wekeza.bank`
- Password: `Treasury@123`
- Access: Securities Trading, Dashboard

**Credit Officer:**
- Username: `credit.officer@wekeza.bank`
- Password: `Credit@123`
- Access: Government Lending, Dashboard

**Finance Officer:**
- Username: `finance.officer@wekeza.bank`
- Password: `Finance@123`
- Access: Government Banking, Dashboard

**CSR Manager:**
- Username: `csr.manager@wekeza.bank`
- Password: `CSR@123`
- Access: Grants & Philanthropy, Dashboard

**Compliance Officer:**
- Username: `compliance.officer@wekeza.bank`
- Password: `Compliance@123`
- Access: All modules (read-only), Dashboard

**Senior Management:**
- Username: `senior.manager@wekeza.bank`
- Password: `Manager@123`
- Access: Dashboard, Analytics, All modules

---

## 📊 API Endpoints

### Core Banking API (http://localhost:5000/api)

**Authentication:**
- POST `/api/authentication/login`
- POST `/api/authentication/logout`
- POST `/api/authentication/refresh`

**Public Sector - Securities:**
- GET `/api/public-sector/securities/treasury-bills`
- POST `/api/public-sector/securities/treasury-bills/order`
- GET `/api/public-sector/securities/bonds`
- POST `/api/public-sector/securities/bonds/order`
- GET `/api/public-sector/securities/stocks`
- POST `/api/public-sector/securities/stocks/order`
- GET `/api/public-sector/securities/portfolio`

**Public Sector - Lending:**
- GET `/api/public-sector/loans/applications`
- GET `/api/public-sector/loans/applications/{id}`
- POST `/api/public-sector/loans/applications/{id}/approve`
- POST `/api/public-sector/loans/applications/{id}/reject`
- POST `/api/public-sector/loans/{id}/disburse`
- GET `/api/public-sector/loans/portfolio`
- GET `/api/public-sector/loans/{id}/schedule`

**Public Sector - Banking:**
- GET `/api/public-sector/accounts`
- GET `/api/public-sector/accounts/{id}/transactions`
- POST `/api/public-sector/payments/bulk`
- GET `/api/public-sector/revenues`
- POST `/api/public-sector/revenues/reconcile`
- GET `/api/public-sector/reports`

**Public Sector - Grants:**
- GET `/api/public-sector/grants/programs`
- GET `/api/public-sector/grants/applications`
- POST `/api/public-sector/grants/applications`
- POST `/api/public-sector/grants/applications/{id}/approve`
- GET `/api/public-sector/grants/impact`

**Dashboard:**
- GET `/api/public-sector/dashboard/metrics`
- GET `/api/public-sector/dashboard/analytics`

---

## 🎯 Quick Start Guide

### 1. Access the Public Sector Portal
```
1. Open browser: http://localhost:3000/public-sector
2. Login with test credentials (see above)
3. Navigate through the modules
```

### 2. Test Securities Trading
```
1. Login as Treasury Officer
2. Navigate to Securities > T-Bills
3. Place an order for a T-Bill
4. View your portfolio
```

### 3. Test Government Lending
```
1. Login as Credit Officer
2. Navigate to Lending > Applications
3. Review loan applications
4. Approve/reject applications
5. Disburse approved loans
```

### 4. Test Government Banking
```
1. Login as Finance Officer
2. Navigate to Banking > Accounts
3. View account balances
4. Upload bulk payments (CSV/Excel)
5. Process payments
```

### 5. Test Grants & Philanthropy
```
1. Login as CSR Manager
2. Navigate to Grants > Programs
3. View available grant programs
4. Review applications
5. Approve grants with two-signatory workflow
```

---

## 🔧 Managing the Services

### View Process Status
```powershell
# List all running processes
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*" -or $_.ProcessName -like "*node*"}
```

### Stop Services
```powershell
# Stop API (Process ID: 2)
# Stop Web Channels (Process ID: 4)
# Or close the terminal windows
```

### Restart Services
```powershell
# Restart API
dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj

# Restart Web Channels
cd Wekeza.Web.Channels
npm run dev
```

### View Logs
```powershell
# API logs are in the terminal running Process ID: 2
# Web channels logs are in the terminal running Process ID: 4
```

---

## 📝 API Documentation

### Swagger UI
- **URL**: http://localhost:5000/swagger
- **Features**:
  - Interactive API documentation
  - Try out API endpoints
  - View request/response schemas
  - Authentication testing

### Health Checks
- **URL**: http://localhost:5000/health
- **Checks**:
  - Database connectivity
  - Redis cache (if configured)
  - External API integrations

---

## 🎨 Features Available

### Public Sector Portal Features
- ✅ Securities Trading (T-Bills, Bonds, Stocks)
- ✅ Government Lending (Applications, Approvals, Disbursements)
- ✅ Government Banking (Accounts, Payments, Revenues, Reports)
- ✅ Grants & Philanthropy (Programs, Applications, Impact)
- ✅ Dashboard & Analytics
- ✅ Role-based Access Control
- ✅ Audit Logging
- ✅ Error Handling
- ✅ Performance Optimizations
- ✅ Keyboard Navigation (Alt+D, Alt+S, Alt+L, Alt+B, Alt+G, Alt+H, Alt+Q)
- ✅ Multi-language Support (English & Swahili)
- ✅ Accessibility Features (Screen readers, ARIA labels)

---

## 🧪 Testing

### Run Property Tests
```bash
cd Wekeza.Web.Channels
npm test -- --run
```

### Run Specific Tests
```bash
# Securities tests
npm test -- pages/securities/*.property.test.ts --run

# Lending tests
npm test -- pages/lending/*.property.test.ts --run

# All property tests
npm test -- **/*.property.test.ts --run
```

---

## 🐛 Troubleshooting

### API Not Starting
1. Check if PostgreSQL is running
2. Verify database connection string in `appsettings.json`
3. Check port 5000 is not in use
4. Review API logs for errors

### Web Channels Not Starting
1. Check if node_modules are installed: `npm install`
2. Verify port 3000 is not in use
3. Check if API is running on port 5000
4. Review browser console for errors

### Cannot Login
1. Verify API is running and accessible
2. Check network tab in browser dev tools
3. Verify credentials are correct
4. Check API logs for authentication errors

### CORS Errors
1. Verify API CORS configuration allows localhost:3000
2. Check browser console for specific CORS errors
3. Ensure API is running on correct port

---

## 📊 System Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Browser (Client)                      │
│                  http://localhost:3000                   │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ HTTP/HTTPS
                     │
┌────────────────────▼────────────────────────────────────┐
│              Wekeza Web Channels (Vite)                  │
│  ┌──────────┬──────────┬──────────┬──────────┬────────┐ │
│  │ Public   │ Personal │Corporate │   SME    │ Public │ │
│  │ Sector   │ Banking  │ Banking  │ Banking  │Website │ │
│  └──────────┴──────────┴──────────┴──────────┴────────┘ │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ REST API
                     │
┌────────────────────▼────────────────────────────────────┐
│           Wekeza Core API (.NET 8)                       │
│              http://localhost:5000                       │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Controllers, Services, Domain Logic              │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────────┬────────────────────────────────────┘
                     │
                     │ Entity Framework Core
                     │
┌────────────────────▼────────────────────────────────────┐
│              PostgreSQL Database                         │
│              WekezaCoreDB                                │
└─────────────────────────────────────────────────────────┘
```

---

## 🎉 Success!

Your Wekeza Banking System is now fully operational!

- ✅ Backend API running on port 5000
- ✅ Frontend channels running on port 3000
- ✅ Public Sector Portal accessible
- ✅ All features implemented and tested
- ✅ Ready for interaction and testing

**Next Steps:**
1. Open http://localhost:3000/public-sector
2. Login with test credentials
3. Explore the features
4. Test the functionality
5. Review the API documentation at http://localhost:5000/swagger

---

**System Started:** February 14, 2026  
**Status:** 🟢 OPERATIONAL  
**API**: http://localhost:5000  
**Web**: http://localhost:3000  
**Swagger**: http://localhost:5000/swagger
