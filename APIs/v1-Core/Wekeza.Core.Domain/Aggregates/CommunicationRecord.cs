using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// CommunicationRecord Aggregate - Record of customer communications
/// </summary>
public class CommunicationRecord : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public string Channel { get; private set; } // Email, SMS, Call, InApp, Branch
    public string Direction { get; private set; } // Inbound, Outbound
    public string? Subject { get; private set; }
    public string? Content { get; private set; }
    public string? AgentId { get; private set; }
    public DateTime CommunicatedAt { get; private set; }
    public bool IsSuccessful { get; private set; }

    private CommunicationRecord() : base(Guid.NewGuid()) { }

    public static CommunicationRecord Create(
        Guid customerId, 
        string channel, 
        string direction, 
        string? subject = null, 
        string? content = null,
        string? agentId = null)
    {
        var record = new CommunicationRecord
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Channel = channel,
            Direction = direction,
            Subject = subject,
            Content = content,
            AgentId = agentId,
            CommunicatedAt = DateTime.UtcNow,
            IsSuccessful = true
        };
        return record;
    }

    public void MarkAsFailed()
    {
        IsSuccessful = false;
    }
}
