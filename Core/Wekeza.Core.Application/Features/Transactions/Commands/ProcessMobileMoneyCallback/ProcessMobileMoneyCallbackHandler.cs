using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Aggregates;
using MediatR;

namespace Wekeza.Core.Application.Features.Transactions.Commands.ProcessMobileMoneyCallback;

public record ProcessMobileMoneyCallbackCommand : IRequest<bool>
{
    public string CheckoutRequestID { get; init; } = string.Empty;
    public int ResultCode { get; init; }
    public decimal Amount { get; init; }
    public string MpesaReceiptNumber { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}

public class ProcessMobileMoneyCallbackHandler : IRequestHandler<ProcessMobileMoneyCallbackCommand, bool>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessMobileMoneyCallbackHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ProcessMobileMoneyCallbackCommand request, CancellationToken ct)
    {
        if (request.ResultCode != 0) return false; // Payment failed

        // Find the account linked to this phone number
        var account = await _accountRepository.GetByPhoneNumberAsync(request.PhoneNumber, ct)
            ?? throw new NotFoundException("Account", request.PhoneNumber);

        // Atomic Credit: Update Balance and Record M-Pesa Receipt
        var depositAmount = new Money(request.Amount, account.Balance.Currency);
        account.Credit(depositAmount);
        
        var transaction = new Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(), // CorrelationId
            account.Id,
            depositAmount,
            TransactionType.Deposit,
            $"M-Pesa Deposit Ref: {request.MpesaReceiptNumber}"
        );

        await _transactionRepository.AddAsync(transaction, ct);
        _accountRepository.Update(account);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return true;
    }
}
