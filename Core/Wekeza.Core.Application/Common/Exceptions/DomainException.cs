///📂 Wekeza.Core.Application/Common/Exceptions/
///1. DomainException.cs (The Rule Breaker)
///This exception is thrown when a Business Rule is violated. It’s not about code crashing; it’s about the "Soul" of the bank saying, "No, you cannot do this." For example: "Account is Frozen" or "Insufficient Funds."
///
///
namespace Wekeza.Core.Application.Common.Exceptions;

/// <summary>
/// Thrown when a core banking business rule is violated.
/// Maps to 400 Bad Request in the Web API layer.
/// </summary>
public class DomainException : Exception
{
    public string? ErrorCode { get; }

    public DomainException(string message) 
        : base(message) { }

    public DomainException(string message, string errorCode) 
        : base(message) 
    {
        ErrorCode = errorCode;
    }
}
