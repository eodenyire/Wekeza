using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.BranchOperations.Commands.ProcessEOD;

/// <summary>
/// Command to process End of Day (EOD) for a branch
/// </summary>
public record ProcessEODCommand(
    Guid BranchId,
    string ProcessedBy) : IRequest<Result<bool>>;