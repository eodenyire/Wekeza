using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Application.DTOs;

public record AuthResponse(
    Guid Id,
    string Username,
    string Email,
    UserRole Role,
    string? FullName,
    string Token
);
