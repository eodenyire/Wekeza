using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Exceptions;

/// <summary>
/// Exception thrown when workflow operations fail
/// </summary>
public class WorkflowException : DomainException
{
    public WorkflowException(string message, string code = "WORKFLOW_ERROR") 
        : base(message, code)
    {
    }
}