using Microsoft.Extensions.Logging;
using System;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// CustomerServiceAdminService Stub Implementation
/// Actual methods are in CustomerServiceAdminService.Stub.cs
/// </summary>
public partial class CustomerServiceAdminService : ICustomerServiceAdminService
{
    private readonly ILogger<CustomerServiceAdminService> _logger;

    public CustomerServiceAdminService(ILogger<CustomerServiceAdminService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
