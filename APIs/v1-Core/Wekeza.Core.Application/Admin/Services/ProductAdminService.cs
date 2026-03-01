using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// ProductAdminService Stub Implementation
/// Actual methods are in ProductAdminService.Stub.cs
/// </summary>
public partial class ProductAdminService : IProductAdminService
{
    private readonly ILogger<ProductAdminService> _logger;

    public ProductAdminService(ILogger<ProductAdminService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
