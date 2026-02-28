using Wekeza.Core.Application.Features.CIF.Commands.PerformAMLScreening;
using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Application.Common.Interfaces;

/// <summary>
/// Service for AML/CFT screening
/// Integrates with external providers (Dow Jones, World-Check, etc.)
/// </summary>
public interface IAMLScreeningService
{
    /// <summary>
    /// Screen a party against sanctions lists, PEP databases, and adverse media
    /// </summary>
    Task<AMLScreeningResult> ScreenPartyAsync(
        Party party, 
        PerformAMLScreeningCommand request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a name appears on sanctions lists
    /// </summary>
    Task<bool> CheckSanctionsAsync(
        string name, 
        string? nationality = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a person is a Politically Exposed Person (PEP)
    /// </summary>
    Task<bool> CheckPEPAsync(
        string name, 
        string? nationality = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check for adverse media mentions
    /// </summary>
    Task<bool> CheckAdverseMediaAsync(
        string name, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Perform ongoing monitoring for existing parties
    /// </summary>
    Task PerformOngoingMonitoringAsync(CancellationToken cancellationToken = default);
}
