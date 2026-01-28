# WekezaERMS Integration Guide

## Overview

This guide explains how to integrate the Wekeza Enterprise Risk Management System (ERMS) with the Wekeza Core Banking System. The integration enables automated risk monitoring, data synchronization, and real-time risk assessment based on banking operations.

---

## Table of Contents

1. [Integration Architecture](#integration-architecture)
2. [Integration Points](#integration-points)
3. [Authentication & Authorization](#authentication--authorization)
4. [Data Synchronization](#data-synchronization)
5. [Real-Time Risk Monitoring](#real-time-risk-monitoring)
6. [API Integration Examples](#api-integration-examples)
7. [Event-Driven Integration](#event-driven-integration)
8. [Error Handling](#error-handling)
9. [Performance Optimization](#performance-optimization)
10. [Security Considerations](#security-considerations)

---

## Integration Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                   Wekeza Core Banking System                │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │   CIF    │  │  Loans   │  │ Accounts │  │ Payments │   │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘   │
│       │             │              │             │          │
└───────┼─────────────┼──────────────┼─────────────┼──────────┘
        │             │              │             │
        │   ┌─────────▼──────────────▼─────────────▼─────┐
        │   │     Integration Layer (API Gateway)        │
        │   │  - Authentication  - Data Transformation   │
        │   │  - Rate Limiting   - Event Bus             │
        │   └────────────────┬───────────────────────────┘
        │                    │
┌───────▼────────────────────▼───────────────────────────────┐
│            Wekeza Enterprise Risk Management System        │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  │
│  │   Risk   │  │ Controls │  │   KRIs   │  │ Reports  │  │
│  │ Register │  │ Mgmt     │  │ Monitor  │  │ Dashboard│  │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘  │
└────────────────────────────────────────────────────────────┘
```

### Integration Patterns

1. **Real-Time Integration**: Event-driven updates for immediate risk assessment
2. **Batch Integration**: Scheduled synchronization for bulk data updates
3. **On-Demand Integration**: API calls triggered by user actions

---

## Integration Points

### 1. Credit Risk Integration

**Data Flow**: Wekeza Loans → ERMS Risk Assessment

**Key Metrics:**
- Loan portfolio concentration
- Non-performing loan ratios
- Credit exposure limits
- Collateral valuations

**Implementation:**

```csharp
// Example: Monitor credit concentration risk
public class CreditRiskIntegrationService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IRiskRepository _riskRepository;
    
    public async Task SyncCreditRisks()
    {
        // Get loan portfolio data from Wekeza Core
        var loans = await _loanRepository.GetAllActiveLoans();
        
        // Calculate concentration by sector
        var sectorConcentration = loans
            .GroupBy(l => l.Industry)
            .Select(g => new {
                Sector = g.Key,
                TotalExposure = g.Sum(l => l.OutstandingBalance),
                Percentage = (g.Sum(l => l.OutstandingBalance) / loans.Sum(l => l.OutstandingBalance)) * 100
            });
        
        // Update KRI for concentration risk
        var concentrationKRI = await _riskRepository.GetKRIByName("Credit Concentration Ratio");
        var maxConcentration = sectorConcentration.Max(s => s.Percentage);
        
        await concentrationKRI.RecordMeasurement(
            maxConcentration,
            $"Highest sector concentration: {sectorConcentration.First().Sector}",
            SystemUserId
        );
        
        // Check if threshold breached
        if (maxConcentration > concentrationKRI.ThresholdCritical)
        {
            // Create or update risk
            await CreateOrUpdateConcentrationRisk(sectorConcentration);
        }
    }
}
```

### 2. Operational Risk Integration

**Data Flow**: Wekeza Transactions/Operations → ERMS Risk Monitoring

**Key Metrics:**
- Transaction failure rates
- System downtime
- Process exceptions
- Fraud incidents

**KRI Example:**

```json
{
  "name": "Transaction Failure Rate",
  "description": "Percentage of failed transactions",
  "measurementUnit": "Percentage",
  "thresholdWarning": 2.0,
  "thresholdCritical": 5.0,
  "frequency": "Daily",
  "dataSource": "Wekeza Core - Transaction Processing"
}
```

### 3. Compliance Risk Integration

**Data Flow**: Wekeza AML/Sanctions → ERMS Compliance Monitoring

**Integration:**

```csharp
public class ComplianceRiskIntegrationService
{
    public async Task SyncComplianceRisks()
    {
        // Get AML cases from Wekeza Core
        var amlCases = await _amlRepository.GetOpenCases();
        
        // Update KRI for AML case volume
        var amlKRI = await _riskRepository.GetKRIByName("Open AML Cases");
        await amlKRI.RecordMeasurement(
            amlCases.Count,
            $"{amlCases.Count(c => c.Severity == "High")} high-severity cases",
            SystemUserId
        );
        
        // Get sanctions screening results
        var sanctionsMatches = await _sanctionsRepository.GetPendingReviews();
        
        if (sanctionsMatches.Any())
        {
            // Create operational risk for sanctions review backlog
            await CreateSanctionsBacklogRisk(sanctionsMatches.Count);
        }
    }
}
```

### 4. Liquidity Risk Integration

**Data Flow**: Wekeza Accounts/Treasury → ERMS Liquidity Monitoring

**Key Metrics:**
- Liquidity Coverage Ratio (LCR)
- Net Stable Funding Ratio (NSFR)
- Cash flow projections
- Large withdrawal trends

### 5. Market Risk Integration

**Data Flow**: Wekeza Treasury/FX → ERMS Market Risk Assessment

**Key Metrics:**
- Foreign exchange exposure
- Interest rate risk
- Trading book valuations

---

## Authentication & Authorization

### JWT Token Integration

ERMS uses the same JWT authentication as Wekeza Core:

```csharp
// Configure JWT validation in ERMS
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Configuration["JwtSettings:Issuer"],
            ValidAudience = Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["JwtSettings:Secret"]))
        };
    });
```

### Role-Based Access Control

Map Wekeza Core roles to ERMS roles:

| Wekeza Core Role | ERMS Role | Permissions |
|------------------|-----------|-------------|
| Administrator | RiskManager | Full access to ERMS |
| RiskOfficer | RiskOfficer | Manage assigned risks |
| LoanOfficer | RiskViewer | View credit risks |
| ComplianceOfficer | RiskOfficer | Manage compliance risks |
| Auditor | Auditor | Read-only with audit trail |
| Executive | Executive | Dashboard and reports |

---

## Data Synchronization

### Scheduled Synchronization

Configure automatic data sync:

```json
{
  "SyncSchedule": {
    "CreditRisk": "0 */6 * * *",  // Every 6 hours
    "OperationalRisk": "0 * * * *",  // Every hour
    "ComplianceRisk": "0 */12 * * *",  // Every 12 hours
    "KRIMeasurements": "0 0 * * *"  // Daily at midnight
  }
}
```

### Sync Implementation

```csharp
public class DataSyncService
{
    private readonly IWekezaCoreApiClient _coreClient;
    private readonly IRiskRepository _riskRepo;
    
    public async Task<SyncResult> SyncAll()
    {
        var result = new SyncResult();
        
        try
        {
            // Sync credit data
            result.CreditSync = await SyncCreditData();
            
            // Sync operational data
            result.OperationalSync = await SyncOperationalData();
            
            // Sync compliance data
            result.ComplianceSync = await SyncComplianceData();
            
            // Update KRIs
            result.KRIUpdate = await UpdateAllKRIs();
            
            result.Success = true;
            result.SyncedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Error = ex.Message;
            _logger.LogError(ex, "Data synchronization failed");
        }
        
        return result;
    }
}
```

---

## Real-Time Risk Monitoring

### Event-Driven Architecture

Use domain events from Wekeza Core to trigger real-time risk assessment:

```csharp
// Subscribe to Wekeza Core events
public class RiskMonitoringEventHandler : 
    INotificationHandler<LoanDisbursedEvent>,
    INotificationHandler<TransactionFailedEvent>,
    INotificationHandler<AMLAlertGeneratedEvent>
{
    public async Task Handle(LoanDisbursedEvent notification, CancellationToken cancellationToken)
    {
        // Check if loan disbursement increases concentration risk
        await _riskAssessmentService.AssessCreditConcentration(notification.LoanId);
    }
    
    public async Task Handle(TransactionFailedEvent notification, CancellationToken cancellationToken)
    {
        // Update operational risk KRI
        await _kriService.RecordTransactionFailure(notification.TransactionId);
    }
    
    public async Task Handle(AMLAlertGeneratedEvent notification, CancellationToken cancellationToken)
    {
        // Assess compliance risk
        await _riskAssessmentService.AssessComplianceRisk(notification.AlertId);
    }
}
```

### Message Queue Integration

Use RabbitMQ or Azure Service Bus for event messaging:

```csharp
// Configure message queue consumer
public class RiskEventConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.QueueDeclare(
            queue: "risk-events",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            await ProcessRiskEvent(message);
            
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        
        channel.BasicConsume(queue: "risk-events", autoAck: false, consumer: consumer);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
```

---

## API Integration Examples

### Example 1: Get Loan Portfolio Risk

```http
GET /api/integration/credit/portfolio-risk
Authorization: Bearer {jwt_token}
```

**Response:**
```json
{
  "totalExposure": 150000000.00,
  "riskWeightedAssets": 120000000.00,
  "concentrationRisks": [
    {
      "sector": "Real Estate",
      "exposure": 45000000.00,
      "percentage": 30.0,
      "riskLevel": "High"
    }
  ],
  "nplRatio": 3.5,
  "provisionCoverage": 85.0
}
```

### Example 2: Record Operational Incident

```http
POST /api/integration/operational/incident
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "incidentType": "SystemOutage",
  "severity": "High",
  "affectedSystem": "Core Banking System",
  "downtime Minutes": 120,
  "financialImpact": 50000.00,
  "description": "Database connection failure causing system outage"
}
```

### Example 3: Sync Compliance Data

```http
POST /api/integration/compliance/sync
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "amlCases": [
    {
      "caseId": "AML-2024-001",
      "status": "UnderInvestigation",
      "riskScore": 85,
      "assignedTo": "compliance-officer-id"
    }
  ],
  "sanctionsScreenings": [
    {
      "screeningId": "SCR-2024-001",
      "matchCount": 2,
      "status": "PendingReview"
    }
  ]
}
```

---

## Event-Driven Integration

### Event Types

1. **Credit Events**
   - LoanDisbursed
   - LoanDefaulted
   - CreditLimitExceeded
   - CollateralValueChanged

2. **Operational Events**
   - TransactionFailed
   - SystemOutage
   - ProcessException
   - FraudDetected

3. **Compliance Events**
   - AMLAlertGenerated
   - SanctionsMatchFound
   - RegulatoryReportDue
   - PolicyViolation

### Event Handler Example

```csharp
public class CreditEventHandler
{
    [EventHandler("LoanDisbursed")]
    public async Task HandleLoanDisbursed(LoanDisbursedEvent @event)
    {
        // Update credit exposure
        await _riskService.UpdateCreditExposure(@event.CustomerId, @event.Amount);
        
        // Check concentration limits
        var concentration = await _riskService.CalculateSectorConcentration(@event.Industry);
        
        if (concentration > 65.0m)
        {
            // Trigger concentration risk alert
            await _alertService.SendConcentrationAlert(concentration, @event.Industry);
        }
        
        // Update KRI
        var kri = await _kriService.GetByName("Credit Concentration Ratio");
        await kri.RecordMeasurement(concentration, $"Loan {event.LoanId} disbursed");
    }
}
```

---

## Error Handling

### Retry Policy

Implement exponential backoff for failed integrations:

```csharp
public class RetryPolicy
{
    public static async Task<T> ExecuteWithRetry<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        int delayMilliseconds = 1000)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                _logger.LogWarning($"Attempt {i + 1} failed: {ex.Message}");
                await Task.Delay(delayMilliseconds * (int)Math.Pow(2, i));
            }
        }
        
        // Final attempt without catching
        return await operation();
    }
}
```

### Circuit Breaker

Prevent cascading failures:

```csharp
public class CircuitBreakerService
{
    private int _failureCount = 0;
    private DateTime _lastFailureTime;
    private const int FailureThreshold = 5;
    private const int CircuitBreakerTimeout = 60; // seconds
    
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        if (_failureCount >= FailureThreshold)
        {
            if ((DateTime.UtcNow - _lastFailureTime).TotalSeconds < CircuitBreakerTimeout)
            {
                throw new CircuitBreakerOpenException("Circuit breaker is open");
            }
            
            // Reset circuit breaker
            _failureCount = 0;
        }
        
        try
        {
            var result = await operation();
            _failureCount = 0; // Reset on success
            return result;
        }
        catch (Exception)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;
            throw;
        }
    }
}
```

---

## Performance Optimization

### Caching Strategy

Cache frequently accessed data:

```csharp
public class CachedRiskDataService
{
    private readonly IDistributedCache _cache;
    private readonly IRiskRepository _repository;
    
    public async Task<Risk> GetRiskAsync(Guid riskId)
    {
        var cacheKey = $"risk:{riskId}";
        var cachedRisk = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedRisk))
        {
            return JsonSerializer.Deserialize<Risk>(cachedRisk);
        }
        
        var risk = await _repository.GetByIdAsync(riskId);
        
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(risk),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });
        
        return risk;
    }
}
```

### Batch Processing

Process large datasets efficiently:

```csharp
public async Task BatchUpdateKRIs()
{
    const int batchSize = 100;
    var kriIds = await _repository.GetAllKRIIds();
    
    for (int i = 0; i < kriIds.Count; i += batchSize)
    {
        var batch = kriIds.Skip(i).Take(batchSize);
        
        var tasks = batch.Select(async id =>
        {
            var kri = await _repository.GetKRIByIdAsync(id);
            await UpdateKRIFromDataSource(kri);
        });
        
        await Task.WhenAll(tasks);
    }
}
```

---

## Security Considerations

### 1. Data Encryption

Encrypt sensitive data in transit and at rest:

```csharp
// Configure HTTPS
services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    options.HttpsPort = 443;
});

// Encrypt sensitive fields
public class EncryptedRiskData
{
    [Encrypted]
    public string SensitiveInformation { get; set; }
}
```

### 2. API Security

Implement rate limiting and IP whitelisting:

```csharp
services.AddRateLimiting(options =>
{
    options.GlobalRules.Add(new RateLimitRule
    {
        Endpoint = "*",
        Period = "1m",
        Limit = 100
    });
});
```

### 3. Audit Logging

Log all integration activities:

```csharp
public class IntegrationAuditLogger
{
    public async Task LogIntegration(
        string operation,
        string source,
        string userId,
        bool success,
        string details)
    {
        await _auditRepository.CreateAsync(new IntegrationAuditLog
        {
            Id = Guid.NewGuid(),
            Operation = operation,
            Source = source,
            UserId = userId,
            Success = success,
            Details = details,
            Timestamp = DateTime.UtcNow,
            IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
        });
    }
}
```

---

## Testing Integration

### Integration Tests

```csharp
[Fact]
public async Task Should_Sync_Credit_Risk_Data_Successfully()
{
    // Arrange
    var mockLoanData = CreateMockLoanData();
    _loanRepositoryMock.Setup(x => x.GetAllActiveLoans())
        .ReturnsAsync(mockLoanData);
    
    // Act
    var result = await _integrationService.SyncCreditRisks();
    
    // Assert
    Assert.True(result.Success);
    Assert.NotNull(result.SyncedRisks);
    Assert.True(result.SyncedRisks.Count > 0);
}
```

---

## Support

For integration support:
- Email: integration@wekeza.com
- Documentation: https://docs.wekeza.com/erms/integration
- API Status: https://status.wekeza.com

---

## Appendix

### Integration Checklist

- [ ] Wekeza Core API access configured
- [ ] JWT authentication working
- [ ] Database connections established
- [ ] Event bus configured
- [ ] Sync schedules defined
- [ ] Error handling implemented
- [ ] Monitoring enabled
- [ ] Security measures in place
- [ ] Integration tests passing
- [ ] Documentation updated
