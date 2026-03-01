using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// ServiceRequest Aggregate - Customer service requests
/// </summary>
public class ServiceRequest : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public string RequestNumber { get; private set; }
    public string Status { get; private set; } // Pending, InProgress, Completed, Cancelled
    public string RequestType { get; private set; }
    public string Description { get; private set; }
    public string? Resolution { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? AssignedTo { get; private set; }

    private ServiceRequest() : base(Guid.NewGuid()) { }

    public static ServiceRequest Create(Guid customerId, string requestNumber, string requestType, string description)
    {
        var request = new ServiceRequest
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            RequestNumber = requestNumber,
            RequestType = requestType,
            Description = description,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        return request;
    }

    public void Complete(string resolution, string completedBy)
    {
        Status = "Completed";
        Resolution = resolution;
        CompletedAt = DateTime.UtcNow;
        AssignedTo = completedBy;
    }
}
