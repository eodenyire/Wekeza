using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.BranchOperations.Commands.ProcessBOD;

/// <summary>
/// Command to process Beginning of Day (BOD) for a branch
/// </summary>
public record ProcessBODCommand(
    Guid BranchId,
    string ProcessedBy) : IRequest<Result<bool>>;