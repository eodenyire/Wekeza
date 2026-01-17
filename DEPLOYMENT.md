# Wekeza Bank - Deployment Guide

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Local Development](#local-development)
3. [Docker Deployment](#docker-deployment)
4. [Kubernetes Deployment](#kubernetes-deployment)
5. [Database Migrations](#database-migrations)
6. [Monitoring & Observability](#monitoring--observability)
7. [CI/CD Pipeline](#cicd-pipeline)

## Prerequisites

### Required Software
- .NET 8 SDK
- Docker Desktop
- PostgreSQL 15+
- Redis (optional, for caching)
- kubectl (for Kubernetes deployment)

### Environment Variables
```bash
# Database
DB_PASSWORD=your_secure_password

# JWT
JWT_SECRET=your_jwt_secret_min_32_chars

# Grafana
GRAFANA_PASSWORD=your_grafana_password

# M-Pesa (Production)
MPESA_CONSUMER_KEY=your_consumer_key
MPESA_CONSUMER_SECRET=your_consumer_secret
```

## Local Development

### 1. Clone Repository
```bash
git clone https://github.com/wekeza-bank/core.git
cd wekeza
```

### 2. Update Configuration
Edit `Core/Wekeza.Core.Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=WekezaCoreDB;Username=postgres;Password=your_password"
  }
}
```

### 3. Run Database Migrations
```bash
# Windows
.\scripts\run-migrations.ps1

# Linux/Mac
chmod +x scripts/run-migrations.sh
./scripts/run-migrations.sh
```

### 4. Run the Application
```bash
cd Core/Wekeza.Core.Api
dotnet run
```

Access the API at: `https://localhost:5001/swagger`

## Docker Deployment

### 1. Build Docker Image
```bash
docker build -t wekeza-bank:latest .
```

### 2. Run with Docker Compose
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

### Services Included
- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger
- **Seq (Logs)**: http://localhost:5341
- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3000
- **PostgreSQL**: localhost:5432
- **Redis**: localhost:6379

### 3. Health Check
```bash
curl http://localhost:8080/health
```

## Kubernetes Deployment

### 1. Create Namespace
```bash
kubectl create namespace wekeza-bank
```

### 2. Create Secrets
```bash
kubectl create secret generic wekeza-secrets \
  --from-literal=database-connection="Host=postgres;Database=WekezaCoreDB;Username=admin;Password=your_password" \
  --from-literal=jwt-secret="your_jwt_secret" \
  -n wekeza-bank
```

### 3. Deploy Application
```bash
kubectl apply -f kubernetes/deployment.yml -n wekeza-bank
```

### 4. Verify Deployment
```bash
# Check pods
kubectl get pods -n wekeza-bank

# Check services
kubectl get svc -n wekeza-bank

# View logs
kubectl logs -f deployment/wekeza-api -n wekeza-bank
```

### 5. Access Application
```bash
# Get external IP
kubectl get svc wekeza-api-service -n wekeza-bank

# Port forward for local access
kubectl port-forward svc/wekeza-api-service 8080:80 -n wekeza-bank
```

## Database Migrations

### Create New Migration
```bash
cd Core/Wekeza.Core.Infrastructure

dotnet ef migrations add MigrationName \
  --startup-project ../Wekeza.Core.Api \
  --output-dir Persistence/Migrations
```

### Apply Migrations
```bash
# Development
dotnet ef database update --startup-project ../Wekeza.Core.Api

# Production (using connection string)
dotnet ef database update \
  --startup-project ../Wekeza.Core.Api \
  --connection "Host=prod-db;Database=WekezaCoreDB;Username=admin;Password=***"
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName \
  --startup-project ../Wekeza.Core.Api
```

## Monitoring & Observability

### Grafana Dashboards
1. Access Grafana: http://localhost:3000
2. Login: admin / admin (change on first login)
3. Navigate to Dashboards → Wekeza Bank Overview

### Key Metrics
- **API Request Rate**: Requests per second
- **Transaction Volume**: Total transactions processed
- **Active Accounts**: Number of active accounts
- **Loan Portfolio**: Outstanding loan amounts
- **Error Rate**: 5xx errors per minute
- **Response Time**: P95 latency

### Seq Structured Logging
1. Access Seq: http://localhost:5341
2. Search logs by:
   - Level (Information, Warning, Error)
   - CorrelationId
   - UserId
   - Timestamp

### Prometheus Queries
```promql
# Request rate
rate(http_requests_total[5m])

# Error rate
rate(http_requests_total{status=~"5.."}[5m])

# Response time (95th percentile)
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))
```

## CI/CD Pipeline

### GitHub Actions Workflow
The pipeline automatically:
1. Runs on push to `main` or `develop`
2. Builds the application
3. Runs unit and integration tests
4. Performs code quality checks
5. Builds and pushes Docker image
6. Deploys to staging (develop branch)
7. Deploys to production (main branch)

### Manual Deployment
```bash
# Tag release
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin v1.0.0

# Trigger deployment
gh workflow run ci-cd.yml
```

### Deployment Verification
```bash
# Check deployment status
kubectl rollout status deployment/wekeza-api -n wekeza-bank

# View deployment history
kubectl rollout history deployment/wekeza-api -n wekeza-bank

# Rollback if needed
kubectl rollout undo deployment/wekeza-api -n wekeza-bank
```

## Production Checklist

### Before Deployment
- [ ] Update all secrets and passwords
- [ ] Configure SSL/TLS certificates
- [ ] Set up database backups
- [ ] Configure monitoring alerts
- [ ] Review security settings
- [ ] Test disaster recovery procedures
- [ ] Update DNS records
- [ ] Configure CDN (if applicable)

### Security
- [ ] Enable HTTPS only
- [ ] Configure firewall rules
- [ ] Set up VPN for database access
- [ ] Enable audit logging
- [ ] Configure rate limiting
- [ ] Set up DDoS protection
- [ ] Review CORS settings
- [ ] Enable security headers

### Performance
- [ ] Configure connection pooling
- [ ] Enable response caching
- [ ] Set up CDN for static assets
- [ ] Configure auto-scaling
- [ ] Optimize database indexes
- [ ] Enable query caching

## Troubleshooting

### Common Issues

**Database Connection Failed**
```bash
# Check database is running
docker ps | grep postgres

# Test connection
psql -h localhost -U wekeza_admin -d WekezaCoreDB
```

**API Not Starting**
```bash
# Check logs
docker logs wekeza-api

# Verify environment variables
docker exec wekeza-api env | grep ConnectionStrings
```

**High Memory Usage**
```bash
# Check container stats
docker stats wekeza-api

# Restart container
docker restart wekeza-api
```

## Support

For deployment issues:
- Email: devops@wekeza.com
- Slack: #wekeza-deployments
- On-call: +254-XXX-XXXXXX

---

**Last Updated**: January 17, 2026
**Version**: 1.0.0
