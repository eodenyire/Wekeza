using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

public class Reconciliation : AggregateRoot
{
    public string ReconciliationCode { get; private set; } = string.Empty;
    public DateTime ReconciliationDate { get; private set; }
    public string Status { get; private set; } = "Open";
    public string? Notes { get; private set; }
    public string CreatedBy { get; private set; } = "SYSTEM";
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Reconciliation() : base(Guid.NewGuid()) { }

    public static Reconciliation Create(string reconciliationCode, DateTime reconciliationDate, string createdBy)
    {
        return new Reconciliation
        {
            Id = Guid.NewGuid(),
            ReconciliationCode = reconciliationCode,
            ReconciliationDate = reconciliationDate,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            Status = "Open"
        };
    }

    public void UpdateStatus(string status, string? notes = null)
    {
        Status = status;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
