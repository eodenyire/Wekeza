using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Application.Common.Services;

namespace Wekeza.Core.Application.Features.Administration.Commands.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserHandler(
        IUserRepository userRepository,
        IBranchRepository branchRepository,
        IPasswordHashingService passwordHashingService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _branchRepository = branchRepository;
        _passwordHashingService = passwordHashingService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate uniqueness
            if (await _userRepository.ExistsByUsernameAsync(request.Username, cancellationToken))
            {
                return Result<Guid>.Failure("Username already exists");
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                return Result<Guid>.Failure("Email already exists");
            }

            if (await _userRepository.ExistsByEmployeeIdAsync(request.EmployeeId, cancellationToken))
            {
                return Result<Guid>.Failure("Employee ID already exists");
            }

            // 2. Validate branch if specified
            if (request.BranchId.HasValue)
            {
                var branch = await _branchRepository.GetByIdAsync(request.BranchId ?? Guid.Empty, cancellationToken);
                if (branch == null)
                {
                    return Result<Guid>.Failure("Branch not found");
                }
            }

            // 3. Generate temporary password if not provided
            var tempPassword = request.TemporaryPassword ?? GenerateTemporaryPassword();
            var passwordHash = _passwordHashingService.HashPassword(tempPassword);

            // 4. Create user aggregate
            var user = new User(
                username: request.Username,
                email: request.Email,
                firstName: request.FirstName,
                lastName: request.LastName,
                employeeId: request.EmployeeId,
                createdBy: _currentUserService.Username ?? "System",
                securityClearance: request.SecurityClearance,
                timeZone: request.TimeZone,
                language: request.Language
            );

            // 5. Set password and security settings
            user.SetPassword(passwordHash, request.MustChangePassword ? "Initial password" : null);
            
            if (request.MfaEnabled)
            {
                user.EnableMfa(MfaMethod.TOTP, "TOTP_SECRET", _currentUserService.Username ?? "System");
            }

            // 6. Assign roles
            foreach (var role in request.Roles)
            {
                user.AssignRole(role.ToString(), _currentUserService.Username ?? "System");
            }

            // 7. Set additional properties
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.UpdatePhoneNumber(request.PhoneNumber, _currentUserService.Username ?? "System");
            
            if (!string.IsNullOrEmpty(request.Department))
                user.UpdateDepartment(request.Department, _currentUserService.Username ?? "System");
            
            if (!string.IsNullOrEmpty(request.JobTitle))
                user.UpdateJobTitle(request.JobTitle, _currentUserService.Username ?? "System");

            if (request.BranchId.HasValue)
                user.AssignToBranch(request.BranchId.ToString(), _currentUserService.Username ?? "System");

            // 8. Save user
            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. Send welcome email with temporary password
            // This would be handled by a domain event handler
            // TODO: Add UserCreatedDomainEvent when domain events are implemented

            return Result<Guid>.Success(user.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create user: {ex.Message}");
        }
    }

    private static string GenerateTemporaryPassword()
    {
        // Generate a secure temporary password
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}