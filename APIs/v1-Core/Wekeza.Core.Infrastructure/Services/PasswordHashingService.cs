using System.Security.Cryptography;
using System.Text;
using Wekeza.Core.Application.Common.Services;

namespace Wekeza.Core.Infrastructure.Services;

public class PasswordHashingService : IPasswordHashingService
{
    public string HashPassword(string password)
    {
        // Simple hash for stub - should use BCrypt or similar in production
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }

    public string GeneratePassword(int length = 12)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789!@#$%";
        var random = new Random();
        return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}
