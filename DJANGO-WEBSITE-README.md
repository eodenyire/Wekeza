# Wekeza Bank - Responsive Dynamic Website

A modern, responsive banking website built with Django backend and JavaScript-heavy frontend.

## 🌟 Features

### Frontend
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Modern UI/UX**: Clean, intuitive interface with smooth animations
- **Dynamic Content**: Real-time updates using JavaScript and AJAX
- **Interactive Dashboard**: Complete banking dashboard with account management
- **Secure Authentication**: JWT-based authentication system

### Backend (Django)
- **REST API**: Full-featured REST API built with Django REST Framework
- **Database Models**: Comprehensive banking models (Customers, Accounts, Transactions, Loans, Cards)
- **Authentication**: JWT authentication with djangorestframework-simplejwt
- **CORS Support**: Configured for cross-origin requests
- **Admin Panel**: Full Django admin interface for management

## 🚀 Getting Started

### Prerequisites
- Python 3.8 or higher
- pip (Python package installer)
- SQLite (included with Python) or PostgreSQL

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/eodenyire/Wekeza.git
cd Wekeza
```

2. **Install dependencies**
```bash
pip install -r requirements.txt
```

3. **Run migrations**
```bash
python manage.py makemigrations
python manage.py migrate
```

4. **Create a superuser (for admin access)**
```bash
python manage.py createsuperuser
```

5. **Collect static files**
```bash
python manage.py collectstatic --noinput
```

6. **Run the development server**
```bash
python manage.py runserver
```

7. **Access the application**
- Main Website: http://localhost:8000
- Admin Panel: http://localhost:8000/admin
- API Root: http://localhost:8000/api/

## 📁 Project Structure

```
Wekeza/
├── BankWebsite/           # Django project settings
│   ├── settings.py        # Project configuration
│   ├── urls.py            # Main URL routing
│   └── wsgi.py            # WSGI configuration
├── banking/               # Main Django app
│   ├── models.py          # Database models
│   ├── views.py           # API views and endpoints
│   ├── serializers.py     # DRF serializers
│   ├── urls.py            # App URL routing
│   ├── admin.py           # Admin configuration
│   └── templates/         # HTML templates
│       └── banking/
│           └── index.html # Main landing page
├── static/                # Static files
│   ├── css/
│   │   └── style.css      # Main stylesheet
│   ├── js/
│   │   └── main.js        # JavaScript functionality
│   └── images/
│       └── bank-logo.png  # Bank logo
├── Assets/                # Original assets
├── manage.py              # Django management script
└── requirements.txt       # Python dependencies
```

## 🔧 Technology Stack

### Backend
- **Django 6.0.1**: Python web framework
- **Django REST Framework 3.16.1**: RESTful API toolkit
- **djangorestframework-simplejwt 5.5.1**: JWT authentication
- **django-cors-headers 4.9.0**: CORS support
- **psycopg2-binary 2.9.11**: PostgreSQL adapter
- **Pillow 12.1.0**: Image processing

### Frontend
- **HTML5**: Semantic markup
- **CSS3**: Modern styling with animations
- **JavaScript (ES6+)**: Dynamic functionality
- **Font Awesome 6.4.0**: Icons

## 🎯 API Endpoints

### Authentication
- `POST /api/token/` - Get JWT token
- `POST /api/token/refresh/` - Refresh JWT token
- `POST /api/register/` - Register new user

### Customers
- `GET /api/customers/` - List customers
- `GET /api/customers/{id}/` - Get customer details

### Accounts
- `GET /api/accounts/` - List accounts
- `GET /api/accounts/{id}/` - Get account details
- `GET /api/accounts/{id}/balance/` - Get account balance
- `GET /api/accounts/{id}/statement/` - Get account statement

### Transactions
- `POST /api/transactions/deposit/` - Make a deposit
- `POST /api/transactions/withdraw/` - Make a withdrawal
- `POST /api/transactions/transfer/` - Transfer funds

### Loans
- `GET /api/loans/` - List loans
- `POST /api/loans/` - Apply for loan
- `POST /api/loans/{id}/approve/` - Approve loan (staff only)
- `POST /api/loans/{id}/disburse/` - Disburse loan (staff only)

### Cards
- `GET /api/cards/` - List cards
- `POST /api/cards/` - Issue new card

## 💻 Usage

### Registering a New User
1. Click "Open Account" on the homepage
2. Fill in the registration form
3. Submit and wait for confirmation
4. Login with your credentials

### Accessing the Dashboard
1. Click "Login" and enter credentials
2. View account balances and transactions
3. Perform banking operations (deposit, withdraw, transfer)
4. Apply for loans

### Admin Operations
1. Access `/admin` and login with superuser credentials
2. Manage customers, accounts, transactions, loans, and cards
3. Approve loan applications
4. Monitor system activity

## 🔒 Security Features

- JWT-based authentication
- Password hashing with Django's built-in system
- CSRF protection
- CORS configuration
- Input validation and sanitization
- Role-based access control

## 📱 Responsive Design

The website is fully responsive and works on:
- Desktop computers (1200px+)
- Tablets (768px - 1199px)
- Mobile phones (< 768px)

## 🎨 Customization

### Changing Colors
Edit the CSS variables in `static/css/style.css`:
```css
:root {
    --primary-color: #2563eb;
    --secondary-color: #10b981;
    /* ... other colors ... */
}
```

### Modifying the Logo
Replace `static/images/bank-logo.png` with your own logo.

### Adding Features
- Backend: Add new models in `banking/models.py` and create API endpoints in `banking/views.py`
- Frontend: Update `static/js/main.js` for new JavaScript functionality

## 🐛 Troubleshooting

### Static files not loading
```bash
python manage.py collectstatic --noinput
```

### Database issues
```bash
python manage.py migrate --run-syncdb
```

### Port already in use
```bash
python manage.py runserver 8001
```

## 📝 Environment Variables

For production, set these environment variables:
- `SECRET_KEY`: Django secret key
- `DEBUG`: Set to False
- `ALLOWED_HOSTS`: Your domain names
- `DATABASE_URL`: PostgreSQL connection string (if using PostgreSQL)

## 🚀 Deployment

### Using Gunicorn (Production)
```bash
pip install gunicorn
gunicorn BankWebsite.wsgi:application --bind 0.0.0.0:8000
```

### Using Docker
```bash
docker build -t wekeza-bank .
docker run -p 8000:8000 wekeza-bank
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

Proprietary - © 2026 Wekeza Bank. All rights reserved.

## 👤 Owner

**Emmanuel Odenyire**
- ID: 28839872
- Phone: 0716478835
- DOB: 17/March/1992

## 📧 Support

For technical support or questions:
- Email: support@wekeza.com
- Phone: 0716478835

---

Built with ❤️ using Django and JavaScript
