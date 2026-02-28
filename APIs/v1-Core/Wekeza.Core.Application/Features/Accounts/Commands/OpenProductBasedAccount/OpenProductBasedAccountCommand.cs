using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Accounts.Commands.OpenProductBasedAccount;

/// <summary>
/// Enhanced Account Opening Command with Product Factory Integration
/// This replaces the basic account opening with product-driven configuration
/// </summary>
public record OpenProductBasedAccountCommand : ICommand<OpenProductBasedAccountResult>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; init; }
    public Guid ProductId { get; init; }
    public string Currency { get; init; } = "KES";
    public decimal InitialDeposit { get; init; }
    public string? AccountAlias { get; init; }
    public Dictionary<string, string> AdditionalAttributes { get; init; } = new();
}

public record OpenProductBasedAccountResult
{
    public Guid AccountId { get; init; }
    public string AccountNumber { get; init; }
    public string CustomerGLCode { get; init; }
    public string JournalNumber { get; init; }
    public string Message { get; init; } = string.Empty;
}
