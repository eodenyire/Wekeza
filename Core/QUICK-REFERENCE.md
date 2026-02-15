# 🎯 Quick Reference Card

## ⚡ Quick Start (Copy & Paste)

### 1. Verify Node.js (in NEW PowerShell window)
```powershell
.\verify-nodejs.ps1
```

### 2. Install & Start Personal Banking
```powershell
cd Wekeza.Web.Channels\personal-banking
npm install
npm run dev
```

### 3. Open Browser
```
http://localhost:3001
Login: admin / test123
```

---

## 📋 Essential Commands

### Check Status
```powershell
# Verify Node.js
node --version
npm --version

# Check API
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get

# Quick test everything
.\quick-test.ps1
```

### Start Services
```powershell
# Start API (if not running)
cd Core
dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj

# Start Personal Banking
cd Wekeza.Web.Channels\personal-banking
npm run dev

# Start All Channels
.\start-all-channels.ps1
```

### Stop Services
```powershell
# Stop web channel: Press Ctrl+C in terminal
# Stop API: Press Ctrl+C in terminal
```

---

## 🌐 URLs

| Service | URL | Credentials |
|---------|-----|-------------|
| API | http://localhost:5000 | - |
| Swagger | http://localhost:5000/swagger | - |
| Public Website | http://localhost:3000 | - |
| Personal Banking | http://localhost:3001 | admin/test123 |
| Corporate Banking | http://localhost:3002 | admin/test123 |
| SME Banking | http://localhost:3003 | admin/test123 |

---

## 🔑 Test Credentials

```
Username: admin
Password: test123
```

---

## 🧪 Quick API Tests

### Test API Health
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get
```

### Test Login
```powershell
$body = @{ username = "admin"; password = "test123" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" -Method Post -Body $body -ContentType "application/json"
```

### Test Protected Endpoint
```powershell
# First login to get token
$login = Invoke-RestMethod -Uri "http://localhost:5000/api/authentication/login" -Method Post -Body (@{ username = "admin"; password = "test123" } | ConvertTo-Json) -ContentType "application/json"

# Then use token
$headers = @{ Authorization = "Bearer $($login.token)" }
Invoke-RestMethod -Uri "http://localhost:5000/api/customer-portal/accounts" -Method Get -Headers $headers
```

---

## 🐛 Common Issues

### "npm is not recognized"
```powershell
# Solution: Close ALL PowerShell windows, open NEW one
# If still fails: Restart computer
```

### "Port already in use"
```powershell
# Find process
netstat -ano | findstr "3001"

# Kill process
taskkill /PID <PID> /F
```

### "Cannot connect to API"
```powershell
# Check if API is running
Invoke-RestMethod -Uri "http://localhost:5000/api" -Method Get

# If not, start it
cd Core
dotnet run --project Wekeza.Core.Api/Wekeza.Core.Api.csproj
```

### "Database connection failed"
```powershell
# Start PostgreSQL
& "C:\Program Files\PostgreSQL\15\bin\pg_ctl.exe" start -D "C:\Program Files\PostgreSQL\15\data"
```

---

## 📁 Project Structure

```
Core/
├── Wekeza.Core.Api/          # Backend API
├── Wekeza.Core.Application/  # Business Logic
├── Wekeza.Core.Domain/       # Domain Models
└── Wekeza.Core.Infrastructure/ # Data Access

Wekeza.Web.Channels/
├── personal-banking/         # Port 3001
├── corporate-banking/        # Port 3002
├── sme-banking/             # Port 3003
└── public-website/          # Port 3000
```

---

## 📚 Documentation Files

| File | Purpose |
|------|---------|
| README.md | Project overview |
| CURRENT-STATUS.md | What's working now |
| NEXT-STEPS.md | What to do next |
| START-PERSONAL-BANKING.md | Step-by-step guide |
| TESTING-GUIDE.md | How to test |
| COMPLETE-SYSTEM-GUIDE.md | Everything |
| INSTALL-NODEJS.md | Node.js installation |

---

## 🎯 Testing Checklist

### Backend
- [ ] API responds at http://localhost:5000
- [ ] Swagger works at http://localhost:5000/swagger
- [ ] Login works (admin/test123)
- [ ] Protected endpoints work

### Personal Banking
- [ ] Opens at http://localhost:3001
- [ ] Login works
- [ ] Dashboard displays
- [ ] Can view accounts
- [ ] Can transfer funds
- [ ] Can view cards
- [ ] Can apply for loan

---

## 💡 Pro Tips

1. **Keep API running** - Don't close the API terminal
2. **Use separate terminals** - One for API, one for each channel
3. **Check browser console** - Press F12 to see errors
4. **Use Swagger** - Test API endpoints directly
5. **Read logs** - Check terminal output for errors

---

## 🆘 Get Help

1. Run: `.\quick-test.ps1`
2. Check: CURRENT-STATUS.md
3. Read: START-PERSONAL-BANKING.md
4. Review: TESTING-GUIDE.md

---

## 🎉 Success Indicators

✅ API returns JSON at http://localhost:5000/api  
✅ Swagger loads at http://localhost:5000/swagger  
✅ Login page loads at http://localhost:3001  
✅ Can login with admin/test123  
✅ Dashboard shows accounts  

---

**Keep this file handy for quick reference!**
