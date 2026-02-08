using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Commands.BlockCard;

public record BlockCardCommand : ICommand<Result<bool>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string CardNumber { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public class BlockCardHandler : IRequestHandler<BlockCardCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public BlockCardHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(BlockCardCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Failed to block card: {ex.Message}");
        }
    }
}
