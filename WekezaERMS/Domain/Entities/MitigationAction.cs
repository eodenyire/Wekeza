namespace WekezaERMS.Domain.Entities;

/// <summary>
/// Represents an action plan to mitigate a risk
/// </summary>
public class MitigationAction
{
    public Guid Id { get; private set; }
    public Guid RiskId { get; private set; }
    public string ActionTitle { get; private set; }
    public string Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public MitigationStatus Status { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    public int ProgressPercentage { get; private set; }
    public decimal EstimatedCost { get; private set; }
    public decimal ActualCost { get; private set; }
    public string Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }

    private MitigationAction() { }

    public static MitigationAction Create(
        Guid riskId,
        string actionTitle,
        string description,
        Guid ownerId,
        DateTime dueDate,
        decimal estimatedCost,
        Guid createdBy)
    {
        return new MitigationAction
        {
            Id = Guid.NewGuid(),
            RiskId = riskId,
            ActionTitle = actionTitle,
            Description = description,
            OwnerId = ownerId,
            Status = MitigationStatus.Planned,
            DueDate = dueDate,
            EstimatedCost = estimatedCost,
            ProgressPercentage = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void UpdateProgress(int progressPercentage, string notes, Guid updatedBy)
    {
        ProgressPercentage = Math.Clamp(progressPercentage, 0, 100);
        Notes = notes;
        
        if (ProgressPercentage > 0 && Status == MitigationStatus.Planned)
        {
            Status = MitigationStatus.InProgress;
        }
        
        if (ProgressPercentage == 100)
        {
            Status = MitigationStatus.Completed;
            CompletedDate = DateTime.UtcNow;
        }

        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateCost(decimal actualCost, Guid updatedBy)
    {
        ActualCost = actualCost;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void MarkAsDelayed(string reason, Guid updatedBy)
    {
        if (DateTime.UtcNow > DueDate && Status != MitigationStatus.Completed)
        {
            Status = MitigationStatus.Delayed;
            Notes = $"Delayed: {reason}";
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy;
        }
    }
}
