using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// ApprovalMatrix Aggregate - Defines approval rules
/// Inspired by Finacle Approval Matrix and T24 Authorization
/// Configures who can approve what and at what level
/// </summary>
public class ApprovalMatrix : AggregateRoot
{
    public string MatrixCode { get; private set; }
    public string MatrixName { get; private set; }
    public string EntityType { get; private set; } // Product, Loan, Transaction, etc.
    public MatrixStatus Status { get; private set; }
    
    // Approval rules
    private readonly List<ApprovalRule> _rules = new();
    public IReadOnlyCollection<ApprovalRule> Rules => _rules.AsReadOnly();
    
    // Metadata
    public DateTime CreatedDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }

    private ApprovalMatrix() : base(Guid.NewGuid()) { }

    public static ApprovalMatrix Create(
        string matrixCode,
        string matrixName,
        string entityType,
        string createdBy)
    {
        return new ApprovalMatrix
        {
            Id = Guid.NewGuid(),
            MatrixCode = matrixCode,
            MatrixName = matrixName,
            EntityType = entityType,
            Status = MatrixStatus.Draft,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AddRule(ApprovalRule rule)
    {
        // Validate no duplicate levels
        if (_rules.Any(r => r.Level == rule.Level))
            throw new DomainException($"Rule for level {rule.Level} already exists.");

        _rules.Add(rule);
        LastModifiedDate = DateTime.UtcNow;
    }

    public void RemoveRule(int level)
    {
        var rule = _rules.FirstOrDefault(r => r.Level == level);
        if (rule != null)
        {
            _rules.Remove(rule);
            LastModifiedDate = DateTime.UtcNow;
        }
    }

    public void Activate(string activatedBy)
    {
        if (!_rules.Any())
            throw new DomainException("Cannot activate matrix without approval rules.");

        Status = MatrixStatus.Active;
        LastModifiedBy = activatedBy;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Deactivate(string deactivatedBy)
    {
        Status = MatrixStatus.Inactive;
        LastModifiedBy = deactivatedBy;
        LastModifiedDate = DateTime.UtcNow;
    }

    public int GetRequiredLevels(decimal amount, string operation)
    {
        var applicableRules = _rules
            .Where(r => r.IsApplicable(amount, operation))
            .OrderBy(r => r.Level)
            .ToList();

        return applicableRules.Any() ? applicableRules.Max(r => r.Level) : 1;
    }

    public List<UserRole> GetApproversForLevel(int level, decimal amount, string operation)
    {
        var rule = _rules.FirstOrDefault(r => r.Level == level && r.IsApplicable(amount, operation));
        return rule?.ApproverRoles ?? new List<UserRole>();
    }
}

// Value Object

public record ApprovalRule(
    int Level,
    List<UserRole> ApproverRoles,
    decimal? MinAmount,
    decimal? MaxAmount,
    string? Operation, // Create, Update, Delete, Approve, etc.
    int SlaHours)
{
    public bool IsApplicable(decimal amount, string operation)
    {
        var amountMatch = (!MinAmount.HasValue || amount >= MinAmount.Value) &&
                         (!MaxAmount.HasValue || amount <= MaxAmount.Value);

        var operationMatch = string.IsNullOrEmpty(Operation) || Operation == operation;

        return amountMatch && operationMatch;
    }
}
