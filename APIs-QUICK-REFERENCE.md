# Wekeza APIs - Quick Reference Guide

## 🚀 Quick Start

### Start Production API (v1-Core)
```bash
# From root directory
docker-compose up -d

# Or explicitly
cd APIs/v1-Core
docker-compose up -d
```

### Access API
- **Base URL:** http://localhost:8080
- **Health:** http://localhost:8080/health
- **Database:** localhost:5432 (PostgreSQL)

## 📁 API Versions at a Glance

| Version | Location | Port | Status | Use Case |
|---------|----------|------|--------|----------|
| **v1-Core** | `APIs/v1-Core/` | 8080 | ✅ Active | Production Banking API |
| **v2-MVP4.0** | `APIs/v2-MVP4.0/` | 9080 | ⚠️ Available | Lightweight MVP |
| **v3-Comprehensive** | `APIs/v3-Comprehensive/` | 9081 | ⚠️ Available | Full-Featured |
| **v4-Minimal** | `APIs/v4-Minimal/` | 9082 | ⚠️ Available | Resource-Constrained |
| **v5-Database** | `APIs/v5-Database/` | 9083 | ⚠️ Available | Data Analytics |
| **v6-Enhanced** | `APIs/v6-Enhanced/` | 9084 | ⚠️ Available | Advanced Features |

## 🛠️ Common Commands

### Single API
```bash
# v1-Core (Default)
cd APIs/v1-Core
docker-compose up -d
docker-compose down

# Specific version
cd APIs/v2-MVP4.0
docker-compose up -d
```

### Multiple APIs
```bash
# Run all versions
docker-compose -f docker-compose.master.yml --profile multi up -d

# Stop all
docker-compose -f docker-compose.master.yml --profile multi down
```

### Development
```bash
# Open solution
cd APIs/v1-Core
code .

# Build
dotnet build

# Test
dotnet test

# Run locally
cd Wekeza.Core.Api
dotnet run
```

## 🔗 Connections

### API Endpoints
- v1-Core: `http://localhost:8080`
- v2-MVP4.0: `http://localhost:9080` (when running)
- v3-Comprehensive: `http://localhost:9081` (when running)
- v4-Minimal: `http://localhost:9082` (when running)
- v5-Database: `http://localhost:9083` (when running)
- v6-Enhanced: `http://localhost:9084` (when running)

### Database Connections
```bash
# v1-Core
psql -h localhost -U wekeza_app -d WekezaCoreDB -W

# v2-MVP4.0
psql -h localhost -p 5433 -U wekeza_app -d WekezaCoreDB -W

# v3-Comprehensive
psql -h localhost -p 5434 -U wekeza_app -d WekezaCoreDB -W
# ... and so on for v4-v6
```

## 📋 File Locations

### Key Files
```
APIs/
├── v1-Core/
│   ├── Wekeza.Core.sln           # Main solution
│   ├── Dockerfile                # Container image
│   ├── docker-compose.yml        # Single-API compose
│   └── Wekeza.Core.Api/          # API Project
│
├── docker-compose.master.yml     # Multi-API orchestration (root)
└── APIs-ORGANIZATION.md          # Full documentation (root)
```

## 🔐 Configuration

### Default Credentials
- **DB User:** wekeza_app
- **DB Password:** WekeZa2026!SecurePass

### Environment Variables (in `.env`)
```env
DB_PASSWORD=YourSecurePassword
JWT_SECRET=your-secret-key
JWT_ISSUER=https://api.wekeza.com
JWT_AUDIENCE=https://wekeza.com
MPESA_CONSUMER_KEY=your-key
MPESA_CONSUMER_SECRET=your-secret
MPESA_SHORTCODE=your-shortcode
MPESA_PASSKEY=your-passkey
```

## ✅ What's Changed

### Organization ✓
- All APIs moved to `/APIs/` with clear version naming
- Each version has own Dockerfile and docker-compose
- Solution files updated with correct paths
- Tests and Mobile projects organized with v1-Core

### Docker ✓
- v1-Core uses .NET 10.0 Alpine images
- Individual docker-compose files for each version
- Master docker-compose for running all simultaneously
- Updated root docker-compose to reference new paths

### Documentation ✓
- APIs-ORGANIZATION.md (full documentation)
- This quick reference guide
- Clear folder structure

## 🐛 Troubleshooting

### Port conflicts?
```bash
# Check what's using port 8080
lsof -i :8080
kill -9 <PID>
```

### Docker build fails?
```bash
cd APIs/v1-Core
docker-compose down
docker system prune -a
docker-compose up -d
```

### Database won't connect?
```bash
# Check database logs
docker logs wekeza-postgres

# Check API logs
docker logs wekeza-api
```

## 📖 Full Documentation

See **[APIs-ORGANIZATION.md](APIs-ORGANIZATION.md)** for comprehensive documentation.

## 🎯 Next Steps

1. ✅ API structure reorganized
2. ✅ Docker Compose files created
3. ✅ Solution paths updated
4. ⏳ Create Dockerfiles for v2-v6 APIs
5. ⏳ Complete API service configurations
6. ⏳ Add API feature matrix documentation
