using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Exceptions;

/// <summary>
/// Generic domain exception for business rule violations
/// </summary>
public class GenericDomainException : DomainException
{
    public GenericDomainException(string message, string code = "DOMAIN_ERROR") 
        : base(message, code)
    {
    }
}