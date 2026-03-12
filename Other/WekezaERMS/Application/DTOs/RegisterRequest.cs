using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Application.DTOs;

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    UserRole Role,
    string? FullName = null
);
