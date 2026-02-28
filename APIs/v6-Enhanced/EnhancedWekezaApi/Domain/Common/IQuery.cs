using MediatR;

namespace EnhancedWekezaApi.Domain.Common;

/// <summary>
/// Marks a request as a Query (Read-only operation).
/// Queries should never change state and should be optimized for fast reads.
/// </summary>
/// <typeparam name="TResponse">The type of the result returned by the query.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}