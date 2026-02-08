namespace WekezaERMS.Application.DTOs;

public record LoginRequest(
    string Username,
    string Password
);
