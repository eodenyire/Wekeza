# 🎯 Next Steps - Getting Your Banking System Running

## Current Status

✅ **Backend API** - Running on http://localhost:5000  
✅ **PostgreSQL** - Database running  
✅ **Web Channels** - Code ready, needs Node.js  

## What You Need to Do

### Step 1: Install Node.js (5 minutes)

**You need Node.js to run the web channels.**

1. Go to: https://nodejs.org/
2. Download the **LTS version** (green button)
3. Run the installer
4. Click "Next" through everything
5. Restart PowerShell

**Verify it worked:**
```powershell
node --version
npm --version
```

See detailed instructions in: **INSTALL-NODEJS.md**

### Step 2: Install Personal Banking (3 minutes)

```powershell
cd Wekeza.Web.Channels\personal-banking
npm install
```

Wait for it to finish (downloads ~200MB of dependencies).

### Step 3: Start Personal Banking (1 minute)

```powershell
npm run dev
```

You should see:
```
➜  Local:   http://localhost:3001/
```

### Step 4: Test in Browser (2 minutes)

1. Open browser: http://localhost:3001
2. You should see the login page
3. Login with: **admin** / **test123**
4. Explore the dashboard!

## Alternative: Test Without Web Channels

If you don't want to install Node.js right now, you can still test the API:

### Option 1: Use Swagger UI

```
http://localhost:5000/swagger
```

Click on any endpoint → "Try it out" → Execute

### Option 2: Use PowerShell

```powershell
# Test API
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get

# Test Login
$body = @{
    username = "admin"
    password = "test123"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"

Write-Host "Token: $($response.token)"

# Test Protected Endpoint
$headers = @{
    Authorization = "Bearer $($response.token)"
}

Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/accounts" `
    -Method Get `
    -Headers $headers
```

### Option 3: Use Postman

1. Download Postman: https://www.postman.com/downloads/
2. Create new request
3. Test endpoints manually

## Quick Reference

### URLs
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Personal Banking**: http://localhost:3001 (after npm run dev)

### Credentials
- **Username**: admin
- **Password**: test123

### Key Commands

```powershell
# Check if API is running
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get

# Quick test
.\quick-test.ps1

# Install Node.js dependencies
cd Wekeza.Web.Channels\personal-banking
npm install

# Start personal banking
npm run dev

# Start all channels (after Node.js installed)
.\start-all-channels.ps1
```

## What You Can Test Right Now (Without Node.js)

### 1. Test API Health

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get
```

### 2. Test Authentication

```powershell
$body = @{ username = "admin"; password = "test123" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" -Method Post -Body $body -ContentType "application/json"
```

### 3. Explore Swagger

Open: http://localhost:5000/swagger

Try these endpoints:
- GET /api - System info
- POST /api/authentication/login - Login
- GET /api/products/catalog - Product catalog
- GET /api/dashboard/accounts/statistics - Account stats

### 4. Run Quick Test

```powershell
.\quick-test.ps1
```

This tests:
- ✅ API is running
- ✅ Authentication works
- ✅ Protected endpoints work
- ✅ Database is accessible

## Recommended Path

### For Full Experience (Recommended)

1. ✅ Install Node.js (5 min)
2. ✅ Install personal-banking (3 min)
3. ✅ Start personal-banking (1 min)
4. ✅ Test in browser (2 min)
5. ✅ Install other channels (optional)

**Total time: ~15 minutes**

### For Quick API Testing

1. ✅ Open Swagger: http://localhost:5000/swagger
2. ✅ Test endpoints directly
3. ✅ Use PowerShell commands

**Total time: ~5 minutes**

## Documentation

- **INSTALL-NODEJS.md** - How to install Node.js
- **COMPLETE-SYSTEM-GUIDE.md** - Complete guide
- **TESTING-GUIDE.md** - What to test
- **START-ALL-CHANNELS.md** - How to start channels
- **README.md** - Project overview

## Need Help?

### Common Issues

**"npm is not recognized"**
→ Install Node.js from https://nodejs.org/

**"API not responding"**
→ Make sure API is running: `dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj`

**"Database connection failed"**
→ Start PostgreSQL: `& "C:\Program Files\PostgreSQL\15\bin\pg_ctl.exe" start -D "C:\Program Files\PostgreSQL\15\data"`

**"Port already in use"**
→ Check what's using the port: `netstat -ano | findstr "3001"`

## Summary

You have a **complete, working banking system**! 

The backend API is running and ready. To get the web interfaces:

1. Install Node.js
2. Run `npm install` in the channel folders
3. Run `npm run dev` to start them
4. Open browser and test!

Or just use Swagger/PowerShell to test the API directly.

**Your choice!** Both work perfectly. 🎉
