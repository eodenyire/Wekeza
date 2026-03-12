#!/bin/bash

# Wekeza Bank - Database Migration Script

set -e

echo "🏦 Wekeza Bank - Running Database Migrations"
echo "=============================================="

# Check if dotnet-ef is installed
if ! command -v dotnet-ef &> /dev/null; then
    echo "Installing dotnet-ef tool..."
    dotnet tool install --global dotnet-ef
fi

# Navigate to Infrastructure project
cd "$(dirname "$0")/../Core/Wekeza.Core.Infrastructure"

echo "📦 Restoring packages..."
dotnet restore

echo "🔨 Building project..."
dotnet build

echo "🗄️  Running migrations..."
dotnet ef database update --startup-project ../Wekeza.Core.Api

echo "✅ Migrations completed successfully!"
echo ""
echo "Database is ready for Wekeza Bank operations."
