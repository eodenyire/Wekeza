using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.ValueObjects;

public class RiskScore : ValueObject
{
    public decimal Score { get; }
    public RiskLevel Level { get; }
    public string Methodology { get; }
    public DateTime CalculatedAt { get; }
    public Dictionary<string, decimal> Factors { get; }

    public RiskScore(decimal score, string methodology = "STANDARD", Dictionary<string, decimal>? factors = null)
    {
        if (score < 0 || score > 100)
            throw new ArgumentException("Risk score must be between 0 and 100", nameof(score));

        if (string.IsNullOrWhiteSpace(methodology))
            throw new ArgumentException("Methodology cannot be empty", nameof(methodology));

        Score = Math.Round(score, 2);
        Level = DetermineRiskLevel(Score);
        Methodology = methodology.ToUpper();
        CalculatedAt = DateTime.UtcNow;
        Factors = factors ?? new Dictionary<string, decimal>();
    }

    private static RiskLevel DetermineRiskLevel(decimal score)
    {
        return score switch
        {
            >= 80 => RiskLevel.Critical,
            >= 60 => RiskLevel.High,
            >= 40 => RiskLevel.Medium,
            >= 20 => RiskLevel.Low,
            _ => RiskLevel.Minimal
        };
    }

    public bool IsHighRisk => Level >= RiskLevel.High;
    public bool IsCriticalRisk => Level == RiskLevel.Critical;
    public bool RequiresEscalation => Score >= 75;
    public bool RequiresEnhancedDueDiligence => Score >= 60;

    public RiskScore AdjustScore(decimal adjustment, string reason)
    {
        var newScore = Math.Max(0, Math.Min(100, Score + adjustment));
        var newFactors = new Dictionary<string, decimal>(Factors)
        {
            [reason] = adjustment
        };

        return new RiskScore(newScore, Methodology, newFactors);
    }

    public RiskScore CombineWith(RiskScore other, decimal weight = 0.5m)
    {
        if (weight < 0 || weight > 1)
            throw new ArgumentException("Weight must be between 0 and 1", nameof(weight));

        var combinedScore = (Score * weight) + (other.Score * (1 - weight));
        var combinedFactors = new Dictionary<string, decimal>(Factors);

        foreach (var factor in other.Factors)
        {
            if (combinedFactors.ContainsKey(factor.Key))
                combinedFactors[factor.Key] = (combinedFactors[factor.Key] + factor.Value) / 2;
            else
                combinedFactors[factor.Key] = factor.Value;
        }

        return new RiskScore(combinedScore, $"{Methodology}+{other.Methodology}", combinedFactors);
    }

    public string GetRiskDescription()
    {
        return Level switch
        {
            RiskLevel.Critical => "Critical risk - Immediate attention required",
            RiskLevel.High => "High risk - Enhanced monitoring required",
            RiskLevel.Medium => "Medium risk - Regular monitoring",
            RiskLevel.Low => "Low risk - Standard monitoring",
            RiskLevel.Minimal => "Minimal risk - Basic monitoring",
            _ => "Unknown risk level"
        };
    }

    public bool IsStale(TimeSpan maxAge)
    {
        return DateTime.UtcNow - CalculatedAt > maxAge;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Score;
        yield return Level;
        yield return Methodology;
        yield return CalculatedAt;
        foreach (var factor in Factors.OrderBy(f => f.Key))
        {
            yield return factor.Key;
            yield return factor.Value;
        }
    }

    public override string ToString()
    {
        return $"Risk Score: {Score:F2} ({Level}) - {Methodology} @ {CalculatedAt:yyyy-MM-dd HH:mm:ss}";
    }

    // Factory methods for common risk scenarios
    public static RiskScore ForNewCustomer(bool isPEP = false, string countryRisk = "LOW")
    {
        var factors = new Dictionary<string, decimal>
        {
            ["NEW_CUSTOMER"] = 10,
            ["COUNTRY_RISK"] = countryRisk.ToUpper() switch
            {
                "HIGH" => 20,
                "MEDIUM" => 10,
                _ => 0
            }
        };

        if (isPEP)
            factors["PEP_STATUS"] = 30;

        var totalScore = factors.Values.Sum();
        return new RiskScore(totalScore, "NEW_CUSTOMER", factors);
    }

    public static RiskScore ForTransaction(decimal amount, string transactionType, bool isCrossBorder = false)
    {
        var factors = new Dictionary<string, decimal>();

        // Amount-based risk
        if (amount >= 1_000_000) factors["HIGH_AMOUNT"] = 25;
        else if (amount >= 100_000) factors["MEDIUM_AMOUNT"] = 15;
        else if (amount >= 10_000) factors["REPORTABLE_AMOUNT"] = 10;

        // Transaction type risk
        factors["TRANSACTION_TYPE"] = transactionType.ToUpper() switch
        {
            "CASH" => 20,
            "WIRE" => 15,
            "CHECK" => 5,
            _ => 0
        };

        if (isCrossBorder)
            factors["CROSS_BORDER"] = 15;

        var totalScore = factors.Values.Sum();
        return new RiskScore(totalScore, "TRANSACTION", factors);
    }

    public static RiskScore ForAMLAlert(AMLAlertType alertType)
    {
        var score = alertType switch
        {
            AMLAlertType.SanctionsMatch => 95,
            AMLAlertType.StructuringActivity => 85,
            AMLAlertType.PEPActivity => 75,
            AMLAlertType.SuspiciousTransaction => 70,
            AMLAlertType.UnusualCashActivity => 65,
            AMLAlertType.RapidMovementOfFunds => 60,
            AMLAlertType.CrossBorderActivity => 55,
            AMLAlertType.ThresholdExceeded => 45,
            AMLAlertType.UnusualAccountActivity => 40,
            AMLAlertType.HighRiskCustomer => 35,
            _ => 30
        };

        var factors = new Dictionary<string, decimal>
        {
            [alertType.ToString()] = score
        };

        return new RiskScore(score, "AML_ALERT", factors);
    }
}

public enum RiskLevel
{
    Minimal = 1,
    Low = 2,
    Medium = 3,
    High = 4,
    Critical = 5
}