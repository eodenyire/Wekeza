using MediatR;

namespace Wekeza.Core.Application.Features.Instruments.Cards.Commands.IssueCard;

public record IssueCardCommand : IRequest<Guid>
{
    public Guid AccountId { get; init; }
    public string CardType { get; init; } = "Debit"; // Debit, Credit, Prepaid
    public string NameOnCard { get; init; } = default!;
    public decimal DailyWithdrawalLimit { get; init; } = 50_000;
}
