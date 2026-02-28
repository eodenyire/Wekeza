using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Administration.Commands.CreateBranch;

public class CreateBranchHandler : IRequestHandler<CreateBranchCommand, Result<Guid>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBranchHandler(
        IBranchRepository branchRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingBranch = await _branchRepository.GetByCodeAsync(request.BranchCode);
            if (existingBranch != null)
                return Result<Guid>.Failure("Branch code already exists");

            var currentUsername = _currentUserService.Username ?? "System";
            var branchId = Guid.NewGuid();
            
            var branch = new Branch(
                branchId,
                request.BranchCode,
                request.BranchName,
                request.Address,
                request.City ?? "Unknown",
                "Kenya",
                request.PhoneNumber ?? "",
                request.Email ?? "",
                Wekeza.Core.Domain.Aggregates.BranchType.Main,
                DateTime.UtcNow,
                "Africa/Nairobi",
                new TimeSpan(8, 0, 0),
                new TimeSpan(17, 0, 0),
                currentUsername,
                new Wekeza.Core.Domain.ValueObjects.Money(1000000, Wekeza.Core.Domain.ValueObjects.Currency.KES),
                new Wekeza.Core.Domain.ValueObjects.Money(500000, Wekeza.Core.Domain.ValueObjects.Currency.KES),
                currentUsername);

            await _branchRepository.AddAsync(branch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(branch.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create branch: {ex.Message}");
        }
    }
}
