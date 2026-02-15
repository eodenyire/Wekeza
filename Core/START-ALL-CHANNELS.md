# 🚀 Wekeza Banking System - Complete Startup Guide

## Overview

This guide will help you start and test the complete Wekeza Banking System with all channels.

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    WEKEZA BANKING SYSTEM                     │
└─────────────────────────────────────────────────────────────┘

Backend API (Port 5000)
├── Wekeza.Core.Api
│   ├── 17 Banking Modules
│   ├── 100+ API Endpoints
│   └── PostgreSQL Database

Frontend Channels
├── Public Website (Port 3000)
│   └── Marketing, Account Opening
├── Personal Banking (Port 3001)
│   └── Retail Customer Portal
├── Corporate Banking (Port 3002)
│   └── Business Customer Portal
└── SME Banking (Port 3003)
    └── Small Business Portal
```

## Prerequisites

✅ .NET 8.0 SDK
✅ Node.js 18+ and npm
✅ PostgreSQL 15+
✅ Running Wekeza.Core.Api on port 5000

## Step-by-Step Startup

### Step 1: Start the Backend API

```powershell
# Make sure PostgreSQL is running
& "C:\Program Files\PostgreSQL\15\bin\pg_ctl.exe" start -D "C:\Program Files\PostgreSQL\15\data"

# Start the Wekeza Core API
cd Core
dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj
```

**Verify API is running:**
- Open browser: http://localhost:5000
- Check Swagger: http://localhost:5000/swagger
- Test endpoint: http://localhost:5000/api

### Step 2: Install Dependencies for All Channels

```powershell
# Navigate to web channels folder
cd Wekeza.Web.Channels

# Install dependencies for each channel
cd personal-banking
npm install
cd ..

cd corporate-banking
npm install
cd ..

cd sme-banking
npm install
cd ..

cd public-website
npm install
cd ..
```

### Step 3: Start All Channels

Open **4 separate PowerShell terminals** and run:

**Terminal 1 - Public Website:**
```powershell
cd Wekeza.Web.Channels/public-website
npm run dev
# Runs on http://localhost:3000
```

**Terminal 2 - Personal Banking:**
```powershell
cd Wekeza.Web.Channels/personal-banking
npm run dev
# Runs on http://localhost:3001
```

**Terminal 3 - Corporate Banking:**
```powershell
cd Wekeza.Web.Channels/corporate-banking
npm run dev
# Runs on http://localhost:3002
```

**Terminal 4 - SME Banking:**
```powershell
cd Wekeza.Web.Channels/sme-banking
npm run dev
# Runs on http://localhost:3003
```

## Testing the System

### 1. Test Backend API

Open PowerShell and test:

```powershell
# Test API is alive
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get

# Test authentication
$body = @{
    username = "admin"
    password = "test123"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" -Method Post -Body $body -ContentType "application/json"
```

### 2. Test Public Website

**URL:** http://localhost:3000

**Test Scenarios:**
- ✅ Navigate to Home page
- ✅ View Products page
- ✅ View About page
- ✅ Submit Contact form
- ✅ Start account opening process
- ✅ Click "Login" to go to Personal Banking

### 3. Test Personal Banking

**URL:** http://localhost:3001

**Test Login:**
```
Username: admin
Password: test123
```

**Test Scenarios:**
- ✅ Login with credentials
- ✅ View Dashboard with account summary
- ✅ View Accounts list
- ✅ View account transactions
- ✅ Transfer funds between accounts
- ✅ Pay bills
- ✅ View and request cards
- ✅ Apply for loan
- ✅ Update profile

### 4. Test Corporate Banking

**URL:** http://localhost:3002

**Test Login:**
```
Username: corporate_admin
Password: test123
```

**Test Scenarios:**
- ✅ Login with corporate credentials
- ✅ View corporate dashboard
- ✅ View multiple accounts
- ✅ Upload bulk payment file
- ✅ Create trade finance request
- ✅ Approve pending transactions
- ✅ Generate reports

### 5. Test SME Banking

**URL:** http://localhost:3003

**Test Login:**
```
Username: sme_user
Password: test123
```

**Test Scenarios:**
- ✅ Login with SME credentials
- ✅ View business dashboard
- ✅ View business accounts
- ✅ Apply for business loan
- ✅ Process payroll
- ✅ View business analytics

## API Endpoints Reference

### Authentication
- `POST /api/authentication/login` - Login
- `GET /api/authentication/me` - Get current user

### Customer Portal (Personal Banking)
- `GET /api/customer-portal/profile` - Get profile
- `GET /api/customer-portal/accounts` - Get accounts
- `GET /api/customer-portal/accounts/{id}/transactions` - Get transactions
- `POST /api/customer-portal/transactions/transfer` - Transfer funds
- `POST /api/customer-portal/transactions/pay-bill` - Pay bill
- `GET /api/customer-portal/cards` - Get cards
- `POST /api/customer-portal/cards/request-virtual` - Request virtual card
- `GET /api/customer-portal/loans` - Get loans
- `POST /api/customer-portal/loans/apply` - Apply for loan

### Accounts (Corporate/SME)
- `POST /api/accounts/individual` - Open individual account
- `POST /api/accounts/business` - Register business
- `GET /api/accounts/{accountNumber}/summary` - Get account summary
- `PATCH /api/accounts/{accountNumber}/freeze` - Freeze account

### Loans
- `POST /api/loans/apply` - Apply for loan
- `POST /api/loans/approve` - Approve loan
- `POST /api/loans/disburse` - Disburse loan
- `POST /api/loans/repayment` - Process repayment
- `GET /api/loans/{loanId}` - Get loan details
- `GET /api/loans/portfolio` - Get loan portfolio

### Products
- `GET /api/products/catalog` - Get product catalog
- `GET /api/products/deposits` - Get deposit products
- `GET /api/products/loans` - Get loan products

### Dashboard
- `GET /api/dashboard/transactions/trends` - Transaction trends
- `GET /api/dashboard/accounts/statistics` - Account statistics
- `GET /api/dashboard/loans/portfolio` - Loan portfolio stats

### Trade Finance (Corporate)
- `POST /api/tradefinance/letter-of-credit` - Create LC
- `GET /api/tradefinance/letters-of-credit` - Get LCs
- `POST /api/tradefinance/bank-guarantee` - Create guarantee

### Treasury (Corporate)
- `POST /api/treasury/fx-deal` - Create FX deal
- `GET /api/treasury/fx-deals` - Get FX deals
- `POST /api/treasury/money-market-deal` - Create MM deal

### Workflows (Corporate)
- `GET /api/workflows/pending-approvals` - Get pending approvals
- `POST /api/workflows/approve` - Approve workflow
- `POST /api/workflows/reject` - Reject workflow

## Common Issues & Solutions

### Issue 1: API Not Responding

**Solution:**
```powershell
# Check if API is running
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*"}

# Check port 5000
netstat -ano | findstr "5000"

# Restart API
cd Core
dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj
```

### Issue 2: CORS Errors

**Solution:** The API already has CORS enabled for all origins. If you still see errors, check browser console.

### Issue 3: 401 Unauthorized

**Solution:** 
- Check if token is stored in localStorage
- Re-login to get fresh token
- Check token expiry (1 hour default)

### Issue 4: Database Connection Failed

**Solution:**
```powershell
# Check PostgreSQL is running
Get-Service postgresql*

# Start PostgreSQL
& "C:\Program Files\PostgreSQL\15\bin\pg_ctl.exe" start -D "C:\Program Files\PostgreSQL\15\data"

# Test connection
$env:PGPASSWORD="the_beast_pass"
psql -h localhost -p 5432 -U admin -d WekezaCoreDB
```

### Issue 5: Port Already in Use

**Solution:**
```powershell
# Find process using port
netstat -ano | findstr "3001"

# Kill process (replace PID)
taskkill /PID <PID> /F

# Or change port in vite.config.ts
```

## Development Workflow

### Making Changes

1. **Backend Changes:**
   - Edit files in `Wekeza.Core.Api`
   - API auto-reloads with hot reload
   - Check logs in terminal

2. **Frontend Changes:**
   - Edit files in respective channel folder
   - Vite auto-reloads browser
   - Check browser console for errors

### Testing Flow

1. **Test API First:**
   ```powershell
   # Test endpoint directly
   Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/accounts" -Headers @{Authorization="Bearer YOUR_TOKEN"}
   ```

2. **Test in Browser:**
   - Open browser DevTools (F12)
   - Go to Network tab
   - Perform action in UI
   - Check API request/response

3. **Check Logs:**
   - Backend: Check terminal running API
   - Frontend: Check browser console
   - Database: Check PostgreSQL logs

## Production Deployment

### Backend Deployment

```powershell
# Build for production
cd Core
dotnet publish Wekeza.Core.Api/Wekeza.Core.Api.csproj -c Release -o ./publish

# Run production build
cd publish
dotnet Wekeza.Core.Api.dll
```

### Frontend Deployment

```powershell
# Build each channel
cd Wekeza.Web.Channels/personal-banking
npm run build
# Output in dist/ folder

# Deploy to web server (IIS, Nginx, etc.)
# Or use services like Netlify, Vercel
```

## Quick Reference

### URLs
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Public Website**: http://localhost:3000
- **Personal Banking**: http://localhost:3001
- **Corporate Banking**: http://localhost:3002
- **SME Banking**: http://localhost:3003

### Default Credentials
- **Admin**: admin / test123
- **Teller**: teller / test123
- **Corporate**: corporate_admin / test123
- **SME**: sme_user / test123

### Key Directories
- **Backend**: `Core/Wekeza.Core.Api`
- **Frontend**: `Wekeza.Web.Channels`
- **Database**: PostgreSQL on port 5432

## Next Steps

1. ✅ Start all services
2. ✅ Test each channel
3. ✅ Create test data
4. ✅ Test end-to-end flows
5. ✅ Customize branding
6. ✅ Add more features
7. ✅ Deploy to production

## Support

For issues or questions:
- Check API logs in terminal
- Check browser console (F12)
- Review Swagger documentation
- Check this guide

---

**Happy Banking! 🏦**
