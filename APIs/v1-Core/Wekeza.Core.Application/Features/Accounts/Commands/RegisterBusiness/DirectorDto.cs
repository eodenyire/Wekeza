///ðŸ“‚ Wekeza.Core.Application/Features/Accounts/Commands/RegisterBusiness/DirectorDto.cs
///This Data Transfer Object (DTO) is designed to be lean but exhaustive. It captures everything needed for a Tier-1 KYC (Know Your Customer) check.
///
///
namespace Wekeza.Core.Application.Features.Accounts.Commands.RegisterBusiness;

/// <summary>
/// Data contract for Business Directors/UBOs during onboarding.
/// This is used by the RegisterBusinessCommand to capture mandate holders.
/// </summary>
public record DirectorDto
{
    public string FullName { get; init; } = default!;
    
    // Kenya National ID or Passport Number
    public string IdNumber { get; init; } = default!;
    
    // Crucial for Risk: Is this director a Politically Exposed Person?
    public bool IsPep { get; init; }
    
    // For business accounts, we need to know the ownership stake
    public decimal ShareholdingPercentage { get; init; }
    
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
}
