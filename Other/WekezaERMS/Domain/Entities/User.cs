using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Username for login
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// BCrypt hashed password
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// User's role in the system
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Account creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Last updated timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Full name of the user
    /// </summary>
    public string? FullName { get; private set; }

    private User() { }

    public static User Create(
        string username,
        string email,
        string passwordHash,
        UserRole role,
        string? fullName = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));

        return new User
        {
            Id = Guid.NewGuid(),
            Username = username.ToLowerInvariant(),
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            FullName = fullName
        };
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string? fullName, string? email)
    {
        if (!string.IsNullOrWhiteSpace(email))
        {
            Email = email.ToLowerInvariant();
        }

        FullName = fullName;
        UpdatedAt = DateTime.UtcNow;
    }
}
