# Wekeza Nexus - Repository Extraction Notice

## Important Update

**Wekeza Nexus has been extracted into its own standalone repository!**

🔗 **New Repository**: https://github.com/eodenyire/WekezaNexus

## What This Means

Wekeza Nexus, the advanced fraud detection system, is now maintained in a separate repository to:

1. **Enable independent development** - Separate release cycles for fraud detection features
2. **Improve modularity** - Can be used by other projects beyond Wekeza Bank
3. **Better maintainability** - Dedicated CI/CD pipelines and issue tracking
4. **Facilitate open-source collaboration** - Can be open-sourced independently

## Directory Structure

The standalone repository can be found in the `WekezaNexus/` directory of this repository. It contains:

```
WekezaNexus/
├── src/                  # Source code (Domain, Application, Infrastructure)
├── tests/                # Unit tests
├── docs/                 # Documentation
├── .github/workflows/    # CI/CD pipelines
├── README.md            # Main documentation
├── LICENSE              # MIT License
├── CONTRIBUTING.md      # Contribution guidelines
├── EXTRACTION-GUIDE.md  # Step-by-step extraction guide
└── extract-repo.sh      # Automated extraction script
```

## How to Use Wekeza Nexus

### Option 1: NuGet Package (Coming Soon)

Once published, you can reference Nexus as a NuGet package:

```xml
<PackageReference Include="Wekeza.Nexus.Application" Version="1.0.0" />
<PackageReference Include="Wekeza.Nexus.Infrastructure" Version="1.0.0" />
```

### Option 2: Git Submodule (For Development)

```bash
cd Wekeza
git submodule add https://github.com/eodenyire/WekezaNexus.git WekezaNexus
```

### Option 3: Local Reference (Current Setup)

The WekezaNexus directory in this repository can be referenced directly:

```xml
<ProjectReference Include="../WekezaNexus/src/Wekeza.Nexus.Application/Wekeza.Nexus.Application.csproj" />
```

## Migration Path

### For New Projects

Simply reference the standalone WekezaNexus repository or NuGet packages.

### For Existing Integration

The existing integration examples in `Core/Wekeza.Core.Application/Features/Transactions/Commands/TransferFunds/` remain valid. Update project references as needed:

**Old (within Wekeza repo):**
```xml
<ProjectReference Include="..\Wekeza.Nexus.Application\Wekeza.Nexus.Application.csproj" />
```

**New (standalone repo):**
```xml
<PackageReference Include="Wekeza.Nexus.Application" Version="1.0.0" />
```

## Creating the Standalone Repository

To create the standalone repository on GitHub:

1. **Navigate to the WekezaNexus directory:**
   ```bash
   cd WekezaNexus
   ```

2. **Run the extraction script:**
   ```bash
   ./extract-repo.sh
   ```

3. **Follow the instructions** provided by the script

4. **Detailed guide:** See `WekezaNexus/EXTRACTION-GUIDE.md`

## Documentation

All Wekeza Nexus documentation has been moved to the `WekezaNexus/docs/` directory:

- **System Architecture**: `WekezaNexus/docs/WEKEZA-NEXUS-README.md`
- **Integration Guide**: `WekezaNexus/docs/WEKEZA-NEXUS-INTEGRATION-GUIDE.md`
- **Implementation Details**: `WekezaNexus/docs/WEKEZA-NEXUS-IMPLEMENTATION-COMPLETE.md`

## Support

### For Wekeza Bank Issues
- Repository: https://github.com/eodenyire/Wekeza
- Issues: https://github.com/eodenyire/Wekeza/issues

### For Wekeza Nexus Issues
- Repository: https://github.com/eodenyire/WekezaNexus
- Issues: https://github.com/eodenyire/WekezaNexus/issues

## Timeline

- **January 29, 2026**: Nexus extraction prepared
- **TBD**: Standalone repository created on GitHub
- **TBD**: NuGet packages published
- **TBD**: Migration of existing integrations

## Questions?

If you have questions about the extraction or migration:

1. Check the `WekezaNexus/EXTRACTION-GUIDE.md` for detailed instructions
2. Open an issue in the appropriate repository
3. Contact the development team

---

**Wekeza Nexus continues to provide world-class fraud detection, now with even better modularity and maintainability!** 🚀
