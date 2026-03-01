# Wekeza Repository Organization Guide

This document explains how the Wekeza repository is organized to maintain clarity and reduce clutter.

## 📂 Repository Structure

### Root Level Files
The root level contains only essential files:
- `README.md` - Main project README
- `docker-compose.yml` - Docker configuration files
- `Dockerfile` - Docker build configuration
- `manage.py` - Django management script
- `requirements.txt` - Python dependencies
- `.gitignore`, `.env.example` - Configuration files
- `wekeza-overview.html` - Project overview

### Main Directories

#### 🔧 `/APIs`
All API implementations organized by version.
- **Documentation:** `/APIs/docs/` - API-specific documentation

#### 💼 `/Core`
Core banking system implementation.
- **Documentation:** `/Core/docs/` - Core system documentation

#### 📱 `/Wekeza Mobile`
Flutter-based mobile application.
- **Documentation:** `/Wekeza Mobile/docs/` - Mobile app documentation

#### 🔗 `/WekezaNexus`
Nexus integration system (C#).
- **Documentation:** `/WekezaNexus/docs/` - Nexus documentation

#### 🛡️ `/WekezaERMS`
Enterprise Risk Management System (C#).
- **Documentation:** `/WekezaERMS/Docs/` - ERMS documentation

#### 🌐 `/BankWebsite`
Django-based bank website.
- **Documentation:** `/BankWebsite/docs/` - Website documentation

#### 🎨 `/Portals`
Admin panels and portal interfaces.
- **Documentation:** `/Portals/docs/` - Portal documentation

#### 📚 `/docs`
Project-level documentation organized into:
- `/docs/weekly-reports/` - Weekly progress reports
- `/docs/deployment/` - Deployment and setup guides
- `/docs/development/` - Development and build documentation
- Root level: Project status, roadmaps, completion audits

See [docs/README.md](docs/README.md) for detailed documentation navigation.

#### 🔨 `/scripts`
Operational scripts organized by purpose:
- `/scripts/api/` - API management scripts
- `/scripts/core/` - Core system scripts
- `/scripts/database/` - Database scripts
- `/scripts/testing/` - Testing scripts
- `/scripts/admin/` - Admin panel scripts
- `/scripts/deployment/` - Deployment scripts

See [scripts/README.md](scripts/README.md) for script usage guide.

#### 🐳 `/kubernetes`
Kubernetes configuration files for container orchestration.

#### 📊 `/monitoring`
System monitoring and observability configurations.

#### 📦 `/banking`
Banking business logic and modules.

#### 🎨 `/Assets`
Static assets, images, and design files.

#### 🔧 `/static`
Static files for web interfaces.

## 🎯 Organization Principles

### 1. Component Isolation
Each major component (Mobile, Nexus, ERMS, APIs, Core, Website, Portals) has its own directory with:
- Source code
- Documentation in `/docs` subdirectory
- Component-specific configuration

### 2. Clear Documentation Structure
- **Component Docs:** Specific to each component in their `/docs` folder
- **Project Docs:** Centralized in `/docs` with categorized subdirectories
- **README Files:** Present in docs/ and scripts/ to guide navigation

### 3. Script Organization
Scripts are categorized by function rather than scattered in the root:
- Easy to find related scripts
- Clear naming conventions
- Reduced root clutter

### 4. Separation of Concerns
- **Source Code:** In component directories
- **Documentation:** In `/docs` hierarchies
- **Scripts:** In `/scripts` with subcategories
- **Configuration:** Docker, Kubernetes, and .env files in root
- **Dependencies:** requirements.txt, package.json in relevant directories

## 📖 Finding What You Need

### For New Developers
1. Start with root `README.md`
2. Read [docs/START-HERE.md](docs/START-HERE.md)
3. Check [docs/GETTING-STARTED-NOW.md](docs/GETTING-STARTED-NOW.md)

### For Deployment
1. Review [docs/deployment/DEPLOYMENT-GUIDE.md](docs/deployment/DEPLOYMENT-GUIDE.md)
2. Use scripts in `/scripts/deployment/`
3. Check Docker and Kubernetes configurations

### For Development
1. Navigate to the specific component directory
2. Check the component's `/docs` folder
3. Review [docs/development/](docs/development/) for build issues

### For API Work
1. Go to `/APIs` directory
2. Read [APIs/docs/](APIs/docs/) documentation
3. Use `/scripts/api/` for API management

### For Testing
1. Use scripts in `/scripts/testing/`
2. Review test reports in `/docs/`
3. Check component-specific test documentation

## 🔄 Maintenance Guidelines

### When Adding Files

#### Documentation
- **Component-specific:** Place in `<Component>/docs/`
- **API-related:** Place in `APIs/docs/`
- **Deployment/Setup:** Place in `docs/deployment/`
- **Development/Build:** Place in `docs/development/`
- **Weekly reports:** Place in `docs/weekly-reports/`
- **Project-level:** Place in `docs/`

#### Scripts
- **API scripts:** Place in `scripts/api/`
- **Core system scripts:** Place in `scripts/core/`
- **Database scripts:** Place in `scripts/database/`
- **Testing scripts:** Place in `scripts/testing/`
- **Admin scripts:** Place in `scripts/admin/`
- **Deployment scripts:** Place in `scripts/deployment/`
- **General scripts:** Place in `scripts/`

#### Source Code
- Place in the appropriate component directory
- Keep related code together
- Maintain clear module boundaries

### Naming Conventions

#### Documentation Files
- Use descriptive, UPPERCASE names with hyphens
- Examples: `API-INTEGRATION-GUIDE.md`, `DEPLOYMENT-CHECKLIST.md`
- Include version when relevant: `MVP5.0-README.md`

#### Scripts
- Use lowercase with hyphens
- Prefix with action: `start-`, `test-`, `setup-`, `run-`
- Examples: `start-wekeza.ps1`, `test-database-connection.ps1`

#### Directories
- Use clear, descriptive names
- Capitalize appropriately: `Wekeza Mobile`, `APIs`, `Core`

## 🚀 Quick Access

| What You Need | Where to Find It |
|---------------|------------------|
| Project Overview | `README.md`, `wekeza-overview.html` |
| Getting Started | `docs/START-HERE.md` |
| API Documentation | `APIs/docs/` |
| Core System Docs | `Core/docs/` |
| Mobile App Docs | `Wekeza Mobile/docs/` |
| Deployment Guides | `docs/deployment/` |
| Testing Scripts | `scripts/testing/` |
| Database Setup | `scripts/database/` |
| Weekly Progress | `docs/weekly-reports/` |
| Build Issues | `docs/development/` |

## 📝 Contributing

Please follow this organization structure when contributing:
1. Place files in appropriate directories
2. Update relevant README files
3. Maintain consistent naming conventions
4. Add documentation for new features
5. Keep the root directory clean

## ❓ Questions?

If you're unsure where to place something:
1. Check this guide
2. Review similar existing files
3. Look at the component's structure
4. When in doubt, ask the team

---

**Last Updated:** March 2026
**Maintained By:** Wekeza Development Team
