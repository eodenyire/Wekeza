# Wekeza Bank - Core Banking System

![Wekeza Bank Logo](/Assets/Logos/BCO.19e37d75-da58-4d51-a993-833739c80b13.png)

## Overview

Wekeza Bank is a modern, production-ready core banking system built with .NET 8, following Domain-Driven Design (DDD) and Clean Architecture principles. Designed specifically for the Nairobi fintech market with full M-Pesa integration.

## Features

### Account Management
- ✅ Open personal and business accounts
- ✅ Multi-currency support (KES, USD, EUR, etc.)
- ✅ Account freeze/unfreeze capabilities
- ✅ Business account verification with KYC
- ✅ Multi-signatory support for corporate accounts

### Transactions
- ✅ Deposits and withdrawals
- ✅ Internal transfers
- ✅ M-Pesa integration (STK Push & callbacks)
- ✅ Cheque deposits with clearing logic
- ✅ Real-time balance updates
- ✅ Transaction history and statements

### Lending
- ✅ Loan applications
- ✅ Automated approval workflows
- ✅ Loan disbursement
- ✅ Repayment tracking
- ✅ Interest calculation
- ✅ Amortization schedules

### Cards
- ✅ Debit/Credit card issuance
- ✅ ATM withdrawal processing
- ✅ Daily withdrawal limits
- ✅ Card cancellation/hotlisting
- ✅ Card-to-account linking

### Security & Compliance
- ✅ JWT-based authentication
- ✅ Role-based access control (RBAC)
- ✅ Rate limiting
- ✅ Audit trails
- ✅ PCI-DSS compliant architecture
- ✅ AML/KYC workflows

## Architecture

```
Wekeza/
├── Core/
│   ├── Wekeza.Core.Domain/          # Domain entities, value objects, aggregates
│   ├── Wekeza.Core.Application/     # Use cases, commands, queries, validators
│   ├── Wekeza.Core.Infrastructure/  # Data access, external services
│   └── Wekeza.Core.Api/            # REST API, controllers, middleware
└── Tests/
    ├── Wekeza.Core.UnitTests/       # Unit tests
    └── Wekeza.Core.IntegrationTests/ # Integration tests
```

### Technology Stack

- **.NET 8** - Latest LTS framework
- **Entity Framework Core** - ORM with PostgreSQL
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Testing framework
- **Dapper** - High-performance queries
- **Hangfire** - Background job processing

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL 15+
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
```bash
git clone https://github.com/wekeza-bank/core.git
cd wekeza
```

2. Update connection string in `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=WekezaCoreDB;Username=your_user;Password=your_password"
  }
}
```

3. Run database migrations
```bash
dotnet ef database update --project Core/Wekeza.Core.Infrastructure
```

4. Run the application
```bash
dotnet run --project Core/Wekeza.Core.Api
```

5. Access Swagger UI
```
https://localhost:5001/swagger
```

### Authentication

1. Login to get JWT token:
```bash
POST /api/authentication/login
{
  "username": "admin",
  "password": "your_password"
}
```

2. Use the token in subsequent requests:
```
Authorization: Bearer {your_token}
```

## User Roles

- **Customer** - Account holders, can view balances and make transactions
- **Teller** - Branch staff, can process deposits/withdrawals
- **LoanOfficer** - Can approve and disburse loans
- **RiskOfficer** - Can verify business accounts, freeze accounts
- **Administrator** - Full system access
- **SystemService** - For automated processes

## API Endpoints

### Authentication
- `POST /api/authentication/login` - Login and get JWT token
- `GET /api/authentication/me` - Get current user info

### Accounts
- `POST /api/accounts/open` - Open new account
- `POST /api/accounts/freeze` - Freeze account
- `POST /api/accounts/close` - Close account
- `GET /api/accounts/{id}/balance` - Get account balance

### Transactions
- `POST /api/transactions/deposit` - Deposit funds
- `POST /api/transactions/withdraw` - Withdraw funds
- `POST /api/transactions/transfer` - Transfer between accounts
- `GET /api/transactions/statement` - Get account statement

### Loans
- `POST /api/loans/apply` - Apply for loan
- `POST /api/loans/approve` - Approve loan (LoanOfficer)
- `POST /api/loans/disburse` - Disburse loan (LoanOfficer)
- `POST /api/loans/repay` - Make loan repayment

### Cards
- `POST /api/cards/issue` - Issue new card
- `POST /api/cards/cancel` - Cancel card
- `POST /api/cards/withdraw` - ATM withdrawal

## Testing

Run unit tests:
```bash
dotnet test Tests/Wekeza.Core.UnitTests
```

Run integration tests:
```bash
dotnet test Tests/Wekeza.Core.IntegrationTests
```

Run all tests with coverage:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Configuration

### JWT Settings
```json
{
  "JwtSettings": {
    "Secret": "your-secret-key-min-32-chars",
    "Issuer": "https://api.wekeza.com",
    "Audience": "https://wekeza.com",
    "ExpiryMinutes": 60
  }
}
```

### M-Pesa Configuration
```json
{
  "MpesaConfig": {
    "ConsumerKey": "YOUR_CONSUMER_KEY",
    "ConsumerSecret": "YOUR_CONSUMER_SECRET",
    "ShortCode": "123456",
    "PassKey": "YOUR_PASS_KEY",
    "CallbackUrl": "https://api.wekeza.com/api/transactions/deposit"
  }
}
```

## Performance

- Sub-millisecond transaction processing
- Dapper for high-performance read queries
- Optimized EF Core queries with proper indexing
- Background job processing for non-critical tasks
- Rate limiting to prevent abuse

## Security Best Practices

1. **Never commit secrets** - Use environment variables or Azure Key Vault
2. **Rotate JWT secrets** regularly
3. **Enable HTTPS** in production
4. **Implement IP whitelisting** for admin endpoints
5. **Regular security audits** and penetration testing
6. **Monitor for suspicious activity** using audit logs

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

Proprietary - © 2026 Wekeza Bank. All rights reserved.

## Support

For technical support or questions:
- Email: dev@wekeza.com
- Documentation: https://docs.wekeza.com
- Issues: https://github.com/wekeza-bank/core/issues

---

Built with ❤️ by the Wekeza Engineering Team
