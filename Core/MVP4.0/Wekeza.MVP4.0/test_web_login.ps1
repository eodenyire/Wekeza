# Test web login functionality
$baseUrl = "http://localhost:5004"

Write-Host "Testing Customer Care web login..." -ForegroundColor Yellow

try {
    # Create a web session to maintain cookies
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    
    # Get the login page first
    $loginPage = Invoke-WebRequest -Uri "$baseUrl/Login/CustomerCareOfficer" -WebSession $session -UseBasicParsing
    Write-Host "Login page loaded successfully" -ForegroundColor Green
    
    # Extract antiforgery token
    $antiForgeryToken = ""
    if ($loginPage.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
        $antiForgeryToken = $matches[1]
        Write-Host "Antiforgery token extracted" -ForegroundColor Green
    }
    
    # Prepare login form data
    $loginForm = @{
        Username = "jacobodenyire"
        Password = "admin123"
        RememberMe = "false"
    }
    
    if ($antiForgeryToken) {
        $loginForm["__RequestVerificationToken"] = $antiForgeryToken
    }
    
    # Submit login form
    Write-Host "Submitting login form..." -ForegroundColor Yellow
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/Login/CustomerCareOfficer" -Method POST -Body $loginForm -WebSession $session -UseBasicParsing
    
    Write-Host "Login response status: $($loginResponse.StatusCode)" -ForegroundColor Cyan
    
    # Check if we were redirected to the dashboard
    if ($loginResponse.Headers.Location -or $loginResponse.Content -match "Customer Care Dashboard") {
        Write-Host "Login successful - redirected to dashboard" -ForegroundColor Green
        
        # Try to access the Customer Care dashboard directly
        $dashboardResponse = Invoke-WebRequest -Uri "$baseUrl/CustomerCare" -WebSession $session -UseBasicParsing
        if ($dashboardResponse.StatusCode -eq 200) {
            Write-Host "Customer Care dashboard accessible" -ForegroundColor Green
            
            # Check if dashboard contains expected content
            if ($dashboardResponse.Content -match "Customer Care" -or $dashboardResponse.Content -match "Active Inquiries") {
                Write-Host "Dashboard contains expected Customer Care content" -ForegroundColor Green
            }
        }
    } else {
        Write-Host "Login may have failed - checking response content" -ForegroundColor Yellow
        if ($loginResponse.Content -match "error") {
            Write-Host "Error detected in login response" -ForegroundColor Red
        }
    }
    
} catch {
    Write-Host "Error during web login test: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "Web login test completed" -ForegroundColor Cyan