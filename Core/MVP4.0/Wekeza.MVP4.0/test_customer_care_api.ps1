# Test Customer Care API endpoints
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjI0MjhjOWMwLWNiYzAtNGJmOC04NDQyLWY5ODU3M2EzNmNmMiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJqYWNvYm9kZW55aXJlIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiamFjb2JvZGVueWlyZUB3ZWtlemEuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQ3VzdG9tZXJDYXJlT2ZmaWNlciIsIkZ1bGxOYW1lIjoiSmFjb2IgT2RlbnlpcmUiLCJleHAiOjE3NjkyMzA5MzQsImlzcyI6Ildla2VlemFNVlA0IiwiYXVkIjoiV2VrZWV6YU1WUDRVc2VycyJ9.3b2vxIPj88DwvZDUFtyUsfk_oi8IoG5impMadXp7UYc"

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

Write-Host "Testing Customer Care API endpoints..."

# Test dashboard stats
Write-Host "`n1. Testing Dashboard Stats:"
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5004/api/customercare/dashboard/stats" -Method GET -Headers $headers
    Write-Host "Dashboard Stats: $($response | ConvertTo-Json -Depth 3)"
} catch {
    Write-Host "Error: $($_.Exception.Message)"
}

# Test customer search
Write-Host "`n2. Testing Customer Search:"
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5004/api/customercare/customers/search?searchTerm=John" -Method GET -Headers $headers
    Write-Host "Search Results: $($response | ConvertTo-Json -Depth 3)"
} catch {
    Write-Host "Error: $($_.Exception.Message)"
}

# Test complaints
Write-Host "`n3. Testing Complaints:"
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5004/api/customercare/complaints" -Method GET -Headers $headers
    Write-Host "Complaints: $($response | ConvertTo-Json -Depth 3)"
} catch {
    Write-Host "Error: $($_.Exception.Message)"
}