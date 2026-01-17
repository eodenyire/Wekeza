using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Product Aggregate - The heart of the Product Factory
/// Inspired by Finacle Product Factory and T24 ARRANGEMENT
/// This enables configuration-driven banking products instead of hardcoded logic
/// </summary>
public class Product : AggregateRoot
{
    public string ProductCode { get; private set; } // e.g., SAV001, CUR001, PL001
    public string ProductName { get; private set; }
    public ProductCategory Category { get; private set; }
    public ProductType Type { get; private set; }
    public ProductStatus Status { get; private set; }
    
    // Lifecycle
    public DateTime EffectiveDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }

    // Product Description
    public string Description { get; private set; }
    public string? MarketingDescription { get; private set; }
    public string Currency { get; private set; }

    // Interest Configuration
    public InterestConfiguration? InterestConfig { get; private set; }

    // Fee Configuration
    private readonly List<FeeConfiguration> _fees = new();
    public IReadOnlyCollection<FeeConfiguration> Fees => _fees.AsReadOnly();

    // Limit Configuration
    public LimitConfiguration? Limits { get; private set; }

    // Eligibility Rules
    private readonly List<EligibilityRule> _eligibilityRules = new();
    public IReadOnlyCollection<EligibilityRule> EligibilityRules => _eligibilityRules.AsReadOnly();

    // Product Features/Attributes (flexible JSON storage)
    private readonly Dictionary<string, string> _attributes = new();
    public IReadOnlyDictionary<string, string> Attributes => _attributes;

    // Accounting Configuration
    public AccountingConfiguration? AccountingConfig { get; private set; }

    private Product() : base(Guid.NewGuid()) { }

    public static Product CreateDepositProduct(
        string productCode,
        string productName,
        ProductType type,
        string currency,
        string createdBy)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            ProductCode = productCode,
            ProductName = productName,
            Category = ProductCategory.Deposits,
            Type = type,
            Status = ProductStatus.Draft,
            Currency = currency,
            EffectiveDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            Description = string.Empty
        };

        return product;
    }

    public static Product CreateLoanProduct(
        string productCode,
        string productName,
        ProductType type,
        string currency,
        string createdBy)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            ProductCode = productCode,
            ProductName = productName,
            Category = ProductCategory.Loans,
            Type = type,
            Status = ProductStatus.Draft,
            Currency = currency,
            EffectiveDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow,
            Description = string.Empty
        };

        return product;
    }

    public void UpdateDescription(string description, string? marketingDescription = null)
    {
        Description = description;
        MarketingDescription = marketingDescription;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void ConfigureInterest(InterestConfiguration config)
    {
        InterestConfig = config;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void AddFee(FeeConfiguration fee)
    {
        _fees.Add(fee);
        LastModifiedDate = DateTime.UtcNow;
    }

    public void RemoveFee(string feeCode)
    {
        var fee = _fees.FirstOrDefault(f => f.FeeCode == feeCode);
        if (fee != null)
        {
            _fees.Remove(fee);
            LastModifiedDate = DateTime.UtcNow;
        }
    }

    public void ConfigureLimits(LimitConfiguration limits)
    {
        Limits = limits;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void AddEligibilityRule(EligibilityRule rule)
    {
        _eligibilityRules.Add(rule);
        LastModifiedDate = DateTime.UtcNow;
    }

    public void SetAttribute(string key, string value)
    {
        _attributes[key] = value;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void ConfigureAccounting(AccountingConfiguration config)
    {
        AccountingConfig = config;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Activate(string activatedBy)
    {
        if (Status != ProductStatus.Draft && Status != ProductStatus.Inactive)
            throw new DomainException("Only draft or inactive products can be activated.");

        Status = ProductStatus.Active;
        LastModifiedBy = activatedBy;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Deactivate(string deactivatedBy, string reason)
    {
        Status = ProductStatus.Inactive;
        LastModifiedBy = deactivatedBy;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void Expire()
    {
        Status = ProductStatus.Expired;
        ExpiryDate = DateTime.UtcNow;
        LastModifiedDate = DateTime.UtcNow;
    }

    public bool IsEligible(Guid customerId, decimal amount, CustomerSegment segment, int customerAge)
    {
        if (Status != ProductStatus.Active)
            return false;

        if (DateTime.UtcNow < EffectiveDate || (ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate))
            return false;

        foreach (var rule in _eligibilityRules)
        {
            if (!rule.Evaluate(amount, segment, customerAge))
                return false;
        }

        return true;
    }

    public decimal CalculateInterest(decimal principal, int days)
    {
        if (InterestConfig == null)
            return 0;

        return InterestConfig.CalculateInterest(principal, days);
    }

    public decimal CalculateFee(string feeType, decimal amount)
    {
        var fee = _fees.FirstOrDefault(f => f.FeeType == feeType);
        if (fee == null)
            return 0;

        return fee.CalculateFee(amount);
    }
}

// Value Objects for Product Configuration

public record InterestConfiguration(
    InterestType Type,
    decimal Rate,
    InterestCalculationMethod CalculationMethod,
    InterestPostingFrequency PostingFrequency,
    bool IsTiered,
    List<InterestTier>? Tiers = null)
{
    public decimal CalculateInterest(decimal principal, int days)
    {
        if (IsTiered && Tiers != null)
        {
            var tier = Tiers.FirstOrDefault(t => principal >= t.MinBalance && principal <= t.MaxBalance);
            if (tier != null)
            {
                return CalculateSimpleInterest(principal, tier.Rate, days);
            }
        }

        return CalculateSimpleInterest(principal, Rate, days);
    }

    private decimal CalculateSimpleInterest(decimal principal, decimal rate, int days)
    {
        return CalculationMethod switch
        {
            InterestCalculationMethod.Simple => (principal * rate * days) / (100 * 365),
            InterestCalculationMethod.Compound => principal * (decimal)Math.Pow((double)(1 + rate / 100 / 365), days) - principal,
            InterestCalculationMethod.ReducingBalance => (principal * rate * days) / (100 * 365), // Simplified
            _ => 0
        };
    }
}

public record InterestTier(
    decimal MinBalance,
    decimal MaxBalance,
    decimal Rate);

public record FeeConfiguration(
    string FeeCode,
    string FeeType, // Opening, Maintenance, Transaction, Closure, etc.
    string FeeName,
    FeeCalculationType CalculationType,
    decimal Amount,
    decimal? Percentage = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    bool IsWaivable = true)
{
    public decimal CalculateFee(decimal transactionAmount)
    {
        decimal calculatedFee = CalculationType switch
        {
            FeeCalculationType.Flat => Amount,
            FeeCalculationType.Percentage => transactionAmount * (Percentage ?? 0) / 100,
            FeeCalculationType.Tiered => Amount, // Simplified, would need tier logic
            _ => 0
        };

        if (MinAmount.HasValue && calculatedFee < MinAmount.Value)
            calculatedFee = MinAmount.Value;

        if (MaxAmount.HasValue && calculatedFee > MaxAmount.Value)
            calculatedFee = MaxAmount.Value;

        return calculatedFee;
    }
}

public record LimitConfiguration(
    decimal? MinBalance,
    decimal? MaxBalance,
    decimal? MinTransactionAmount,
    decimal? MaxTransactionAmount,
    decimal? DailyTransactionLimit,
    int? MaxTransactionsPerDay,
    decimal? MonthlyTransactionLimit);

public record EligibilityRule(
    string RuleType, // MinAge, MaxAge, MinIncome, Segment, etc.
    string Operator, // GreaterThan, LessThan, Equals, In, etc.
    string Value)
{
    public bool Evaluate(decimal amount, CustomerSegment segment, int customerAge)
    {
        return RuleType switch
        {
            "MinAge" => customerAge >= int.Parse(Value),
            "MaxAge" => customerAge <= int.Parse(Value),
            "MinAmount" => amount >= decimal.Parse(Value),
            "MaxAmount" => amount <= decimal.Parse(Value),
            "Segment" => segment.ToString() == Value,
            _ => true
        };
    }
}

public record AccountingConfiguration(
    string AssetGLCode,
    string LiabilityGLCode,
    string IncomeGLCode,
    string ExpenseGLCode,
    string InterestPayableGLCode,
    string InterestReceivableGLCode);
