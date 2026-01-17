using FluentValidation;
using MediatR;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Common.Behaviors;

/// <summary>
/// 📂 Wekeza.Core.Application/Common/Behaviors
/// These classes are Cross-Cutting Concerns. They sit between the API and the Business Logic. Every request entering the system must flow through them.
/// 🧱 2. ValidationBehavior.cs
/// Purpose: The "Bouncer." Prevents invalid data from ever reaching the Domain Layer. Future-Proofing: Runs multiple validators in parallel. If a request has 5 validation rules, it checks them all at once, maximizing throughput for our 2026 "Beast."
/// Automates data validation. If a command fails validation, 
/// the request is terminated before it reaches the database.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);

        // Execute all registered validators for the specific Request type
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            // Throws our custom exception which the API Global Exception Filter will catch
            throw new WekezaValidationException(failures);
        }

        return await next();
    }
}
