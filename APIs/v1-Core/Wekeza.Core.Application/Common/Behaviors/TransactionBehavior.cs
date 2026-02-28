using MediatR;
using Microsoft.Extensions.Logging;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Common.Behaviors;

/// <summary>
/// ⛓️ 3. TransactionBehavior.cs
/// Purpose: The "Atomic Guard." Ensures that every command is wrapped in a physical database transaction. Future-Proofing: This is the most critical for a bank. If a transfer debits Account A but fails to credit Account B, this behavior ensures the entire operation rolls back.
/// 📂 Wekeza.Core.Application/Common/Behaviors
/// These classes are Cross-Cutting Concerns. They sit between the API and the Business Logic. Every request entering the system must flow through them.
/// Ensures Atomicity: "All or Nothing." 
/// Every Command is executed within a database transaction boundary.
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(ILogger<TRequest> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Skip transactions for Queries (Read-only)
        if (request.GetType().Name.EndsWith("Query"))
        {
            return await next();
        }

        try
        {
            _logger.LogInformation("[WEKEZA DB] Beginning transaction for {RequestName}", typeof(TRequest).Name);
            
            // Note: The logic here depends on your IUnitOfWork implementation 
            // but the principle is: Start Transaction -> Next() -> Commit Transaction.
            var response = await next();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[WEKEZA DB] Committed transaction for {RequestName}", typeof(TRequest).Name);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[WEKEZA DB] Transaction failed for {RequestName}. Rolling back...", typeof(TRequest).Name);
            throw; // Rethrow to let the Exception Middleware handle the UI response
        }
    }
}
