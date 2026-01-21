using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Exceptions;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// JournalEntry Aggregate - Double-entry bookkeeping
/// Inspired by Finacle GL Posting and T24 TRANSACTION
/// Every financial transaction creates a balanced journal entry
/// </summary>
public class JournalEntry : AggregateRoot
{
    public string JournalNumber { get; private set; } // Unique journal ID
    public DateTime PostingDate { get; private set; }
    public DateTime ValueDate { get; private set; }
    public JournalType Type { get; private set; }
    public JournalStatus Status { get; private set; }
    
    // Source transaction
    public string SourceType { get; private set; } // Transaction, Loan, Interest, Fee, etc.
    public Guid SourceId { get; private set; }
    public string SourceReference { get; private set; }
    
    // Journal lines (must balance)
    private readonly List<JournalLine> _lines = new();
    public IReadOnlyCollection<JournalLine> Lines => _lines.AsReadOnly();
    
    // Totals
    public decimal TotalDebit => _lines.Sum(l => l.DebitAmount);
    public decimal TotalCredit => _lines.Sum(l => l.CreditAmount);
    public bool IsBalanced => TotalDebit == TotalCredit;
    
    // Metadata
    public string Currency { get; private set; }
    public string? Description { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? PostedBy { get; private set; }
    public DateTime? PostedDate { get; private set; }
    public string? ReversedBy { get; private set; }
    public DateTime? ReversedDate { get; private set; }
    public Guid? ReversalJournalId { get; private set; }

    private JournalEntry() : base(Guid.NewGuid()) { }

    public static JournalEntry Create(
        string journalNumber,
        DateTime postingDate,
        DateTime valueDate,
        JournalType type,
        string sourceType,
        Guid sourceId,
        string sourceReference,
        string currency,
        string createdBy,
        string? description = null)
    {
        return new JournalEntry
        {
            Id = Guid.NewGuid(),
            JournalNumber = journalNumber,
            PostingDate = postingDate,
            ValueDate = valueDate,
            Type = type,
            Status = JournalStatus.Draft,
            SourceType = sourceType,
            SourceId = sourceId,
            SourceReference = sourceReference,
            Currency = currency,
            Description = description,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow
        };
    }

    public void AddLine(JournalLine line)
    {
        if (Status != JournalStatus.Draft)
            throw new GenericDomainException("Cannot add lines to non-draft journal entry.");

        _lines.Add(line);
    }

    public void AddDebitLine(string glCode, decimal amount, string? costCenter = null, string? profitCenter = null, string? description = null)
    {
        var line = new JournalLine(
            LineNumber: _lines.Count + 1,
            GLCode: glCode,
            DebitAmount: amount,
            CreditAmount: 0,
            CostCenter: costCenter,
            ProfitCenter: profitCenter,
            Description: description);

        AddLine(line);
    }

    public void AddCreditLine(string glCode, decimal amount, string? costCenter = null, string? profitCenter = null, string? description = null)
    {
        var line = new JournalLine(
            LineNumber: _lines.Count + 1,
            GLCode: glCode,
            DebitAmount: 0,
            CreditAmount: amount,
            CostCenter: costCenter,
            ProfitCenter: profitCenter,
            Description: description);

        AddLine(line);
    }

    // Alias methods for compatibility
    public void AddDebitEntry(string glCode, decimal amount, string? description = null)
    {
        AddDebitLine(glCode, amount, description: description);
    }

    public void AddCreditEntry(string glCode, decimal amount, string? description = null)
    {
        AddCreditLine(glCode, amount, description: description);
    }

    public void Post(string postedBy)
    {
        if (Status != JournalStatus.Draft)
            throw new GenericDomainException($"Cannot post journal entry in {Status} status.");

        if (!_lines.Any())
            throw new GenericDomainException("Cannot post journal entry without lines.");

        if (!IsBalanced)
            throw new GenericDomainException($"Journal entry is not balanced. Debit: {TotalDebit}, Credit: {TotalCredit}");

        Status = JournalStatus.Posted;
        PostedBy = postedBy;
        PostedDate = DateTime.UtcNow;
    }

    public void Reverse(string reversedBy, Guid reversalJournalId)
    {
        if (Status != JournalStatus.Posted)
            throw new GenericDomainException("Can only reverse posted journal entries.");

        if (ReversedDate.HasValue)
            throw new GenericDomainException("Journal entry already reversed.");

        Status = JournalStatus.Reversed;
        ReversedBy = reversedBy;
        ReversedDate = DateTime.UtcNow;
        ReversalJournalId = reversalJournalId;
    }

    public JournalEntry CreateReversalEntry(string reversalJournalNumber, string createdBy)
    {
        if (Status != JournalStatus.Posted)
            throw new GenericDomainException("Can only reverse posted journal entries.");

        var reversal = Create(
            reversalJournalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Reversal,
            SourceType,
            SourceId,
            $"Reversal of {SourceReference}",
            Currency,
            createdBy,
            $"Reversal of Journal {JournalNumber}");

        // Reverse all lines (swap debit/credit)
        foreach (var line in _lines)
        {
            reversal.AddLine(new JournalLine(
                LineNumber: line.LineNumber,
                GLCode: line.GLCode,
                DebitAmount: line.CreditAmount, // Swap
                CreditAmount: line.DebitAmount, // Swap
                CostCenter: line.CostCenter,
                ProfitCenter: line.ProfitCenter,
                Description: $"Reversal: {line.Description}"));
        }

        return reversal;
    }
}

// Value Object

public record JournalLine(
    int LineNumber,
    string GLCode,
    decimal DebitAmount,
    decimal CreditAmount,
    string? CostCenter = null,
    string? ProfitCenter = null,
    string? Description = null);


