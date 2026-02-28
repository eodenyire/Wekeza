using FluentValidation.Results;

namespace Wekeza.Core.Application.Common.Exceptions;

/// <summary>
/// 📂 Wekeza.Core.Application/Common/Exceptions/
/// 1. WekezaValidationException.cs (The Data Guard)
/// This exception works hand-in-hand with our ValidationBehavior. It doesn't just say "Something is wrong"; it provides a structured dictionary of every single field that failed and why. This is vital for a billion-dollar fintech's UX.
/// Principal-Grade Validation Exception.
/// Encapsulates multiple validation failures into a structured format for API consumption.
/// </summary>
public class WekezaValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public WekezaValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public WekezaValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(
                failureGroup => failureGroup.Key, 
                failureGroup => failureGroup.ToArray()
            );
    }
}
