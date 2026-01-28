#!/bin/bash

# WekezaERMS Repository Push Script
# This script automates the process of pushing WekezaERMS to a separate repository

set -e  # Exit on error

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
SOURCE_REPO="https://github.com/eodenyire/Wekeza.git"
TARGET_REPO="https://github.com/eodenyire/WekezaERMS.git"
ERMS_FOLDER="WekezaERMS"
WORK_DIR=$(mktemp -d)

echo -e "${BLUE}===============================================${NC}"
echo -e "${BLUE}WekezaERMS Repository Migration Script${NC}"
echo -e "${BLUE}===============================================${NC}"
echo ""

# Function to print status messages
print_status() {
    echo -e "${GREEN}[✓]${NC} $1"
}

print_error() {
    echo -e "${RED}[✗]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[!]${NC} $1"
}

print_info() {
    echo -e "${BLUE}[i]${NC} $1"
}

# Check prerequisites
echo -e "${YELLOW}Checking prerequisites...${NC}"
echo ""

# Check if git is installed
if ! command -v git &> /dev/null; then
    print_error "Git is not installed. Please install git first."
    exit 1
fi
print_status "Git is installed"

# Check if gh CLI is available (optional but helpful)
if command -v gh &> /dev/null; then
    print_status "GitHub CLI (gh) is available"
    GH_AVAILABLE=true
else
    print_warning "GitHub CLI (gh) is not available. Repository creation will be manual."
    GH_AVAILABLE=false
fi

# Ask user which method to use
echo ""
echo -e "${YELLOW}Select migration method:${NC}"
echo "1) Git Subtree (preserves history) - RECOMMENDED"
echo "2) Manual Copy (quick and simple)"
echo "3) Git Filter-Repo (advanced, requires git-filter-repo)"
echo ""
read -p "Enter choice [1-3]: " method_choice

case $method_choice in
    1)
        METHOD="subtree"
        print_info "Using Git Subtree method"
        ;;
    2)
        METHOD="manual"
        print_info "Using Manual Copy method"
        ;;
    3)
        METHOD="filter-repo"
        print_info "Using Git Filter-Repo method"
        if ! command -v git-filter-repo &> /dev/null; then
            print_error "git-filter-repo is not installed."
            echo "Install it with: pip install git-filter-repo"
            exit 1
        fi
        ;;
    *)
        print_error "Invalid choice"
        exit 1
        ;;
esac

# Create working directory
echo ""
print_info "Creating working directory: $WORK_DIR"
mkdir -p "$WORK_DIR"
cd "$WORK_DIR"

# Method 1: Git Subtree
if [ "$METHOD" = "subtree" ]; then
    print_info "Cloning source repository..."
    git clone "$SOURCE_REPO" wekeza-source
    cd wekeza-source
    
    print_info "Extracting WekezaERMS with history using git subtree..."
    git subtree split --prefix="$ERMS_FOLDER" -b erms-only
    
    print_info "Creating standalone repository..."
    cd ..
    mkdir erms-standalone
    cd erms-standalone
    git init
    git fetch ../wekeza-source erms-only
    git merge FETCH_HEAD
    
    print_info "Adding remote for target repository..."
    git remote add origin "$TARGET_REPO"
    
    print_status "Repository prepared successfully!"
    echo ""
    print_warning "To push to GitHub, run:"
    echo "cd $WORK_DIR/erms-standalone"
    echo "git push -u origin main"
fi

# Method 2: Manual Copy
if [ "$METHOD" = "manual" ]; then
    print_info "Cloning source repository..."
    git clone "$SOURCE_REPO" wekeza-source
    
    print_info "Initializing target repository..."
    mkdir erms-standalone
    cd erms-standalone
    git init
    
    print_info "Copying WekezaERMS files..."
    cp -r ../wekeza-source/"$ERMS_FOLDER"/* .
    
    print_info "Adding files to git..."
    git add .
    git commit -m "Initial commit: WekezaERMS from Wekeza repository"
    
    print_info "Adding remote for target repository..."
    git remote add origin "$TARGET_REPO"
    
    print_status "Repository prepared successfully!"
    echo ""
    print_warning "To push to GitHub, run:"
    echo "cd $WORK_DIR/erms-standalone"
    echo "git push -u origin main"
fi

# Method 3: Git Filter-Repo
if [ "$METHOD" = "filter-repo" ]; then
    print_info "Cloning source repository..."
    git clone "$SOURCE_REPO" erms-standalone
    cd erms-standalone
    
    print_info "Filtering repository to keep only WekezaERMS..."
    git filter-repo --path "$ERMS_FOLDER/" --path-rename "$ERMS_FOLDER/:"
    
    print_info "Adding remote for target repository..."
    git remote add origin "$TARGET_REPO"
    
    print_status "Repository prepared successfully!"
    echo ""
    print_warning "To push to GitHub, run:"
    echo "cd $WORK_DIR/erms-standalone"
    echo "git push -u origin main"
fi

echo ""
echo -e "${BLUE}===============================================${NC}"
echo -e "${GREEN}Migration preparation complete!${NC}"
echo -e "${BLUE}===============================================${NC}"
echo ""
echo "Working directory: $WORK_DIR/erms-standalone"
echo ""
echo "Next steps:"
echo "1. Review the extracted files in $WORK_DIR/erms-standalone"
echo "2. Ensure the target repository exists at $TARGET_REPO"
echo "3. Run: cd $WORK_DIR/erms-standalone && git push -u origin main"
echo ""
echo "For more details, see: WekezaERMS/REPOSITORY-SETUP-GUIDE.md"
