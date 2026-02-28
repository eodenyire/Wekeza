using MediatR;

namespace Wekeza.Core.Application.Common;

/// <summary>
/// 2. IQuery.cs (The Read Contract)
/// A query is side-effect-free. It simply asks the bank for data. By separating this, we can eventually point our Queries to a read-replica database to "run these streets" at lightning speed.
/// Marks a request as a Query (Read-only operation).
/// Queries never change the state of the bank's ledger.
/// </summary>
/// <typeparam name="TResponse">The data structure returned to the UI or calling system.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
    // Metadata for caching or analytics could be added here
}
