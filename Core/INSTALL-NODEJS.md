# 📦 Install Node.js and npm - Quick Guide

## Why You Need Node.js

Node.js and npm are required to run the web channels (Personal Banking, Corporate Banking, SME Banking, Public Website).

## Installation Steps

### Option 1: Using Official Installer (Recommended)

1. **Download Node.js**
   - Go to: https://nodejs.org/
   - Download the **LTS version** (Long Term Support)
   - Current LTS: Node.js 20.x

2. **Run the Installer**
   - Double-click the downloaded `.msi` file
   - Click "Next" through the installation wizard
   - Accept the license agreement
   - Keep default installation path: `C:\Program Files\nodejs\`
   - Make sure "Add to PATH" is checked ✅
   - Click "Install"

3. **Verify Installation**
   ```powershell
   # Open a NEW PowerShell window (important!)
   node --version
   # Should show: v20.x.x
   
   npm --version
   # Should show: 10.x.x
   ```

### Option 2: Using Chocolatey (If you have it)

```powershell
# Run as Administrator
choco install nodejs-lts -y

# Verify
node --version
npm --version
```

### Option 3: Using Winget (Windows 11)

```powershell
# Run as Administrator
winget install OpenJS.NodeJS.LTS

# Verify
node --version
npm --version
```

## After Installation

### Step 1: Verify Installation

Open a **NEW** PowerShell window and run:

```powershell
node --version
npm --version
```

You should see version numbers like:
```
v20.11.0
10.2.4
```

### Step 2: Install Personal Banking Channel

```powershell
cd Wekeza.Web.Channels\personal-banking
npm install
```

This will install all dependencies (React, TypeScript, etc.). It may take 2-5 minutes.

### Step 3: Start Personal Banking

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

### Step 4: Open in Browser

Open your browser and go to:
```
http://localhost:3001
```

You should see the Personal Banking login page!

## Install All Channels

Once Node.js is installed, you can install all channels:

```powershell
# Personal Banking
cd Wekeza.Web.Channels\personal-banking
npm install

# Corporate Banking
cd ..\corporate-banking
npm install

# SME Banking
cd ..\sme-banking
npm install

# Public Website
cd ..\public-website
npm install
```

## Start All Channels

After installation, use the helper script:

```powershell
cd Core
.\start-all-channels.ps1
```

This will start all 4 channels in separate windows.

## Troubleshooting

### Issue: "npm is not recognized"

**Solution**: 
1. Close ALL PowerShell windows
2. Open a NEW PowerShell window
3. Try again

If still not working:
1. Restart your computer
2. Open PowerShell
3. Try `npm --version`

### Issue: "node is not recognized"

**Solution**: Node.js is not in your PATH
1. Reinstall Node.js
2. Make sure "Add to PATH" is checked
3. Restart computer

### Issue: npm install is slow

**Solution**: This is normal for first install
- It downloads ~200MB of dependencies
- Takes 2-5 minutes depending on internet speed
- Be patient!

### Issue: EACCES permission errors

**Solution**: Run PowerShell as Administrator
```powershell
# Right-click PowerShell
# Select "Run as Administrator"
```

## Quick Test After Installation

```powershell
# Test Node.js
node --version

# Test npm
npm --version

# Test installation
cd Wekeza.Web.Channels\personal-banking
npm install
npm run dev

# Open browser
# Go to http://localhost:3001
```

## What Gets Installed

When you run `npm install`, it installs:

- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **Axios** - HTTP client
- **React Router** - Navigation
- **Zustand** - State management
- **Lucide Icons** - Icons
- **Recharts** - Charts

Total size: ~200MB per channel

## Alternative: Use the API Only

If you don't want to install Node.js right now, you can still test the backend API:

1. **Use Swagger UI**
   - Go to http://localhost:5000/swagger
   - Test all endpoints directly

2. **Use PowerShell**
   ```powershell
   # Test login
   $body = @{ username = "admin"; password = "test123" } | ConvertTo-Json
   Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" -Method Post -Body $body -ContentType "application/json"
   ```

3. **Use Postman**
   - Download Postman
   - Import API endpoints
   - Test manually

But for the full experience with web interfaces, you'll need Node.js!

## Next Steps

After installing Node.js:

1. ✅ Install dependencies: `npm install`
2. ✅ Start dev server: `npm run dev`
3. ✅ Open browser: http://localhost:3001
4. ✅ Login: admin / test123
5. ✅ Test features!

---

**Need help?** Check the COMPLETE-SYSTEM-GUIDE.md for more details.
