#!/bin/bash
# Wekeza Bank - Codespace Post-Create Setup Script
# Runs automatically after the container is created

set -e

echo "🏦 Setting up Wekeza Bank development environment..."

# ── Frontend (React / Node) ─────────────────────────────────────────────────
echo ""
echo "📦 Installing frontend dependencies..."
cd "${CODESPACE_VSCODE_FOLDER:-/workspaces/Wekeza}/Portals/wekeza-unified-shell"
npm install
echo "✅ Frontend dependencies installed."

# ── Python / Django ─────────────────────────────────────────────────────────
echo ""
echo "🐍 Installing Python dependencies..."
cd "${CODESPACE_VSCODE_FOLDER:-/workspaces/Wekeza}"
pip install --quiet --upgrade pip
if [ -f "requirements.txt" ]; then
  pip install --quiet -r requirements.txt
  echo "✅ Python dependencies installed."
fi

# ── .NET ────────────────────────────────────────────────────────────────────
echo ""
echo "🔷 Restoring .NET dependencies..."
cd "${CODESPACE_VSCODE_FOLDER:-/workspaces/Wekeza}/APIs/v1-Core"
if [ -f "Wekeza.Core.sln" ]; then
  dotnet restore Wekeza.Core.sln
  echo "✅ .NET dependencies restored."
fi

# ── Environment file ─────────────────────────────────────────────────────────
echo ""
cd "${CODESPACE_VSCODE_FOLDER:-/workspaces/Wekeza}"
if [ ! -f ".env" ] && [ -f ".env.example" ]; then
  cp .env.example .env
  echo "📄 Created .env from .env.example (update values as needed)."
fi

echo ""
echo "🎉 Wekeza Bank development environment is ready!"
echo ""
echo "   Frontend   : cd Portals/wekeza-unified-shell && npm run dev        → http://localhost:3000"
echo "   API        : cd APIs/v1-Core && dotnet run --project Wekeza.Core.Api → http://localhost:8080"
echo "   Full stack : docker-compose up"
