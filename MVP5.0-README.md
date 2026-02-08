# 🏦 Wekeza Banking System - MVP5.0

## **Full End-to-End Operational Core Banking System**

Welcome to MVP5.0 - The complete, production-ready core banking system with multiple specialized APIs working together to provide comprehensive banking functionality.

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [What's New in MVP5.0](#whats-new-in-mvp50)
3. [Quick Start](#quick-start)
4. [Architecture](#architecture)
5. [Available Services](#available-services)
6. [API Documentation](#api-documentation)
7. [Local Development](#local-development)
8. [Docker Deployment](#docker-deployment)
9. [Testing](#testing)
10. [Troubleshooting](#troubleshooting)

---

## 🎯 Overview

MVP5.0 is a microservices-based core banking system that provides:

✅ **Complete Banking Operations** - Accounts, Transactions, Loans, Cards
✅ **Multi-API Architecture** - Four specialized APIs for different use cases
✅ **Production Ready** - Docker containerization with health checks
✅ **Database Integration** - PostgreSQL with Entity Framework Core
✅ **Admin Panel** - Comprehensive administrative interface
✅ **API Documentation** - Swagger/OpenAPI for all endpoints

---

## 🚀 What's New in MVP5.0

### Key Improvements:

1. **Microservices Architecture**
   - 4 specialized APIs instead of monolithic approach
   - Each API serves specific banking needs
   - Independent scalability and deployment

2. **Operational Stability**
   - All APIs compile successfully
   - No compilation errors
   - Production-ready Docker containers

3. **Comprehensive Coverage**
   - Customer Management (CIF)
   - Account Operations
   - Transaction Processing
   - Loan Management
   - Card Services
   - Admin Panel with Dashboard
   - Reporting and Analytics

4. **Easy Deployment**
   - Single command startup
   - Docker Compose orchestration
   - Automated health checks

---

## ⚡ Quick Start

### Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose
- 4GB RAM minimum
- 10GB disk space

### Start All Services

**Linux/Mac:**
```bash
chmod +x start-mvp5.sh
./start-mvp5.sh
```

**Windows (PowerShell):**
```powershell
.\start-mvp5.ps1
```

**Manual Docker Compose:**
```bash
docker-compose -f docker-compose.mvp5.yml up -d
```

### Access the System

Once started, access the services at:

- **Minimal API**: http://localhost:8081
- **Database API**: http://localhost:8082/swagger
- **Enhanced API**: http://localhost:8083/swagger
- **Comprehensive API**: http://localhost:8084/swagger

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────┐
│              Wekeza Banking MVP5.0                   │
│                                                      │
│  ┌──────────────┐  ┌──────────────┐               │
│  │   Client     │  │   Admin      │               │
│  │ Applications │  │   Panel      │               │
│  └──────┬───────┘  └──────┬───────┘               │
│         │                  │                        │
│         └──────────┬───────┘                        │
│                    │                                │
│   ┌────────────────┴────────────────┐              │
│   │      API Gateway / Load Bal     │              │
│   └────────────┬────────────────────┘              │
│                │                                    │
│   ┌────────────┼────────────────┬─────────┐       │
│   │            │                │         │       │
│   ▼            ▼                ▼         ▼       │
│ ┌──────┐  ┌─────────┐  ┌─────────┐  ┌──────────┐│
│ │Minimal│  │Database │  │Enhanced │  │Comprehen-││
│ │  API  │  │   API   │  │   API   │  │ sive API ││
│ │      │  │         │  │         │  │          ││
│ │:8081 │  │  :8082  │  │  :8083  │  │   :8084  ││
│ └──┬───┘  └────┬────┘  └────┬────┘  └─────┬────┘│
│    │           │             │              │     │
│    └───────────┴─────────────┴──────────────┘     │
│                       │                            │
│              ┌────────▼────────┐                  │
│              │   PostgreSQL    │                  │
│              │   Database      │                  │
│              │    :5432        │                  │
│              └─────────────────┘                  │
└─────────────────────────────────────────────────────┘
```

---

## 🔧 Available Services

### 1. **Minimal API** (Port 8081)

**Purpose**: Lightweight API for basic banking operations

**Key Features:**
- Customer creation
- Account management
- Simple transactions (deposit/withdraw)
- Balance inquiries

**Best For:** Mobile apps, quick integrations, microservices

---

### 2. **Database API** (Port 8082)

**Purpose**: Data-centric banking operations with comprehensive endpoints

**Key Features:**
- Advanced customer management
- Multiple account types
- Transaction history
- Loan processing
- Reporting endpoints

**Best For:** Backend systems, data analytics, reporting

**Swagger**: http://localhost:8082/swagger

---

### 3. **Enhanced API** (Port 8083)

**Purpose**: Extended banking features and services

**Key Features:**
- Card management
- Advanced transactions
- Multi-currency support
- Investment products
- Trade finance

**Best For:** Corporate banking, advanced features

**Swagger**: http://localhost:8083/swagger

---

### 4. **Comprehensive API** (Port 8084)

**Purpose**: Full administrative panel and complete banking operations

**Key Features:**
- Complete admin dashboard
- Staff management
- Product catalog
- Branch management
- Comprehensive reporting
- System configuration
- All banking operations

**Best For:** Bank administrators, complete system management

**Swagger**: http://localhost:8084/swagger

---

## 📚 API Documentation

All APIs provide Swagger/OpenAPI documentation at `/swagger` endpoint.

### Common Endpoints Across APIs

#### Authentication
```http
POST /api/auth/login
POST /api/auth/register
```

#### Customer Management
```http
POST   /api/customers
GET    /api/customers/{id}
PUT    /api/customers/{id}
GET    /api/customers
```

#### Account Operations
```http
POST   /api/accounts
GET    /api/accounts/{id}
GET    /api/accounts/{accountNumber}/balance
POST   /api/accounts/{id}/freeze
POST   /api/accounts/{id}/unfreeze
```

#### Transactions
```http
POST   /api/transactions/deposit
POST   /api/transactions/withdraw
POST   /api/transactions/transfer
GET    /api/transactions/{id}
GET    /api/transactions/statement
```

#### Loans
```http
POST   /api/loans/apply
POST   /api/loans/{id}/approve
POST   /api/loans/{id}/disburse
POST   /api/loans/{id}/repay
GET    /api/loans/{id}
```

---

## 💻 Local Development

### Running Individual APIs

Each API can be run independently for development:

```bash
# Minimal API
cd MinimalWekezaApi
dotnet run

# Database API
cd DatabaseWekezaApi
dotnet run

# Enhanced API
cd EnhancedWekezaApi
dotnet run

# Comprehensive API
cd ComprehensiveWekezaApi
dotnet run
```

### Building APIs

```bash
# Build all APIs
dotnet build MinimalWekezaApi/MinimalWekezaApi.csproj
dotnet build DatabaseWekezaApi/DatabaseWekezaApi.csproj
dotnet build EnhancedWekezaApi/EnhancedWekezaApi.csproj
dotnet build ComprehensiveWekezaApi/ComprehensiveWekezaApi.csproj
```

### Database Setup

The system uses PostgreSQL. Connection string:

```
Host=localhost;Port=5432;Database=WekezaCoreDB;Username=wekeza_app;Password=WekeZa2026!SecurePass
```

---

## 🐳 Docker Deployment

### Start All Services

```bash
docker-compose -f docker-compose.mvp5.yml up -d
```

### View Logs

```bash
# All services
docker-compose -f docker-compose.mvp5.yml logs -f

# Specific service
docker-compose -f docker-compose.mvp5.yml logs -f comprehensive-api
```

### Stop Services

```bash
docker-compose -f docker-compose.mvp5.yml down
```

### Rebuild Services

```bash
docker-compose -f docker-compose.mvp5.yml build --no-cache
docker-compose -f docker-compose.mvp5.yml up -d
```

### Health Checks

```bash
# Check service status
docker-compose -f docker-compose.mvp5.yml ps

# Check individual container
docker inspect --format='{{.State.Health.Status}}' wekeza-comprehensive-api
```

---

## 🧪 Testing

### Testing with Swagger UI

1. Navigate to http://localhost:8084/swagger
2. Try the "GET /api/status" endpoint
3. Create a test customer
4. Open an account
5. Perform transactions

### Testing with cURL

```bash
# Get API status
curl http://localhost:8084/api/status

# Create a customer (example)
curl -X POST http://localhost:8084/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phone": "+254712345678",
    "identificationNumber": "12345678"
  }'
```

### End-to-End Testing

Complete banking workflow:

1. **Create Customer** → POST /api/customers
2. **Open Account** → POST /api/accounts
3. **Deposit Funds** → POST /api/transactions/deposit
4. **Check Balance** → GET /api/accounts/{id}/balance
5. **Transfer Money** → POST /api/transactions/transfer
6. **Apply for Loan** → POST /api/loans/apply

---

## 🔧 Troubleshooting

### Common Issues

#### Services Won't Start

```bash
# Check Docker daemon
docker ps

# Check Docker Compose version
docker-compose --version

# View service logs
docker-compose -f docker-compose.mvp5.yml logs
```

#### Port Already in Use

```bash
# Find process using port
lsof -i :8084  # Linux/Mac
netstat -ano | findstr :8084  # Windows

# Change ports in docker-compose.mvp5.yml if needed
```

#### Database Connection Issues

```bash
# Verify PostgreSQL is running
docker-compose -f docker-compose.mvp5.yml ps postgres

# Check database logs
docker-compose -f docker-compose.mvp5.yml logs postgres

# Restart database
docker-compose -f docker-compose.mvp5.yml restart postgres
```

#### API Not Responding

```bash
# Check API logs
docker-compose -f docker-compose.mvp5.yml logs comprehensive-api

# Restart specific service
docker-compose -f docker-compose.mvp5.yml restart comprehensive-api

# Rebuild and restart
docker-compose -f docker-compose.mvp5.yml build comprehensive-api
docker-compose -f docker-compose.mvp5.yml up -d comprehensive-api
```

### Getting Help

1. Check service logs
2. Verify all containers are running
3. Ensure ports are not blocked
4. Check database connectivity
5. Review environment variables

---

## 📊 System Requirements

### Minimum Requirements

- **CPU**: 2 cores
- **RAM**: 4GB
- **Disk**: 10GB free space
- **OS**: Linux, macOS, Windows 10+

### Recommended Requirements

- **CPU**: 4+ cores
- **RAM**: 8GB+
- **Disk**: 20GB+ SSD
- **OS**: Linux (Ubuntu 20.04+), macOS, Windows 11

---

## 🎯 Next Steps

1. **Explore the APIs**: Open Swagger UI and test endpoints
2. **Create Test Data**: Use the admin panel to set up test accounts
3. **Test Transactions**: Perform deposits, withdrawals, and transfers
4. **Review Logs**: Monitor system behavior
5. **Customize**: Modify configurations for your needs

---

## 📝 Notes

- All services share the same PostgreSQL database
- Each API has independent scaling capabilities
- Docker containers include health checks
- Services automatically restart on failure
- Use pgAdmin (port 5050) for database management

---

## 🏆 Success Criteria

✅ All 4 APIs running and accessible
✅ Database connected and healthy
✅ Swagger documentation available
✅ Customer creation working
✅ Account operations functional
✅ Transactions processing correctly
✅ Admin panel operational

---

## 🎉 Congratulations!

You now have a fully operational core banking system running end-to-end!

**For additional help, check the logs or review the API documentation in Swagger UI.**

---

*Wekeza Banking System MVP5.0 - Built with ❤️ using .NET 8 and Docker*
