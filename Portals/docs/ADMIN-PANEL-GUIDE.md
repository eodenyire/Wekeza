# Wekeza Bank Administrator Panel

## Overview

The Wekeza Bank Administrator Panel is a comprehensive web-based interface that provides role-based access to banking operations. Different banking staff can log in and access features based on their assigned roles and permissions.

## Features

### 🏛️ Role-Based Access Control
- **Administrators**: Full system access, user management, system configuration
- **Branch Managers**: Branch operations, staff management, transaction approvals
- **Supervisors**: Operations oversight, transaction approvals
- **Tellers**: Customer transactions, cash handling
- **Customer Service**: Customer onboarding, account inquiries
- **Back Office Staff**: Transaction processing, reconciliation
- **Loan Officers**: Loan processing, credit assessment
- **Cash Officers**: Treasury operations, CSDC management
- **Compliance Officers**: AML monitoring, risk assessment
- **Risk Officers**: Risk management, limit controls

### 🎯 Key Capabilities

#### User Management
- Create banking staff with appropriate roles
- Manage user permissions and access levels
- Reset passwords and manage user status
- View user activity and audit logs

#### Staff Operations
- **Tellers**: Process deposits, withdrawals, transfers, balance inquiries
- **Customer Care**: Customer onboarding, support tickets, account maintenance
- **Back Office**: Transaction processing, batch operations, reconciliation
- **Treasury**: Cash management, CSDC operations, FX transactions
- **Loans**: Application processing, credit assessment, disbursements
- **Compliance**: AML monitoring, sanctions screening, regulatory reporting

#### System Administration
- System configuration and parameters
- Branch and department management
- Product configuration and pricing
- Audit logs and system monitoring

## Getting Started

### Prerequisites
- .NET 8.0 or later
- Modern web browser (Chrome, Firefox, Safari, Edge)

### Quick Start

1. **Start the Admin Panel**
   ```powershell
   ./start-admin-panel.ps1
   ```

2. **Open your browser and navigate to:**
   ```
   http://localhost:5000/admin
   ```

3. **Login with test credentials:**
   - **Administrator**: `admin` / `password`
   - **Teller**: `teller` / `password`
   - **Loan Officer**: `loanofficer` / `password`
   - **Risk Officer**: `riskofficer` / `password`

### Testing the System

Run the test script to verify all endpoints:
```powershell
./test-admin-panel.ps1
```

## User Interface

### Dashboard
The main dashboard shows role-specific tiles for quick access to operations:
- **User Management** (Admin only)
- **Teller Operations** (Tellers, Supervisors, Managers)
- **Customer Care** (Customer Service, Supervisors, Managers)
- **Back Office** (Back Office Staff, Supervisors, Managers)
- **Treasury** (Cash Officers, Managers)
- **Loan Management** (Loan Officers, Supervisors, Managers)
- **Compliance** (Compliance/Risk Officers)

### Navigation
- **Sidebar Navigation**: Role-based menu with collapsible sections
- **User Profile**: Current user info with roles and permissions
- **Breadcrumb Navigation**: Easy navigation between sections

### Staff Management
- **Create Staff**: Comprehensive form for adding new banking staff
- **Staff Directory**: Searchable list of all staff members
- **Role Assignment**: Assign and manage user roles
- **Permission Management**: Fine-grained access control

## API Endpoints

### Authentication
- `POST /api/authentication/login` - User login
- `GET /api/authentication/me` - Current user info

### Admin Panel
- `GET /admin` - Dashboard data
- `GET /admin/users` - User management interface
- `GET /admin/staff/create` - Staff creation interface
- `GET /admin/teller` - Teller operations interface
- `GET /admin/customer-care` - Customer care interface
- `GET /admin/back-office` - Back office interface
- `GET /admin/treasury` - Treasury operations interface
- `GET /admin/loans` - Loan management interface
- `GET /admin/compliance` - Compliance interface

### Staff Management API
- `GET /api/staffmanagement` - Get all staff
- `POST /api/staffmanagement/create` - Create new staff
- `PUT /api/staffmanagement/{id}` - Update staff
- `POST /api/staffmanagement/{id}/deactivate` - Deactivate staff
- `POST /api/staffmanagement/{id}/reset-password` - Reset password
- `GET /api/staffmanagement/statistics` - Staff statistics

## Role Permissions

### Administrator Roles
- **Administrator**: Full system access
- **IT Administrator**: Technical system management

### Management Roles
- **Branch Manager**: Branch operations, staff management
- **Supervisor**: Operations oversight, approvals

### Operational Roles
- **Teller**: Customer transactions
- **Customer Service**: Customer support
- **Back Office Staff**: Transaction processing
- **Loan Officer**: Loan operations
- **Cash Officer**: Treasury operations

### Specialized Roles
- **Compliance Officer**: AML and compliance
- **Risk Officer**: Risk management
- **Insurance Officer**: Insurance products

## Security Features

### Authentication & Authorization
- JWT-based authentication
- Role-based access control (RBAC)
- Session management and timeout
- Secure password handling

### Audit & Compliance
- Complete audit trail of all actions
- User activity logging
- Access attempt monitoring
- Compliance reporting

### Data Protection
- Encrypted data transmission (HTTPS)
- Secure API endpoints
- Input validation and sanitization
- XSS and CSRF protection

## Customization

### Adding New Roles
1. Update `UserRole` enum in `Core/Wekeza.Core.Domain/Enums/UserRole.cs`
2. Add role permissions in `AdminPanelController.cs`
3. Update navigation menu logic
4. Add role-specific operations

### Adding New Operations
1. Create new controller endpoints
2. Add navigation menu items
3. Update role permissions
4. Create UI components

### Styling and Branding
- Modify CSS variables in `/admin/index.html`
- Update logo and branding elements
- Customize color scheme and fonts

## Troubleshooting

### Common Issues

1. **Login Failed**
   - Check if server is running
   - Verify credentials (case-sensitive)
   - Check browser console for errors

2. **Access Denied**
   - Verify user has required role
   - Check token expiration
   - Refresh browser and re-login

3. **Server Not Starting**
   - Check .NET installation
   - Verify port 5000 is available
   - Check project build errors

### Debug Mode
Enable detailed logging by setting environment variable:
```
ASPNETCORE_ENVIRONMENT=Development
```

## Support

For technical support or questions:
- Check the API documentation at `/swagger`
- Review audit logs for troubleshooting
- Contact system administrator

## Future Enhancements

### Planned Features
- Mobile-responsive design
- Real-time notifications
- Advanced reporting dashboard
- Multi-language support
- Dark mode theme
- Advanced user analytics
- Integration with external systems
- Workflow automation tools

### Performance Optimizations
- Caching implementation
- Database query optimization
- Lazy loading for large datasets
- Progressive web app (PWA) features