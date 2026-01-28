using WekezaERMS.Domain.Enums;

namespace WekezaERMS.Domain.Entities;

/// <summary>
/// AI System Entity - Represents AI and machine learning systems requiring governance
/// Aligned with Riskonnect AI governance capabilities
/// </summary>
public class AISystem
{
    /// <summary>
    /// Unique identifier for the AI system
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Human-readable AI system ID (e.g., AI-2024-001)
    /// </summary>
    public string SystemCode { get; private set; }

    /// <summary>
    /// AI system name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Detailed description of the AI system
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Purpose and use case of the AI system
    /// </summary>
    public string Purpose { get; private set; }

    /// <summary>
    /// Type of AI/ML model (e.g., Neural Network, Decision Tree, NLP)
    /// </summary>
    public string ModelType { get; private set; }

    /// <summary>
    /// Current status of the AI system
    /// </summary>
    public AISystemStatus Status { get; private set; }

    /// <summary>
    /// AI risk level classification
    /// </summary>
    public AIRiskLevel RiskLevel { get; private set; }

    /// <summary>
    /// Department using the AI system
    /// </summary>
    public string Department { get; private set; }

    /// <summary>
    /// AI system owner
    /// </summary>
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// Data sources used by the AI system
    /// </summary>
    public string DataSources { get; private set; }

    /// <summary>
    /// Whether the system processes personal data
    /// </summary>
    public bool ProcessesPersonalData { get; private set; }

    /// <summary>
    /// Whether the system makes automated decisions
    /// </summary>
    public bool MakesAutomatedDecisions { get; private set; }

    /// <summary>
    /// Algorithm bias assessment results
    /// </summary>
    public string? BiasAssessment { get; private set; }

    /// <summary>
    /// Bias score (0-100, with 0 being no bias detected)
    /// </summary>
    public decimal? BiasScore { get; private set; }

    /// <summary>
    /// Fairness metrics
    /// </summary>
    public string? FairnessMetrics { get; private set; }

    /// <summary>
    /// Explainability assessment
    /// </summary>
    public string? ExplainabilityAssessment { get; private set; }

    /// <summary>
    /// AI ethics compliance framework
    /// </summary>
    public string? EthicsFramework { get; private set; }

    /// <summary>
    /// Whether ethics review was completed
    /// </summary>
    public bool EthicsReviewCompleted { get; private set; }

    /// <summary>
    /// Date of ethics review
    /// </summary>
    public DateTime? EthicsReviewDate { get; private set; }

    /// <summary>
    /// Model accuracy percentage
    /// </summary>
    public decimal? ModelAccuracy { get; private set; }

    /// <summary>
    /// Model performance metrics
    /// </summary>
    public string? PerformanceMetrics { get; private set; }

    /// <summary>
    /// Monitoring frequency (in days)
    /// </summary>
    public int MonitoringFrequencyDays { get; private set; }

    /// <summary>
    /// Date of last monitoring
    /// </summary>
    public DateTime? LastMonitoringDate { get; private set; }

    /// <summary>
    /// Date of next monitoring
    /// </summary>
    public DateTime NextMonitoringDate { get; private set; }

    /// <summary>
    /// Human oversight requirements
    /// </summary>
    public string? HumanOversightRequirements { get; private set; }

    /// <summary>
    /// Transparency and disclosure requirements
    /// </summary>
    public string? TransparencyRequirements { get; private set; }

    /// <summary>
    /// Date deployed
    /// </summary>
    public DateTime? DeploymentDate { get; private set; }

    /// <summary>
    /// Impact assessment
    /// </summary>
    public string? ImpactAssessment { get; private set; }

    /// <summary>
    /// Regulatory compliance requirements
    /// </summary>
    public string? RegulatoryCompliance { get; private set; }

    /// <summary>
    /// Audit trail
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private AISystem() { }

    public static AISystem Create(
        string systemCode,
        string name,
        string description,
        string purpose,
        string modelType,
        string department,
        Guid ownerId,
        string dataSources,
        bool processesPersonalData,
        bool makesAutomatedDecisions,
        int monitoringFrequencyDays,
        Guid createdBy)
    {
        var aiSystem = new AISystem
        {
            Id = Guid.NewGuid(),
            SystemCode = systemCode,
            Name = name,
            Description = description,
            Purpose = purpose,
            ModelType = modelType,
            Status = AISystemStatus.Development,
            Department = department,
            OwnerId = ownerId,
            DataSources = dataSources,
            ProcessesPersonalData = processesPersonalData,
            MakesAutomatedDecisions = makesAutomatedDecisions,
            MonitoringFrequencyDays = monitoringFrequencyDays,
            NextMonitoringDate = DateTime.UtcNow.AddDays(monitoringFrequencyDays),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        aiSystem.DetermineRiskLevel();
        return aiSystem;
    }

    public void PerformBiasAssessment(string assessment, decimal biasScore, Guid assessedBy)
    {
        BiasAssessment = assessment;
        BiasScore = biasScore;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = assessedBy;

        // Adjust risk level based on bias score
        if (biasScore > 50)
        {
            RiskLevel = AIRiskLevel.High;
        }
    }

    public void CompleteEthicsReview(string framework, bool approved, Guid reviewedBy)
    {
        EthicsFramework = framework;
        EthicsReviewCompleted = true;
        EthicsReviewDate = DateTime.UtcNow;
        
        if (approved)
        {
            Status = AISystemStatus.Approved;
        }
        
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = reviewedBy;
    }

    public void Deploy(Guid deployedBy)
    {
        if (!EthicsReviewCompleted)
        {
            throw new InvalidOperationException("AI system cannot be deployed without ethics review");
        }

        Status = AISystemStatus.Deployed;
        DeploymentDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deployedBy;
    }

    public void RecordMonitoring(decimal modelAccuracy, string performanceMetrics, Guid monitoredBy)
    {
        ModelAccuracy = modelAccuracy;
        PerformanceMetrics = performanceMetrics;
        LastMonitoringDate = DateTime.UtcNow;
        NextMonitoringDate = DateTime.UtcNow.AddDays(MonitoringFrequencyDays);
        Status = AISystemStatus.Monitoring;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = monitoredBy;
    }

    public void Suspend(string reason, Guid suspendedBy)
    {
        Status = AISystemStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = suspendedBy;
    }

    public void Decommission(string reason, Guid decommissionedBy)
    {
        Status = AISystemStatus.Decommissioned;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = decommissionedBy;
    }

    private void DetermineRiskLevel()
    {
        // Determine risk level based on characteristics
        if (MakesAutomatedDecisions && ProcessesPersonalData)
        {
            RiskLevel = AIRiskLevel.High;
        }
        else if (MakesAutomatedDecisions || ProcessesPersonalData)
        {
            RiskLevel = AIRiskLevel.Limited;
        }
        else
        {
            RiskLevel = AIRiskLevel.Minimal;
        }
    }
}
