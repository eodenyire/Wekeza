#!/bin/bash

# Wekeza Nexus Repository Extraction Script
# This script helps extract Wekeza Nexus into a standalone repository

set -e

echo "=========================================="
echo "Wekeza Nexus Repository Extraction Script"
echo "=========================================="
echo ""

# Check if we're in the WekezaNexus directory
if [ ! -f "WekezaNexus.sln" ]; then
    echo "Error: This script must be run from the WekezaNexus directory"
    echo "Current directory: $(pwd)"
    exit 1
fi

# Check if git is installed
if ! command -v git &> /dev/null; then
    echo "Error: git is not installed"
    exit 1
fi

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed"
    exit 1
fi

echo "Step 1: Verifying project structure..."
echo "----------------------------------------"

# Verify all required files exist
required_files=(
    "README.md"
    "LICENSE"
    "CONTRIBUTING.md"
    ".gitignore"
    "WekezaNexus.sln"
    "EXTRACTION-GUIDE.md"
)

for file in "${required_files[@]}"; do
    if [ ! -f "$file" ]; then
        echo "Error: Missing required file: $file"
        exit 1
    fi
    echo "  ✓ Found: $file"
done

required_dirs=(
    "src/Wekeza.Nexus.Domain"
    "src/Wekeza.Nexus.Application"
    "src/Wekeza.Nexus.Infrastructure"
    "tests/Wekeza.Nexus.UnitTests"
    "docs"
    ".github/workflows"
)

for dir in "${required_dirs[@]}"; do
    if [ ! -d "$dir" ]; then
        echo "Error: Missing required directory: $dir"
        exit 1
    fi
    echo "  ✓ Found: $dir"
done

echo ""
echo "Step 2: Building and testing the project..."
echo "----------------------------------------"

# Restore dependencies
echo "  → Restoring NuGet packages..."
dotnet restore WekezaNexus.sln

# Build the solution
echo "  → Building solution..."
dotnet build WekezaNexus.sln --configuration Release

# Run tests
echo "  → Running tests..."
dotnet test WekezaNexus.sln --configuration Release --no-build

echo ""
echo "Step 3: Preparing for Git initialization..."
echo "----------------------------------------"

# Check if already a git repository
if [ -d ".git" ]; then
    echo "  ⚠ Warning: This directory is already a git repository"
    echo "  Do you want to remove the existing .git directory and start fresh? (y/N)"
    read -r response
    if [[ "$response" =~ ^[Yy]$ ]]; then
        rm -rf .git
        echo "  ✓ Removed existing .git directory"
    else
        echo "  ℹ Keeping existing .git directory"
    fi
fi

echo ""
echo "Step 4: Next steps..."
echo "----------------------------------------"
echo ""
echo "The project is ready for extraction! Follow these steps:"
echo ""
echo "1. Create a new repository on GitHub:"
echo "   - Go to: https://github.com/new"
echo "   - Name: WekezaNexus"
echo "   - Description: Advanced Real-Time Fraud Detection & Prevention System"
echo "   - Do NOT initialize with README, .gitignore, or license"
echo ""
echo "2. Initialize and push this repository:"
echo "   cd $(pwd)"
echo "   git init"
echo "   git add ."
echo "   git commit -m \"Initial commit: Wekeza Nexus v1.0.0\""
echo "   git branch -M main"
echo "   git remote add origin https://github.com/eodenyire/WekezaNexus.git"
echo "   git push -u origin main"
echo ""
echo "3. Configure repository settings on GitHub:"
echo "   - Add topics: fraud-detection, fintech, dotnet, csharp, security"
echo "   - Enable Issues and Discussions"
echo "   - Set up branch protection for 'main'"
echo ""
echo "=========================================="
echo "Extraction preparation complete!"
echo "=========================================="
echo ""
echo "For detailed instructions, see: EXTRACTION-GUIDE.md"
echo ""
