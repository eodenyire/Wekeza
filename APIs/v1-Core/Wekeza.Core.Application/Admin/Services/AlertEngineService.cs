using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// AlertEngineService Stub Implementation
/// Actual methods are in AlertEngineService.Stub.cs
/// </summary>
public partial class AlertEngineService : IAlertEngineService
{
    private readonly ILogger<AlertEngineService> _logger;

    public AlertEngineService(ILogger<AlertEngineService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
