#!/bin/bash

# MVP5.0 - Wekeza Core Banking System Startup Script
# This script starts all banking services for full end-to-end operational testing

set -e

echo "=================================================="
echo "  Wekeza Core Banking System MVP5.0"
echo "  Full End-to-End Banking Platform"
echo "=================================================="
echo ""

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Function to print colored messages
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed. Please install Docker first."
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
    print_error "Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

print_info "Starting Wekeza Banking System MVP5.0..."
echo ""

# Stop any existing containers
print_info "Stopping any existing MVP5.0 containers..."
docker-compose -f docker-compose.mvp5.yml down 2>/dev/null || true

# Build all services
print_info "Building all banking services..."
docker-compose -f docker-compose.mvp5.yml build --no-cache

# Start all services
print_info "Starting all services..."
docker-compose -f docker-compose.mvp5.yml up -d

# Wait for services to be healthy
print_info "Waiting for services to become healthy..."
sleep 10

# Check service status
print_info "Checking service status..."
echo ""

docker-compose -f docker-compose.mvp5.yml ps

echo ""
print_success "MVP5.0 Banking System is now running!"
echo ""

echo "=================================================="
echo "  Available Services:"
echo "=================================================="
echo ""
echo -e "${GREEN}Database:${NC}"
echo "  PostgreSQL      : localhost:5432"
echo "  Database Name   : WekezaCoreDB"
echo "  Username        : wekeza_app"
echo ""
echo -e "${GREEN}Banking APIs:${NC}"
echo "  Minimal API     : http://localhost:8081"
echo "  Database API    : http://localhost:8082/swagger"
echo "  Enhanced API    : http://localhost:8083/swagger"
echo "  Comprehensive   : http://localhost:8084/swagger"
echo ""
echo -e "${GREEN}Management Tools:${NC}"
echo "  pgAdmin         : http://localhost:5050 (optional, use --profile tools)"
echo ""
echo "=================================================="
echo "  Quick Start Commands:"
echo "=================================================="
echo ""
echo "View logs:"
echo "  docker-compose -f docker-compose.mvp5.yml logs -f"
echo ""
echo "Stop all services:"
echo "  docker-compose -f docker-compose.mvp5.yml down"
echo ""
echo "Restart a service:"
echo "  docker-compose -f docker-compose.mvp5.yml restart [service-name]"
echo ""
echo "Execute command in container:"
echo "  docker-compose -f docker-compose.mvp5.yml exec [service-name] bash"
echo ""
echo "=================================================="
echo ""
print_success "System is operational! Open your browser and navigate to the APIs above."
echo ""

# Optional: Open browser automatically (uncomment if desired)
# sleep 5
# xdg-open http://localhost:8084/swagger 2>/dev/null || open http://localhost:8084/swagger 2>/dev/null || start http://localhost:8084/swagger 2>/dev/null || true
