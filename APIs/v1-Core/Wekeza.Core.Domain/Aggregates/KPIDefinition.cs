using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// KPI Definition Aggregate - Defines Key Performance Indicators
/// Supports enterprise performance tracking and reporting
/// </summary>
public class KPIDefinition : AggregateRoot
{
    public string KPIName { get; private set; }
    public string KPICode { get; private set; }
    public string Category { get; private set; } // Financial, Operational, Risk, Compliance, etc.
    public string MeasurementType { get; private set; } // Ratio, Percentage, Average, Sum, Count
    public string Formula { get; private set; } // Mathematical formula or logic
    public string Unit { get; private set; } // %, Amount, Days, Count, etc.
    public string Status { get; private set; } // Active, Inactive, Archived
    
    // Targets and Thresholds
    public decimal? TargetValue { get; private set; }
    public decimal? LowerThreshold { get; private set; }
    public decimal? UpperThreshold { get; private set; }
    public string Frequency { get; private set; } // Daily, Weekly, Monthly, Quarterly, Annual
    
    // Metadata
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private KPIDefinition() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static KPIDefinition Create(
        string kpiName,
        string kpiCode,
        string category,
        string measurementType,
        string unit,
        string createdBy)
    {
        var kpi = new KPIDefinition
        {
            Id = Guid.NewGuid(),
            KPIName = kpiName,
            KPICode = kpiCode,
            Category = category,
            MeasurementType = measurementType,
            Unit = unit,
            Status = "Active",
            Frequency = "Monthly",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Description = string.Empty,
            Metadata = new Dictionary<string, object>()
        };

        return kpi;
    }

    public void SetTargets(decimal? target, decimal? lower, decimal? upper, string updatedBy)
    {
        TargetValue = target;
        LowerThreshold = lower;
        UpperThreshold = upper;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateFormula(string formula, string updatedBy)
    {
        Formula = formula;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate(string deactivatedBy)
    {
        Status = "Inactive";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;
    }
}
