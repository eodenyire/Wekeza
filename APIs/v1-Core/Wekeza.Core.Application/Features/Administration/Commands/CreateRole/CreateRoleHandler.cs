using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Commands.CreateRole;

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleHandler(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var roleId = Guid.NewGuid();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(roleId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create role: {ex.Message}");
        }
    }
}
