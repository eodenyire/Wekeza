namespace Wekeza.Core.Application.Common.Exceptions;

/// <summary>
/// 📂 Wekeza.Core.Application/Common/Exceptions/
/// 2. NotFoundException.cs (The Existence Guard)
/// When a user tries to fetch an account or a customer that isn't in our database, we throw this. It identifies exactly what was missing and which ID was used.
/// Thrown when a requested resource (Account, Customer, etc.) cannot be found.
/// </summary>
public class NotFoundException : Exception
{
    public string ResourceName { get; }
    public object Key { get; }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
        ResourceName = name;
        Key = key;
    }
}
