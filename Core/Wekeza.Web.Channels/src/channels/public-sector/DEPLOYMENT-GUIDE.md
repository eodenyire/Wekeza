# Public Sector Portal - Deployment Guide

## Prerequisites

Before deploying the Public Sector Portal, ensure you have:

1. **Backend API** - Wekeza.Core.Api running and accessible
2. **Database** - SQL Server with required tables and data
3. **Node.js** - Version 18 or higher
4. **Build Tools** - npm or yarn
5. **Web Server** - Nginx, Apache, or IIS for hosting static files

## Environment Configuration

### 1. Environment Variables

Create a `.env` file in the `Wekeza.Web.Channels` directory:

```env
# API Configuration
VITE_API_URL=https://api.wekeza.bank
VITE_API_TIMEOUT=30000

# Authentication
VITE_JWT_SECRET=your-jwt-secret-key
VITE_TOKEN_EXPIRY=3600

# Feature Flags
VITE_ENABLE_REAL_TIME_UPDATES=true
VITE_ENABLE_EXPORT=true
VITE_ENABLE_NOTIFICATIONS=true

# External Integrations
VITE_CBK_API_URL=https://api.cbk.go.ke
VITE_NSE_API_URL=https://api.nse.co.ke
VITE_IFMIS_API_URL=https://ifmis.treasury.go.ke

# Monitoring
VITE_SENTRY_DSN=your-sentry-dsn
VITE_ANALYTICS_ID=your-analytics-id

# Environment
VITE_ENV=production
```

### 2. Build Configuration

Update `vite.config.ts` for production:

```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: false,
    minify: 'terser',
    terserOptions: {
      compress: {
        drop_console: true,
        drop_debugger: true,
      },
    },
    rollupOptions: {
      output: {
        manualChunks: {
          'react-vendor': ['react', 'react-dom', 'react-router-dom'],
          'chart-vendor': ['recharts'],
          'icon-vendor': ['lucide-react'],
        },
      },
    },
  },
  server: {
    proxy: {
      '/api': {
        target: process.env.VITE_API_URL,
        changeOrigin: true,
      },
    },
  },
});
```

## Build Process

### 1. Install Dependencies

```bash
cd Wekeza.Web.Channels
npm install
```

### 2. Run Tests (Optional)

```bash
npm run test
npm run test:e2e
```

### 3. Build for Production

```bash
npm run build
```

This creates an optimized production build in the `dist/` directory.

### 4. Preview Build Locally

```bash
npm run preview
```

## Deployment Options

### Option 1: Azure Static Web Apps

1. **Create Azure Static Web App**
```bash
az staticwebapp create \
  --name wekeza-public-sector \
  --resource-group wekeza-rg \
  --location "East US" \
  --source https://github.com/wekeza/web-channels \
  --branch main \
  --app-location "/Wekeza.Web.Channels" \
  --output-location "dist"
```

2. **Configure Environment Variables** in Azure Portal

3. **Deploy**
```bash
npm run build
az staticwebapp deploy \
  --name wekeza-public-sector \
  --resource-group wekeza-rg \
  --app-location dist
```

### Option 2: Netlify

1. **Install Netlify CLI**
```bash
npm install -g netlify-cli
```

2. **Build and Deploy**
```bash
npm run build
netlify deploy --prod --dir=dist
```

3. **Configure Environment Variables** in Netlify dashboard

### Option 3: Vercel

1. **Install Vercel CLI**
```bash
npm install -g vercel
```

2. **Deploy**
```bash
vercel --prod
```

### Option 4: Traditional Web Server (Nginx)

1. **Build the Application**
```bash
npm run build
```

2. **Copy Files to Server**
```bash
scp -r dist/* user@server:/var/www/public-sector
```

3. **Configure Nginx**

Create `/etc/nginx/sites-available/public-sector`:

```nginx
server {
    listen 80;
    server_name public-sector.wekeza.bank;

    root /var/www/public-sector;
    index index.html;

    # Gzip compression
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;

    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Referrer-Policy "no-referrer-when-downgrade" always;

    # SPA routing
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API proxy
    location /api {
        proxy_pass https://api.wekeza.bank;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Cache static assets
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
        expires 1y;
        add_header Cache-Control "public, immutable";
    }
}
```

4. **Enable Site and Restart Nginx**
```bash
sudo ln -s /etc/nginx/sites-available/public-sector /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

5. **Setup SSL with Let's Encrypt**
```bash
sudo certbot --nginx -d public-sector.wekeza.bank
```

### Option 5: IIS (Windows Server)

1. **Build the Application**
```bash
npm run build
```

2. **Copy Files to IIS Directory**
```powershell
Copy-Item -Path dist\* -Destination C:\inetpub\wwwroot\public-sector -Recurse
```

3. **Create IIS Site**
   - Open IIS Manager
   - Add New Website
   - Set Physical Path to `C:\inetpub\wwwroot\public-sector`
   - Configure bindings (HTTP/HTTPS)

4. **Configure URL Rewrite** (for SPA routing)

Create `web.config` in the root:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="React Routes" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
    <staticContent>
      <mimeMap fileExtension=".json" mimeType="application/json" />
    </staticContent>
  </system.webServer>
</configuration>
```

## Post-Deployment

### 1. Verify Deployment

Test all critical paths:
- [ ] Login functionality
- [ ] Securities trading
- [ ] Loan applications
- [ ] Payment processing
- [ ] Grant applications
- [ ] Dashboard loading
- [ ] Export functionality

### 2. Configure Monitoring

**Application Insights (Azure)**
```typescript
import { ApplicationInsights } from '@microsoft/applicationinsights-web';

const appInsights = new ApplicationInsights({
  config: {
    instrumentationKey: process.env.VITE_APPINSIGHTS_KEY
  }
});
appInsights.loadAppInsights();
```

**Sentry (Error Tracking)**
```typescript
import * as Sentry from '@sentry/react';

Sentry.init({
  dsn: process.env.VITE_SENTRY_DSN,
  environment: process.env.VITE_ENV,
  tracesSampleRate: 1.0,
});
```

### 3. Setup Backup & Recovery

- Configure automated backups
- Test restore procedures
- Document rollback process

### 4. Performance Optimization

- Enable CDN for static assets
- Configure caching headers
- Enable HTTP/2
- Implement service worker for offline support

### 5. Security Hardening

- Enable HTTPS only
- Configure CSP headers
- Implement rate limiting
- Setup WAF (Web Application Firewall)
- Regular security audits

## Rollback Procedure

If issues occur after deployment:

1. **Immediate Rollback**
```bash
# For Azure
az staticwebapp deployment rollback --name wekeza-public-sector

# For Netlify
netlify rollback

# For traditional server
sudo cp -r /var/www/public-sector-backup/* /var/www/public-sector/
```

2. **Verify Rollback**
- Test critical functionality
- Check error logs
- Monitor user reports

3. **Investigate Issues**
- Review deployment logs
- Check error tracking
- Analyze user feedback

## Maintenance

### Regular Tasks

**Daily**
- Monitor error logs
- Check system health
- Review user feedback

**Weekly**
- Review performance metrics
- Check security alerts
- Update dependencies (if needed)

**Monthly**
- Security patches
- Performance optimization
- Backup verification

### Update Procedure

1. Test updates in staging environment
2. Schedule maintenance window
3. Notify users of downtime
4. Deploy updates
5. Verify functionality
6. Monitor for issues

## Troubleshooting

### Common Issues

**Issue: White screen after deployment**
- Check browser console for errors
- Verify API URL configuration
- Check CORS settings

**Issue: API calls failing**
- Verify API endpoint URLs
- Check authentication tokens
- Review CORS configuration

**Issue: Slow loading**
- Enable gzip compression
- Optimize bundle size
- Configure CDN

**Issue: Authentication not working**
- Verify JWT configuration
- Check token expiry settings
- Review CORS headers

## Support

For deployment issues, contact:
- DevOps Team: devops@wekeza.bank
- Technical Support: support@wekeza.bank

---

**Last Updated**: February 13, 2026  
**Version**: 1.0.0
