using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

public class InterestAccrual : AggregateRoot
{
    public string ProductCode { get; private set; } = string.Empty;
    public DateTime AccrualDate { get; private set; }
    public string Status { get; private set; } = "Pending";
    public decimal AccrualAmount { get; private set; }
    public string CreatedBy { get; private set; } = "SYSTEM";
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private InterestAccrual() : base(Guid.NewGuid()) { }

    public static InterestAccrual Create(string productCode, DateTime accrualDate, decimal accrualAmount, string createdBy)
    {
        return new InterestAccrual
        {
            Id = Guid.NewGuid(),
            ProductCode = productCode,
            AccrualDate = accrualDate,
            AccrualAmount = accrualAmount,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            Status = "Pending"
        };
    }

    public void MarkPosted()
    {
        Status = "Posted";
        UpdatedAt = DateTime.UtcNow;
    }
}
