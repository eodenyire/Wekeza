using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Complaint Aggregate - Customer complaints and issues
/// </summary>
public class Complaint : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public string ComplaintNumber { get; private set; }
    public string Status { get; private set; } // Open, InProgress, Resolved, Closed
    public string Priority { get; private set; } // Low, Medium, High, Critical
    public string Category { get; private set; }
    public string Description { get; private set; }
    public string? Resolution { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? AssignedTo { get; private set; }

    private Complaint() : base(Guid.NewGuid()) { }

    public static Complaint Create(Guid customerId, string complaintNumber, string category, string description, string priority = "Medium")
    {
        var complaint = new Complaint
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ComplaintNumber = complaintNumber,
            Category = category,
            Description = description,
            Priority = priority,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };
        return complaint;
    }

    public void Resolve(string resolution, string resolvedBy)
    {
        Status = "Resolved";
        Resolution = resolution;
        ResolvedAt = DateTime.UtcNow;
        AssignedTo = resolvedBy;
    }
}
