using Microsoft.Extensions.Diagnostics.HealthChecks;
using Wekeza.Core.Infrastructure.ApiGateway;

namespace Wekeza.Core.Infrastructure.HealthChecks;

public class ApiGatewayHealthCheck : IHealthCheck
{
    private readonly IApiGatewayService _apiGatewayService;

    public ApiGatewayHealthCheck(IApiGatewayService apiGatewayService)
    {
        _apiGatewayService = apiGatewayService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var isHealthy = await _apiGatewayService.IsHealthyAsync();
            return isHealthy 
                ? HealthCheckResult.Healthy("API Gateway is healthy")
                : HealthCheckResult.Unhealthy("API Gateway is unhealthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("API Gateway is unhealthy", ex);
        }
    }
}