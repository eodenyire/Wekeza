# 🚀 Quick Start Guide - Wekeza Bank Website

This guide will help you get the Wekeza Bank website up and running in minutes.

## Prerequisites

- Python 3.8 or higher installed
- pip (Python package installer)
- Basic terminal/command line knowledge

## Step-by-Step Setup

### 1. Navigate to the Project Directory

```bash
cd /path/to/Wekeza
```

### 2. Install Dependencies

```bash
pip install -r requirements.txt
```

This will install:
- Django 6.0.1
- Django REST Framework
- JWT Authentication
- CORS Headers
- PostgreSQL support (optional)
- Image processing (Pillow)

### 3. Set Up the Database

Run the database migrations to create all necessary tables:

```bash
python manage.py migrate
```

### 4. Create an Admin User (Optional but Recommended)

```bash
python manage.py createsuperuser
```

Follow the prompts to create your admin account.

### 5. Start the Development Server

```bash
python manage.py runserver
```

You should see output like:
```
Starting development server at http://127.0.0.1:8000/
Quit the server with CONTROL-C.
```

### 6. Access the Website

Open your web browser and visit:
- **Main Website**: http://localhost:8000
- **Admin Panel**: http://localhost:8000/admin
- **API Root**: http://localhost:8000/api/

## First Time Using the Website

### As a Customer:

1. **Register**: Click "Open Account" button
2. **Fill in the form** with your details:
   - First Name, Last Name
   - Username, Email
   - Phone Number, ID Number
   - Date of Birth
   - Address
   - Password (confirm twice)
3. **Login**: After registration, login with your credentials
4. **Explore**: View your dashboard, accounts, and transactions

### As an Administrator:

1. Visit http://localhost:8000/admin
2. Login with the superuser credentials you created
3. Manage:
   - Customers
   - Accounts
   - Transactions
   - Loans
   - Cards
   - Loan Payments

## Testing the API

### Get JWT Token

```bash
curl -X POST http://localhost:8000/api/token/ \
  -H "Content-Type: application/json" \
  -d '{"username":"your_username","password":"your_password"}'
```

### Use the Token

```bash
curl http://localhost:8000/api/accounts/ \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

## Common Tasks

### Create Test Data

Access the admin panel and manually create:
1. A customer (linked to a user)
2. Accounts for that customer
3. Test transactions

### Reset Database

```bash
rm db.sqlite3
python manage.py migrate
python manage.py createsuperuser
```

### Collect Static Files (for Production)

```bash
python manage.py collectstatic
```

## Features to Try

1. **Registration**: Create a new account through the website
2. **Login**: Authenticate and see the JWT token in localStorage
3. **Dashboard**: View accounts and balances (after creating test accounts in admin)
4. **Transactions**: Use the API to make deposits, withdrawals, transfers
5. **Loans**: Apply for loans through the API
6. **Admin Panel**: Manage all banking operations

## Troubleshooting

### Port Already in Use

```bash
python manage.py runserver 8001
```

### Static Files Not Loading

```bash
python manage.py collectstatic --noinput
```

### Database Errors

```bash
python manage.py migrate --run-syncdb
```

### CORS Issues (if accessing from different domain)

Check `CORS_ALLOW_ALL_ORIGINS` in `BankWebsite/settings.py`

## Development Tips

1. **Enable Debug Mode**: Already enabled in settings.py
2. **Check Logs**: Watch the terminal for request logs
3. **Use Admin Panel**: Easiest way to create test data
4. **API Browser**: Visit http://localhost:8000/api/ in browser for interactive API docs

## Production Deployment

For production deployment, see `DJANGO-WEBSITE-README.md` for detailed instructions including:
- Environment variable configuration
- Using Gunicorn
- Database setup (PostgreSQL)
- Static file serving
- Security settings

## Getting Help

- Check `DJANGO-WEBSITE-README.md` for comprehensive documentation
- Review Django documentation: https://docs.djangoproject.com/
- Check REST Framework docs: https://www.django-rest-framework.org/

## Contact

**Owner**: Emmanuel Odenyire
- **ID**: 28839872
- **Phone**: 0716478835
- **DOB**: 17/March/1992

---

Happy Banking! 🏦
