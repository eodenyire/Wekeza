using MediatR;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Application.Features.Teller.Commands.ProcessCashDeposit;

/// <summary>
/// Process Cash Deposit Command - Teller cash deposit processing
/// Processes cash deposits with customer verification and GL posting
/// </summary>
[Authorize(UserRole.Teller, UserRole.BranchManager, UserRole.Administrator)]
public record ProcessCashDepositCommand : IRequest<ProcessCashDepositResult>
{
    public Guid SessionId { get; init; }
    public Guid AccountId { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public decimal DepositAmount { get; init; }
    public string Currency { get; init; } = "KES";
    public CustomerVerificationMethod VerificationMethod { get; init; } = CustomerVerificationMethod.IDCard;
    public string? VerificationDetails { get; init; }
    public bool CustomerPresent { get; init; } = true;
    public string? Reference { get; init; }
    public string? Notes { get; init; }
    public string? CustomerComments { get; init; }
}

public record ProcessCashDepositResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string? TransactionNumber { get; init; }
    public decimal? DepositAmount { get; init; }
    public decimal? NewAccountBalance { get; init; }
    public string? AccountNumber { get; init; }
    public string? JournalNumber { get; init; }
    public DateTime? TransactionDate { get; init; }
    public string? Message { get; init; }

    public static ProcessCashDepositResult Success(
        string transactionNumber,
        decimal depositAmount,
        decimal newAccountBalance,
        string accountNumber,
        string? journalNumber = null,
        string? message = null)
    {
        return new ProcessCashDepositResult
        {
            IsSuccess = true,
            TransactionNumber = transactionNumber,
            DepositAmount = depositAmount,
            NewAccountBalance = newAccountBalance,
            AccountNumber = accountNumber,
            JournalNumber = journalNumber,
            TransactionDate = DateTime.UtcNow,
            Message = message ?? "Cash deposit processed successfully"
        };
    }

    public static ProcessCashDepositResult Failed(string errorMessage)
    {
        return new ProcessCashDepositResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}