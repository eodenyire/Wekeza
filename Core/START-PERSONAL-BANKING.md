# 🚀 Start Personal Banking - Step by Step

## Step 1: Verify Node.js Installation

**Close this PowerShell window and open a NEW one**, then run:

```powershell
.\verify-nodejs.ps1
```

If you see ✅ Node.js is ready, continue to Step 2.

If you see ❌ errors:
1. Close ALL PowerShell windows
2. Open a NEW PowerShell window
3. Try again

If still not working:
1. Restart your computer
2. Open PowerShell
3. Try again

## Step 2: Navigate to Personal Banking

```powershell
cd Wekeza.Web.Channels\personal-banking
```

## Step 3: Install Dependencies

```powershell
npm install
```

**This will take 2-5 minutes.** You'll see:
- Downloading packages...
- Installing dependencies...
- Building...

**Wait for it to complete!** You should see:
```
added 234 packages in 2m
```

## Step 4: Start the Development Server

```powershell
npm run dev
```

You should see:
```
  VITE v5.0.8  ready in 1234 ms

  ➜  Local:   http://localhost:3001/
  ➜  Network: use --host to expose
  ➜  press h + enter to show help
```

**Leave this window open!** The server is running.

## Step 5: Open in Browser

Open your browser and go to:
```
http://localhost:3001
```

You should see the **Personal Banking Login Page**!

## Step 6: Login

Use these credentials:
```
Username: admin
Password: test123
```

Click "Login" and you should see the dashboard!

## 🎉 Success!

You should now see:
- Account dashboard
- Account balances
- Recent transactions
- Quick action buttons

## What You Can Test

1. **Dashboard** - View account summary
2. **Accounts** - View all accounts
3. **Transfer** - Transfer funds
4. **Payments** - Pay bills, buy airtime
5. **Cards** - View and manage cards
6. **Loans** - Apply for loans
7. **Profile** - Update your profile

## Troubleshooting

### Issue: "npm is not recognized"

**Solution**: Node.js is not in PATH
1. Close ALL PowerShell windows
2. Open a NEW PowerShell window
3. Try `npm --version`

If still not working:
1. Restart your computer
2. Try again

### Issue: npm install fails

**Solution**: Run as Administrator
1. Right-click PowerShell
2. Select "Run as Administrator"
3. Try again

### Issue: Port 3001 already in use

**Solution**: Kill the process using the port
```powershell
# Find process
netstat -ano | findstr "3001"

# Kill process (replace PID with actual number)
taskkill /PID <PID> /F
```

### Issue: Cannot connect to API

**Solution**: Make sure backend API is running
```powershell
# Check if API is running
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get

# If not running, start it
cd Core
dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj
```

### Issue: Login fails

**Solution**: Check browser console (F12)
- Look for network errors
- Check if API is responding
- Verify credentials: admin / test123

## Quick Commands Reference

```powershell
# Verify Node.js
.\verify-nodejs.ps1

# Navigate to personal banking
cd Wekeza.Web.Channels\personal-banking

# Install dependencies
npm install

# Start development server
npm run dev

# Stop server
# Press Ctrl+C in the terminal

# Check if API is running
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get
```

## Start All Channels at Once

After personal banking works, you can start all channels:

```powershell
cd Core
.\start-all-channels.ps1
```

This will start:
- Public Website (Port 3000)
- Personal Banking (Port 3001)
- Corporate Banking (Port 3002)
- SME Banking (Port 3003)

## URLs

- **Personal Banking**: http://localhost:3001
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## Credentials

- **Username**: admin
- **Password**: test123

## Next Steps

After personal banking works:

1. Test all features
2. Try other channels (corporate, SME)
3. Customize the UI
4. Add more features
5. Deploy to production

## Need Help?

Check these files:
- **TESTING-GUIDE.md** - What to test
- **COMPLETE-SYSTEM-GUIDE.md** - Complete guide
- **TROUBLESHOOTING.md** - Common issues

---

**Happy Banking! 🏦**
