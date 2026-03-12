@echo off
echo 🏦 Starting Wekeza Core Banking System...
echo.

REM Set the dotnet path
set DOTNET_PATH="C:\Program Files\dotnet\dotnet.exe"

echo 📋 Checking .NET installation...
%DOTNET_PATH% --version
if %ERRORLEVEL% neq 0 (
    echo ❌ .NET not found at expected location
    echo Please ensure .NET 8.0 is installed
    pause
    exit /b 1
)

echo ✅ .NET 8.0 found and working
echo.

echo 📦 Restoring NuGet packages...
%DOTNET_PATH% restore Wekeza.Core.sln
if %ERRORLEVEL% neq 0 (
    echo ❌ Failed to restore packages
    pause
    exit /b 1
)

echo ✅ Packages restored successfully
echo.

echo 🔨 Building solution...
%DOTNET_PATH% build Wekeza.Core.sln --configuration Debug --no-restore
if %ERRORLEVEL% neq 0 (
    echo ❌ Build failed
    pause
    exit /b 1
)

echo ✅ Build completed successfully
echo.

echo 🗄️ Setting up database (if needed)...
echo Note: This requires PostgreSQL to be running
echo Database: WekezaCoreDB, User: admin, Password: the_beast_pass
echo.

REM Try to run migrations (will fail gracefully if DB not available)
echo 🔄 Running database migrations...
%DOTNET_PATH% ef database update --project Core/Wekeza.Core.Infrastructure --startup-project Core/Wekeza.Core.Api
if %ERRORLEVEL% neq 0 (
    echo ⚠️ Database migrations failed - PostgreSQL may not be running
    echo You can start PostgreSQL with Docker:
    echo docker run --name wekeza-postgres -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=the_beast_pass -e POSTGRES_DB=WekezaCoreDB -p 5432:5432 -d postgres:15
    echo.
    echo Or continue without database (some features won't work)
    echo.
)

echo 🚀 Starting Wekeza Core Banking API...
echo.
echo 🌐 API will be available at:
echo   • HTTPS: https://localhost:7001
echo   • HTTP:  http://localhost:5001  
echo   • Swagger: https://localhost:7001/swagger
echo.
echo 📊 Health Check: https://localhost:7001/health
echo.
echo Press Ctrl+C to stop the server
echo ═══════════════════════════════════════════════════════════
echo.

REM Change to API directory and run
cd Core\Wekeza.Core.Api
%DOTNET_PATH% run --configuration Debug

echo.
echo 🏦 Wekeza Core Banking System stopped.
pause