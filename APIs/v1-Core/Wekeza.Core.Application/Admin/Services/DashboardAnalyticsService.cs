using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// DashboardAnalyticsService Stub Implementation
/// Actual methods are in DashboardAnalyticsService.Stub.cs
/// </summary>
public partial class DashboardAnalyticsService : IDashboardAnalyticsService
{
    private readonly ILogger<DashboardAnalyticsService> _logger;

    public DashboardAnalyticsService(ILogger<DashboardAnalyticsService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
