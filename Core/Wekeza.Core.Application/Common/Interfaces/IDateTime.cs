namespace Wekeza.Core.Application.Common.Interfaces;

/// <summary>
/// 📂 Wekeza.Core.Application/Common/Interfaces/
/// 1. IDateTime.cs (The Global Chronometer)
/// In banking, time is everything—interest calculations, transaction timestamps, and audit logs depend on it. By using an interface, we can "freeze" time in our unit tests to verify interest accrual logic without waiting for a real clock.
/// Ensures a unified time source across the entire Wekeza ecosystem.
/// Prevents issues with disparate server times in distributed Docker clusters.
/// </summary>
public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
