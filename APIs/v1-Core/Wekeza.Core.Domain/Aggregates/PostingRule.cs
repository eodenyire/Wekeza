using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Posting Rule Aggregate - Defines GL posting logic for products
/// Core to GL integration and accounting accuracy
/// </summary>
public class PostingRule : AggregateRoot
{
    public string RuleName { get; private set; }
    public string RuleCode { get; private set; }
    public Guid ProductId { get; private set; }
    public string TransactionType { get; private set; } // Debit, Credit, Interest, Fee, etc.
    public string GLAccountCode { get; private set; }
    public string CounterAccountCode { get; private set; }
    public string Status { get; private set; } // Draft, Active, Inactive, Archived
    public int Priority { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    
    // Rule Conditions (stored as JSON)
    public string PostingLogic { get; private set; } // JSON: conditions, GL mapping rules
    public bool IsActive => Status == "Active";
    public Dictionary<string, object> Metadata { get; private set; }

    private PostingRule() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static PostingRule Create(
        string ruleName,
        string ruleCode,
        Guid productId,
        string transactionType,
        string glAccountCode,
        string counterAccountCode,
        string createdBy)
    {
        var rule = new PostingRule
        {
            Id = Guid.NewGuid(),
            RuleName = ruleName,
            RuleCode = ruleCode,
            ProductId = productId,
            TransactionType = transactionType,
            GLAccountCode = glAccountCode,
            CounterAccountCode = counterAccountCode,
            Status = "Draft",
            Priority = 10,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            PostingLogic = "{}",
            Metadata = new Dictionary<string, object>()
        };

        return rule;
    }

    public void Activate(string activatedBy)
    {
        Status = "Active";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = activatedBy;
    }

    public void Deactivate(string deactivatedBy)
    {
        Status = "Inactive";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;
    }

    public void UpdatePriority(int priority, string updatedBy)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
