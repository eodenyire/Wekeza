using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// SecurityAdminService Stub Implementation
/// Actual methods are in SecurityAdminService.Stub.cs
/// </summary>
public partial class SecurityAdminService : ISecurityAdminService
{
    private readonly ILogger<SecurityAdminService> _logger;

    public SecurityAdminService(ILogger<SecurityAdminService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
