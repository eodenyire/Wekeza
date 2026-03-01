using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// GlobalSearchService Stub Implementation
/// Actual methods are in GlobalSearchService.Stub.cs
/// </summary>
public partial class GlobalSearchService
{
    private readonly ILogger<GlobalSearchService> _logger;

    public GlobalSearchService(ILogger<GlobalSearchService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
