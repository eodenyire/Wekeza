# Wekeza Nexus - Advanced Fraud Detection System

## Overview

**Wekeza Nexus** is an advanced, real-time fraud detection and prevention system designed specifically for Wekeza Bank. It represents a significant leap beyond traditional rule-based fraud systems by combining:

- **Behavioral Biometrics** (inspired by BioCatch & ThreatMark)
- **Transaction Velocity & Pattern Analysis** (inspired by Feedzai)
- **Graph-Based Relationship Detection** (inspired by NICE Actimize)
- **Entity-Centric Risk Scoring** (inspired by RembrandtAi & SAS)

## The "Big Tech" Value Proposition

Wekeza Nexus doesn't just ask "Is this transaction fraudulent?" It asks: **"Is this sequence of actions consistent with the entity's evolving life story?"**

### Key Differentiators

1. **Entity-First Analytics**: We score users, not just transactions
2. **Real-Time Context**: Sub-150ms fraud evaluation
3. **Ensemble Scoring**: Combines 5 independent fraud signals
4. **Explainable AI**: Human-readable fraud explanations for investigators
5. **Challenge-Based Friction**: Smart step-up authentication only when needed

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Transaction Request                       │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│              WekezaNexusClient (The Hook)                    │
│  • Captures device fingerprint                               │
│  • Captures behavioral metrics                               │
│  • Builds transaction context                                │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│           FraudEvaluationService (The Deep Brain)            │
│                                                              │
│  Parallel Scoring Engines:                                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Velocity   │  │  Behavioral  │  │ Relationship │      │
│  │   30% weight │  │  25% weight  │  │  25% weight  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│  ┌──────────────┐  ┌──────────────┐                         │
│  │    Amount    │  │    Device    │                         │
│  │   15% weight │  │   5% weight  │                         │
│  └──────────────┘  └──────────────┘                         │
│                                                              │
│  Outputs: FraudScore (0-1000) + Decision + Explanation      │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                    Decision Engine                           │
│                                                              │
│  Score 0-400:    ✅ ALLOW      (Proceed normally)           │
│  Score 401-700:  ⚠️  CHALLENGE  (Step-up auth required)     │
│  Score 701-850:  🔍 REVIEW     (Flag for analyst)           │
│  Score 851-1000: 🚫 BLOCK      (Reject transaction)         │
└─────────────────────────────────────────────────────────────┘
```

## Fraud Detection Signals

### 1. Velocity Scoring (30% Weight)

Detects rapid-fire fraud attempts:

- Transaction count in last 10 minutes
- Transaction amount in last 10 minutes
- Daily transaction patterns
- Historical velocity baselines

**Flags**:
- 5+ transactions in 10 minutes → +300 risk points
- Amount velocity 10x normal → +250 risk points
- 20+ daily transactions → +200 risk points

### 2. Behavioral Biometrics (25% Weight)

Detects social engineering and account takeover:

- Active phone call during transaction → +400 risk points (Grandparent scam indicator)
- Screen sharing active → +350 risk points
- Behavioral anomaly score > 0.7 → +300 risk points
- Session duration < 5 seconds → +200 risk points (automation)
- Excessive copy-paste (>3) → +150 risk points

### 3. Relationship Analysis (25% Weight)

Detects mule accounts and fraud rings:

- First-time beneficiary → +200 risk points
- Beneficiary account < 7 days old → +350 risk points (Mule account)
- Circular transaction pattern detected (A→B→C→A) → +400 risk points

### 4. Amount Anomalies (15% Weight)

Detects unusual transaction amounts:

- 5x average transaction amount → +300 risk points
- 2x average transaction amount → +150 risk points
- Round amount pattern (e.g., 100,000 exactly) → +100 risk points
- Very large amount (>1,000,000) → +200 risk points

### 5. Device Intelligence (5% Weight)

Detects device-based risks:

- Unrecognized device → +150 risk points
- VPN/Proxy usage → +100 risk points
- Unknown location → +100 risk points

## Integration with Existing Payment Flow

### Before (Without Nexus)

```csharp
public async Task<Guid> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
{
    var sourceAccount = await _accountRepository.GetByAccountNumberAsync(
        new AccountNumber(request.FromAccountNumber), cancellationToken);
    
    var destinationAccount = await _accountRepository.GetByAccountNumberAsync(
        new AccountNumber(request.ToAccountNumber), cancellationToken);
    
    var transferAmount = new Money(request.Amount, Currency.FromCode(request.Currency));
    
    _transferService.Transfer(sourceAccount, destinationAccount, transferAmount);
    
    return request.CorrelationId;
}
```

### After (With Nexus Integration)

```csharp
public async Task<Guid> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
{
    // 1. Evaluate fraud risk BEFORE processing
    var verdict = await _nexusClient.EvaluateTransactionAsync(
        userId: request.UserId,
        fromAccountNumber: request.FromAccountNumber,
        toAccountNumber: request.ToAccountNumber,
        amount: request.Amount,
        currency: request.Currency,
        deviceInfo: request.DeviceInfo,
        behavioralData: request.BehavioralData,
        cancellationToken: cancellationToken
    );
    
    // 2. Enforce decision
    if (verdict.Decision == FraudDecision.Block)
    {
        throw new FraudDetectedException(
            verdict.Reason, 
            verdict.RiskScore, 
            verdict.RiskLevel.ToString(),
            verdict.TransactionContextId);
    }
    
    if (verdict.Decision == FraudDecision.Challenge)
    {
        throw new StepUpAuthenticationRequiredException(
            verdict.Reason,
            verdict.TransactionContextId,
            "OTP"); // Trigger OTP/Biometric challenge
    }
    
    // 3. Proceed with transaction if safe
    var sourceAccount = await _accountRepository.GetByAccountNumberAsync(...);
    var destinationAccount = await _accountRepository.GetByAccountNumberAsync(...);
    
    _transferService.Transfer(sourceAccount, destinationAccount, transferAmount);
    
    return request.CorrelationId;
}
```

## Usage Examples

### Basic Fraud Check

```csharp
// Inject WekezaNexusClient in your service
public class PaymentService
{
    private readonly WekezaNexusClient _nexusClient;
    
    public PaymentService(WekezaNexusClient nexusClient)
    {
        _nexusClient = nexusClient;
    }
    
    public async Task ProcessPayment(PaymentRequest payment)
    {
        // Evaluate fraud
        var verdict = await _nexusClient.EvaluateTransactionAsync(
            userId: payment.UserId,
            fromAccountNumber: payment.FromAccount,
            toAccountNumber: payment.ToAccount,
            amount: payment.Amount,
            currency: payment.Currency
        );
        
        // Handle result
        switch (verdict.Decision)
        {
            case FraudDecision.Allow:
                // Process normally
                break;
                
            case FraudDecision.Challenge:
                // Trigger step-up authentication
                await TriggerOTP(payment.UserId);
                break;
                
            case FraudDecision.Block:
                // Reject transaction
                throw new FraudDetectedException(verdict.Reason);
                
            case FraudDecision.Review:
                // Allow but flag for manual review
                await FlagForReview(verdict.TransactionContextId);
                break;
        }
    }
}
```

### With Device & Behavioral Data

```csharp
var verdict = await _nexusClient.EvaluateTransactionAsync(
    userId: user.Id,
    fromAccountNumber: "ACC001",
    toAccountNumber: "ACC002",
    amount: 50000,
    currency: "KES",
    deviceInfo: new DeviceFingerprint
    {
        DeviceId = "hash_of_device_id",
        DeviceType = "Mobile",
        OperatingSystem = "iOS 17",
        IpAddress = "41.90.x.x",
        Location = "Nairobi, Kenya",
        IsRecognizedDevice = true,
        IsVpnOrProxy = false
    },
    behavioralData: new BehavioralMetrics
    {
        TypingSpeedMs = 120,
        SessionDuration = 45,
        IsOnActiveCall = false,
        BiometricAuthUsed = true,
        BehaviorAnomalyScore = 0.2
    }
);
```

## Installation & Setup

### 1. Add NuGet Reference

Add to your API project:

```xml
<ProjectReference Include="..\Wekeza.Nexus.Application\Wekeza.Nexus.Application.csproj" />
```

### 2. Register Services

In `Program.cs` or `Startup.cs`:

```csharp
using Wekeza.Nexus.Application;

var builder = WebApplication.CreateBuilder(args);

// Add Wekeza Nexus
builder.Services.AddWekezaNexus();
```

### 3. Inject and Use

```csharp
public class TransferFundsHandler : IRequestHandler<TransferFundsCommand, Guid>
{
    private readonly WekezaNexusClient _nexusClient;
    
    public TransferFundsHandler(WekezaNexusClient nexusClient)
    {
        _nexusClient = nexusClient;
    }
}
```

## Performance Targets

- **Latency**: < 150ms for complete fraud evaluation
- **Throughput**: 1000+ transactions per second
- **False Positive Rate**: < 0.5% (vs industry standard of 2-5%)
- **True Positive Rate**: > 95%

## Current Implementation Status (MVP)

✅ **Completed**:
- Core domain entities (TransactionContext, FraudEvaluation, FraudScore)
- Value objects (DeviceFingerprint, BehavioralMetrics)
- Fraud evaluation service with ensemble scoring
- 5 parallel scoring engines (Velocity, Behavioral, Relationship, Amount, Device)
- Explainable AI fraud explanations
- In-memory repository implementation
- Integration client (WekezaNexusClient)
- Dependency injection setup

🚧 **In Progress**:
- Integration with existing TransferFundsHandler
- Integration with PaymentProcessingService
- Database persistence (Entity Framework Core)
- API endpoints for fraud investigation

📋 **Future Enhancements** (Phase 2+):
- Graph Neural Network integration with Neo4j
- Transformer models for sequence analysis
- Real-time streaming with Apache Kafka
- Behavioral biometrics SDK (JavaScript/TypeScript)
- Machine learning model training pipeline
- A/B testing framework for model improvements
- External API integrations (KYC providers, watchlists)

## API Endpoints (Planned)

### Evaluate Transaction
```
POST /api/nexus/evaluate
```

### Get Fraud Evaluation
```
GET /api/nexus/evaluation/{id}
```

### Get Pending Reviews
```
GET /api/nexus/reviews/pending
```

### Update Review Status
```
PUT /api/nexus/reviews/{id}
```

## Testing

Unit tests can be found in `Tests/Wekeza.Nexus.UnitTests/`

```bash
dotnet test Tests/Wekeza.Nexus.UnitTests
```

## Security Considerations

1. **Data Privacy**: All behavioral data is hashed and anonymized
2. **Audit Trail**: Every fraud evaluation is logged for compliance
3. **Fail-Safe**: On error, system defaults to "flag for review" not "block"
4. **No False Negatives**: Better to challenge than to miss fraud
5. **Explainability**: Every decision includes human-readable reasoning

## Roadmap to "Big Tech" Acquisition

### Month 1-2: MVP ✅ (Current)
- Basic fraud detection with rule-based scoring
- Integration with existing payment flow

### Month 3-4: Beta
- Behavioral biometrics SDK
- Graph database for relationship analysis
- Real-time velocity tracking with Redis

### Month 6: Gold
- Transformer models for pattern recognition
- Self-learning fraud detection
- < 0.1% false positive rate

### Exit Strategy
- Demonstrate false positive rate < 0.1%
- Show 99%+ fraud catch rate
- Prove sub-100ms latency at scale
- Document cost savings vs manual review

## License

Proprietary - © 2026 Wekeza Bank. All rights reserved.

## Contact

For questions or support regarding Wekeza Nexus:
- Email: nexus-team@wekeza.com
- Slack: #wekeza-nexus

---

**Built with ❤️ by the Wekeza Engineering Team**
