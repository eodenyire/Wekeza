using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Wekeza.Core.Api.Authentication;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Authentication endpoints for Wekeza Bank
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IJwtTokenGenerator jwtTokenGenerator, IConfiguration configuration)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _configuration = configuration;
    }

    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Database connection is not configured" });
        }

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var user = await GetUserForLoginAsync(connection, request.Username);
        if (user is null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var roles = ResolveRoles(user.Role);
        var permissions = ResolvePermissions(roles);

        // For customer users, look up their CustomerId from the Customers table
        // and include it as a claim so the CustomerPortalController can use it
        List<Claim>? extraClaims = null;
        if (roles.Contains(UserRole.Customer))
        {
            await using var cmd = new NpgsqlCommand(
                @"SELECT ""Id"" FROM ""Customers"" WHERE ""Email"" = @email LIMIT 1",
                connection);
            cmd.Parameters.AddWithValue("email", user.Email);
            var customerId = await cmd.ExecuteScalarAsync();
            // Fallback: use userId as customerId for demo customer accounts
            var customerIdValue = customerId?.ToString() ?? user.Id.ToString();
            extraClaims = new List<Claim> { new("CustomerId", customerIdValue) };
        }

        var token = _jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Username,
            user.Email,
            roles,
            extraClaims
        );

        await UpdateLastLoginAsync(connection, user.Id);

        return Ok(new LoginResponse
        {
            Token = token,
            RefreshToken = Guid.NewGuid().ToString("N"),
            ExpiresIn = 3600,
            User = new AuthUserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles.Select(MapRoleToPortalRole).ToList(),
                Permissions = permissions,
                Department = user.Department
            }
        });
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(AuthUserResponse), StatusCodes.Status200OK)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value
            ?? User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? User.FindFirst(ClaimTypes.Email)?.Value
            ?? string.Empty;
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var permissions = ResolvePermissions(roles.Select(ParseUserRole));

        return Ok(new AuthUserResponse
        {
            Id = Guid.TryParse(userId, out var parsedId) ? parsedId : Guid.Empty,
            Username = username ?? string.Empty,
            Email = email ?? string.Empty,
            FullName = username ?? string.Empty,
            Roles = roles.Select(MapRoleNameToPortalRole).ToList(),
            Permissions = permissions
        });
    }

    private static async Task<AuthenticatedUser?> GetUserForLoginAsync(NpgsqlConnection connection, string username)
    {
        await using var command = new NpgsqlCommand(@"
            SELECT ""Id"", ""Username"", ""Email"", ""PasswordHash"", ""FullName"", ""Role"", ""IsActive"", ""Department""
            FROM ""Users""
            WHERE lower(""Username"") = lower(@username)
            LIMIT 1", connection);

        command.Parameters.AddWithValue("username", username);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new AuthenticatedUser
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            Username = reader.GetString(reader.GetOrdinal("Username")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            Role = reader.GetString(reader.GetOrdinal("Role")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            Department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department"))
        };
    }

    private static async Task UpdateLastLoginAsync(NpgsqlConnection connection, Guid userId)
    {
        await using var command = new NpgsqlCommand(@"
            UPDATE ""Users""
            SET ""LastLoginAt"" = @lastLoginAt,
                ""UpdatedAt"" = @updatedAt
            WHERE ""Id"" = @id", connection);

        command.Parameters.AddWithValue("lastLoginAt", DateTime.UtcNow);
        command.Parameters.AddWithValue("updatedAt", DateTime.UtcNow);
        command.Parameters.AddWithValue("id", userId);

        await command.ExecuteNonQueryAsync();
    }

    private static IEnumerable<UserRole> ResolveRoles(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return new[] { UserRole.Customer };
        }

        return role.Trim().ToLowerInvariant() switch
        {
            "administrator" => new[] { UserRole.Administrator },
            "teller" => new[] { UserRole.Teller },
            "manager" or "branchmanager" => new[] { UserRole.BranchManager },
            "supervisor" => new[] { UserRole.Supervisor },
            "loanofficer" or "financecontroller" => new[] { UserRole.LoanOfficer },
            "riskofficer" => new[] { UserRole.RiskOfficer },
            "complianceofficer" or "compliancemanager" => new[] { UserRole.ComplianceOfficer },
            "treasurydealer" or "treasury" => new[] { UserRole.TreasuryDealer },
            "tradefinanceofficer" or "tradefinance" => new[] { UserRole.TradeFinanceOfficer },
            "paymentsofficer" or "payments" => new[] { UserRole.PaymentsOfficer },
            "clearingofficer" or "clearing" => new[] { UserRole.ClearingOfficer },
            "productmanager" => new[] { UserRole.ProductManager },
            "vaultofficer" => new[] { UserRole.VaultOfficer },
            "corporatebankingofficer" => new[] { UserRole.CorporateBankingOfficer },
            "ceo" or "executive" or "chiefexecutiveofficer" => new[] { UserRole.CEO },
            "customer" or "retailcustomer" => new[] { UserRole.Customer },
            _ => new[] { UserRole.Customer }
        };
    }

    private static UserRole ParseUserRole(string role)
    {
        return Enum.TryParse<UserRole>(role, true, out var parsedRole)
            ? parsedRole
            : UserRole.Customer;
    }

    private static List<string> ResolvePermissions(IEnumerable<UserRole> roles)
    {
        var permissionSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "portal:access",
            "dashboard:view"
        };

        foreach (var role in roles)
        {
            switch (role)
            {
                case UserRole.Administrator:
                    permissionSet.Add("users:manage");
                    permissionSet.Add("system:configure");
                    permissionSet.Add("transactions:approve");
                    break;
                case UserRole.Teller:
                    permissionSet.Add("transactions:create");
                    permissionSet.Add("accounts:view");
                    break;
                case UserRole.BranchManager:
                    permissionSet.Add("branch:manage");
                    permissionSet.Add("transactions:approve");
                    break;
                case UserRole.Supervisor:
                    permissionSet.Add("transactions:approve");
                    permissionSet.Add("accounts:view");
                    break;
                case UserRole.LoanOfficer:
                    permissionSet.Add("loans:manage");
                    break;
                case UserRole.RiskOfficer:
                    permissionSet.Add("risk:manage");
                    break;
                case UserRole.ComplianceOfficer:
                    permissionSet.Add("compliance:manage");
                    break;
                case UserRole.TreasuryDealer:
                    permissionSet.Add("treasury:manage");
                    break;
                case UserRole.TradeFinanceOfficer:
                    permissionSet.Add("tradefinance:manage");
                    break;
                case UserRole.PaymentsOfficer:
                case UserRole.ClearingOfficer:
                    permissionSet.Add("payments:manage");
                    break;
                case UserRole.ProductManager:
                    permissionSet.Add("products:manage");
                    break;
                case UserRole.VaultOfficer:
                    permissionSet.Add("vault:manage");
                    permissionSet.Add("transactions:approve");
                    break;
                case UserRole.CEO:
                    permissionSet.Add("executive:view");
                    permissionSet.Add("reports:view");
                    break;
            }
        }

        return permissionSet.ToList();
    }

    private static string MapRoleToPortalRole(UserRole role)
    {
        return role switch
        {
            // Must match allowedRoles in frontend src/config/portals.ts exactly
            UserRole.Administrator      => "SystemAdministrator",
            UserRole.ITAdministrator    => "ITSecurityAdmin",
            UserRole.CEO                => "CEO",
            UserRole.BranchManager      => "BranchManager",
            UserRole.RegionalManager    => "RegionalManager",
            UserRole.Supervisor         => "Supervisor",
            UserRole.Teller             => "Teller",
            UserRole.VaultOfficer       => "VaultOfficer",
            UserRole.ComplianceOfficer  => "ComplianceManager",   // portal expects ComplianceManager
            UserRole.RiskOfficer        => "RiskOfficer",
            UserRole.TreasuryDealer     => "TreasuryDealer",
            UserRole.TradeFinanceOfficer=> "TradeFinanceOfficer",
            UserRole.CorporateBankingOfficer => "CorporateBankingOfficer",
            UserRole.ProductManager     => "ProductManager",
            UserRole.LoanOfficer        => "FinanceController",
            UserRole.FinanceController  => "FinanceController",
            UserRole.PaymentsOfficer    => "PaymentsOfficer",
            UserRole.ClearingOfficer    => "ClearingOfficer",
            UserRole.Customer           => "RetailCustomer",      // portal expects RetailCustomer
            UserRole.BackOfficeStaff    => "BackOfficeStaff",
            UserRole.CustomerService    => "CustomerService",
            _ => "RetailCustomer"
        };
    }

    private static string MapRoleNameToPortalRole(string roleName)
    {
        if (!Enum.TryParse<UserRole>(roleName, true, out var parsedRole))
        {
            return "RetailCustomer";
        }

        return MapRoleToPortalRole(parsedRole);
    }
}

public record LoginRequest
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public AuthUserResponse User { get; init; } = new();
    public int ExpiresIn { get; init; }
}

public record AuthUserResponse
{
    public Guid Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
    public List<string> Permissions { get; init; } = new();
    public string? Department { get; init; }
    public string? BranchCode { get; init; }
    public string? BranchName { get; init; }
}

internal sealed class AuthenticatedUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Department { get; set; }
}
