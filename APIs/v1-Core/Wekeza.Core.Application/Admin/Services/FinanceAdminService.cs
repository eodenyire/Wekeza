using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// FinanceAdminService Stub Implementation
/// Actual methods are in FinanceAdminService.Stub.cs
/// </summary>
public partial class FinanceAdminService : IFinanceAdminService
{
    private readonly ILogger<FinanceAdminService> _logger;

    public FinanceAdminService(ILogger<FinanceAdminService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
