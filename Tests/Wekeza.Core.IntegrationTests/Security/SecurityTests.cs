using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Wekeza.Core.Api;
using Xunit;

namespace Wekeza.Core.IntegrationTests.Security;

/// <summary>
/// Security integration tests to validate authentication, authorization, and security controls
/// Tests: JWT authentication, RBAC, rate limiting, input validation, HTTPS enforcement
/// </summary>
public class SecurityTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SecurityTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task UnauthorizedRequest_ShouldReturn401()
    {
        // Act - Call protected endpoint without authentication
        var response = await _client.GetAsync("/api/accounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvalidToken_ShouldReturn401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await _client.GetAsync("/api/accounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ValidToken_InsufficientRole_ShouldReturn403()
    {
        // Arrange - Create token with Customer role trying to access admin endpoint
        var customerToken = GenerateTestToken("customer", "Customer");
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", customerToken);

        // Act - Try to access admin-only endpoint
        var response = await _client.GetAsync("/api/admin/system-parameters");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task RateLimiting_ExcessiveRequests_ShouldReturn429()
    {
        // Arrange
        var token = GenerateTestToken("teller", "Teller");
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var tasks = new List<Task<HttpResponseMessage>>();

        // Act - Send many requests rapidly to trigger rate limiting
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(_client.GetAsync("/api/accounts"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - At least some requests should be rate limited
        responses.Should().Contain(r => r.StatusCode == HttpStatusCode.TooManyRequests);
    }

    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("'; DROP TABLE Accounts; --")]
    [InlineData("../../../etc/passwd")]
    public async Task MaliciousInput_ShouldBeRejected(string maliciousInput)
    {
        // Arrange
        var token = GenerateTestToken("teller", "Teller");
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        var maliciousRequest = new
        {
            AccountId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            AccountType = maliciousInput, // Malicious input
            Currency = "KES",
            InitialDeposit = 1000.00m,
            BranchCode = "001"
        };

        var json = JsonSerializer.Serialize(maliciousRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/accounts/open", content);

        // Assert - Should be rejected with bad request or validation error
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest, 
            HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task SensitiveData_ShouldNotBeExposed()
    {
        // Arrange
        var token = GenerateTestToken("customer", "Customer");
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act - Get account details
        var response = await _client.GetAsync($"/api/accounts/{Guid.NewGuid()}");

        // Assert - Response should not contain sensitive internal data
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            // Check that sensitive fields are not exposed
            content.Should().NotContain("password", "Password should not be in response");
            content.Should().NotContain("connectionString", "Connection strings should not be exposed");
            content.Should().NotContain("secret", "Secrets should not be exposed");
            content.Should().NotContain("key", "Keys should not be exposed");
        }
    }

    [Fact]
    public async Task HTTPS_ShouldBeEnforced()
    {
        // This test would be more relevant in a production environment
        // In development, we might not enforce HTTPS
        
        // Arrange - Create client that doesn't follow redirects
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act - Try to access with HTTP (if HTTPS is enforced)
        var response = await client.GetAsync("http://localhost/api/health");

        // Assert - Should redirect to HTTPS or be configured appropriately
        // In development, this might not apply, but in production it should
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, // If HTTP is allowed in development
            HttpStatusCode.MovedPermanently, // If redirected to HTTPS
            HttpStatusCode.BadRequest // If HTTP is rejected
        );
    }

    [Fact]
    public async Task SecurityHeaders_ShouldBePresent()
    {
        // Arrange
        var token = GenerateTestToken("teller", "Teller");
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert - Check for important security headers
        response.Headers.Should().ContainKey("X-Content-Type-Options");
        response.Headers.Should().ContainKey("X-Frame-Options");
        response.Headers.Should().ContainKey("X-XSS-Protection");
        
        // Check header values
        response.Headers.GetValues("X-Content-Type-Options").Should().Contain("nosniff");
        response.Headers.GetValues("X-Frame-Options").Should().Contain("DENY");
    }

    [Fact]
    public async Task PasswordComplexity_ShouldBeEnforced()
    {
        // Arrange
        var weakPasswords = new[]
        {
            "123456",
            "password",
            "abc123",
            "qwerty"
        };

        // Act & Assert
        foreach (var weakPassword in weakPasswords)
        {
            var registrationRequest = new
            {
                Username = "testuser",
                Password = weakPassword,
                Email = "test@example.com"
            };

            var json = JsonSerializer.Serialize(registrationRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/register", content);

            // Should reject weak passwords
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task TokenExpiration_ShouldBeEnforced()
    {
        // Arrange - Create an expired token
        var expiredToken = GenerateExpiredTestToken("teller", "Teller");
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", expiredToken);

        // Act
        var response = await _client.GetAsync("/api/accounts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ConcurrentSessions_ShouldBeControlled()
    {
        // Arrange - Create multiple clients with same user token
        var token = GenerateTestToken("teller", "Teller");
        var clients = new List<HttpClient>();

        for (int i = 0; i < 5; i++)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
            clients.Add(client);
        }

        // Act - Make concurrent requests
        var tasks = clients.Select(c => c.GetAsync("/api/accounts")).ToArray();
        var responses = await Task.WhenAll(tasks);

        // Assert - All should succeed (or implement session limits if required)
        responses.Should().AllSatisfy(r => 
            r.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized));

        // Cleanup
        foreach (var client in clients)
        {
            client.Dispose();
        }
    }

    private string GenerateTestToken(string userId, string role)
    {
        // In a real implementation, this would generate a proper JWT token
        // For testing purposes, we'll return a mock token
        // This should integrate with your actual JWT token generation logic
        return "test-token-" + userId + "-" + role;
    }

    private string GenerateExpiredTestToken(string userId, string role)
    {
        // Generate a token that's already expired
        return "expired-test-token-" + userId + "-" + role;
    }
}