# Wekeza Bank - Complete Deployment Guide

This guide covers the complete journey from local development to production deployment.

## 🚀 Deployment Path

```
Local Development → Docker Local → Docker Production → Cloud Deployment
     (Day 1)           (Day 2)         (Week 1)          (Week 2+)
```

## Phase 1: Local Development (Start Here!)

**Goal**: Get the system running on your Windows machine

### Quick Start (5 minutes)

```powershell
# 1. Setup database
.\scripts\setup-local-db.ps1

# 2. Run migrations
.\scripts\run-migrations.ps1

# 3. Start application
.\scripts\start-local.ps1
```

**Access**: https://localhost:5001/swagger

**Full Guide**: See `QUICKSTART.md` and `SETUP-LOCAL.md`

### What You Get
- ✅ Full development environment
- ✅ Hot reload for code changes
- ✅ Direct database access
- ✅ Easy debugging in Visual Studio
- ✅ Fast iteration cycle

### When to Use
- Initial development
- Feature development
- Debugging issues
- Learning the system
- Running tests

---

## Phase 2: Docker Local (After Local Works)

**Goal**: Containerize for consistency and easier deployment

### Quick Start

```powershell
# 1. Copy environment file
Copy-Item .env.example .env

# 2. Start containers
docker-compose up -d

# 3. Run migrations
docker-compose exec api dotnet ef database update

# 4. Check status
docker-compose ps
```

**Access**: http://localhost:8080/swagger

**Full Guide**: See `SETUP-DOCKER.md`

### What You Get
- ✅ Isolated environment
- ✅ Easy to share with team
- ✅ Matches production setup
- ✅ Database in container
- ✅ One-command startup

### When to Use
- Testing deployment setup
- Sharing with team
- CI/CD testing
- Pre-production validation

---

## Phase 3: Production Deployment

### Option A: Cloud VM (Azure/AWS/DigitalOcean)

#### 1. Provision Server

**Minimum Requirements**:
- 2 vCPUs
- 4 GB RAM
- 50 GB SSD
- Ubuntu 22.04 LTS

#### 2. Install Docker

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo apt install docker-compose-plugin -y

# Verify
docker --version
docker compose version
```

#### 3. Deploy Application

```bash
# Clone repository
git clone https://github.com/your-org/wekeza-bank.git
cd wekeza-bank

# Configure environment
cp .env.example .env
nano .env  # Update with production values

# Start services
docker compose up -d

# Run migrations
docker compose exec api dotnet ef database update

# Check logs
docker compose logs -f
```

#### 4. Configure Reverse Proxy (nginx)

```nginx
# /etc/nginx/sites-available/wekeza
server {
    listen 80;
    server_name api.wekeza.com;

    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# Enable site
sudo ln -s /etc/nginx/sites-available/wekeza /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx

# Install SSL with Let's Encrypt
sudo apt install certbot python3-certbot-nginx -y
sudo certbot --nginx -d api.wekeza.com
```

### Option B: Azure App Service

#### 1. Create Resources

```powershell
# Login to Azure
az login

# Create resource group
az group create --name wekeza-rg --location eastus

# Create PostgreSQL
az postgres flexible-server create `
  --resource-group wekeza-rg `
  --name wekeza-db `
  --location eastus `
  --admin-user wekeza_admin `
  --admin-password <strong-password> `
  --sku-name Standard_B2s `
  --version 15

# Create App Service Plan
az appservice plan create `
  --name wekeza-plan `
  --resource-group wekeza-rg `
  --sku P1V2 `
  --is-linux

# Create Web App
az webapp create `
  --resource-group wekeza-rg `
  --plan wekeza-plan `
  --name wekeza-api `
  --runtime "DOTNETCORE:8.0"
```

#### 2. Configure App Settings

```powershell
az webapp config appsettings set `
  --resource-group wekeza-rg `
  --name wekeza-api `
  --settings `
    ConnectionStrings__DefaultConnection="Host=wekeza-db.postgres.database.azure.com;..." `
    JwtSettings__Secret="<your-secret>" `
    ASPNETCORE_ENVIRONMENT="Production"
```

#### 3. Deploy

```powershell
# Build and publish
dotnet publish Core/Wekeza.Core.Api -c Release -o ./publish

# Create deployment package
Compress-Archive -Path ./publish/* -DestinationPath deploy.zip

# Deploy to Azure
az webapp deployment source config-zip `
  --resource-group wekeza-rg `
  --name wekeza-api `
  --src deploy.zip
```

### Option C: AWS ECS (Elastic Container Service)

#### 1. Push to ECR

```bash
# Authenticate
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com

# Build and tag
docker build -t wekeza-bank .
docker tag wekeza-bank:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/wekeza-bank:latest

# Push
docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/wekeza-bank:latest
```

#### 2. Create ECS Service

```bash
# Create cluster
aws ecs create-cluster --cluster-name wekeza-cluster

# Create task definition (see task-definition.json)
aws ecs register-task-definition --cli-input-json file://task-definition.json

# Create service
aws ecs create-service `
  --cluster wekeza-cluster `
  --service-name wekeza-api `
  --task-definition wekeza-task `
  --desired-count 2 `
  --launch-type FARGATE
```

---

## Security Checklist

### Before Going Live

- [ ] Change all default passwords
- [ ] Use environment variables for secrets
- [ ] Enable HTTPS/SSL
- [ ] Configure CORS properly
- [ ] Set up rate limiting
- [ ] Enable authentication on all endpoints
- [ ] Review and update JWT settings
- [ ] Set up database backups
- [ ] Configure monitoring and alerts
- [ ] Set up logging aggregation
- [ ] Review firewall rules
- [ ] Enable database encryption at rest
- [ ] Set up WAF (Web Application Firewall)
- [ ] Implement DDoS protection
- [ ] Set up intrusion detection

### Secrets Management

**Never commit**:
- `appsettings.Production.json`
- `.env` files
- SSL certificates
- API keys

**Use instead**:
- Azure Key Vault
- AWS Secrets Manager
- HashiCorp Vault
- Environment variables

---

## Monitoring & Observability

### Application Insights (Azure)

```csharp
// Add to Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Prometheus + Grafana

```yaml
# docker-compose.monitoring.yml
services:
  prometheus:
    image: prom/prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./prometheus.yml:/etc/prometheus/prometheus.yml

  grafana:
    image: grafana/grafana
    ports:
      - "3000:3000"
```

### ELK Stack (Elasticsearch, Logstash, Kibana)

```yaml
# docker-compose.logging.yml
services:
  elasticsearch:
    image: elasticsearch:8.11.0
  
  logstash:
    image: logstash:8.11.0
  
  kibana:
    image: kibana:8.11.0
```

---

## Backup Strategy

### Automated Database Backups

```bash
# Backup script (backup.sh)
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
docker compose exec -T postgres pg_dump -U wekeza_app WekezaCoreDB | gzip > backups/backup_$DATE.sql.gz

# Keep only last 30 days
find backups/ -name "*.sql.gz" -mtime +30 -delete
```

```bash
# Schedule with cron
0 2 * * * /path/to/backup.sh
```

### Disaster Recovery

1. **Regular backups**: Daily automated backups
2. **Off-site storage**: S3, Azure Blob, or similar
3. **Test restores**: Monthly restore tests
4. **Documentation**: Recovery procedures documented
5. **RTO/RPO**: Define Recovery Time/Point Objectives

---

## Performance Optimization

### Database

```sql
-- Add indexes for common queries
CREATE INDEX idx_accounts_customer_id ON accounts(customer_id);
CREATE INDEX idx_transactions_account_id ON transactions(account_id);
CREATE INDEX idx_transactions_timestamp ON transactions(timestamp);
```

### Caching

```csharp
// Add Redis caching
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration["Redis:ConnectionString"];
});
```

### Load Balancing

```nginx
upstream wekeza_backend {
    server api1:8080;
    server api2:8080;
    server api3:8080;
}
```

---

## CI/CD Pipeline

### GitHub Actions

```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Run tests
        run: dotnet test

  build:
    needs: test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build Docker image
        run: docker build -t wekeza-bank .
      - name: Push to registry
        run: docker push registry.example.com/wekeza-bank

  deploy:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to production
        run: |
          ssh user@server 'cd /app && docker compose pull && docker compose up -d'
```

---

## Troubleshooting Production Issues

### High CPU Usage

```bash
# Check container stats
docker stats

# Check application metrics
curl http://localhost:8080/metrics

# Review slow queries
docker compose exec postgres psql -U wekeza_app -d WekezaCoreDB -c "SELECT * FROM pg_stat_statements ORDER BY total_time DESC LIMIT 10;"
```

### Memory Leaks

```bash
# Monitor memory
docker stats --no-stream

# Restart service
docker compose restart api

# Check logs for errors
docker compose logs --tail=1000 api | grep -i "error\|exception"
```

### Database Connection Pool Exhausted

```json
// Increase pool size in connection string
"ConnectionStrings": {
  "DefaultConnection": "Host=...;Maximum Pool Size=100;..."
}
```

---

## Support & Maintenance

### Regular Tasks

**Daily**:
- Monitor error logs
- Check system health
- Review security alerts

**Weekly**:
- Review performance metrics
- Check backup integrity
- Update dependencies

**Monthly**:
- Security patches
- Performance optimization
- Capacity planning
- Disaster recovery test

### Getting Help

- Documentation: Check all .md files
- Logs: `docker compose logs -f`
- Health: `curl http://localhost:8080/health`
- Database: Connect via pgAdmin

---

## Next Steps

1. ✅ Complete local setup
2. ✅ Test all features locally
3. ✅ Set up Docker locally
4. ✅ Configure production environment
5. ✅ Deploy to staging
6. ✅ Run security audit
7. ✅ Deploy to production
8. ✅ Set up monitoring
9. ✅ Configure backups
10. ✅ Document procedures

---

**Questions?** Review the specific guides:
- Local: `SETUP-LOCAL.md`
- Docker: `SETUP-DOCKER.md`
- Quick Start: `QUICKSTART.md`
