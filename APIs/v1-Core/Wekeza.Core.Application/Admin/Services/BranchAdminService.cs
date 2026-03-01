using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// BranchAdminService Stub Implementation
/// Actual methods are in BranchAdminService.Stub.cs
/// </summary>
public partial class BranchAdminService : IBranchAdminService
{
    private readonly ILogger<BranchAdminService> _logger;

    public BranchAdminService(ILogger<BranchAdminService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
