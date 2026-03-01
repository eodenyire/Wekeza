# 🏦 Wekeza Banking System - API Ports Guide

## Port Configuration

Each API version runs on a dedicated port for easy access and testing:

| API Version | Port | URL | Description |
|-------------|------|-----|-------------|
| **Minimal** | 5000 | http://localhost:5000 | Simple prototype with basic banking operations |
| **Database** | 5001 | http://localhost:5001 | Database-connected core banking with PostgreSQL |
| **Enhanced** | 5002 | http://localhost:5002 | CQRS architecture with Domain-Driven Design |
| **Comprehensive** | 5003 | http://localhost:5003 | Full enterprise banking platform (85+ endpoints) |

## Quick Start

### Option 1: Start All APIs at Once
```powershell
# PowerShell
.\start-all-apis.ps1

# Or Windows Batch
.\start-all-apis.bat
```

### Option 2: Start Individual APIs
```powershell
# Minimal API (Port 5000)
cd MinimalWekezaApi
dotnet run

# Database API (Port 5001)
cd DatabaseWekezaApi
dotnet run

# Enhanced API (Port 5002)
cd EnhancedWekezaApi
dotnet run

# Comprehensive API (Port 5003)
cd ComprehensiveWekezaApi
dotnet run
```

## Access Points

### Web Interfaces
- **Minimal**: http://localhost:5000
- **Database**: http://localhost:5001
- **Enhanced**: http://localhost:5002
- **Comprehensive**: http://localhost:5003

### API Documentation (Swagger)
- **Minimal**: http://localhost:5000/swagger
- **Database**: http://localhost:5001/swagger
- **Enhanced**: http://localhost:5002/swagger
- **Comprehensive**: http://localhost:5003/swagger

### System Status APIs
- **Minimal**: http://localhost:5000/api/status
- **Database**: http://localhost:5001/api/status
- **Enhanced**: http://localhost:5002/api/status
- **Comprehensive**: http://localhost:5003/api/status

## Overview Dashboard

Open `wekeza-overview.html` in your browser for a unified dashboard with links to all APIs.

## API Comparison

### 🚀 Minimal API (Port 5000)
- **Purpose**: Quick prototyping and testing
- **Features**: Basic CRUD operations
- **Data**: Mock responses (no database)
- **Best For**: Demos, initial testing

### 🗄️ Database API (Port 5001)
- **Purpose**: Real data persistence
- **Features**: Full banking modules with database
- **Data**: PostgreSQL with Entity Framework
- **Best For**: Development, data testing

### 🏗️ Enhanced API (Port 5002)
- **Purpose**: Clean architecture showcase
- **Features**: CQRS, DDD, Value Objects
- **Data**: PostgreSQL with proper patterns
- **Best For**: Architecture reference, enterprise patterns

### 🏛️ Comprehensive API (Port 5003)
- **Purpose**: Complete banking platform
- **Features**: 18 modules, 85+ endpoints
- **Data**: Mock responses with full feature set
- **Best For**: Feature showcase, client demos

## Owner Information
- **Name**: Emmanuel Odenyire
- **ID**: 28839872
- **Contact**: 0716478835
- **DOB**: 17/March/1992

## Notes
- All APIs can run simultaneously on different ports
- Each API has its own Swagger documentation
- Database APIs require PostgreSQL connection
- Use the overview HTML file for easy navigation between APIs