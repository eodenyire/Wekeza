using Wekeza.Core.Infrastructure.ApiGateway;

namespace Wekeza.Core.Infrastructure.Services;

public class ApiGatewayService : IApiGatewayService
{
    public Task<bool> IsHealthyAsync()
    {
        return Task.FromResult(true);
    }

    public Task<T?> ForwardRequestAsync<T>(string endpoint, object request)
    {
        // Implementation for forwarding requests through API gateway
        return Task.FromResult<T?>(default);
    }

    public Task<Dictionary<string, object>> GetGatewayMetricsAsync()
    {
        return Task.FromResult(new Dictionary<string, object>());
    }
}