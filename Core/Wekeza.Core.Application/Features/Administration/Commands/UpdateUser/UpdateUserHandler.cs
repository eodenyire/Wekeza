using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Commands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result<bool>.Failure("User not found");

            var currentUsername = _currentUserService.Username ?? "System";

            if (request.Email != null && request.Email != user.Email)
            {
                if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                    return Result<bool>.Failure("Email already exists");
            }

            // Note: User aggregate doesn't have UpdateFirstName/UpdateLastName methods
            // These would need to be added to the domain model or use UpdateProfile method
            
            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.UpdatePhoneNumber(request.PhoneNumber, currentUsername);
            
            if (!string.IsNullOrEmpty(request.Department))
                user.UpdateDepartment(request.Department, currentUsername);
            
            if (!string.IsNullOrEmpty(request.JobTitle))
                user.UpdateJobTitle(request.JobTitle, currentUsername);

            if (request.BranchId.HasValue)
                user.AssignToBranch(request.BranchId.Value.ToString(), currentUsername);

            if (request.IsActive.HasValue && !request.IsActive.Value)
            {
                user.Deactivate(currentUsername);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to update user: {ex.Message}");
        }
    }
}
