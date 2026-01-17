///2. ValidationException.cs (The Data Gatekeeper)
///This exception is specifically for FluentValidation. When the ValidationBehavior catches a request with bad formatting (e.g., a phone number with letters), it packages all the failures into this exception so the Mobile App can show the user exactly which fields to fix.
///
///
using FluentValidation.Results;

namespace Wekeza.Core.Application.Common.Exceptions;

/// <summary>
/// Thrown when input data fails the structural validation rules.
/// Contains a dictionary of property names and their corresponding error messages.
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
