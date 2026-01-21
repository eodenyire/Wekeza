namespace Wekeza.Core.Application.Common.Services;

/// <summary>
/// Service for password hashing and verification
/// </summary>
public interface IPasswordHashingService
{
    /// <summary>
    /// Hash a password using secure hashing algorithm
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verify a password against its hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hash">Stored password hash</param>
    /// <returns>True if password matches hash</returns>
    bool VerifyPassword(string password, string hash);

    /// <summary>
    /// Generate a secure random password
    /// </summary>
    /// <param name="length">Password length (default 12)</param>
    /// <returns>Generated password</returns>
    string GeneratePassword(int length = 12);
}