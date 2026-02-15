using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Models;
using Wekeza.MVP4._0.Services;
using Xunit;

namespace Wekeza.MVP4._0.Tests;

/// <summary>
/// Unit tests for authentication edge cases
/// Tests invalid credentials, session expiry, and lockout scenarios
/// **Validates: Requirements 7.1**
/// </summary>
public class AuthenticationEdgeCaseTests : IDisposable
{
    private readonly MVP4DbContext _context;
    private readonly IRBACService _rbacService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<RBACService>> _mockLogger;

    public AuthenticationEdgeCaseTests()
    {
        var options = new DbContextOptionsBuilder<MVP4DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MVP4DbContext(options);
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<RBACService>>();

        // Setup configuration for JWT
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("WekeezaMVP4SecretKeyThatIsAtLeast32CharactersLong123456");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("WekeezaMVP4");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("WekeezaMVP4Users");

        _rbacService = new RBACService(_context, _mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AuthenticateUser_WithInvalidUsername_ShouldReturnFailure()
    {
        // Arrange
        var credentials = new UserCredentials
        {
            Username = "nonexistentuser",
            Password = "AnyPassword123!"
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(credentials);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
        Assert.Null(result.User);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task AuthenticateUser_WithInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "testuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            FullName = "Test User",
            Role = UserRole.BackOfficeStaff,
            IsActive = true,
            FailedLoginAttempts = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var credentials = new UserCredentials
        {
            Username = "testuser",
            Password = "WrongPassword123!"
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(credentials);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
        Assert.Null(result.User);
        Assert.Null(result.Token);

        // Verify failed login attempts were incremented
        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(1, updatedUser.FailedLoginAttempts);
    }

    [Fact]
    public async Task AuthenticateUser_WithInactiveUser_ShouldReturnFailure()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "inactiveuser",
            Email = "inactiveuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ValidPassword123!"),
            FullName = "Inactive User",
            Role = UserRole.BackOfficeStaff,
            IsActive = false, // User is inactive
            FailedLoginAttempts = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var credentials = new UserCredentials
        {
            Username = "inactiveuser",
            Password = "ValidPassword123!"
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(credentials);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
        Assert.Null(result.User);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task AuthenticateUser_WithLockedAccount_ShouldReturnFailure()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "lockeduser",
            Email = "lockeduser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ValidPassword123!"),
            FullName = "Locked User",
            Role = UserRole.BackOfficeStaff,
            IsActive = true,
            FailedLoginAttempts = 5 // Account is locked due to failed attempts
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var credentials = new UserCredentials
        {
            Username = "lockeduser",
            Password = "ValidPassword123!"
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(credentials);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Account is locked due to multiple failed attempts", result.ErrorMessage);
        Assert.Null(result.User);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task AuthenticateUser_MultipleFailedAttempts_ShouldIncrementCounter()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "testuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            FullName = "Test User",
            Role = UserRole.BackOfficeStaff,
            IsActive = true,
            FailedLoginAttempts = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var invalidCredentials = new UserCredentials
        {
            Username = "testuser",
            Password = "WrongPassword123!"
        };

        // Act - Multiple failed attempts
        for (int i = 1; i <= 3; i++)
        {
            var result = await _rbacService.AuthenticateUserAsync(invalidCredentials);
            
            // Assert each attempt fails
            Assert.False(result.IsSuccess);
            
            // Verify counter increments
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(i, updatedUser.FailedLoginAttempts);
        }
    }

    [Fact]
    public async Task AuthenticateUser_SuccessfulLoginAfterFailedAttempts_ShouldResetCounter()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "testuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            FullName = "Test User",
            Role = UserRole.BackOfficeStaff,
            IsActive = true,
            FailedLoginAttempts = 3 // User has some failed attempts
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var validCredentials = new UserCredentials
        {
            Username = "testuser",
            Password = "CorrectPassword123!"
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(validCredentials);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.User);
        Assert.NotNull(result.Token);

        // Verify failed attempts counter was reset
        var updatedUser = await _context.Users.FindAsync(user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal(0, updatedUser.FailedLoginAttempts);
        Assert.NotNull(updatedUser.LastLoginAt);
    }

    [Fact]
    public async Task ValidateSession_WithValidUserId_ShouldReturnTrue()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "testuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ValidPassword123!"),
            FullName = "Test User",
            Role = UserRole.BackOfficeStaff,
            IsActive = true,
            FailedLoginAttempts = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _rbacService.ValidateSessionAsync(user.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateSession_WithInvalidUserId_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await _rbacService.ValidateSessionAsync(nonExistentUserId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateSession_WithInactiveUser_ShouldReturnFalse()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "inactiveuser",
            Email = "inactiveuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ValidPassword123!"),
            FullName = "Inactive User",
            Role = UserRole.BackOfficeStaff,
            IsActive = false, // User is inactive
            FailedLoginAttempts = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _rbacService.ValidateSessionAsync(user.Id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AuthenticateUser_WithEmptyCredentials_ShouldHandleGracefully()
    {
        // Arrange
        var emptyCredentials = new UserCredentials
        {
            Username = "",
            Password = ""
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(emptyCredentials);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
    }

    [Fact]
    public async Task AuthenticateUser_WithNullCredentials_ShouldHandleGracefully()
    {
        // Arrange
        var nullCredentials = new UserCredentials
        {
            Username = null!,
            Password = null!
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(nullCredentials);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
    }

    [Fact]
    public async Task AuthenticateUser_WithWhitespaceCredentials_ShouldHandleGracefully()
    {
        // Arrange
        var whitespaceCredentials = new UserCredentials
        {
            Username = "   ",
            Password = "   "
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(whitespaceCredentials);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
    }

    [Fact]
    public async Task LogoutUser_ShouldCompleteSuccessfully()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "testuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ValidPassword123!"),
            FullName = "Test User",
            Role = UserRole.BackOfficeStaff,
            IsActive = true,
            FailedLoginAttempts = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act - Should not throw exception
        await _rbacService.LogoutUserAsync(user.Id);

        // Assert - No exception thrown means success
        Assert.True(true);
    }

    [Fact]
    public async Task AuthenticateUser_CaseSensitiveUsername_ShouldReturnFailure()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Username = "TestUser", // Mixed case
            Email = "testuser@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ValidPassword123!"),
            FullName = "Test User",
            Role = UserRole.BackOfficeStaff,
            IsActive = true,
            FailedLoginAttempts = 0
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var credentials = new UserCredentials
        {
            Username = "testuser", // Different case
            Password = "ValidPassword123!"
        };

        // Act
        var result = await _rbacService.AuthenticateUserAsync(credentials);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}