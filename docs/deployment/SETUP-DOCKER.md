# Wekeza Bank - Docker Setup Guide

This guide shows you how to run Wekeza Bank using Docker containers.

## Prerequisites

1. **Docker Desktop for Windows**
   - Download: https://www.docker.com/products/docker-desktop/
   - Install and start Docker Desktop
   - Verify: `docker --version` and `docker-compose --version`

2. **Working Local Setup** (Recommended)
   - Complete `SETUP-LOCAL.md` first
   - Verify everything works locally before containerizing

## Quick Start with Docker

### Step 1: Configure Environment

```powershell
# Copy environment template
Copy-Item .env.example .env

# Edit .env file with your values
notepad .env
```

Update these critical values:
- `DB_PASSWORD` - Strong database password
- `JWT_SECRET` - 32+ character secret key
- `MPESA_*` - Your M-Pesa credentials

### Step 2: Build and Start

```powershell
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Check status
docker-compose ps
```

### Step 3: Run Migrations

```powershell
# Run migrations inside the container
docker-compose exec api dotnet ef database update

# Or use the migration script
docker-compose exec api /bin/sh -c "cd /app && dotnet ef database update"
```

### Step 4: Access the Application

- **API**: http://localhost:8080/swagger
- **Health Check**: http://localhost:8080/health
- **pgAdmin** (optional): http://localhost:5050

## Docker Commands Reference

### Starting/Stopping

```powershell
# Start all services
docker-compose up -d

# Start specific service
docker-compose up -d api

# Stop all services
docker-compose down

# Stop and remove volumes (CAUTION: deletes data!)
docker-compose down -v
```

### Viewing Logs

```powershell
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f api
docker-compose logs -f postgres

# Last 100 lines
docker-compose logs --tail=100 api
```

### Database Management

```powershell
# Connect to PostgreSQL
docker-compose exec postgres psql -U wekeza_app -d WekezaCoreDB

# Backup database
docker-compose exec postgres pg_dump -U wekeza_app WekezaCoreDB > backup.sql

# Restore database
docker-compose exec -T postgres psql -U wekeza_app WekezaCoreDB < backup.sql
```

### Container Management

```powershell
# Restart service
docker-compose restart api

# Rebuild after code changes
docker-compose build api
docker-compose up -d api

# View resource usage
docker stats

# Clean up unused resources
docker system prune -a
```

## Production Deployment

### Security Checklist

- [ ] Change all default passwords
- [ ] Use strong JWT secret (32+ characters)
- [ ] Enable HTTPS with valid SSL certificate
- [ ] Configure firewall rules
- [ ] Set up backup strategy
- [ ] Enable monitoring and logging
- [ ] Review and update CORS settings
- [ ] Implement rate limiting
- [ ] Set up database replication

### Environment Variables for Production

```env
# Production .env
ASPNETCORE_ENVIRONMENT=Production
DB_PASSWORD=<strong-random-password>
JWT_SECRET=<64-character-random-string>
JWT_ISSUER=https://api.yourdomain.com
JWT_AUDIENCE=https://yourdomain.com
```

### Using Docker Compose Profiles

```powershell
# Start with pgAdmin
docker-compose --profile tools up -d

# Production (no tools)
docker-compose up -d
```

### Scaling

```powershell
# Run multiple API instances
docker-compose up -d --scale api=3

# Requires load balancer (nginx/traefik)
```

## Troubleshooting

### Container won't start

```powershell
# Check logs
docker-compose logs api

# Check if port is in use
netstat -ano | findstr :8080

# Remove and recreate
docker-compose down
docker-compose up -d
```

### Database connection issues

```powershell
# Verify postgres is healthy
docker-compose ps postgres

# Check network
docker network inspect wekeza-network

# Test connection
docker-compose exec api ping postgres
```

### Migration errors

```powershell
# Drop and recreate database
docker-compose down -v
docker-compose up -d postgres
# Wait 10 seconds
docker-compose up -d api
docker-compose exec api dotnet ef database update
```

### Performance issues

```powershell
# Check resource usage
docker stats

# Increase resources in docker-compose.yml:
# resources:
#   limits:
#     cpus: '4'
#     memory: 4G
```

## Development with Docker

### Hot Reload Setup

For development with hot reload, use this override:

```yaml
# docker-compose.override.yml
version: '3.8'
services:
  api:
    build:
      target: build
    volumes:
      - ./Core:/src/Core
    environment:
      ASPNETCORE_ENVIRONMENT: Development
```

### Debugging

```powershell
# Run in foreground with logs
docker-compose up

# Attach debugger (VS 2022)
# 1. Set breakpoints
# 2. Debug > Attach to Process
# 3. Select "Docker" connection
# 4. Find "Wekeza.Core.Api" process
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Build and Deploy

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Build Docker image
        run: docker build -t wekeza-bank:latest .
      
      - name: Run tests
        run: docker-compose run --rm api dotnet test
      
      - name: Push to registry
        run: |
          docker tag wekeza-bank:latest registry.example.com/wekeza-bank:latest
          docker push registry.example.com/wekeza-bank:latest
```

## Monitoring

### Health Checks

```powershell
# Check health status
docker-compose ps

# Manual health check
curl http://localhost:8080/health
```

### Logs

```powershell
# Export logs
docker-compose logs > logs.txt

# Send to monitoring service
docker-compose logs | your-log-aggregator
```

## Backup Strategy

### Automated Backups

```powershell
# Create backup script (backup.ps1)
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
docker-compose exec -T postgres pg_dump -U wekeza_app WekezaCoreDB > "backups/backup-$timestamp.sql"

# Schedule with Task Scheduler
```

### Restore from Backup

```powershell
# Stop API
docker-compose stop api

# Restore database
docker-compose exec -T postgres psql -U wekeza_app WekezaCoreDB < backup.sql

# Start API
docker-compose start api
```

## Next Steps

- Set up reverse proxy (nginx/Traefik)
- Configure SSL certificates
- Implement monitoring (Prometheus/Grafana)
- Set up log aggregation (ELK stack)
- Configure automated backups
- Set up CI/CD pipeline

---

**Ready for production?** Review security checklist and deploy to your cloud provider!
