# WekezaERMS Implementation Guide

## Table of Contents
1. [Overview](#overview)
2. [System Requirements](#system-requirements)
3. [Installation](#installation)
4. [Configuration](#configuration)
5. [Database Setup](#database-setup)
6. [User Management](#user-management)
7. [Risk Management Workflow](#risk-management-workflow)
8. [Best Practices](#best-practices)
9. [Troubleshooting](#troubleshooting)

---

## Overview

The Wekeza Enterprise Risk Management System (ERMS) provides comprehensive risk management capabilities for Wekeza Bank. This guide will walk you through the implementation process from initial setup to production deployment.

### Key Capabilities
- Risk identification and assessment
- Risk treatment and control management
- Key Risk Indicator (KRI) monitoring
- Risk reporting and dashboards
- Integration with Wekeza Core Banking System

---

## System Requirements

### Hardware Requirements
- **CPU**: 4 cores minimum (8 cores recommended)
- **RAM**: 8 GB minimum (16 GB recommended)
- **Storage**: 50 GB SSD minimum (100 GB recommended)
- **Network**: 1 Gbps network interface

### Software Requirements
- **.NET 8 SDK** or later
- **PostgreSQL 15** or later
- **Redis** (for caching and session management)
- **Operating System**: 
  - Ubuntu 22.04 LTS or later
  - Windows Server 2019 or later
  - macOS 12 or later (for development)

### Client Requirements
- Modern web browser (Chrome, Firefox, Safari, Edge)
- JavaScript enabled
- Minimum screen resolution: 1366x768

---

## Installation

### Step 1: Clone or Download ERMS

```bash
# Navigate to Wekeza repository
cd /path/to/Wekeza

# ERMS is located in WekezaERMS folder
cd WekezaERMS
```

### Step 2: Install .NET Dependencies

```bash
# Restore NuGet packages (if .NET projects are created)
dotnet restore

# Build the solution
dotnet build
```

### Step 3: Install Database

```bash
# Install PostgreSQL
# Ubuntu/Debian
sudo apt update
sudo apt install postgresql-15 postgresql-contrib

# macOS
brew install postgresql@15

# Windows
# Download installer from https://www.postgresql.org/download/
```

### Step 4: Install Redis (Optional but Recommended)

```bash
# Ubuntu/Debian
sudo apt install redis-server

# macOS
brew install redis

# Start Redis
sudo systemctl start redis
# or
redis-server
```

---

## Configuration

### Database Configuration

Create a `appsettings.json` file (when .NET API project is created):

```json
{
  "ConnectionStrings": {
    "ERMSConnection": "Host=localhost;Port=5432;Database=WekezaERMS;Username=wekeza_user;Password=your_secure_password",
    "WekezaCoreConnection": "Host=localhost;Port=5432;Database=WekezaCoreDB;Username=wekeza_user;Password=your_secure_password"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-at-least-32-characters-long",
    "Issuer": "https://api.wekeza.com",
    "Audience": "https://wekeza.com",
    "ExpiryMinutes": 60
  },
  "Redis": {
    "Configuration": "localhost:6379",
    "InstanceName": "WekezaERMS:"
  },
  "ERMSSettings": {
    "DefaultRiskReviewPeriodMonths": 3,
    "CriticalRiskEscalationHours": 24,
    "KRIMeasurementReminderDays": 3,
    "EnableAutoSync": true,
    "SyncIntervalHours": 12
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "WekezaERMS": "Debug"
    }
  }
}
```

### Environment Variables

For production, use environment variables instead of configuration files:

```bash
# Database
export ERMS_DB_HOST=localhost
export ERMS_DB_PORT=5432
export ERMS_DB_NAME=WekezaERMS
export ERMS_DB_USER=wekeza_user
export ERMS_DB_PASSWORD=your_secure_password

# JWT
export JWT_SECRET=your-secret-key-at-least-32-characters-long
export JWT_ISSUER=https://api.wekeza.com
export JWT_AUDIENCE=https://wekeza.com

# Redis
export REDIS_CONNECTION=localhost:6379
```

---

## Database Setup

### Step 1: Create Database User

```sql
-- Connect to PostgreSQL as postgres user
psql -U postgres

-- Create dedicated user for ERMS
CREATE USER wekeza_user WITH PASSWORD 'your_secure_password';

-- Create database
CREATE DATABASE WekezaERMS OWNER wekeza_user;

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE WekezaERMS TO wekeza_user;
```

### Step 2: Database Schema

The ERMS uses the following main tables:

#### Risks Table
```sql
CREATE TABLE risks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    risk_code VARCHAR(50) UNIQUE NOT NULL,
    title VARCHAR(500) NOT NULL,
    description TEXT NOT NULL,
    category INTEGER NOT NULL,
    status INTEGER NOT NULL,
    inherent_likelihood INTEGER NOT NULL,
    inherent_impact INTEGER NOT NULL,
    inherent_risk_score INTEGER NOT NULL,
    inherent_risk_level INTEGER NOT NULL,
    residual_likelihood INTEGER,
    residual_impact INTEGER,
    residual_risk_score INTEGER,
    residual_risk_level INTEGER,
    treatment_strategy INTEGER NOT NULL,
    owner_id UUID NOT NULL,
    department VARCHAR(200) NOT NULL,
    identified_date TIMESTAMP NOT NULL,
    last_assessment_date TIMESTAMP,
    next_review_date TIMESTAMP NOT NULL,
    risk_appetite INTEGER NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_at TIMESTAMP,
    updated_by UUID
);

-- Indexes for performance
CREATE INDEX idx_risks_category ON risks(category);
CREATE INDEX idx_risks_status ON risks(status);
CREATE INDEX idx_risks_risk_level ON risks(inherent_risk_level);
CREATE INDEX idx_risks_owner ON risks(owner_id);
CREATE INDEX idx_risks_department ON risks(department);
```

#### Risk Controls Table
```sql
CREATE TABLE risk_controls (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    risk_id UUID NOT NULL REFERENCES risks(id) ON DELETE CASCADE,
    control_name VARCHAR(500) NOT NULL,
    description TEXT NOT NULL,
    control_type VARCHAR(50) NOT NULL,
    effectiveness INTEGER,
    last_tested_date TIMESTAMP,
    next_test_date TIMESTAMP,
    owner_id UUID NOT NULL,
    testing_frequency VARCHAR(50) NOT NULL,
    testing_evidence TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_at TIMESTAMP,
    updated_by UUID
);

CREATE INDEX idx_controls_risk ON risk_controls(risk_id);
CREATE INDEX idx_controls_owner ON risk_controls(owner_id);
```

#### Mitigation Actions Table
```sql
CREATE TABLE mitigation_actions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    risk_id UUID NOT NULL REFERENCES risks(id) ON DELETE CASCADE,
    action_title VARCHAR(500) NOT NULL,
    description TEXT NOT NULL,
    owner_id UUID NOT NULL,
    status INTEGER NOT NULL,
    due_date TIMESTAMP NOT NULL,
    completed_date TIMESTAMP,
    progress_percentage INTEGER NOT NULL DEFAULT 0,
    estimated_cost DECIMAL(18,2) NOT NULL DEFAULT 0,
    actual_cost DECIMAL(18,2) NOT NULL DEFAULT 0,
    notes TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_at TIMESTAMP,
    updated_by UUID
);

CREATE INDEX idx_mitigations_risk ON mitigation_actions(risk_id);
CREATE INDEX idx_mitigations_status ON mitigation_actions(status);
CREATE INDEX idx_mitigations_due_date ON mitigation_actions(due_date);
```

#### Key Risk Indicators Table
```sql
CREATE TABLE key_risk_indicators (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    risk_id UUID NOT NULL REFERENCES risks(id) ON DELETE CASCADE,
    name VARCHAR(500) NOT NULL,
    description TEXT NOT NULL,
    measurement_unit VARCHAR(50) NOT NULL,
    current_value DECIMAL(18,4) NOT NULL DEFAULT 0,
    threshold_warning DECIMAL(18,4) NOT NULL,
    threshold_critical DECIMAL(18,4) NOT NULL,
    frequency VARCHAR(50) NOT NULL,
    last_measured_date TIMESTAMP,
    next_measurement_date TIMESTAMP,
    status INTEGER NOT NULL,
    data_source VARCHAR(200) NOT NULL,
    owner_id UUID NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_at TIMESTAMP,
    updated_by UUID
);

CREATE TABLE kri_measurements (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    kri_id UUID NOT NULL REFERENCES key_risk_indicators(id) ON DELETE CASCADE,
    value DECIMAL(18,4) NOT NULL,
    measured_date TIMESTAMP NOT NULL,
    status INTEGER NOT NULL,
    notes TEXT,
    recorded_by UUID NOT NULL
);

CREATE INDEX idx_kris_risk ON key_risk_indicators(risk_id);
CREATE INDEX idx_kri_measurements_kri ON kri_measurements(kri_id);
```

### Step 3: Run Migrations

```bash
# When EF Core migrations are created
dotnet ef database update --project Infrastructure --startup-project API
```

---

## User Management

### User Roles

The ERMS defines the following roles:

1. **RiskManager** - Full access to all risk management functions
2. **RiskOfficer** - Manage risks in assigned areas
3. **RiskViewer** - Read-only access to risk data
4. **Auditor** - Read-only with audit trail access
5. **Executive** - Dashboard and summary reports
6. **Administrator** - System configuration

### Creating Users

Users are managed through the Wekeza Core authentication system. Assign ERMS roles to existing Wekeza users:

```sql
-- Example: Grant RiskManager role to a user
INSERT INTO user_roles (user_id, role_name, created_at)
VALUES ('user-uuid-here', 'RiskManager', NOW());
```

---

## Risk Management Workflow

### 1. Risk Identification

```
Identify Risk → Create Risk Entry → Assign Owner → Assess Inherent Risk
```

**Steps:**
1. Create new risk with basic information
2. Assign risk owner and department
3. Classify risk category
4. Perform initial inherent risk assessment

### 2. Risk Assessment

```
Assess Likelihood → Assess Impact → Calculate Risk Score → Determine Risk Level
```

**Risk Matrix:**
- Score 1-4: Low Risk
- Score 5-9: Medium Risk
- Score 10-15: High Risk
- Score 16-20: Very High Risk
- Score 21-25: Critical Risk

### 3. Risk Treatment

```
Select Strategy → Implement Controls → Create Mitigation Actions → Monitor Progress
```

**Treatment Strategies:**
- **Accept**: Low risks within appetite
- **Mitigate**: Implement controls to reduce risk
- **Transfer**: Insurance or outsourcing
- **Avoid**: Eliminate the activity
- **Share**: Partnership or joint ventures

### 4. Risk Monitoring

```
Define KRIs → Set Thresholds → Record Measurements → Generate Alerts
```

**KRI Types:**
- Leading indicators (predictive)
- Lagging indicators (historical)
- Threshold-based alerts

### 5. Risk Reporting

```
Generate Reports → Executive Dashboard → Regulatory Reports → Board Reporting
```

---

## Best Practices

### Risk Identification
1. Conduct regular risk assessment workshops
2. Engage all business units in identification
3. Use structured risk taxonomies
4. Document assumptions and methodology

### Risk Assessment
1. Use consistent rating scales
2. Document assessment rationale
3. Involve subject matter experts
4. Review and update regularly (at least quarterly)

### Control Design
1. Design controls based on root causes
2. Ensure controls are cost-effective
3. Test controls regularly
4. Document control procedures

### KRI Management
1. Select meaningful indicators
2. Set realistic thresholds
3. Automate data collection where possible
4. Review KRI effectiveness regularly

### Reporting
1. Tailor reports to audience
2. Focus on actionable insights
3. Use visualizations effectively
4. Maintain consistent reporting schedules

---

## Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to database

**Solution**:
```bash
# Check PostgreSQL is running
sudo systemctl status postgresql

# Test connection
psql -h localhost -U wekeza_user -d WekezaERMS

# Check firewall rules
sudo ufw status
```

### Performance Issues

**Problem**: Slow query performance

**Solution**:
```sql
-- Analyze query performance
EXPLAIN ANALYZE SELECT * FROM risks WHERE category = 1;

-- Update statistics
ANALYZE risks;

-- Rebuild indexes
REINDEX TABLE risks;
```

### Integration Issues

**Problem**: Cannot sync with Wekeza Core

**Solution**:
1. Verify network connectivity
2. Check authentication credentials
3. Review integration logs
4. Verify database permissions

### Common Errors

#### Error: "Risk appetite exceeded"
- Review risk assessment
- Check if controls are effective
- Consider escalation to management

#### Error: "KRI threshold breached"
- Investigate root cause
- Review related risks
- Update mitigation actions

---

## Production Deployment

### Pre-Deployment Checklist

- [ ] Database backup completed
- [ ] Connection strings configured
- [ ] SSL certificates installed
- [ ] Firewall rules configured
- [ ] Monitoring tools enabled
- [ ] Load balancer configured (if applicable)
- [ ] Backup strategy implemented
- [ ] Disaster recovery plan documented

### Deployment Steps

1. **Backup existing data**
```bash
pg_dump -U wekeza_user WekezaERMS > backup_$(date +%Y%m%d).sql
```

2. **Deploy application**
```bash
# Build release version
dotnet publish -c Release -o /var/www/erms

# Set permissions
sudo chown -R www-data:www-data /var/www/erms
```

3. **Configure web server** (Nginx example)
```nginx
server {
    listen 443 ssl;
    server_name erms.wekeza.com;

    ssl_certificate /etc/ssl/certs/wekeza.crt;
    ssl_certificate_key /etc/ssl/private/wekeza.key;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

4. **Start application**
```bash
sudo systemctl start erms
sudo systemctl enable erms
```

---

## Support

For implementation support:
- Email: support@wekeza.com
- Documentation: https://docs.wekeza.com/erms
- Slack: #erms-support

---

## Appendix

### Sample Risk Entry

```json
{
  "riskCode": "RISK-2024-001",
  "title": "Credit Concentration Risk - Corporate Sector",
  "description": "The bank has significant exposure to the corporate lending sector, with over 65% of the loan portfolio concentrated in this segment. This creates vulnerability to economic downturns affecting corporate borrowers.",
  "category": "Credit",
  "inherentLikelihood": "Likely",
  "inherentImpact": "Major",
  "treatmentStrategy": "Mitigate",
  "department": "Credit Risk Management",
  "riskAppetite": 12
}
```

### Regulatory References

- **Basel III**: Risk Management Framework
- **ISO 31000**: Risk Management Guidelines
- **COSO ERM**: Enterprise Risk Management Framework
- **CBK Prudential Guidelines**: Central Bank of Kenya Requirements
