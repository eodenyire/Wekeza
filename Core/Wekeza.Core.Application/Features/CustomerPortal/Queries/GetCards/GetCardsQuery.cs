using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.CustomerPortal.Queries.GetCards;

public record GetCardsQuery : IRequest<Result<List<CardDto>>>
{
    public Guid CustomerId { get; init; }
}

public class CardDto
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public string LinkedAccount { get; set; } = string.Empty;
}

public class GetCardsHandler : IRequestHandler<GetCardsQuery, Result<List<CardDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCardsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<CardDto>>> Handle(GetCardsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cards = new List<CardDto>();
            return Result<List<CardDto>>.Success(cards);
        }
        catch (Exception ex)
        {
            return Result<List<CardDto>>.Failure($"Failed to get cards: {ex.Message}");
        }
    }
}
