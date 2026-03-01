# 🏦 Welcome to Wekeza Bank!

## Your Complete Banking System is Ready!

This is a production-ready core banking system built with .NET 8, featuring:
- ✅ Account Management
- ✅ Transactions (including M-Pesa)
- ✅ Loans & Lending
- ✅ Card Management
- ✅ Security & Authentication
- ✅ Complete API Documentation

---

## 🚀 Get Started in 3 Steps

### Option 1: Quick Start (Recommended for First Time)

```powershell
# One command to set everything up!
.\wekeza.ps1 setup

# Then start the application
.\wekeza.ps1 start
```

**Access**: https://localhost:5001/swagger

### Option 2: Manual Setup (If you prefer step-by-step)

```powershell
# 1. Setup database
.\scripts\setup-local-db.ps1

# 2. Run migrations
.\scripts\run-migrations.ps1

# 3. Start application
.\scripts\start-local.ps1
```

### Option 3: Docker (For containerized deployment)

```powershell
# Start with Docker
.\wekeza.ps1 docker-up
```

**Access**: http://localhost:8080/swagger

---

## 📚 Documentation Guide

Choose your path based on your needs:

### 🎯 I want to get started quickly
→ Read: **`QUICKSTART.md`** (5 minutes)

### 💻 I want to develop locally
→ Read: **`SETUP-LOCAL.md`** (Complete local setup guide)

### 🐳 I want to use Docker
→ Read: **`SETUP-DOCKER.md`** (Docker containerization)

### 🚀 I want to deploy to production
→ Read: **`DEPLOYMENT-GUIDE.md`** (Complete deployment path)

### 📊 I want to see what's implemented
→ Read: **`PROJECT-STATUS.md`** (Feature list & statistics)

### 📖 I want the full overview
→ Read: **`README.md`** (Complete project documentation)

---

## 🎮 Master Control Script

Use the `wekeza.ps1` script for common tasks:

```powershell
# Setup everything
.\wekeza.ps1 setup

# Start application
.\wekeza.ps1 start

# Run tests
.\wekeza.ps1 test

# Create migration
.\wekeza.ps1 migrate -MigrationName "YourMigration"

# Docker commands
.\wekeza.ps1 docker-up
.\wekeza.ps1 docker-down
.\wekeza.ps1 logs

# Get help
.\wekeza.ps1 help
```

---

## 🔑 First API Call

Once the application is running:

1. **Open Swagger UI**: https://localhost:5001/swagger

2. **Login** (POST `/api/authentication/login`):
   ```json
   {
     "username": "admin",
     "password": "admin123"
   }
   ```

3. **Copy the token** from the response

4. **Click "Authorize"** button (🔒 icon)

5. **Paste your token** and click "Authorize"

6. **Try any endpoint!** For example, open an account:
   ```json
   {
     "firstName": "John",
     "lastName": "Doe",
     "email": "john@example.com",
     "phoneNumber": "254712345678",
     "identificationNumber": "12345678",
     "currencyCode": "KES",
     "initialDeposit": 1000
   }
   ```

---

## 🏗️ Project Structure

```
Wekeza/
├── Core/
│   ├── Wekeza.Core.Domain/          # Business logic & entities
│   ├── Wekeza.Core.Application/     # Use cases & commands
│   ├── Wekeza.Core.Infrastructure/  # Data access & services
│   └── Wekeza.Core.Api/            # REST API & controllers
├── Tests/
│   ├── Wekeza.Core.UnitTests/       # 32 unit tests
│   └── Wekeza.Core.IntegrationTests/
├── scripts/                          # Helper scripts
│   ├── setup-local-db.ps1           # Database setup
│   ├── run-migrations.ps1           # Migration management
│   ├── start-local.ps1              # Start application
│   └── test-api.ps1                 # API testing
├── wekeza.ps1                       # Master control script
├── docker-compose.yml               # Docker orchestration
├── Dockerfile                       # Container definition
└── Documentation files (.md)
```

---

## ✨ Key Features

### Account Management
- Open personal & business accounts
- Multi-currency support (KES, USD, EUR, etc.)
- Freeze/unfreeze accounts
- Business verification with KYC
- Multi-signatory support

### Transactions
- Deposits (cash, cheque, mobile money)
- Withdrawals
- Internal transfers
- M-Pesa integration (STK Push & callbacks)
- Real-time balance updates
- Transaction history & statements

### Lending
- Loan applications
- Approval workflows
- Automated disbursement
- Repayment tracking
- Interest calculation
- Amortization schedules

### Cards
- Issue debit/credit/prepaid cards
- ATM withdrawal processing
- Daily withdrawal limits
- Card cancellation/hotlisting

### Security
- JWT authentication
- Role-based authorization (6 roles)
- Rate limiting
- Audit trails
- Input validation

---

## 🛠️ Technology Stack

- **.NET 8** - Latest LTS framework
- **PostgreSQL 15** - Reliable database
- **Entity Framework Core** - ORM
- **MediatR** - CQRS pattern
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization
- **xUnit** - Testing framework

---

## 🎯 What to Do Next

### Day 1: Local Development
1. ✅ Run `.\wekeza.ps1 setup`
2. ✅ Explore Swagger UI
3. ✅ Test authentication
4. ✅ Create test accounts
5. ✅ Make test transactions

### Day 2: Understanding the Code
1. ✅ Review Domain layer (business logic)
2. ✅ Check Application layer (use cases)
3. ✅ Explore API endpoints
4. ✅ Run unit tests: `.\wekeza.ps1 test`

### Week 1: Docker & Testing
1. ✅ Set up Docker: `.\wekeza.ps1 docker-up`
2. ✅ Test all features
3. ✅ Configure M-Pesa (sandbox)
4. ✅ Review security settings

### Week 2+: Production Deployment
1. ✅ Follow `DEPLOYMENT-GUIDE.md`
2. ✅ Set up monitoring
3. ✅ Configure backups
4. ✅ Security audit
5. ✅ Go live!

---

## 🆘 Need Help?

### Common Issues

**"Cannot connect to database"**
```powershell
# Check if PostgreSQL is running
Get-Service postgresql*

# If stopped, start it
Start-Service postgresql-x64-15
```

**"Port already in use"**
- Edit `Core/Wekeza.Core.Api/Properties/launchSettings.json`
- Change ports from 5000/5001 to different values

**"Migration failed"**
```powershell
# Reset and try again
.\scripts\setup-local-db.ps1  # Answer 'yes' to recreate
.\scripts\run-migrations.ps1
```

### Getting Support

1. **Check logs**: Console output or `docker-compose logs -f`
2. **Health check**: Visit `/health` endpoint
3. **Review docs**: Check the relevant .md file
4. **Test connection**: Use pgAdmin to verify database

---

## 📊 Project Statistics

- **5 Domain Aggregates** (Account, Customer, Loan, Transaction, Card)
- **18 Commands** with validators
- **5 Queries** for data retrieval
- **25+ API Endpoints**
- **32 Unit Tests** (100% validator coverage)
- **6 User Roles** for authorization
- **4 Value Objects** for domain modeling

---

## 🎓 Learning Path

### Beginner
1. Start with `QUICKSTART.md`
2. Run the application
3. Test endpoints in Swagger
4. Review `README.md`

### Intermediate
1. Read `SETUP-LOCAL.md`
2. Understand the architecture
3. Review domain models
4. Run and write tests

### Advanced
1. Study `DEPLOYMENT-GUIDE.md`
2. Set up Docker
3. Configure CI/CD
4. Deploy to production

---

## 🏆 You're All Set!

Your Wekeza Bank system is ready to go. Choose your starting point:

- **Quick Start**: `.\wekeza.ps1 setup` → `.\wekeza.ps1 start`
- **Read First**: Open `QUICKSTART.md`
- **Docker**: `.\wekeza.ps1 docker-up`

**Happy Banking! 🎉**

---

*Built with ❤️ using .NET 8 and Clean Architecture principles*
