using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// RiskManagementService Stub Implementation
/// Actual methods are in RiskManagementService.Stub.cs
/// </summary>
public partial class RiskManagementService : IRiskManagementService
{
    private readonly ILogger<RiskManagementService> _logger;

    public RiskManagementService(ILogger<RiskManagementService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
