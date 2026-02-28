namespace Wekeza.Core.Application.Common.Exceptions;

/// <summary>
/// ðŸ“‚ Wekeza.Core.Application/Common/Exceptions/
/// 3. ForbiddenAccessException.cs (The Security Guard)
/// If a user tries to access a branch's data they don't belong to, or a customer tries to view another customer's statement, this is the hammer. It's a key part of our Model Risk and Security protocol.
/// Thrown when a user attempts to perform an action they are not authorized for.
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("You do not have permission to access this resource.") { }
    
    public ForbiddenAccessException(string message) : base(message) { }
}
