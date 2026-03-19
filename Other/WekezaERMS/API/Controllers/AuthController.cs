using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WekezaERMS.Application.Commands.Users;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Application.Services;
using WekezaERMS.Domain.Entities;
using BCrypt.Net;

namespace WekezaERMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthController(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        
        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        
        return Ok(new AuthResponse(
            user.Id,
            user.Username,
            user.Email,
            user.Role,
            user.FullName,
            token
        ));
    }

    /// <summary>
    /// Register a new user (Admin only)
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        // Check if username already exists
        if (await _userRepository.UsernameExistsAsync(request.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create user
        var user = WekezaERMS.Domain.Entities.User.Create(
            request.Username,
            request.Email,
            passwordHash,
            request.Role,
            request.FullName
        );

        await _userRepository.CreateAsync(user);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return CreatedAtAction(
            nameof(GetCurrentUser),
            null,
            new AuthResponse(
                user.Id,
                user.Username,
                user.Email,
                user.Role,
                user.FullName,
                token
            )
        );
    }

    /// <summary>
    /// Get current authenticated user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized();
        }

        var user = await _userRepository.GetByIdAsync(parsedUserId);
        
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.Role,
            user.FullName,
            user.IsActive,
            user.CreatedAt
        ));
    }
}
