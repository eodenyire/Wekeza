using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Application.DTOs;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    UserRole Role,
    string? FullName,
    bool IsActive,
    DateTime CreatedAt
);
