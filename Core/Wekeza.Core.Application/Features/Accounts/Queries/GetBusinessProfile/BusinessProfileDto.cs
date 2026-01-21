namespace Wekeza.Core.Application.Features.Accounts.Queries.GetBusinessProfile;

///📂 Phase 9: Detailed Business Onboarding DTOs
/// We mentioned we "jumped" the specific Business DTOs. Here they are, designed for UBO (Ultimate Beneficial Ownership) visibility.
/// 1. 📂 Features/Accounts/Queries/GetBusinessProfile/
/// BusinessProfileDto.cs This returns the full "KYC Jacket" for a business, including directors and their shareholding percentages.
///
///

public record BusinessProfileDto
{
    public string BusinessName { get; init; } = string.Empty;
    public string RegistrationNumber { get; init; } = string.Empty;
    public string KraPin { get; init; } = string.Empty;
    public List<DirectorDetailsDto> Directors { get; init; } = new();
    public List<AccountSummaryDto> LinkedAccounts { get; init; } = new();
}

public record DirectorDetailsDto
{
    public string Name { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public bool IsVerified { get; init; }
    public DateTime AppointedDate { get; init; }
}