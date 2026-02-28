using MediatR;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Teller.Commands.StartTellerSession;

/// <summary>
/// Start Teller Session Command - Initiates a new teller session
/// Opens teller session with cash drawer and establishes transaction limits
/// </summary>
[Authorize(UserRole.Teller, UserRole.BranchManager, UserRole.Administrator)]
public record StartTellerSessionCommand : IRequest<StartTellerSessionResult>
{
    public Guid TellerId { get; init; }
    public string TellerCode { get; init; } = string.Empty;
    public string TellerName { get; init; } = string.Empty;
    public Guid BranchId { get; init; }
    public string BranchCode { get; init; } = string.Empty;
    public decimal OpeningCashAmount { get; init; }
    public string Currency { get; init; } = "KES";
    public decimal DailyTransactionLimit { get; init; } = 1000000; // 1M default
    public decimal SingleTransactionLimit { get; init; } = 100000; // 100K default
    public decimal CashWithdrawalLimit { get; init; } = 500000; // 500K default
    public string? Notes { get; init; }
}

public record StartTellerSessionResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public Guid? SessionId { get; init; }
    public string? SessionNumber { get; init; }
    public string? DrawerId { get; init; }
    public decimal? OpeningCashBalance { get; init; }
    public DateTime? SessionStartTime { get; init; }
    public string? Message { get; init; }

    public static StartTellerSessionResult Success(
        Guid sessionId,
        string sessionNumber,
        string drawerId,
        decimal openingCashBalance,
        DateTime sessionStartTime,
        string? message = null)
    {
        return new StartTellerSessionResult
        {
            IsSuccess = true,
            SessionId = sessionId,
            SessionNumber = sessionNumber,
            DrawerId = drawerId,
            OpeningCashBalance = openingCashBalance,
            SessionStartTime = sessionStartTime,
            Message = message ?? "Teller session started successfully"
        };
    }

    public static StartTellerSessionResult Failed(string errorMessage)
    {
        return new StartTellerSessionResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}