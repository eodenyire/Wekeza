using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Exceptions;

/// <summary>
/// Exception thrown when card management operations fail
/// </summary>
public class CardManagementException : DomainException
{
    public CardManagementException(string message, string code = "CARD_ERROR") 
        : base(message, code)
    {
    }
}