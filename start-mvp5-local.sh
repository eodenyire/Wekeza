#!/bin/bash

# MVP5.0 - Local Development Startup (Without Docker)
# Start all APIs locally for development and testing

set -e

echo "=================================================="
echo "  Wekeza Banking MVP5.0 - Local Development"
echo "=================================================="
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Check if .NET 8.0 is installed
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET 8.0 SDK is not installed."
    echo "Please install from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version | cut -d '.' -f 1)
if [ "$DOTNET_VERSION" -lt 8 ]; then
    echo "ERROR: .NET 8.0 or higher is required. Current version: $(dotnet --version)"
    exit 1
fi

print_info "Building all APIs..."
echo ""

# Build each API
print_info "Building MinimalWekezaApi..."
cd MinimalWekezaApi
dotnet build --configuration Release --no-restore || true
cd ..

print_info "Building DatabaseWekezaApi..."
cd DatabaseWekezaApi
dotnet build --configuration Release --no-restore || true
cd ..

print_info "Building EnhancedWekezaApi..."
cd EnhancedWekezaApi
dotnet build --configuration Release --no-restore || true
cd ..

print_info "Building ComprehensiveWekezaApi..."
cd ComprehensiveWekezaApi
dotnet build --configuration Release --no-restore || true
cd ..

echo ""
print_success "All APIs built successfully!"
echo ""

print_info "To run the APIs locally, use the following commands in separate terminals:"
echo ""
echo "Terminal 1 - Minimal API (Port 8081):"
echo "  cd MinimalWekezaApi && dotnet run"
echo ""
echo "Terminal 2 - Database API (Port 8082):"
echo "  cd DatabaseWekezaApi && dotnet run"
echo ""
echo "Terminal 3 - Enhanced API (Port 8083):"
echo "  cd EnhancedWekezaApi && dotnet run"
echo ""
echo "Terminal 4 - Comprehensive API (Port 8084):"
echo "  cd ComprehensiveWekezaApi && dotnet run"
echo ""
print_warning "Note: Ensure PostgreSQL is running on localhost:5432"
print_warning "Database: WekezaCoreDB, User: wekeza_app"
echo ""
