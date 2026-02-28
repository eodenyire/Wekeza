using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.BranchOperations.Commands.ProcessEOD;

/// <summary>
/// Handler for processing End of Day (EOD) operations
/// </summary>
public class ProcessEODHandler : IRequestHandler<ProcessEODCommand, Result<bool>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessEODHandler(
        IBranchRepository branchRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ProcessEODCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(request.BranchId);
            if (branch == null)
                return Result<bool>.Failure("Branch not found");

            // Process EOD
            branch.ProcessEOD(request.ProcessedBy);

            // Save changes
            await _branchRepository.UpdateAsync(branch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to process EOD: {ex.Message}");
        }
    }
}