using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Exceptions;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// GLAccount Aggregate - Chart of Accounts
/// Inspired by Finacle GL and T24 ACCOUNT
/// Represents a General Ledger account in the Chart of Accounts
/// </summary>
public class GLAccount : AggregateRoot
{
    public string GLCode { get; private set; } // e.g., 1001, 2001, 4001
    public string GLName { get; private set; }
    public GLAccountType AccountType { get; private set; }
    public GLAccountCategory Category { get; private set; }
    public GLAccountStatus Status { get; private set; }
    
    // Hierarchy
    public string? ParentGLCode { get; private set; }
    public int Level { get; private set; } // 1=Main, 2=Sub, 3=Detail
    public bool IsLeaf { get; private set; } // Can post transactions
    
    // Balance tracking
    public decimal DebitBalance { get; private set; }
    public decimal CreditBalance { get; private set; }
    public decimal NetBalance => AccountType == GLAccountType.Asset || AccountType == GLAccountType.Expense
        ? DebitBalance - CreditBalance
        : CreditBalance - DebitBalance;
    
    // Currency
    public string Currency { get; private set; }
    public bool IsMultiCurrency { get; private set; }
    
    // Control flags
    public bool AllowManualPosting { get; private set; }
    public bool RequiresCostCenter { get; private set; }
    public bool RequiresProfitCenter { get; private set; }
    
    // Metadata
    public DateTime CreatedDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime? LastPostingDate { get; private set; }

    private GLAccount() : base(Guid.NewGuid()) { }

    public static GLAccount Create(
        string glCode,
        string glName,
        GLAccountType accountType,
        GLAccountCategory category,
        string currency,
        int level,
        bool isLeaf,
        string createdBy,
        string? parentGLCode = null)
    {
        return new GLAccount
        {
            Id = Guid.NewGuid(),
            GLCode = glCode,
            GLName = glName,
            AccountType = accountType,
            Category = category,
            Status = GLAccountStatus.Active,
            ParentGLCode = parentGLCode,
            Level = level,
            IsLeaf = isLeaf,
            Currency = currency,
            IsMultiCurrency = false,
            DebitBalance = 0,
            CreditBalance = 0,
            AllowManualPosting = isLeaf,
            RequiresCostCenter = false,
            RequiresProfitCenter = false,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void PostDebit(decimal amount)
    {
        if (!IsLeaf)
            throw new GenericDomainException("Cannot post to non-leaf GL account.");

        if (Status != GLAccountStatus.Active)
            throw new GenericDomainException($"Cannot post to {Status} GL account.");

        DebitBalance += amount;
        LastPostingDate = DateTime.UtcNow;
    }

    public void PostCredit(decimal amount)
    {
        if (!IsLeaf)
            throw new GenericDomainException("Cannot post to non-leaf GL account.");

        if (Status != GLAccountStatus.Active)
            throw new GenericDomainException($"Cannot post to {Status} GL account.");

        CreditBalance += amount;
        LastPostingDate = DateTime.UtcNow;
    }

    public void Activate(string activatedBy)
    {
        Status = GLAccountStatus.Active;
        LastModifiedBy = activatedBy;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Suspend(string suspendedBy, string reason)
    {
        Status = GLAccountStatus.Suspended;
        LastModifiedBy = suspendedBy;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Close(string closedBy)
    {
        if (NetBalance != 0)
            throw new GenericDomainException("Cannot close GL account with non-zero balance.");

        Status = GLAccountStatus.Closed;
        LastModifiedBy = closedBy;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void UpdateControlFlags(
        bool allowManualPosting,
        bool requiresCostCenter,
        bool requiresProfitCenter,
        string modifiedBy)
    {
        AllowManualPosting = allowManualPosting;
        RequiresCostCenter = requiresCostCenter;
        RequiresProfitCenter = requiresProfitCenter;
        LastModifiedBy = modifiedBy;
        LastModifiedDate = DateTime.UtcNow;
    }
}


