using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Exceptions;

/// <summary>
/// Exception thrown when POS processing operations fail
/// </summary>
public class POSProcessingException : DomainException
{
    public POSProcessingException(string message, string code = "POS_ERROR") 
        : base(message, code)
    {
    }
}