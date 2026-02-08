/// 📂 Wekeza.Core.Infrastructure/Services
/// 1. DateTimeService.cs (The Pulse)
/// We never use DateTime.Now inside our business logic. Why? Because it makes testing impossible and causes "clock drift" issues across distributed servers. We use an abstraction to ensure every transaction in the bank uses a synchronized, universal clock.
///
///
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Infrastructure.Services;

/// <summary>
/// Ensures a single, consistent source of time for the entire banking core.
/// </summary>
public class DateTimeService : IDateTime
{
    // We use UtcNow to avoid daylight savings or local timezone confusion
    public DateTime Now => DateTime.UtcNow;
    
    public DateTime UtcNow => DateTime.UtcNow;
    
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
