# Extracting Wekeza Nexus to Standalone Repository

This guide explains how to extract the Wekeza Nexus fraud detection system into its own standalone repository.

## Prerequisites

- Git installed on your system
- Access to create repositories on GitHub
- GitHub account: `eodenyire`

## Steps to Create the Standalone Repository

### 1. Create the Repository on GitHub

1. Go to https://github.com/new
2. Repository name: `WekezaNexus`
3. Description: "Advanced Real-Time Fraud Detection & Prevention System"
4. Public or Private: Choose as needed
5. **Do NOT** initialize with README, .gitignore, or license (we already have these)
6. Click "Create repository"

### 2. Extract and Push Wekeza Nexus

```bash
# Navigate to the WekezaNexus directory
cd /path/to/Wekeza/WekezaNexus

# Initialize as a new git repository
git init

# Add all files
git add .

# Create initial commit
git commit -m "Initial commit: Wekeza Nexus v1.0.0"

# Add the remote repository
git remote add origin https://github.com/eodenyire/WekezaNexus.git

# Push to GitHub
git branch -M main
git push -u origin main
```

### 3. Configure Repository Settings

After pushing, configure the following on GitHub:

**Repository Settings:**
- Set repository description
- Add topics: `fraud-detection`, `fintech`, `dotnet`, `csharp`, `security`, `machine-learning`
- Enable Issues
- Enable Discussions (optional)

**Branch Protection:**
- Protect `main` branch
- Require pull request reviews
- Require status checks to pass

**Secrets (for CI/CD):**
- Add `NUGET_API_KEY` for package publishing

### 4. Verify the Setup

1. Clone the new repository in a different location:
   ```bash
   git clone https://github.com/eodenyire/WekezaNexus.git
   cd WekezaNexus
   ```

2. Build and test:
   ```bash
   dotnet restore
   dotnet build
   dotnet test
   ```

3. Verify all tests pass and documentation is accessible

## Directory Structure

The standalone repository has the following structure:

```
WekezaNexus/
├── .github/
│   └── workflows/
│       ├── ci.yml           # CI pipeline
│       └── release.yml      # Release pipeline
├── src/
│   ├── Wekeza.Nexus.Domain/
│   ├── Wekeza.Nexus.Application/
│   └── Wekeza.Nexus.Infrastructure/
├── tests/
│   └── Wekeza.Nexus.UnitTests/
├── docs/
│   ├── WEKEZA-NEXUS-README.md
│   ├── WEKEZA-NEXUS-INTEGRATION-GUIDE.md
│   └── WEKEZA-NEXUS-IMPLEMENTATION-COMPLETE.md
├── .gitignore
├── CONTRIBUTING.md
├── LICENSE
├── README.md
└── WekezaNexus.sln
```

## Integration with Main Wekeza Repository

After extraction, you can reference Wekeza Nexus in the main Wekeza repository as:

### Option 1: NuGet Package (Recommended)

```xml
<PackageReference Include="Wekeza.Nexus.Application" Version="1.0.0" />
<PackageReference Include="Wekeza.Nexus.Infrastructure" Version="1.0.0" />
```

### Option 2: Git Submodule

```bash
cd /path/to/Wekeza
git submodule add https://github.com/eodenyire/WekezaNexus.git WekezaNexus
```

### Option 3: Direct Source Reference (Development)

```xml
<ProjectReference Include="../../WekezaNexus/src/Wekeza.Nexus.Application/Wekeza.Nexus.Application.csproj" />
```

## Maintaining Both Repositories

### Development Workflow

1. **For Nexus-specific features:**
   - Work in the WekezaNexus repository
   - Create feature branches
   - Submit PRs to WekezaNexus

2. **For Wekeza Bank integration:**
   - Work in the Wekeza repository
   - Reference Nexus as a dependency
   - Update Nexus version as needed

### Version Management

- Use semantic versioning (SemVer) for Nexus releases
- Tag releases in WekezaNexus repository
- Update dependency version in Wekeza repository when needed

## Benefits of Separate Repository

1. **Modularity**: Nexus can be used by other projects
2. **Independent Development**: Separate release cycles
3. **Clear Ownership**: Dedicated team for fraud detection
4. **Open Source Potential**: Can be open-sourced independently
5. **Better CI/CD**: Focused pipelines for each project

## Troubleshooting

### Build Issues

If you encounter build issues after extraction:

1. Ensure all project references are correct
2. Run `dotnet restore` to download dependencies
3. Check .NET SDK version (8.0 required)

### Test Failures

If tests fail after extraction:

1. Verify all test files were copied
2. Check test project references
3. Ensure test dependencies are installed

## Support

For questions or issues with the extraction process:
- Create an issue in the WekezaNexus repository
- Contact the development team

---

**Last Updated**: January 29, 2026
