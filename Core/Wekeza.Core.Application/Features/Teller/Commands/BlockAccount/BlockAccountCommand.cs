using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.BlockAccount;

public record BlockAccountCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public string BlockType { get; init; } = string.Empty;
}

public class BlockAccountHandler : IRequestHandler<BlockAccountCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public BlockAccountHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(BlockAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to block account: {ex.Message}");
        }
    }
}
