using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.BranchOperations.Commands.ProcessBOD;

/// <summary>
/// Handler for processing Beginning of Day (BOD) operations
/// </summary>
public class ProcessBODHandler : IRequestHandler<ProcessBODCommand, Result<BODResult>>
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

    public async Task<Result<BODResult>> Handle(ProcessBODCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var branch = await _branchRepository.GetByIdAsync(request.BranchId);
            if (branch == null)
                return Result<BODResult>.Failure("Branch not found");

            // Process BOD
            branch.ProcessBOD(request.ProcessedBy);

            // Save changes
            await _branchRepository.UpdateAsync(branch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new BODResult
            {
                ProcessId = Guid.NewGuid(),
                Status = "COMPLETED",
                ProcessedAt = DateTime.UtcNow,
                CompletedTasks = new List<string> { "BOD Processing" },
                Message = "BOD processed successfully"
            };

            return Result<BODResult>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<BODResult>.Failure($"Failed to process BOD: {ex.Message}");
        }
    }
}