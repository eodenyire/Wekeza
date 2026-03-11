#!/bin/bash
# Wekeza Bank - Codespace Post-Create Setup Script
# Runs automatically after the container is created

set -e

WORKSPACE="${CODESPACE_VSCODE_FOLDER:-/workspaces/Wekeza}"

echo "🏦 Setting up Wekeza Bank development environment..."

# ── Frontend (React / Node) ─────────────────────────────────────────────────
echo ""
echo "📦 Installing frontend dependencies..."
cd "${WORKSPACE}/Portals/wekeza-unified-shell"
npm install
echo "✅ Frontend dependencies installed."

# ── Python virtual environment ───────────────────────────────────────────────
echo ""
echo "🐍 Creating Python virtual environment at ${WORKSPACE}/.venv ..."
cd "${WORKSPACE}"
python3 -m venv .venv
echo "✅ Virtual environment created."

echo ""
echo "🐍 Installing Python dependencies into .venv ..."
.venv/bin/pip install --quiet --upgrade pip
if [ -f "requirements.txt" ]; then
  .venv/bin/pip install --quiet -r requirements.txt
  echo "✅ Python (root requirements.txt) dependencies installed."
fi

# ── .NET ────────────────────────────────────────────────────────────────────
echo ""
echo "🔷 Restoring .NET dependencies..."
cd "${WORKSPACE}/APIs/v1-Core"
if [ -f "Wekeza.Core.sln" ]; then
  dotnet restore Wekeza.Core.sln
  echo "✅ .NET dependencies restored."
fi

# ── Make test/regression scripts executable ──────────────────────────────────
echo ""
echo "🔧 Making regression scripts executable..."
chmod +x "${WORKSPACE}/APIs/v1-Core/scripts/"*.sh 2>/dev/null || true
chmod +x "${WORKSPACE}/scripts/"*.sh 2>/dev/null || true
echo "✅ Scripts are executable."

# ── Environment file ─────────────────────────────────────────────────────────
echo ""
cd "${WORKSPACE}"
if [ ! -f ".env" ] && [ -f ".env.example" ]; then
  cp .env.example .env
  echo "📄 Created .env from .env.example (update values as needed)."
fi

echo ""
echo "🎉 Wekeza Bank development environment is ready!"
echo ""
echo "   Frontend      : cd Portals/wekeza-unified-shell && npm run dev          → http://localhost:3000"
echo "   API           : cd APIs/v1-Core && dotnet run --project Wekeza.Core.Api  → http://localhost:8080"
echo "   Full stack    : cd APIs/v1-Core && docker compose up -d"
echo ""
echo "   Smoke tests   : cd APIs/v1-Core && ./scripts/05_run_all_tests.sh"
echo "   Regression    : cd APIs/v1-Core && ./scripts/13_run_backend_frontend_regression.sh"
echo "   VIF journey   : cd APIs/v1-Core && \${WORKSPACE}/.venv/bin/python scripts/15_vif_customer_journey.py"
