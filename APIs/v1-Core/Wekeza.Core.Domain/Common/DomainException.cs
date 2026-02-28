namespace Wekeza.Core.Domain.Common;
/// <summary>
/// 3. DomainException.cs
/// In Wekeza Bank, we distinguish between Technical Exceptions (database down, network timeout) and Domain Exceptions (insufficient funds, invalid currency). The latter are expected business outcomes that follow specific rules.
/// Base class for all business rule violations in Wekeza Bank.
/// Unlike technical exceptions, these represent a breach of banking logic.
/// </summary>
public abstract class DomainException : Exception
{
    public string Code { get; }

    protected DomainException(string message, string code) 
        : base(message)
    {
        Code = code;
    }
}
