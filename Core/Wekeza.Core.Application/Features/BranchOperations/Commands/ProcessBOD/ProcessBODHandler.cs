using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.BranchOperations.Commands.ProcessBOD;

/// <summary>
/// Handler for processing Beginning of Day (BOD) operations
/// </summary>
public class ProcessBODHandler : IRequestHandler<ProcessBODCommand, Result<bool>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessBODHandler(
        IBranchRepository branchRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ProcessBODCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(request.BranchId);
            if (branch == null)
                return Result<bool>.Failure("Branch not found");

            // Process BOD
            branch.ProcessBOD(request.ProcessedBy);

            // Save changes
            await _branchRepository.UpdateAsync(branch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to process BOD: {ex.Message}");
        }
    }
}