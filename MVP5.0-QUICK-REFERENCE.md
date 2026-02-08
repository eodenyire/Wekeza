# MVP5.0 - Quick Reference Card

## One-Command Start

```bash
# Linux/Mac
./start-mvp5.sh

# Windows
.\start-mvp5.ps1
```

## Access URLs

| Service | URL |
|---------|-----|
| Minimal API | http://localhost:8081 |
| Database API | http://localhost:8082/swagger |
| Enhanced API | http://localhost:8083/swagger |
| Comprehensive API | http://localhost:8084/swagger |
| PostgreSQL | localhost:5432 |

## Common Commands

```bash
# View all services
docker-compose -f docker-compose.mvp5.yml ps

# View logs
docker-compose -f docker-compose.mvp5.yml logs -f

# Stop all
docker-compose -f docker-compose.mvp5.yml down

# Restart service
docker-compose -f docker-compose.mvp5.yml restart [service-name]
```

## Database Connection

```
Host: localhost
Port: 5432
Database: WekezaCoreDB
Username: wekeza_app
Password: WekeZa2026!SecurePass
```

## API Test Flow

1. Create Customer → POST /api/customers
2. Open Account → POST /api/accounts  
3. Deposit Funds → POST /api/transactions/deposit
4. Check Balance → GET /api/accounts/{id}/balance
5. Transfer Money → POST /api/transactions/transfer

## Quick Status Check

✅ All 4 APIs: Building successfully
✅ Docker: Ready for deployment
✅ Documentation: Complete (MVP5.0-README.md)
✅ Status: FULLY OPERATIONAL

## Need Help?

1. Read: `MVP5.0-README.md`
2. Check: `MVP5.0-SUCCESS-REPORT.md`
3. Logs: `docker-compose -f docker-compose.mvp5.yml logs`

---

**MVP5.0 - Core Banking System is READY!** 🎉
