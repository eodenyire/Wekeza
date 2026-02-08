#!/bin/bash

echo "🚀 Starting WekezaERMS API..."
echo ""
echo "This will start the Enterprise Risk Management System API"
echo "Swagger UI will be available at: http://localhost:5000"
echo ""

cd "$(dirname "$0")/API"

dotnet run
