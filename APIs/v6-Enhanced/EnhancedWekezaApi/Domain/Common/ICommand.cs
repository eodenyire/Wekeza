using MediatR;

namespace EnhancedWekezaApi.Domain.Common;

/// <summary>
/// Marks a request as a Command (State-changing operation).
/// Every command in Wekeza Bank must return a Result or a specific Data Transfer Object.
/// </summary>
/// <typeparam name="TResponse">The type of the result returned by the command.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
    // Future-proofing: Every command carries a unique tracking ID for distributed tracing
    Guid CorrelationId { get; }
}