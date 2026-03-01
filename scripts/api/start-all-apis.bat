@echo off
echo.
echo 🏦 Starting Wekeza Banking System - All APIs
echo =============================================
echo.

echo 🚀 Starting Minimal API on port 5000...
start "Wekeza Minimal API (Port 5000)" cmd /k "cd MinimalWekezaApi && dotnet run"
timeout /t 3 /nobreak >nul

echo 🚀 Starting Database API on port 5001...
start "Wekeza Database API (Port 5001)" cmd /k "cd DatabaseWekezaApi && dotnet run"
timeout /t 3 /nobreak >nul

echo 🚀 Starting Enhanced API on port 5002...
start "Wekeza Enhanced API (Port 5002)" cmd /k "cd EnhancedWekezaApi && dotnet run"
timeout /t 3 /nobreak >nul

echo 🚀 Starting Comprehensive API on port 5003...
start "Wekeza Comprehensive API (Port 5003)" cmd /k "cd ComprehensiveWekezaApi && dotnet run"

echo.
echo 🎉 All APIs Started Successfully!
echo =================================
echo.
echo 📊 API Access URLs:
echo    🔹 Minimal API:       http://localhost:5000
echo    🔹 Database API:      http://localhost:5001
echo    🔹 Enhanced API:      http://localhost:5002
echo    🔹 Comprehensive API: http://localhost:5003
echo.
echo 📚 Swagger Documentation:
echo    🔹 Minimal:       http://localhost:5000/swagger
echo    🔹 Database:      http://localhost:5001/swagger
echo    🔹 Enhanced:      http://localhost:5002/swagger
echo    🔹 Comprehensive: http://localhost:5003/swagger
echo.
echo 👤 Owner: Emmanuel Odenyire (ID: 28839872) ^| Contact: 0716478835
echo.
pause