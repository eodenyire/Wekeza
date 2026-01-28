# Wekeza ERMS - Quick Start Guide

## Getting Started with Enterprise Risk Management System

This guide will help you quickly understand and start working with the Wekeza Enterprise Risk Management System (ERMS) MVP 4.0.

---

## 🚀 What is ERMS?

The Enterprise Risk Management System (ERMS) is a comprehensive platform for managing all types of risks in Wekeza Bank. It provides:

- **Risk Identification & Assessment** - Systematic approach to identifying and evaluating risks
- **Risk Treatment** - Control implementation and mitigation planning
- **Risk Monitoring** - Real-time monitoring using Key Risk Indicators (KRIs)
- **Risk Reporting** - Dashboards and reports for stakeholders
- **Wekeza Core Integration** - Seamless integration with the banking system

---

## 📁 Project Structure

```
WekezaERMS/
├── Domain/                   # Core business logic
│   ├── Entities/            # Main business objects (Risk, Control, etc.)
│   ├── Enums/               # Risk classifications
│   ├── ValueObjects/        # Domain value objects
│   └── Events/              # Domain events
├── Application/             # Use cases and business workflows
│   ├── Commands/            # Write operations
│   ├── Queries/             # Read operations
│   ├── DTOs/                # Data transfer objects
│   └── Validators/          # Business validation
├── Infrastructure/          # Technical implementation
│   ├── Persistence/         # Database access
│   ├── Integration/         # External integrations
│   └── Services/            # External services
├── API/                     # REST API
│   ├── Controllers/         # API endpoints
│   ├── Middleware/          # Custom middleware
│   └── Configuration/       # API configuration
└── Docs/                    # Documentation
    ├── API-REFERENCE.md
    ├── IMPLEMENTATION-GUIDE.md
    └── INTEGRATION-GUIDE.md
```

---

## 🎯 Core Concepts

### 1. Risk Categories

The system supports 8 risk categories:

| Category | Description | Example |
|----------|-------------|---------|
| **Credit** | Default risk | Loan portfolio concentration |
| **Operational** | Process/system failures | System outage |
| **Market** | Market price movements | FX rate changes |
| **Liquidity** | Inability to meet obligations | Cash flow shortfall |
| **Strategic** | Business decisions | Market entry failure |
| **Compliance** | Regulatory violations | AML policy breach |
| **Reputation** | Brand damage | Negative publicity |
| **Technology** | IT/cyber risks | Data breach |

### 2. Risk Assessment (5x5 Matrix)

**Step 1: Assess Likelihood**
- Rare (1) - < 5% probability
- Unlikely (2) - 5-25% probability
- Possible (3) - 25-50% probability
- Likely (4) - 50-75% probability
- Almost Certain (5) - > 75% probability

**Step 2: Assess Impact**
- Insignificant (1) - < $10K
- Minor (2) - $10K-$100K
- Moderate (3) - $100K-$1M
- Major (4) - $1M-$10M
- Catastrophic (5) - > $10M

**Step 3: Calculate Risk Score**
```
Risk Score = Likelihood × Impact
Risk Level = Determined from score:
  - 1-4: Low
  - 5-9: Medium
  - 10-15: High
  - 16-20: Very High
  - 21-25: Critical
```

### 3. Risk Treatment Strategies

| Strategy | When to Use | Example |
|----------|-------------|---------|
| **Accept** | Low risk within appetite | Minor operational inefficiency |
| **Mitigate** | High risk, controls available | Implement backup systems |
| **Transfer** | Risk can be insured/outsourced | Cyber insurance |
| **Avoid** | Risk unacceptable | Exit high-risk market |
| **Share** | Partnership possible | Joint venture |

### 4. Key Risk Indicators (KRIs)

KRIs are metrics that provide early warning of increasing risk exposure:

**Example KRIs:**
- Credit Concentration Ratio (%)
- Transaction Failure Rate (%)
- System Uptime (%)
- Open AML Cases (count)
- NPL Ratio (%)

**KRI Configuration:**
```csharp
var kri = KeyRiskIndicator.Create(
    riskId: riskId,
    name: "Credit Concentration Ratio",
    description: "Percentage of loans to largest sector",
    measurementUnit: "Percentage",
    thresholdWarning: 60.0m,      // Yellow alert at 60%
    thresholdCritical: 75.0m,     // Red alert at 75%
    frequency: "Monthly",
    dataSource: "Loan Management System",
    ownerId: userId,
    createdBy: userId
);
```

---

## 💻 Code Examples

### Example 1: Create a New Risk

```csharp
using WekezaERMS.Domain.Entities;
using WekezaERMS.Domain.Enums;

// Create a credit concentration risk
var risk = Risk.Create(
    riskCode: "RISK-2024-001",
    title: "Credit Concentration Risk - Real Estate Sector",
    description: "Significant exposure to real estate lending (65% of portfolio)",
    category: RiskCategory.Credit,
    inherentLikelihood: RiskLikelihood.Likely,
    inherentImpact: RiskImpact.Major,
    ownerId: creditManagerId,
    department: "Credit Risk Management",
    treatmentStrategy: RiskTreatmentStrategy.Mitigate,
    riskAppetite: 12,  // Maximum acceptable score
    createdBy: userId
);

// Risk is automatically scored:
// inherentRiskScore = 4 (Likely) × 4 (Major) = 16
// inherentRiskLevel = VeryHigh (score 16)
```

### Example 2: Add a Control

```csharp
// Add a control to mitigate the risk
var control = RiskControl.Create(
    riskId: risk.Id,
    controlName: "Sector Concentration Limits",
    description: "Maximum 50% exposure to any single sector",
    controlType: "Preventive",
    ownerId: creditManagerId,
    testingFrequency: "Quarterly",
    createdBy: userId
);

risk.AddControl(control);

// Update control effectiveness after testing
control.UpdateEffectiveness(
    effectiveness: ControlEffectiveness.Effective,
    testingEvidence: "Tested 500 loans, all within sector limits",
    updatedBy: userId
);
```

### Example 3: Create a Mitigation Action

```csharp
// Create an action plan to reduce the risk
var mitigation = MitigationAction.Create(
    riskId: risk.Id,
    actionTitle: "Diversify Loan Portfolio",
    description: "Actively seek borrowers in underrepresented sectors",
    ownerId: creditManagerId,
    dueDate: DateTime.UtcNow.AddMonths(6),
    estimatedCost: 50000m,
    createdBy: userId
);

risk.AddMitigationAction(mitigation);

// Update progress
mitigation.UpdateProgress(
    progressPercentage: 35,
    notes: "Onboarded 10 new clients in healthcare sector",
    updatedBy: userId
);
```

### Example 4: Set Up a KRI

```csharp
// Create a KRI to monitor the risk
var kri = KeyRiskIndicator.Create(
    riskId: risk.Id,
    name: "Real Estate Concentration Ratio",
    description: "Percentage of total loans to real estate sector",
    measurementUnit: "Percentage",
    thresholdWarning: 60.0m,
    thresholdCritical: 70.0m,
    frequency: "Monthly",
    dataSource: "Loan Management System",
    ownerId: creditManagerId,
    createdBy: userId
);

risk.AddKeyRiskIndicator(kri);

// Record a measurement
kri.RecordMeasurement(
    value: 65.5m,
    notes: "Slight increase from last month (64.2%)",
    recordedBy: userId
);
// Status will be Warning (65.5 > 60.0)
```

### Example 5: Update Residual Risk

```csharp
// After implementing controls, calculate residual risk
risk.UpdateResidualRisk(
    residualLikelihood: RiskLikelihood.Possible,  // Reduced from Likely
    residualImpact: RiskImpact.Major,             // Impact unchanged
    updatedBy: userId
);

// residualRiskScore = 3 × 4 = 12 (High)
// Reduced from inherentRiskScore of 16 (VeryHigh)
```

---

## 📊 Risk Workflow

```
1. IDENTIFY RISK
   ↓
2. CREATE RISK ENTRY
   - Assign owner
   - Classify category
   - Set department
   ↓
3. ASSESS RISK
   - Rate likelihood (1-5)
   - Rate impact (1-5)
   - Calculate score
   - Determine level
   ↓
4. SELECT TREATMENT STRATEGY
   - Accept / Mitigate / Transfer / Avoid / Share
   ↓
5. IMPLEMENT CONTROLS
   - Define controls
   - Assign owners
   - Set testing schedule
   ↓
6. CREATE MITIGATION ACTIONS
   - Define action plans
   - Set deadlines
   - Assign responsibility
   ↓
7. CALCULATE RESIDUAL RISK
   - Reassess with controls
   - Update risk score
   ↓
8. SET UP KRIs
   - Define indicators
   - Set thresholds
   - Configure measurement frequency
   ↓
9. MONITOR & REPORT
   - Track KRI measurements
   - Monitor action progress
   - Generate reports
   ↓
10. REVIEW & UPDATE
    - Quarterly reviews
    - Annual reassessment
    - Continuous monitoring
```

---

## 🔌 Integration with Wekeza Core

### Credit Risk Integration

```csharp
// Automatic sync from Loan Management
public async Task SyncCreditRisks()
{
    // Get loan data from Wekeza Core
    var loans = await _wekezaCoreClient.GetActiveLoans();
    
    // Calculate sector concentration
    var concentration = CalculateSectorConcentration(loans);
    
    // Update KRI
    var kri = await _riskRepo.GetKRIByName("Credit Concentration Ratio");
    await kri.RecordMeasurement(concentration, "Auto-sync", systemUserId);
    
    // Check threshold breach
    if (concentration > kri.ThresholdCritical)
    {
        await _alertService.SendAlert(
            $"Credit concentration threshold breached: {concentration}%"
        );
    }
}
```

### Operational Risk Integration

```csharp
// Real-time event handling
[EventHandler("TransactionFailed")]
public async Task HandleTransactionFailure(TransactionFailedEvent @event)
{
    // Update operational risk KRI
    var kri = await _riskRepo.GetKRIByName("Transaction Failure Rate");
    var failureRate = await CalculateFailureRate();
    
    await kri.RecordMeasurement(failureRate, $"Transaction {@event.Id} failed", systemUserId);
}
```

---

## 📈 Dashboard & Reporting

### Risk Dashboard Data

```csharp
// Get dashboard summary
public async Task<RiskDashboard> GetDashboard()
{
    var risks = await _riskRepo.GetAllActive();
    
    return new RiskDashboard
    {
        TotalRisks = risks.Count,
        CriticalRisks = risks.Count(r => r.InherentRiskLevel == RiskLevel.Critical),
        VeryHighRisks = risks.Count(r => r.InherentRiskLevel == RiskLevel.VeryHigh),
        HighRisks = risks.Count(r => r.InherentRiskLevel == RiskLevel.High),
        MediumRisks = risks.Count(r => r.InherentRiskLevel == RiskLevel.Medium),
        LowRisks = risks.Count(r => r.InherentRiskLevel == RiskLevel.Low),
        RisksByCategory = risks.GroupBy(r => r.Category)
                                .ToDictionary(g => g.Key, g => g.Count())
    };
}
```

### Heat Map Generation

```csharp
// Generate risk heat map
public async Task<List<HeatMapCell>> GetHeatMap()
{
    var risks = await _riskRepo.GetAllActive();
    
    var heatMap = new List<HeatMapCell>();
    
    for (int likelihood = 1; likelihood <= 5; likelihood++)
    {
        for (int impact = 1; impact <= 5; impact++)
        {
            var count = risks.Count(r => 
                (int)r.InherentLikelihood == likelihood && 
                (int)r.InherentImpact == impact);
            
            heatMap.Add(new HeatMapCell
            {
                Likelihood = (RiskLikelihood)likelihood,
                Impact = (RiskImpact)impact,
                Count = count,
                Score = likelihood * impact,
                RiskLevel = Risk.DetermineRiskLevel(likelihood * impact)
            });
        }
    }
    
    return heatMap;
}
```

---

## 🔒 Security & Permissions

### Role-Based Access Control

```csharp
// Check permissions before operations
[Authorize(Roles = "RiskManager,RiskOfficer")]
public async Task<IActionResult> CreateRisk([FromBody] CreateRiskCommand command)
{
    // Only RiskManager and RiskOfficer can create risks
    var result = await _mediator.Send(command);
    return Ok(result);
}

[Authorize(Roles = "RiskManager")]
public async Task<IActionResult> DeleteRisk(Guid id)
{
    // Only RiskManager can delete risks
    await _riskRepo.DeleteAsync(id);
    return NoContent();
}

[Authorize(Roles = "RiskManager,RiskOfficer,RiskViewer,Auditor,Executive")]
public async Task<IActionResult> GetRisk(Guid id)
{
    // All roles can view risks
    var risk = await _riskRepo.GetByIdAsync(id);
    return Ok(risk);
}
```

---

## 🧪 Testing Examples

### Unit Test Example

```csharp
[Fact]
public void Risk_Creation_Should_Calculate_Correct_Score()
{
    // Arrange & Act
    var risk = Risk.Create(
        riskCode: "RISK-TEST-001",
        title: "Test Risk",
        description: "Test Description",
        category: RiskCategory.Operational,
        inherentLikelihood: RiskLikelihood.Likely,      // 4
        inherentImpact: RiskImpact.Major,               // 4
        ownerId: Guid.NewGuid(),
        department: "IT",
        treatmentStrategy: RiskTreatmentStrategy.Mitigate,
        riskAppetite: 10,
        createdBy: Guid.NewGuid()
    );
    
    // Assert
    Assert.Equal(16, risk.InherentRiskScore);           // 4 × 4 = 16
    Assert.Equal(RiskLevel.VeryHigh, risk.InherentRiskLevel);
}

[Fact]
public void KRI_Should_Trigger_Warning_When_Threshold_Exceeded()
{
    // Arrange
    var kri = KeyRiskIndicator.Create(
        riskId: Guid.NewGuid(),
        name: "Test KRI",
        description: "Test",
        measurementUnit: "%",
        thresholdWarning: 60.0m,
        thresholdCritical: 75.0m,
        frequency: "Daily",
        dataSource: "Test",
        ownerId: Guid.NewGuid(),
        createdBy: Guid.NewGuid()
    );
    
    // Act
    kri.RecordMeasurement(65.0m, "Test measurement", Guid.NewGuid());
    
    // Assert
    Assert.Equal(KRIStatus.Warning, kri.Status);
}
```

---

## 📚 Further Reading

- **Complete Documentation**: See [README.md](./README.md)
- **API Reference**: See [Docs/API-REFERENCE.md](./Docs/API-REFERENCE.md)
- **Implementation Guide**: See [Docs/IMPLEMENTATION-GUIDE.md](./Docs/IMPLEMENTATION-GUIDE.md)
- **Integration Guide**: See [Docs/INTEGRATION-GUIDE.md](./Docs/INTEGRATION-GUIDE.md)
- **MVP 4.0 Summary**: See [MVP4.0-SUMMARY.md](./MVP4.0-SUMMARY.md)

---

## 🆘 Need Help?

- **Technical Support**: dev@wekeza.com
- **Risk Management**: risk@wekeza.com
- **Documentation**: https://docs.wekeza.com/erms

---

## ✅ Quick Checklist

Before you start developing:
- [ ] Read this Quick Start Guide
- [ ] Review the domain model (Domain/Entities/)
- [ ] Understand the risk matrix (5x5)
- [ ] Familiarize with risk categories
- [ ] Review integration points
- [ ] Check API documentation
- [ ] Set up development environment
- [ ] Run initial tests

**You're ready to build enterprise-grade risk management! 🚀**
