using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Exceptions;

/// <summary>
/// Exception thrown when ATM processing operations fail
/// </summary>
public class ATMProcessingException : DomainException
{
    public ATMProcessingException(string message, string code = "ATM_ERROR") 
        : base(message, code)
    {
    }
}