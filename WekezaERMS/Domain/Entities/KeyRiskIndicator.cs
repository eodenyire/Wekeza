namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Key Risk Indicator - Metric used to monitor risk levels
/// </summary>
public class KeyRiskIndicator
{
    public Guid Id { get; private set; }
    public Guid RiskId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string MeasurementUnit { get; private set; }
    public decimal CurrentValue { get; private set; }
    public decimal ThresholdWarning { get; private set; }
    public decimal ThresholdCritical { get; private set; }
    public string Frequency { get; private set; } // Daily, Weekly, Monthly
    public DateTime LastMeasuredDate { get; private set; }
    public DateTime NextMeasurementDate { get; private set; }
    public KRIStatus Status { get; private set; }
    public string DataSource { get; private set; }
    public Guid OwnerId { get; private set; }
    public List<KRIMeasurement> Measurements { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private KeyRiskIndicator() 
    { 
        Measurements = new List<KRIMeasurement>();
    }

    public static KeyRiskIndicator Create(
        Guid riskId,
        string name,
        string description,
        string measurementUnit,
        decimal thresholdWarning,
        decimal thresholdCritical,
        string frequency,
        string dataSource,
        Guid ownerId,
        Guid createdBy)
    {
        return new KeyRiskIndicator
        {
            Id = Guid.NewGuid(),
            RiskId = riskId,
            Name = name,
            Description = description,
            MeasurementUnit = measurementUnit,
            ThresholdWarning = thresholdWarning,
            ThresholdCritical = thresholdCritical,
            Frequency = frequency,
            DataSource = dataSource,
            OwnerId = ownerId,
            Status = KRIStatus.Normal,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void RecordMeasurement(decimal value, string notes, Guid recordedBy)
    {
        CurrentValue = value;
        LastMeasuredDate = DateTime.UtcNow;
        NextMeasurementDate = CalculateNextMeasurementDate();

        var measurement = new KRIMeasurement
        {
            Id = Guid.NewGuid(),
            KRIId = Id,
            Value = value,
            MeasuredDate = DateTime.UtcNow,
            Status = DetermineStatus(value),
            Notes = notes,
            RecordedBy = recordedBy
        };

        Measurements.Add(measurement);
        Status = measurement.Status;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = recordedBy;
    }

    private KRIStatus DetermineStatus(decimal value)
    {
        if (value >= ThresholdCritical)
            return KRIStatus.Critical;
        if (value >= ThresholdWarning)
            return KRIStatus.Warning;
        return KRIStatus.Normal;
    }

    private DateTime CalculateNextMeasurementDate()
    {
        return Frequency?.ToLower() switch
        {
            "daily" => DateTime.UtcNow.AddDays(1),
            "weekly" => DateTime.UtcNow.AddDays(7),
            "monthly" => DateTime.UtcNow.AddMonths(1),
            _ => DateTime.UtcNow.AddDays(7) // Default to weekly
        };
    }
}

/// <summary>
/// Individual measurement record for a KRI
/// </summary>
public class KRIMeasurement
{
    public Guid Id { get; set; }
    public Guid KRIId { get; set; }
    public decimal Value { get; set; }
    public DateTime MeasuredDate { get; set; }
    public KRIStatus Status { get; set; }
    public string Notes { get; set; }
    public Guid RecordedBy { get; set; }
}

/// <summary>
/// Status of KRI based on threshold comparison
/// </summary>
public enum KRIStatus
{
    Normal = 1,
    Warning = 2,
    Critical = 3
}
