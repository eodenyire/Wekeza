using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Transactions.Commands.ProcessMobileMoneyCallback;

public record ProcessMobileMoneyCallbackCommand : IRequest<bool>
{
    public string CheckoutRequestID { get; init; } = string.Empty;
    public int ResultCode { get; init; }
    public decimal Amount { get; init; }
    public string MpesaReceiptNumber { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
