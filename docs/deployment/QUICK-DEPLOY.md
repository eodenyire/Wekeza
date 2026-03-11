# 🚀 DEPLOYMENT GUIDE - Quick Start

## ⚡ 5-Minute Deployment (Netlify)

### Step 1: Commit Your Changes (2 minutes)
```bash
# From the repository root (e.g. ~/Wekeza  or  /workspaces/Wekeza in Codespaces)
cd "$(git rev-parse --show-toplevel)"

# Add all changes
git add .

# Commit with descriptive message
git commit -m "feat: deploy 14-portal unified shell with domain entities and CI/CD"

# Push to GitHub
git push origin main
```

### Step 2: Deploy on Netlify (3 minutes)

**Option A: Direct Connection (Recommended)**
1. Visit [netlify.com](https://netlify.com)
2. Sign in with GitHub
3. Click **"New site from Git"**
4. Choose your **Wekeza repository**
5. **Auto-detect settings**:
   - Base directory: `Portals/wekeza-unified-shell`
   - Build command: `npm run build`
   - Publish directory: `dist`
6. Click **"Deploy site"**
7. Wait 2-3 minutes for deployment
8. **Done!** 🎉

**Option B: Via CLI (If installed)**
```bash
npm install -g netlify-cli

cd Portals/wekeza-unified-shell

netlify deploy --prod --dir=dist
```

---

## 🔑 Netlify Environment Variables

After deployment, configure in **Netlify Dashboard → Site Settings → Build & Deploy → Environment**:

```
VITE_AUTH_MODE = mock
VITE_API_URL = http://localhost:8080
```

Or for production backend:
```
VITE_AUTH_MODE = real
VITE_API_URL = https://api.wekeza.bank
```

---

## ✅ Test Your Deployment

Once deployed (Netlify gives you a URL):

1. **Visit the URL** in browser
2. **Login with test credentials**:
   - Username: `teller` (or any role)
   - Password: anything
3. **Click each portal** in the sidebar:
   - Teller Portal
   - Branch Manager
   - Admin
   - Executive
   - ... (14 total)
4. **Verify** each portal loads data
5. **Check console** for errors (F12)

---

## 🔄 Continuous Deployment (CI/CD)

GitHub Actions workflow is ready to auto-deploy on every push to `main`:

1. Push code → GitHub
2. GitHub Actions runs tests → `npm run build`
3. Auto-deploys to Netlify
4. Site updates in 1-2 minutes

See `.github/workflows/deploy-frontend.yml` for details.

---

## 📱 Access Your Live Portal

After Netlify deployment:

**Live URL Format**:
```
https://[your-site-name].netlify.app
```

**Example**:
```
https://wekeza-unified-shell.netlify.app
```

---

## 🔧 Alternative Deployment Options

### Vercel (Similar to Netlify)
```bash
vercel --prod
```

### Docker (Self-hosted)
```bash
docker build -t wekeza-shell .
docker run -p 3000:3000 wekeza-shell
```

### GitHub Pages (Free)
See `DEPLOYMENT.md` for setup instructions.

---

## ✨ What Gets Deployed

```
dist/
├── index.html          (Entry point)
├── assets/
│   ├── index-*.js      (App code - 444 KB)
│   ├── react-vendor-*.js      (React - 160 KB)
│   ├── query-vendor-*.js       (React Query - 79 KB)
│   └── antd-vendor-*.js        (Ant Design - 1 MB)
└── favicon.ico

Total: 1.76 MB uncompressed → 417 KB gzipped
```

---

## 🎯 14 Portals Available

After deployment, access:

| Portal | Route | Features |
|--------|-------|----------|
| Teller | /portals/teller | Sessions, Cash ops, Customer service |
| Branch Manager | /portals/branch-manager | Team, Cash, Reports |
| Admin | /portals/admin | Users, Security, Config |
| Executive | /portals/executive | KPIs, Board packs, Risks |
| Branch Ops | /portals/branch-operations | Vault, EOD/BOD, Cash transfer |
| Supervisor | /portals/supervisor | Approvals, Queue, Controls |
| Compliance | /portals/compliance | AML, Risk, Regulatory |
| Treasury | /portals/treasury | Liquidity, FX, MM |
| Trade Finance | /portals/trade-finance | LCs, Guarantees, Collections |
| Product & GL | /portals/product-gl | Catalog, Controls, Pricing |
| Payments | /portals/payments | Queue, Metrics, Returns |
| Customer | /portals/customer | Accounts, Activity, Actions |
| Staff | /portals/staff | Requests, Payslips |
| Workflow | /portals/workflow | Tasks, Monitor, Escalations |

---

## 🐛 Troubleshooting

### Build fails on Netlify
1. Check build logs (Netlify Dashboard → Deploys → View log)
2. Verify `npm run build` works locally: `npm run build`
3. Ensure correct base directory selected

### Login not working
1. Check browser console (F12)
2. Verify `VITE_AUTH_MODE=mock` is set
3. Clear cookies and localStorage
4. Try incognito window

### Portals don't load
1. Check if backend is running (for real auth mode)
2. Verify API URL is correct
3. Check CORS headers
4. Look at Network tab (F12) for failed requests

### Blank page
1. Reload with Ctrl+Shift+R (hard refresh)
2. Check browser console for errors
3. Verify JavaScript is enabled
4. Try different browser

---

## 📊 Deployment Checklist

- [ ] Git changes committed
- [ ] Code pushed to main branch
- [ ] Netlify connected to GitHub account
- [ ] Site deployed and URL shows ✅
- [ ] Can login with test credentials
- [ ] All 14 portals load in sidebar
- [ ] Click each portal - data displays
- [ ] No console errors (F12)
- [ ] Responsive on mobile view (F12)
- [ ] Share link with team! 🎉

---

## 🎊 Success!

Your frontend is now live!

**Next Steps**:
1. **Share the live URL** with team/stakeholders
2. **Continue fixing backend** compilation errors
3. **When backend ready**: Update `VITE_API_URL` and switch from mock to real auth
4. **Full system integration**: Deploy backend API alongside frontend

---

## 📞 Need Help?

**Netlify Support**: https://support.netlify.com  
**Deployment Guide**: See `Portals/wekeza-unified-shell/DEPLOYMENT.md`  
**Backend Fixes**: See `APIs/v1-Core/BACKEND-FIX-GUIDE.md`  

---

## 🎯 What Happens Next

After frontend deployment:

✅ **Phase 1: Frontend Live** (You are here)
- Portal accessible 24/7
- Mock authentication working
- Users can explore all 14 portals

⏳ **Phase 2: Backend Fixes** (Continue after deployment)
- Fix remaining 305 compilation errors
- Generate EF migrations
- Apply database schema updates
- Test real API integration

✅ **Phase 3: Full Integration** (After backend compiles)
- Switch frontend to real authentication
- Connect to live backend API
- Full system testing
- Production deployment

---

**Deploy now, fix backend later. You've got this!** 🚀
