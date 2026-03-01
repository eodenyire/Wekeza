# 🏗️ Real Data Architecture Overview

## System Flow: From Database to UI

```
┌─────────────────────────────────────────────────────────────────┐
│                      WEKEZA BANK PORTAL                         │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  1. USER LOGS IN                                               │
│  ┌──────────────────────┐                                      │
│  │ manager1             │ (Real credentials from database)     │
│  │ Manager@123          │                                      │
│  └──────────────────────┘                                      │
│           │                                                     │
│           ↓                                                     │
│  ┌──────────────────────────────────────────────┐             │
│  │ POST /api/Authentication/login               │             │
│  │ ├─ Hash password with BCrypt                 │             │
│  │ ├─ Query: SELECT FROM Users WHERE username= │             │
│  │ ├─ Verify password hash matches              │             │
│  │ └─ Return JWT token if valid                 │             │
│  └──────────────────────────────────────────────┘             │
│           │                                                     │
│           ↓                                                     │
│  ┌──────────────────────┐                                      │
│  │ JWT Token Generated  │ (Valid for 1 hour)                 │
│  │ Claims:              │                                      │
│  │ - user_id (sub)      │                                      │
│  │ - username           │                                      │
│  │ - email              │                                      │
│  │ - roles              │                                      │
│  └──────────────────────┘                                      │
│                                                                 │
│  2. DASHBOARD LOADS                                            │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ BranchDashboard Component mounts (React)               │  │
│  │ useEffect → loadDashboardData()                        │  │
│  └────────────────────────────────────────────────────────┘  │
│           │                                                     │
│           ├─ Call: branchManagerApi.getDashboard()            │
│           ├─ Call: branchManagerApi.getTellerPerformance()    │
│           └─ Call: branchManagerApi.getPendingRequests()      │
│                                                                 │
│  3. API CALLS WITH JWT TOKEN                                   │
│  ┌────────────────────────────┐                               │
│  │ GET /api/branch-manager/   │ + Bearer Token header         │
│  │  dashboard                 │                               │
│  └────────────────────────────┘                               │
│           │                                                     │
│           ↓                                                     │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ BACKEND AUTHORIZATION CHECK                             │ │
│  │ [Authorize(Roles = "BranchManager")]                    │ │
│  │ ├─ Extract JWT token from header                        │ │
│  │ ├─ Validate token signature                             │ │
│  │ ├─ Check role claim includes "BranchManager"            │ │
│  │ └─ Proceed if valid, return 401 if not                  │ │
│  └──────────────────────────────────────────────────────────┘ │
│           │                                                     │
│           ↓                                                     │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ DATABASE QUERIES EXECUTE                                │ │
│  │                                                          │ │
│  │ SELECT COUNT(*) FROM "Users"                            │ │
│  │ WHERE "IsActive"=true AND lower("Role")='teller'        │ │
│  │ → Result: 1 active teller                               │ │
│  │                                                          │ │
│  │ SELECT COUNT(*) FROM "Customers"                        │ │
│  │ WHERE "IsActive"=true                                   │ │
│  │ → Result: 3 customers                                   │ │
│  │                                                          │ │
│  │ SELECT COUNT(*) FROM "Accounts"                         │ │
│  │ WHERE "Status"='Active'                                 │ │
│  │ → Result: 3 accounts                                    │ │
│  │                                                          │ │
│  │ SELECT COALESCE(SUM("Balance"), 0)                      │ │
│  │ FROM "Accounts" WHERE "Status"='Active'                 │ │
│  │ → Result: 142,184.00 (KES)                              │ │
│  │                                                          │ │
│  │ SELECT COUNT(*) FROM "Transactions"                     │ │
│  │ WHERE DATE("CreatedAt") = CURRENT_DATE                  │ │
│  │ → Result: 0 transactions today                          │ │
│  └──────────────────────────────────────────────────────────┘ │
│           │                                                     │
│           ↓                                                     │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ RESPONSE OBJECT CREATED                                 │ │
│  │ {                                                        │ │
│  │   "totalCustomers": 3,                                  │ │
│  │   "totalAccounts": 3,                                   │ │
│  │   "dailyTransactions": 0,                               │ │
│  │   "dailyTransactionValue": 0,                           │ │
│  │   "cashOnHand": 142184.00,                              │ │
│  │   "pendingApprovals": 0,                                │ │
│  │   "activeTellers": 1,                                   │ │
│  │   "branchHealth": "Needs Attention"                     │ │
│  │ }                                                        │ │
│  └──────────────────────────────────────────────────────────┘ │
│           │                                                     │
│           ↓ HTTP 200 OK                                         │
│           │                                                     │
│    FRONTEND RECEIVES REAL DATA                                  │
│           │                                                     │
│           ↓                                                     │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │ React Component State Updated                           │  │
│  │ setDashboardData(dashboard)                             │  │
│  │ setTellerPerformance(performance)                       │  │
│  └─────────────────────────────────────────────────────────┘  │
│           │                                                     │
│           ↓                                                     │
│  ┌──────────────────────────────────────────────────────────┐ │
│  │ REAL DATA DISPLAYED IN UI                               │ │
│  │                                                          │ │
│  │ ┌─────────────────┐  ┌─────────────────┐                │ │
│  │ │ Active Staff    │  │ Today's Trans   │                │ │
│  │ │      1          │  │       0         │                │ │
│  │ └─────────────────┘  └─────────────────┘                │ │
│  │                                                          │ │
│  │ ┌─────────────────┐  ┌─────────────────┐                │ │
│  │ │ Cash Position   │  │ Branch Health   │                │ │
│  │ │  KES 142,184    │  │ Needs Attention │                │ │
│  │ └─────────────────┘  └─────────────────┘                │ │
│  │                                                          │ │
│  │ Teller Performance Table:                                │ │
│  │ ┌──────────────┬─────────┬────────┐                      │ │
│  │ │ Name         │ Trans   │ Status │                      │ │
│  │ │ John Doe     │    0    │ Active │                      │ │
│  │ └──────────────┴─────────┴────────┘                      │ │
│  │                                                          │ │
│  │ Pending Approvals: 0 (No pending)                        │ │
│  └──────────────────────────────────────────────────────────┘ │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

## Database Layer (PostgreSQL)

```
┌──────────────────────────────────────────────────────┐
│          PostgreSQL 15 (wekeza_core)                 │
├──────────────────────────────────────────────────────┤
│                                                      │
│  USERS TABLE                CUSTOMERS TABLE          │
│  ┌─────────────────┐       ┌──────────────────┐     │
│  │ Id (PK)         │       │ Id (PK)          │     │
│  │ Username        │       │ FirstName        │     │
│  │ Email           │       │ LastName         │     │
│  │ PasswordHash    │       │ Email            │     │
│  │ FullName        │       │ PhoneNumber      │     │
│  │ Role ──┐        │       │ IsActive         │     │
│  │ IsActive │      │       └──────────────────┘     │
│  │ CreatedAt│      │                                │
│  └─────────────────┘       ACCOUNTS TABLE          │
│         │                  ┌──────────────────┐     │
│         │                  │ Id (PK)          │     │
│  SAMPLE:│                  │ AccountNumber    │     │
│  --------+--────────────   │ CustomerId (FK)  │     │
│         │  admin           │ Balance          │     │
│         │  teller1    ┌──→ │ AvailableBalance │     │
│         │  manager1   │    │ Status           │     │
│         │             │    │ OpenedDate       │     │
│         │             │    └──────────────────┘     │
│         │             │                             │
│         │             │    TRANSACTIONS TABLE       │
│         │             │    ┌──────────────────┐     │
│         │             └───→│ Id (PK)          │     │
│         │                  │ TransRef         │     │
│  ACTIVE: 3                 │ AccountId (FK)   │     │
│  Admins: 1                 │ Type             │     │
│  Tellers: 1                │ Amount           │     │
│  Managers: 1               │ Status           │     │
│                            │ CreatedAt        │     │
│                            └──────────────────┘     │
│                                                      │
│  RELATIONS:                                          │
│  Customers 1──→ M Accounts 1──→ M Transactions      │
│                                                      │
└──────────────────────────────────────────────────────┘
```

## API Endpoints (Real Data)

```
GET /api/Authentication/login
├─ Query Users table by username (case-insensitive)
├─ Verify BCrypt password hash
├─ Generate JWT token with claims
└─ Return: token, refreshToken, user object

GET /api/branch-manager/dashboard
├─ Query COUNT(Users) WHERE IsActive, Role='teller'
├─ Query COUNT(Customers) WHERE IsActive
├─ Query COUNT(Accounts) WHERE Status='Active'
├─ Query SUM(Balance) FROM Accounts
├─ Query COUNT(Transactions) WHERE DATE=TODAY
└─ Return: Dashboard metrics object

GET /api/branch-manager/tellers/performance
├─ Query Users WHERE Role='teller'
├─ For each teller:
│  ├─ COUNT(Transactions) WHERE teller's work
│  └─ Calculate efficiency score
└─ Return: Array of teller performance

GET /api/branch-manager/transactions/daily
├─ Query transactions for specified date
├─ Group by: deposits, withdrawals, transfers
├─ Calculate: count and sum per type
└─ Return: Daily transaction breakdown

GET /api/branch-manager/staff
└─ Query Users WHERE Role != 'administrator'
   Return: Staff list with roles and status
```

## Frontend Component Tree

```
App
└── BranchManagerPortalPage
    ├── State:
    │  ├─ dashboardData (Object)
    │  ├─ tellerPerformance (Array)
    │  ├─ pendingApprovals (Array)
    │  ├─ loading (Boolean)
    │  └─ error (String)
    │
    ├── Effects:
    │  └─ useEffect(() => loadDashboardData(), [])
    │     ├─ branchManagerApi.getDashboard()
    │     ├─ branchManagerApi.getTellerPerformance()
    │     └─ branchManagerApi.getPendingRequests()
    │
    └── Render:
       ├── Row[Col] - Statistics Cards
       │  ├─ Active Staff: {dashboardData.activeTellers}
       │  ├─ Today's Transactions: {dashboardData.dailyTransactions}
       │  ├─ Cash Position: {dashboardData.cashOnHand}
       │  └─ Branch Health: {dashboardData.branchHealth}
       │
       └── Row[Col] - Performance Table
          ├─ Table (dataSource={tellerPerformance})
          │  ├─ Name
          │  ├─ Transactions
          │  ├─ Amount
          │  ├─ Efficiency (Progress bar)
          │  └─ Status (Badge)
          │
          └── Pending Approvals List
             └─ .map(approval => Badge + timestamp)
```

## Data Flow Summary

```
User Login
    ↓
JWT Token Generated
    ↓
Dashboard Component Mounts
    ↓  (useEffect)
Fetch 3x API Endpoints
    ↓
Database Queries Execute
    ↓
PostgreSQL Returns Real Data
    ↓
Response JSON Objects
    ↓
Frontend State Updated
    ↓
React Re-renders
    ↓
Real Data Displayed in UI ✅
```

---

## Key Improvements

| Aspect | Before | After |
|--------|--------|-------|
| Data Source | Hardcoded in component | PostgreSQL database |
| Updates | Static on page load | Dynamic on every request |
| Scale | Limited mock data | Grows with database |
| Accuracy | Always fake | Always real |
| Users | Can't change data | Database is single source of truth |
| Persistence | Resets on refresh | Persists permanently |
| Multi-user | All users see same fake data | Each user sees real filtered data |

---

**Status**: ✅ Real data flowing from database through API to UI dashboards
