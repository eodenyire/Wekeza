namespace Wekeza.Core.Domain.ValueObjects;
///<summary>
/// 4. InterestRate.cs (The Quantitative Engine)
/// Banking is about yield. This object encapsulates the difference between APR (Nominal) and APY (Effective) rates.
///</summary>

public record InterestRate
{
    public decimal Percentage { get; init; }
    public decimal DecimalValue => Percentage / 100;

    public InterestRate(decimal percentage)
    {
        if (percentage < 0) throw new ArgumentException("Interest rate cannot be negative.");
        Percentage = percentage;
    }

    /// <summary>
    /// Calculates Annual Percentage Yield (APY) based on compounding frequency.
    /// Formula: APY = (1 + r/n)^n – 1
    /// </summary>
    public decimal CalculateApy(int compoundingPeriodsPerYear)
    {
        var ratePerPeriod = (double)DecimalValue / compoundingPeriodsPerYear;
        var apy = Math.Pow(1 + ratePerPeriod, compoundingPeriodsPerYear) - 1;
        return (decimal)apy;
    }
}
