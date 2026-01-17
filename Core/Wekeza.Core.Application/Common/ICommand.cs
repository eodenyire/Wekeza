using MediatR;

namespace Wekeza.Core.Application.Common;

/// <summary>
/// 📂 Wekeza.Core.Application/Common
/// We will implement these using MediatR. To future-proof for 2026, we ensure every request is traceable by including a CorrelationId.
/// 1. ICommand.cs (The Write Contract)
/// A command represents an action that changes the state of the bank. It always returns a result (like a new Account ID or a Success flag).
/// Marks a request as a Command (State-changing operation).
/// Every command in Wekeza Bank must return a Result or a specific Data Transfer Object.
/// </summary>
/// <typeparam name="TResponse">The type of the result returned by the command.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
    // Future-proofing: Every command carries a unique tracking ID for distributed tracing
    Guid CorrelationId { get; }
}
