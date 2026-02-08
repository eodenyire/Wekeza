using MediatR;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessCashWithdrawal;

public record ProcessCashWithdrawalCommand : ICommand<Result<Guid>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
}
