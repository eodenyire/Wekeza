# Wekeza Banking System - APIs Organization

## Overview
The Wekeza banking system now has a well-organized API structure with multiple versions tailored for different use cases and deployment scenarios.

## Directory Structure

```
Wekeza/
├── APIs/
│   ├── v1-Core/                    # PRODUCTION - Main Core API (Complete Implementation)
│   │   ├── Wekeza.Core.Api/
│   │   ├── Wekeza.Core.Application/
│   │   ├── Wekeza.Core.Domain/
│   │   ├── Wekeza.Core.Infrastructure/
│   │   ├── Wekeza.Nexus.Domain/
│   │   ├── Wekeza.Nexus.Application/
│   │   ├── Wekeza.Nexus.Infrastructure/
│   │   ├── Mobile/
│   │   ├── Tests/
│   │   ├── Wekeza.Core.sln
│   │   ├── Dockerfile
│   │   └── docker-compose.yml
│   │
│   ├── v2-MVP4.0/                  # Lightweight MVP Version
│   │   ├── MVP4.0/
│   │   └── docker-compose.yml
│   │
│   ├── v3-Comprehensive/           # Full-Featured Comprehensive API
│   │   ├── ComprehensiveWekezaApi/
│   │   └── docker-compose.yml
│   │
│   ├── v4-Minimal/                 # Minimal/Resource-Constrained Version
│   │   ├── MinimalWekezaApi/
│   │   └── docker-compose.yml
│   │
│   ├── v5-Database/                # Database-Focused/Analytics Version
│   │   ├── DatabaseWekezaApi/
│   │   └── docker-compose.yml
│   │
│   └── v6-Enhanced/                # Enhanced with Advanced Features
│       ├── EnhancedWekezaApi/
│       └── docker-compose.yml
│
├── docker-compose.yml              # Main (v1-Core) - Default deployment
├── docker-compose.master.yml       # Multi-version orchestration
├── Dockerfile                      # Root Dockerfile (legacy - superseded by v1-Core)
└── README.md                       # Main documentation
```

## API Versions

### v1-Core (Production)
**Status:** ✅ **ACTIVE - PRIMARY**
- **Purpose:** Main production banking API
- **Port:** `8080`
- **Database:** PostgreSQL on port `5432`
- **Tech Stack:** .NET 10.0, ASP.NET Core, Entity Framework Core
- **Features:** Complete banking functionality, Nexus integration, Mobile support
- **Location:** `/APIs/v1-Core/`

**Run Locally:**
```bash
cd /workspaces/Wekeza/APIs/v1-Core
docker-compose up -d
```

### v2-MVP4.0 (Lightweight)
**Status:** ⚠️ **AVAILABLE**
- **Purpose:** Minimal Viable Product for quick testing/demo
- **Port:** `9080` (when enabled)
- **Database:** PostgreSQL on port `5433`
- **Use Case:** Resource-constrained environments, CI/CD demos
- **Location:** `/APIs/v2-MVP4.0/`

### v3-Comprehensive (Full-Featured)
**Status:** ⚠️ **AVAILABLE**
- **Purpose:** Comprehensive implementation with all features
- **Port:** `9081` (when enabled)
- **Database:** PostgreSQL on port `5434`
- **Features:** Extended modules, analytics, advanced workflows
- **Location:** `/APIs/v3-Comprehensive/`

### v4-Minimal (Lightweight)
**Status:** ⚠️ **AVAILABLE**
- **Purpose:** Minimal API for resource-constrained environments
- **Port:** `9082` (when enabled)
- **Database:** PostgreSQL on port `5435`
- **Use Case:** Edge computing, IoT banking terminals, low-bandwidth scenarios
- **Location:** `/APIs/v4-Minimal/`

### v5-Database (Data-Focused)
**Status:** ⚠️ **AVAILABLE**
- **Purpose:** Database operations and analytics focused
- **Port:** `9083` (when enabled)
- **Database:** PostgreSQL on port `5436`
- **Features:** Query optimization, reporting, data analytics
- **Location:** `/APIs/v5-Database/`

### v6-Enhanced (Advanced)
**Status:** ⚠️ **AVAILABLE**
- **Purpose:** Enhanced version with advanced optimizations
- **Port:** `9084` (when enabled)
- **Database:** PostgreSQL on port `5437`
- **Features:** Performance optimizations, caching, advanced middleware
- **Location:** `/APIs/v6-Enhanced/`

## Deployment

### Single Version (v1-Core - Recommended)
```bash
# From root directory
docker-compose up -d

# OR from v1-Core directory
cd APIs/v1-Core
docker-compose up -d
```

### All Versions (Multi-API)
```bash
# Run all versions simultaneously
docker-compose -f docker-compose.master.yml --profile multi up -d

# Run only v1-Core + specific versions
docker-compose -f docker-compose.master.yml up -d
```

### Individual Version
```bash
# Run specific version
cd APIs/v1-Core && docker-compose up -d
cd APIs/v2-MVP4.0 && docker-compose up -d
# ... etc
```

## API Endpoints

### v1-Core (Port 8080)
```
Base URL: http://localhost:8080

Health Check:    GET /health
API Docs:        GET /swagger (when enabled)
```

### Multi-API Access
- v1-Core:       `http://localhost:8080`
- v2-MVP4.0:     `http://localhost:9080` (when enabled)
- v3-Comprehensive: `http://localhost:9081` (when enabled)
- v4-Minimal:    `http://localhost:9082` (when enabled)
- v5-Database:   `http://localhost:9083` (when enabled)
- v6-Enhanced:   `http://localhost:9084` (when enabled)

## Development

### Opening Solution
```bash
# v1-Core solution
cd APIs/v1-Core
dotnet sln open Wekeza.Core.sln

# Or in VS Code
code .
```

### Building
```bash
cd APIs/v1-Core
dotnet build
```

### Running Locally
```bash
cd APIs/v1-Core/Wekeza.Core.Api
dotnet run
```

### Running Tests
```bash
cd APIs/v1-Core
dotnet test
```

## Database

### Connection Strings
Each API version has its own database instance:
- v1-Core: `postgres://wekeza_app@localhost:5432/WekezaCoreDB`
- v2-MVP4.0: `postgres://wekeza_app@localhost:5433/WekezaCoreDB`
- v3-Comprehensive: `postgres://wekeza_app@localhost:5434/WekezaCoreDB`
- v4-Minimal: `postgres://wekeza_app@localhost:5435/WekezaCoreDB`
- v5-Database: `postgres://wekeza_app@localhost:5436/WekezaCoreDB`
- v6-Enhanced: `postgres://wekeza_app@localhost:5437/WekezaCoreDB`

### Default Credentials
- **Username:** `wekeza_app`
- **Password:** `WekeZa2026!SecurePass` (or set via `DB_PASSWORD` env var)

## Configuration

### Environment Variables
Create a `.env` file or export:
```bash
# Database
DB_PASSWORD=YourSecurePassword

# JWT Configuration
JWT_SECRET=your-secret-key-here
JWT_ISSUER=https://api.wekeza.com
JWT_AUDIENCE=https://wekeza.com

# M-Pesa Configuration
MPESA_CONSUMER_KEY=your-consumer-key
MPESA_CONSUMER_SECRET=your-consumer-secret
MPESA_SHORTCODE=your-shortcode
MPESA_PASSKEY=your-passkey
```

## Migration to New Structure

The following items have been reorganized:
- ✅ Core API projects moved to `APIs/v1-Core/`
- ✅ MVP4.0 moved to `APIs/v2-MVP4.0/`
- ✅ ComprehensiveWekezaApi moved to `APIs/v3-Comprehensive/`
- ✅ MinimalWekezaApi moved to `APIs/v4-Minimal/`
- ✅ DatabaseWekezaApi moved to `APIs/v5-Database/`
- ✅ EnhancedWekezaApi moved to `APIs/v6-Enhanced/`
- ✅ Solution file paths updated
- ✅ Docker Compose files created for each version
- ✅ Dockerfile updated for new paths

## Troubleshooting

### Port Already in Use
```bash
# Find process using port 8080
lsof -i :8080

# Kill process
kill -9 <PID>
```

### Database Connection Issues
```bash
# Test database connection
psql -h localhost -U wekeza_app -d WekezaCoreDB -W

# Check Docker logs
docker logs wekeza-postgres
```

### Build Issues
```bash
# Clean build
cd APIs/v1-Core
dotnet clean
dotnet build --no-restore
```

## Support

For issues or questions about the new API structure:
1. Check the individual `docker-compose.yml` files
2. Review this documentation
3. Check API logs: `docker logs wekeza-api`
4. Check database logs: `docker logs wekeza-postgres`

## Next Steps

1. ✅ Test v1-Core deployment
2. ⏳ Create Dockerfiles for other API versions
3. ⏳ Add API service configurations to v2-v6 docker-compose files
4. ⏳ Update CI/CD pipelines for new structure
5. ⏳ Document API differences and feature matrices
