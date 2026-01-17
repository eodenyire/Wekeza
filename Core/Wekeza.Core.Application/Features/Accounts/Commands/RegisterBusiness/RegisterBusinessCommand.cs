using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Accounts.Commands.RegisterBusiness;

/// <summary>
/// 1. The Intent: RegisterBusinessCommand.cs
/// We extend our standard registration to include corporate-specific identifiers. This is future-proofed for the KRA iTax integration we might need in 2026.
/// Intent to onboard a Corporate/Business entity.
/// Captures legal identifiers required for Tier 2/3 KYC.
/// </summary>
public record RegisterBusinessCommand : ICommand<Guid>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string BusinessName { get; init; } = string.Empty;
    public string RegistrationNumber { get; init; } = string.Empty; // Certificate of Inc.
    public string KraPin { get; init; } = string.Empty;
    public string BusinessType { get; init; } = "LLC"; // Sole Prop, Partnership, LLC
    public string Email { get; init; } = string.Empty;
    public string Industry { get; init; } = string.Empty;
    public string PrimaryContactPerson { get; init; } = string.Empty;
}
