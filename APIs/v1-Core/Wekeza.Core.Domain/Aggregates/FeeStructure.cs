using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Fee Structure Aggregate - Manages product fees and charges
/// Enables flexible fee configuration per product
/// </summary>
public class FeeStructure : AggregateRoot
{
    public string FeeName { get; private set; }
    public string FeeCode { get; private set; }
    public Guid ProductId { get; private set; }
    public string FeeType { get; private set; } // Fixed, Variable, Percentage
    public decimal Amount { get; private set; }
    public decimal? Percentage { get; private set; }
    public string Currency { get; private set; }
    public string ChargeMethod { get; private set; } // Upfront, Monthly, Annual, etc.
    public string Status { get; private set; } // Active, Inactive, Archived
    
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    
    // Waiver Configuration
    public bool IsWaivable { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private FeeStructure() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static FeeStructure Create(
        string feeName,
        string feeCode,
        Guid productId,
        string feeType,
        decimal amount,
        string currency,
        string createdBy)
    {
        var fee = new FeeStructure
        {
            Id = Guid.NewGuid(),
            FeeName = feeName,
            FeeCode = feeCode,
            ProductId = productId,
            FeeType = feeType,
            Amount = amount,
            Currency = currency,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            EffectiveDate = DateTime.UtcNow,
            IsWaivable = false,
            Metadata = new Dictionary<string, object>()
        };

        return fee;
    }

    public void UpdateAmount(decimal amount, string updatedBy)
    {
        Amount = amount;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate(string deactivatedBy)
    {
        Status = "Inactive";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deactivatedBy;
    }
}
