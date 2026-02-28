using FluentValidation;
using System.Text.RegularExpressions;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Accounts.Commands.RegisterBusiness;
/// 2. The Gatekeeper: RegisterBusinessValidator.cs
/// We enforce Kenyan regulatory standards. A KRA PIN for a company usually starts with "P" or "A" and has a specific length. We check that here.
///

public class RegisterBusinessValidator : AbstractValidator<RegisterBusinessCommand>
{
    public RegisterBusinessValidator()
    {
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(200);
        
        RuleFor(x => x.RegistrationNumber)
            .NotEmpty()
            .WithMessage("Business Registration Number (BN/PVT) is required.");

        RuleFor(x => x.KraPin)
            .NotEmpty()
            .Matches(@"^[A-P]\d{9}[A-Z]$")
            .WithMessage("Invalid KRA PIN format for a business entity.");

        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        
        RuleFor(x => x.BusinessType)
            .Must(x => new[] { "SoleProp", "Partnership", "LLC", "NGO" }.Contains(x))
            .WithMessage("Invalid Business Type.");
    }
}
