using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.UpdateProfile;

public record UpdateCustomerProfileCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
}

public class UpdateCustomerProfileHandler : IRequestHandler<UpdateCustomerProfileCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerProfileHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to update profile: {ex.Message}");
        }
    }
}
