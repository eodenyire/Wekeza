# Wekeza Bank - Enterprise Risk Management System (ERMS)

> 📦 **Want to push this ERMS to a separate repository?**
> 
> **👉 START HERE: [MIGRATION-INDEX.md](./MIGRATION-INDEX.md)** - Complete migration guide with step-by-step instructions, automation scripts, and documentation.
>
> Quick links:
> - [Step-by-Step Checklist](./STEP-BY-STEP-CHECKLIST.md) - Interactive checklist (RECOMMENDED)
> - [Quick Reference](./QUICK-REFERENCE.md) - 5-minute quick start
> - [Detailed Guide](./REPOSITORY-SETUP-GUIDE.md) - Comprehensive instructions

## Overview

The Wekeza Enterprise Risk Management System (ERMS) is a comprehensive, production-ready risk management platform designed for Wekeza Bank. This system implements industry best practices for identifying, assessing, monitoring, and mitigating risks across the organization.

## MVP 4.0 Features

### Risk Assessment & Classification
- ✅ Multi-dimensional risk categorization (Credit, Operational, Market, Liquidity, Strategic)
- ✅ Risk severity matrix (5x5 matrix: Rare to Almost Certain vs Insignificant to Catastrophic)
- ✅ Automated risk scoring and rating
- ✅ Risk appetite and tolerance thresholds
- ✅ Risk heat maps and visualization

### Risk Register Management
- ✅ Centralized risk register with unique risk IDs
- ✅ Risk ownership and accountability tracking
- ✅ Risk status lifecycle management
- ✅ Risk dependencies and relationships
- ✅ Historical risk tracking and trending

### Risk Treatment & Controls
- ✅ Control effectiveness assessment
- ✅ Mitigation action plans with deadlines
- ✅ Control testing and validation
- ✅ Residual risk calculation
- ✅ Treatment strategy tracking (Accept, Mitigate, Transfer, Avoid)

### Risk Monitoring & Reporting
- ✅ Key Risk Indicators (KRIs) with thresholds
- ✅ Real-time risk dashboards
- ✅ Automated risk reporting
- ✅ Executive risk summaries
- ✅ Regulatory compliance reporting
- ✅ Audit trail for all risk activities

### Integration & Compliance
- ✅ Integration with existing Wekeza Core Banking System
- ✅ AML/CFT risk integration
- ✅ Compliance and regulatory reporting
- ✅ Basel III framework alignment
- ✅ COSO ERM framework implementation
- ✅ ISO 31000 risk management standards

## Architecture

```
WekezaERMS/
├── Domain/                          # Domain models and business logic
│   ├── Entities/                    # Risk entities and aggregates
│   ├── ValueObjects/                # Risk-specific value objects
│   ├── Enums/                       # Risk classifications and statuses
│   └── Events/                      # Domain events for risk activities
├── Application/                     # Application services and use cases
│   ├── Commands/                    # CQRS commands for risk operations
│   ├── Queries/                     # CQRS queries for risk data
│   ├── DTOs/                        # Data transfer objects
│   └── Validators/                  # Input validation rules
├── Infrastructure/                  # Data access and external services
│   ├── Persistence/                 # Database repositories
│   ├── Integration/                 # Integration with Wekeza Core
│   └── Services/                    # External services integration
├── API/                            # REST API endpoints
│   ├── Controllers/                 # API controllers
│   ├── Middleware/                  # Custom middleware
│   └── Configuration/               # API configuration
└── Docs/                           # Documentation
    ├── API-REFERENCE.md            # API documentation
    ├── IMPLEMENTATION-GUIDE.md     # Implementation guide
    └── INTEGRATION-GUIDE.md        # Integration with Wekeza Core
```

## Technology Stack

- **.NET 8** - Latest LTS framework
- **Entity Framework Core** - ORM with PostgreSQL
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Swagger/OpenAPI** - API documentation
- **Serilog** - Structured logging
- **Hangfire** - Background job processing for risk calculations

## Risk Categories

### 1. Credit Risk
- Counterparty default risk
- Concentration risk
- Credit rating deterioration
- Collateral valuation risk

### 2. Operational Risk
- Process failures
- System outages
- Human error
- Fraud and security breaches
- Business continuity

### 3. Market Risk
- Interest rate risk
- Foreign exchange risk
- Equity price risk
- Commodity price risk

### 4. Liquidity Risk
- Funding liquidity risk
- Market liquidity risk
- Withdrawal risk
- Contingent liquidity risk

### 5. Strategic Risk
- Business model risk
- Competition risk
- Regulatory change risk
- Reputation risk
- Technology disruption

### 6. Compliance Risk
- Regulatory compliance
- Policy violations
- Legal risk
- Sanctions risk

## Risk Matrix (5x5)

### Likelihood Scale
1. **Rare** - May occur only in exceptional circumstances (< 5% probability)
2. **Unlikely** - Could occur at some time (5-25% probability)
3. **Possible** - Might occur at some time (25-50% probability)
4. **Likely** - Will probably occur in most circumstances (50-75% probability)
5. **Almost Certain** - Expected to occur in most circumstances (> 75% probability)

### Impact Scale
1. **Insignificant** - Minimal financial impact (< $10K), no regulatory concern
2. **Minor** - Small financial impact ($10K-$100K), minor regulatory concern
3. **Moderate** - Moderate financial impact ($100K-$1M), some regulatory concern
4. **Major** - Significant financial impact ($1M-$10M), major regulatory concern
5. **Catastrophic** - Severe financial impact (> $10M), severe regulatory/reputational damage

### Risk Levels (Calculated from Matrix)
- **Low Risk** (Score 1-4): Accept with monitoring
- **Medium Risk** (Score 5-9): Accept with enhanced controls
- **High Risk** (Score 10-15): Mitigate with action plans
- **Very High Risk** (Score 16-20): Immediate action required
- **Critical Risk** (Score 21-25): Escalate to Board/Executive

## Getting Started

### Prerequisites
- .NET 8 SDK
- PostgreSQL 15+
- Access to Wekeza Core Banking System
- Visual Studio 2022 or VS Code

### Installation

1. Navigate to WekezaERMS directory
```bash
cd WekezaERMS
```

2. Build the solution
```bash
dotnet build
```

3. Update database connection string
```json
{
  "ConnectionStrings": {
    "ERMSConnection": "Host=localhost;Database=WekezaERMS;Username=your_user;Password=your_password"
  }
}
```

4. Run database migrations
```bash
dotnet ef database update
```

5. Start the ERMS API
```bash
dotnet run --project API
```

## API Endpoints

### Risk Register
- `POST /api/risks` - Create new risk entry
- `GET /api/risks` - List all risks with filtering
- `GET /api/risks/{id}` - Get risk details
- `PUT /api/risks/{id}` - Update risk entry
- `DELETE /api/risks/{id}` - Delete risk entry

### Risk Assessment
- `POST /api/risks/{id}/assess` - Perform risk assessment
- `POST /api/risks/{id}/reassess` - Reassess existing risk
- `GET /api/risks/{id}/history` - Get assessment history

### Risk Treatment
- `POST /api/risks/{id}/controls` - Add control to risk
- `PUT /api/controls/{id}` - Update control
- `POST /api/risks/{id}/mitigations` - Add mitigation action
- `GET /api/risks/{id}/treatment-plan` - Get treatment plan

### Risk Monitoring
- `GET /api/risks/dashboard` - Risk dashboard data
- `GET /api/risks/kris` - Key Risk Indicators
- `POST /api/risks/kris` - Create KRI
- `GET /api/risks/reports` - Generate risk reports

### Integration
- `POST /api/integration/sync` - Sync with Wekeza Core
- `GET /api/integration/status` - Integration status

## Key Features

### Real-Time Risk Monitoring
- Automated risk score calculations
- Real-time dashboard updates
- Alert notifications for threshold breaches
- Trend analysis and forecasting

### Compliance & Audit
- Complete audit trail for all risk activities
- Regulatory reporting automation
- Compliance status tracking
- Policy adherence monitoring

### Risk Analytics
- Heat maps and risk distribution
- Historical trend analysis
- Correlation analysis between risks
- Predictive risk modeling

## Security & Access Control

### User Roles
- **Risk Manager** - Full access to ERMS
- **Risk Officer** - Manage risks in assigned areas
- **Risk Viewer** - Read-only access to risk data
- **Auditor** - Read-only access with audit trail
- **Executive** - Dashboard and summary reports
- **Administrator** - System configuration and user management

## Integration with Wekeza Core

The ERMS integrates seamlessly with Wekeza Core Banking System:
- **Credit Risk** - Loan portfolio risk monitoring
- **Operational Risk** - Transaction processing risk tracking
- **Compliance Risk** - AML/CFT and sanctions screening
- **Market Risk** - Treasury and FX exposure monitoring
- **Liquidity Risk** - Cash flow and funding analysis

## Regulatory Compliance

### Basel III Framework
- Capital adequacy risk assessment
- Risk-weighted assets calculation
- Stress testing framework
- Liquidity coverage ratio monitoring

### ISO 31000 Standards
- Risk management principles
- Risk management framework
- Risk management process

### COSO ERM Framework
- Governance and culture
- Strategy and objective-setting
- Performance monitoring
- Review and revision
- Information, communication, and reporting

## Performance & Scalability

- Sub-second risk score calculations
- Optimized database queries with proper indexing
- Caching for frequently accessed risk data
- Background processing for complex calculations
- Horizontal scaling support

## Support & Documentation

For detailed documentation, see:
- [API Reference](./Docs/API-REFERENCE.md) - Complete API documentation
- [Implementation Guide](./Docs/IMPLEMENTATION-GUIDE.md) - Setup and configuration
- [Integration Guide](./Docs/INTEGRATION-GUIDE.md) - Wekeza Core integration

## License

Proprietary - © 2026 Wekeza Bank. All rights reserved.

## Contact

For support or questions:
- Email: risk@wekeza.com
- Documentation: https://docs.wekeza.com/erms
- Issues: https://github.com/eodenyire/WekezaERMS/issues

---

Built with ❤️ by the Wekeza Risk Management Team
