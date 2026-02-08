using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Nexus.Application.Services;
using Wekeza.Nexus.Application.Exceptions;
using Wekeza.Nexus.Domain.Enums;

namespace Wekeza.Core.Application.Features.Transactions.Commands.TransferFunds;

/// <summary>
/// Enhanced TransferFundsHandler with Wekeza Nexus fraud detection integration
/// 
/// This demonstrates how to integrate the fraud detection system into the existing payment flow.
/// 
/// Key Changes from Original:
/// 1. Added WekezaNexusClient dependency
/// 2. Added fraud evaluation BEFORE processing transaction
/// 3. Added decision enforcement (Block, Challenge, Review, Allow)
/// 4. Added proper exception handling for fraud scenarios
/// </summary>
public class TransferFundsHandlerWithNexus : IRequestHandler<TransferFundsCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TransferService _transferService;
    private readonly WekezaNexusClient _nexusClient; // NEW: Fraud detection client
    
    public TransferFundsHandlerWithNexus(
        IAccountRepository accountRepository, 
        IUnitOfWork unitOfWork,
        TransferService transferService,
        WekezaNexusClient nexusClient) // NEW: Inject Nexus client
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _transferService = transferService;
        _nexusClient = nexusClient;
    }
    
    public async Task<Guid> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
    {
        // NEW STEP 1: Evaluate fraud risk BEFORE fetching accounts
        // This provides early exit if transaction is clearly fraudulent
        var fraudVerdict = await _nexusClient.EvaluateTransactionAsync(
            userId: request.UserId ?? Guid.Empty,
            fromAccountNumber: request.FromAccountNumber,
            toAccountNumber: request.ToAccountNumber,
            amount: request.Amount,
            currency: request.Currency,
            transactionType: "Transfer",
            description: request.Description,
            deviceInfo: request.DeviceInfo,
            behavioralData: request.BehavioralData,
            channel: request.Channel ?? "Web",
            sessionId: request.SessionId ?? string.Empty,
            cancellationToken: cancellationToken
        );
        
        // NEW STEP 2: Enforce fraud decision
        switch (fraudVerdict.Decision)
        {
            case FraudDecision.Block:
                // Transaction is blocked - throw exception
                throw new FraudDetectedException(
                    fraudVerdict.Reason,
                    fraudVerdict.RiskScore,
                    fraudVerdict.RiskLevel.ToString(),
                    fraudVerdict.TransactionContextId
                );
                
            case FraudDecision.Challenge:
                // Transaction requires step-up authentication
                // In a real implementation, this would trigger OTP/Biometric challenge
                // For now, we throw an exception that the API layer can catch
                throw new StepUpAuthenticationRequiredException(
                    fraudVerdict.Reason,
                    fraudVerdict.TransactionContextId,
                    challengeType: "OTP"
                );
                
            case FraudDecision.Review:
                // Transaction is flagged for review but allowed to proceed
                // Log this for investigation
                // In production, you might want to send this to a fraud analyst queue
                // For now, we proceed with the transaction
                break;
                
            case FraudDecision.Allow:
                // Transaction is safe - proceed normally
                break;
        }
        
        // ORIGINAL STEP 1: Fetch Aggregates (Infrastructure handles Row-Level Locking via the repository)
        var sourceAccount = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.FromAccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.FromAccountNumber);
        
        var destinationAccount = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.ToAccountNumber), cancellationToken)
            ?? throw new NotFoundException("Account", request.ToAccountNumber);
        
        // ORIGINAL STEP 2: Map to Value Object
        var transferAmount = new Money(request.Amount, Currency.FromCode(request.Currency));
        
        // ORIGINAL STEP 3: Delegate to Domain Service (Encapsulated Business Logic)
        // This method performs the Debit, Credit, and Currency Checks.
        _transferService.Transfer(sourceAccount, destinationAccount, transferAmount);
        
        // ORIGINAL STEP 4: Update Repositories
        _accountRepository.Update(sourceAccount);
        _accountRepository.Update(destinationAccount);
        
        // ORIGINAL STEP 5: Commit (The TransactionBehavior will handle the physical SQL Transaction & Outbox)
        // We return the CorrelationId so the UI can track the status.
        return request.CorrelationId;
    }
}
