# Fix JSON deserialization in all login pages
$loginPages = @(
    "Pages/Login/BranchManager.cshtml.cs",
    "Pages/Login/CashOfficer.cshtml.cs", 
    "Pages/Login/BackOfficeStaff.cshtml.cs",
    "Pages/Login/BancassuranceAgent.cshtml.cs",
    "Pages/Login/ITAdministrator.cshtml.cs",
    "Pages/Login/Auditor.cshtml.cs",
    "Pages/Login/ComplianceOfficer.cshtml.cs",
    "Pages/Login/LoanOfficer.cshtml.cs",
    "Pages/Login/RiskOfficer.cshtml.cs",
    "Pages/Login/Supervisor.cshtml.cs"
)

$oldPattern = 'var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });'
$newPattern = @'
                var options = new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, options);
'@

foreach ($page in $loginPages) {
    if (Test-Path $page) {
        Write-Host "Fixing $page..." -ForegroundColor Yellow
        $content = Get-Content $page -Raw
        $content = $content -replace [regex]::Escape($oldPattern), $newPattern
        Set-Content $page -Value $content -NoNewline
        Write-Host "Fixed $page" -ForegroundColor Green
    }
}