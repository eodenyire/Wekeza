# Wekeza Nexus Integration Guide

This guide shows you how to integrate Wekeza Nexus fraud detection into your existing Wekeza Bank payment flows.

## Table of Contents
1. [Quick Start](#quick-start)
2. [Step-by-Step Integration](#step-by-step-integration)
3. [API Integration](#api-integration)
4. [Client-Side Integration](#client-side-integration)
5. [Testing](#testing)
6. [Troubleshooting](#troubleshooting)

## Quick Start

### 1. Add Dependencies

Add the Nexus projects to your API project:

```xml
<!-- In your Wekeza.Core.Api.csproj or similar -->
<ItemGroup>
  <ProjectReference Include="..\Wekeza.Nexus.Application\Wekeza.Nexus.Application.csproj" />
  <ProjectReference Include="..\Wekeza.Nexus.Infrastructure\Wekeza.Nexus.Infrastructure.csproj" />
</ItemGroup>
```

### 2. Register Services

In `Program.cs`:

```csharp
using Wekeza.Nexus.Application;
using Wekeza.Nexus.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ... existing services ...

// Add Wekeza Nexus
builder.Services.AddWekezaNexus();
builder.Services.AddWekezaNexusInfrastructure();

var app = builder.Build();
```

### 3. Inject and Use

In your handler or service:

```csharp
using Wekeza.Nexus.Application.Services;
using Wekeza.Nexus.Application.Exceptions;
using Wekeza.Nexus.Domain.Enums;

public class YourPaymentHandler
{
    private readonly WekezaNexusClient _nexusClient;
    
    public YourPaymentHandler(WekezaNexusClient nexusClient)
    {
        _nexusClient = nexusClient;
    }
    
    public async Task ProcessPayment(PaymentRequest request)
    {
        // Evaluate fraud
        var verdict = await _nexusClient.EvaluateTransactionAsync(
            userId: request.UserId,
            fromAccountNumber: request.FromAccount,
            toAccountNumber: request.ToAccount,
            amount: request.Amount,
            currency: request.Currency
        );
        
        // Handle decision
        if (verdict.Decision == FraudDecision.Block)
        {
            throw new FraudDetectedException(verdict.Reason);
        }
        
        // Continue with payment...
    }
}
```

## Step-by-Step Integration

### Option 1: Integrate into Existing Handler (Recommended)

Update your existing `TransferFundsHandler.cs`:

```csharp
public class TransferFundsHandler : IRequestHandler<TransferFundsCommand, Guid>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TransferService _transferService;
    private readonly WekezaNexusClient _nexusClient; // ADD THIS
    
    public TransferFundsHandler(
        IAccountRepository accountRepository, 
        IUnitOfWork unitOfWork,
        TransferService transferService,
        WekezaNexusClient nexusClient) // ADD THIS
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _transferService = transferService;
        _nexusClient = nexusClient; // ADD THIS
    }
    
    public async Task<Guid> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
    {
        // ADD FRAUD CHECK BEFORE PROCESSING
        var verdict = await _nexusClient.EvaluateTransactionAsync(
            userId: request.UserId,
            fromAccountNumber: request.FromAccountNumber,
            toAccountNumber: request.ToAccountNumber,
            amount: request.Amount,
            currency: request.Currency,
            cancellationToken: cancellationToken
        );
        
        // ADD DECISION ENFORCEMENT
        if (verdict.Decision == FraudDecision.Block)
        {
            throw new FraudDetectedException(
                verdict.Reason,
                verdict.RiskScore,
                verdict.RiskLevel.ToString(),
                verdict.TransactionContextId
            );
        }
        
        if (verdict.Decision == FraudDecision.Challenge)
        {
            throw new StepUpAuthenticationRequiredException(
                verdict.Reason,
                verdict.TransactionContextId
            );
        }
        
        // EXISTING CODE CONTINUES...
        var sourceAccount = await _accountRepository.GetByAccountNumberAsync(...);
        var destinationAccount = await _accountRepository.GetByAccountNumberAsync(...);
        
        _transferService.Transfer(sourceAccount, destinationAccount, transferAmount);
        
        return request.CorrelationId;
    }
}
```

### Option 2: Use the Pre-Built Handler

Copy `TransferFundsHandlerWithNexus.cs` to replace your existing handler.

### Option 3: Use a Decorator Pattern

Create a fraud check decorator:

```csharp
public class FraudCheckDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IPaymentCommand, IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _innerHandler;
    private readonly WekezaNexusClient _nexusClient;
    
    public FraudCheckDecorator(
        IRequestHandler<TRequest, TResponse> innerHandler,
        WekezaNexusClient nexusClient)
    {
        _innerHandler = innerHandler;
        _nexusClient = nexusClient;
    }
    
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        // Check fraud first
        var verdict = await _nexusClient.EvaluateTransactionAsync(...);
        
        if (verdict.Decision == FraudDecision.Block)
            throw new FraudDetectedException(verdict.Reason);
        
        // Call inner handler
        return await _innerHandler.Handle(request, cancellationToken);
    }
}
```

## API Integration

### Update Your Command/Request Models

Add fraud detection properties to your commands:

```csharp
public record TransferFundsCommand : ICommand<Guid>
{
    // Existing properties
    public string FromAccountNumber { get; init; }
    public string ToAccountNumber { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    
    // NEW: Add these for fraud detection
    public Guid UserId { get; init; }
    public DeviceFingerprint? DeviceInfo { get; init; }
    public BehavioralMetrics? BehavioralData { get; init; }
    public string? Channel { get; init; }
    public string? SessionId { get; init; }
}
```

### Update Your API Controllers

Add fraud context to your API requests:

```csharp
[HttpPost("transfer")]
public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
{
    try
    {
        var command = new TransferFundsCommand
        {
            FromAccountNumber = request.FromAccount,
            ToAccountNumber = request.ToAccount,
            Amount = request.Amount,
            Currency = request.Currency,
            
            // Fraud detection context
            UserId = User.GetUserId(),
            DeviceInfo = ExtractDeviceInfo(Request),
            Channel = "Web",
            SessionId = HttpContext.Session.Id
        };
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    catch (FraudDetectedException ex)
    {
        return StatusCode(403, new
        {
            Error = "Transaction blocked due to fraud detection",
            Reason = ex.Message,
            RiskScore = ex.RiskScore,
            TransactionId = ex.TransactionContextId
        });
    }
    catch (StepUpAuthenticationRequiredException ex)
    {
        return StatusCode(428, new // 428 Precondition Required
        {
            Error = "Additional authentication required",
            Reason = ex.Message,
            ChallengeType = ex.ChallengeType,
            TransactionId = ex.TransactionContextId
        });
    }
}
```

### Helper Method for Device Info

```csharp
private DeviceFingerprint ExtractDeviceInfo(HttpRequest request)
{
    return new DeviceFingerprint
    {
        IpAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
        UserAgent = request.Headers["User-Agent"].ToString(),
        DeviceId = request.Headers["X-Device-Id"].ToString(),
        IsRecognizedDevice = IsRecognizedDevice(request),
        Location = GetLocationFromIP(request.HttpContext.Connection.RemoteIpAddress),
        CapturedAt = DateTime.UtcNow
    };
}
```

## Client-Side Integration

### JavaScript/TypeScript SDK (Future)

A full behavioral biometrics SDK is planned for Phase 2. For now, you can send basic device info:

```javascript
// In your frontend
async function submitTransfer(transferData) {
    const deviceInfo = {
        deviceId: getDeviceId(),
        deviceType: getMobileDeviceType(),
        operatingSystem: getOS(),
        browser: getBrowser(),
        screenResolution: `${screen.width}x${screen.height}`,
        userAgent: navigator.userAgent
    };
    
    const response = await fetch('/api/transfers', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            ...transferData,
            deviceInfo: deviceInfo
        })
    });
    
    if (response.status === 403) {
        // Transaction blocked
        const error = await response.json();
        alert(`Transaction blocked: ${error.Reason}`);
    } else if (response.status === 428) {
        // Step-up auth required
        const challenge = await response.json();
        await showOTPChallenge(challenge.TransactionId);
    }
}
```

### Step-Up Authentication Flow

```javascript
async function showOTPChallenge(transactionId) {
    const otp = await promptUserForOTP();
    
    const response = await fetch('/api/nexus/challenge', {
        method: 'POST',
        body: JSON.stringify({
            transactionId: transactionId,
            otp: otp
        })
    });
    
    if (response.ok) {
        // Re-submit transaction
        await submitTransfer(originalTransferData);
    }
}
```

## Testing

### Unit Testing with Nexus

```csharp
[Fact]
public async Task Handle_ShouldBlockTransaction_WhenHighRisk()
{
    // Arrange
    var mockNexusClient = new Mock<WekezaNexusClient>();
    mockNexusClient
        .Setup(x => x.EvaluateTransactionAsync(It.IsAny<Guid>(), ...))
        .ReturnsAsync(new NexusVerdict
        {
            Decision = FraudDecision.Block,
            RiskScore = 950,
            Reason = "High velocity detected"
        });
    
    var handler = new TransferFundsHandler(..., mockNexusClient.Object);
    var command = new TransferFundsCommand { Amount = 100000, ... };
    
    // Act & Assert
    await Assert.ThrowsAsync<FraudDetectedException>(
        () => handler.Handle(command, CancellationToken.None)
    );
}
```

### Integration Testing

```csharp
[Fact]
public async Task TransferFunds_ShouldSucceed_WhenLowRisk()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new TransferRequest
    {
        FromAccount = "ACC001",
        ToAccount = "ACC002",
        Amount = 100,
        DeviceInfo = new DeviceFingerprint { IsRecognizedDevice = true }
    };
    
    // Act
    var response = await client.PostAsJsonAsync("/api/transfers", request);
    
    // Assert
    response.EnsureSuccessStatusCode();
}
```

## Troubleshooting

### Issue: Nexus services not found

**Solution**: Make sure you've registered both application and infrastructure services:

```csharp
builder.Services.AddWekezaNexus();
builder.Services.AddWekezaNexusInfrastructure();
```

### Issue: All transactions are being blocked

**Solution**: Check that velocity service is returning reasonable values. The stub implementation returns 0 for all metrics, which should result in low risk scores.

### Issue: Performance degradation

**Solution**: 
1. Ensure fraud checks are happening in parallel with database queries where possible
2. Consider caching user velocity metrics in Redis
3. Monitor fraud evaluation latency - target is < 150ms

### Issue: False positives

**Solution**:
1. Adjust scoring thresholds in `FraudEvaluationService.cs`
2. Fine-tune individual scoring engine weights
3. Implement machine learning model training based on actual fraud data

## Performance Optimization

### Recommended Optimizations for Production

1. **Replace In-Memory Repository**: Use Entity Framework Core with PostgreSQL
2. **Add Redis Caching**: Cache velocity metrics for 10-minute windows
3. **Async Processing**: Use background jobs for non-blocking fraud analysis
4. **Connection Pooling**: Properly configure database connection pools
5. **Monitoring**: Add Application Insights or similar for performance tracking

### Example Redis Integration

```csharp
public class RedisCachedVelocityService : ITransactionVelocityService
{
    private readonly IDistributedCache _cache;
    private readonly ITransactionRepository _repository;
    
    public async Task<int> GetTransactionCountAsync(Guid userId, int minutes, ...)
    {
        var cacheKey = $"velocity:{userId}:{minutes}min";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (cached != null)
            return int.Parse(cached);
        
        var count = await _repository.GetTransactionCountAsync(userId, minutes);
        await _cache.SetStringAsync(cacheKey, count.ToString(), 
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) });
        
        return count;
    }
}
```

## Next Steps

1. **Deploy to staging**: Test with real transaction patterns
2. **Monitor metrics**: Track false positive rate, true positive rate, latency
3. **Tune thresholds**: Adjust based on your bank's risk tolerance
4. **Implement Phase 2**: Add behavioral biometrics SDK, graph database
5. **Train ML models**: Use actual fraud data to improve accuracy

## Support

For questions or issues:
- Documentation: See `Core/WEKEZA-NEXUS-README.md`
- GitHub Issues: https://github.com/eodenyire/Wekeza/issues
- Email: nexus-support@wekeza.com

---

**Last Updated**: January 2026
**Version**: 1.0.0 (MVP)
