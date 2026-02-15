using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.ValueObjects;

/// <summary>
/// Report Metrics Value Object - Represents financial and operational metrics for reports
/// Immutable value object containing key banking metrics and ratios
/// Industry Standard: Basel III, IFRS, and Central Bank reporting requirements
/// </summary>
public class ReportMetrics : ValueObject
{
    // Balance Sheet Metrics
    public decimal TotalAssets { get; private set; }
    public decimal TotalLiabilities { get; private set; }
    public decimal TotalEquity { get; private set; }
    public decimal CustomerDeposits { get; private set; }
    public decimal LoanPortfolio { get; private set; }
    public decimal CashAndEquivalents { get; private set; }
    public decimal Investments { get; private set; }

    // Income Statement Metrics
    public decimal NetIncome { get; private set; }
    public decimal OperatingIncome { get; private set; }
    public decimal InterestIncome { get; private set; }
    public decimal InterestExpense { get; private set; }
    public decimal NetInterestIncome { get; private set; }
    public decimal NonInterestIncome { get; private set; }
    public decimal OperatingExpenses { get; private set; }
    public decimal ProvisionForLosses { get; private set; }

    // Asset Quality Metrics
    public decimal NonPerformingLoans { get; private set; }
    public decimal GrossNPLRatio { get; private set; }
    public decimal NetNPLRatio { get; private set; }
    public decimal LoanLossProvisions { get; private set; }
    public decimal CoverageRatio { get; private set; }
    public decimal ChargeOffAmount { get; private set; }

    // Capital Adequacy Metrics
    public decimal Tier1Capital { get; private set; }
    public decimal Tier2Capital { get; private set; }
    public decimal TotalCapital { get; private set; }
    public decimal RiskWeightedAssets { get; private set; }
    public decimal CapitalAdequacyRatio { get; private set; }
    public decimal Tier1CapitalRatio { get; private set; }
    public decimal LeverageRatio { get; private set; }

    // Liquidity Metrics
    public decimal LiquidAssets { get; private set; }
    public decimal ShortTermLiabilities { get; private set; }
    public decimal LiquidityRatio { get; private set; }
    public decimal LiquidityCoverageRatio { get; private set; }
    public decimal NetStableFundingRatio { get; private set; }
    public decimal DepositToLoanRatio { get; private set; }

    // Profitability Metrics
    public decimal ReturnOnAssets { get; private set; }
    public decimal ReturnOnEquity { get; private set; }
    public decimal NetInterestMargin { get; private set; }
    public decimal CostToIncomeRatio { get; private set; }
    public decimal OperatingMargin { get; private set; }
    public decimal EfficiencyRatio { get; private set; }

    // Operational Metrics
    public int TotalCustomers { get; private set; }
    public int ActiveAccounts { get; private set; }
    public int TotalTransactions { get; private set; }
    public decimal TransactionVolume { get; private set; }
    public int NumberOfBranches { get; private set; }
    public int NumberOfATMs { get; private set; }
    public int NumberOfEmployees { get; private set; }

    // Market Risk Metrics
    public decimal ValueAtRisk { get; private set; }
    public decimal InterestRateRisk { get; private set; }
    public decimal ForeignExchangeRisk { get; private set; }
    public decimal TradingBookValue { get; private set; }
    public decimal BankingBookValue { get; private set; }

    // Parameterless constructor for EF Core
    private ReportMetrics() { }

    public ReportMetrics(
        // Balance Sheet
        decimal totalAssets,
        decimal totalLiabilities,
        decimal totalEquity,
        decimal customerDeposits,
        decimal loanPortfolio,
        decimal cashAndEquivalents,
        decimal investments,
        
        // Income Statement
        decimal netIncome,
        decimal operatingIncome,
        decimal interestIncome,
        decimal interestExpense,
        decimal nonInterestIncome,
        decimal operatingExpenses,
        decimal provisionForLosses,
        
        // Asset Quality
        decimal nonPerformingLoans,
        decimal loanLossProvisions,
        decimal chargeOffAmount,
        
        // Capital
        decimal tier1Capital,
        decimal tier2Capital,
        decimal riskWeightedAssets,
        
        // Liquidity
        decimal liquidAssets,
        decimal shortTermLiabilities,
        
        // Operational
        int totalCustomers = 0,
        int activeAccounts = 0,
        int totalTransactions = 0,
        decimal transactionVolume = 0,
        int numberOfBranches = 0,
        int numberOfATMs = 0,
        int numberOfEmployees = 0,
        
        // Market Risk
        decimal valueAtRisk = 0,
        decimal interestRateRisk = 0,
        decimal foreignExchangeRisk = 0,
        decimal tradingBookValue = 0,
        decimal bankingBookValue = 0)
    {
        // Validation
        if (totalAssets < 0) throw new ArgumentException("Total assets cannot be negative");
        if (totalLiabilities < 0) throw new ArgumentException("Total liabilities cannot be negative");
        if (totalEquity < 0) throw new ArgumentException("Total equity cannot be negative");
        if (customerDeposits < 0) throw new ArgumentException("Customer deposits cannot be negative");
        if (loanPortfolio < 0) throw new ArgumentException("Loan portfolio cannot be negative");

        // Basic validation: Assets = Liabilities + Equity (with small tolerance for rounding)
        var balanceSheetDifference = Math.Abs(totalAssets - (totalLiabilities + totalEquity));
        if (balanceSheetDifference > 1000) // Allow 1000 units tolerance
            throw new ArgumentException("Balance sheet does not balance: Assets must equal Liabilities + Equity");

        // Assign values
        TotalAssets = totalAssets;
        TotalLiabilities = totalLiabilities;
        TotalEquity = totalEquity;
        CustomerDeposits = customerDeposits;
        LoanPortfolio = loanPortfolio;
        CashAndEquivalents = cashAndEquivalents;
        Investments = investments;

        NetIncome = netIncome;
        OperatingIncome = operatingIncome;
        InterestIncome = interestIncome;
        InterestExpense = interestExpense;
        NetInterestIncome = interestIncome - interestExpense;
        NonInterestIncome = nonInterestIncome;
        OperatingExpenses = operatingExpenses;
        ProvisionForLosses = provisionForLosses;

        NonPerformingLoans = nonPerformingLoans;
        LoanLossProvisions = loanLossProvisions;
        ChargeOffAmount = chargeOffAmount;

        Tier1Capital = tier1Capital;
        Tier2Capital = tier2Capital;
        TotalCapital = tier1Capital + tier2Capital;
        RiskWeightedAssets = riskWeightedAssets;

        LiquidAssets = liquidAssets;
        ShortTermLiabilities = shortTermLiabilities;

        TotalCustomers = totalCustomers;
        ActiveAccounts = activeAccounts;
        TotalTransactions = totalTransactions;
        TransactionVolume = transactionVolume;
        NumberOfBranches = numberOfBranches;
        NumberOfATMs = numberOfATMs;
        NumberOfEmployees = numberOfEmployees;

        ValueAtRisk = valueAtRisk;
        InterestRateRisk = interestRateRisk;
        ForeignExchangeRisk = foreignExchangeRisk;
        TradingBookValue = tradingBookValue;
        BankingBookValue = bankingBookValue;

        // Calculate derived ratios
        GrossNPLRatio = CalculateGrossNPLRatio();
        NetNPLRatio = CalculateNetNPLRatio();
        CoverageRatio = CalculateCoverageRatio();
        CapitalAdequacyRatio = CalculateCapitalAdequacyRatio();
        Tier1CapitalRatio = CalculateTier1CapitalRatio();
        LeverageRatio = CalculateLeverageRatio();
        LiquidityRatio = CalculateLiquidityRatio();
        LiquidityCoverageRatio = CalculateLiquidityCoverageRatio();
        NetStableFundingRatio = CalculateNetStableFundingRatio();
        DepositToLoanRatio = CalculateDepositToLoanRatio();
        ReturnOnAssets = CalculateReturnOnAssets();
        ReturnOnEquity = CalculateReturnOnEquity();
        NetInterestMargin = CalculateNetInterestMargin();
        CostToIncomeRatio = CalculateCostToIncomeRatio();
        OperatingMargin = CalculateOperatingMargin();
        EfficiencyRatio = CalculateEfficiencyRatio();
    }

    /// <summary>
    /// Calculate Gross NPL Ratio (NPL / Total Loans)
    /// </summary>
    public decimal CalculateGrossNPLRatio()
    {
        return LoanPortfolio > 0 ? Math.Round((NonPerformingLoans / LoanPortfolio) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Net NPL Ratio ((NPL - Provisions) / Total Loans)
    /// </summary>
    public decimal CalculateNetNPLRatio()
    {
        var netNPL = Math.Max(0, NonPerformingLoans - LoanLossProvisions);
        return LoanPortfolio > 0 ? Math.Round((netNPL / LoanPortfolio) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Coverage Ratio (Provisions / NPL)
    /// </summary>
    public decimal CalculateCoverageRatio()
    {
        return NonPerformingLoans > 0 ? Math.Round((LoanLossProvisions / NonPerformingLoans) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Capital Adequacy Ratio (Total Capital / Risk Weighted Assets)
    /// Basel III minimum: 8%
    /// </summary>
    public decimal CalculateCapitalAdequacyRatio()
    {
        return RiskWeightedAssets > 0 ? Math.Round((TotalCapital / RiskWeightedAssets) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Tier 1 Capital Ratio (Tier 1 Capital / Risk Weighted Assets)
    /// Basel III minimum: 6%
    /// </summary>
    public decimal CalculateTier1CapitalRatio()
    {
        return RiskWeightedAssets > 0 ? Math.Round((Tier1Capital / RiskWeightedAssets) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Leverage Ratio (Tier 1 Capital / Total Assets)
    /// Basel III minimum: 3%
    /// </summary>
    public decimal CalculateLeverageRatio()
    {
        return TotalAssets > 0 ? Math.Round((Tier1Capital / TotalAssets) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Liquidity Ratio (Liquid Assets / Short Term Liabilities)
    /// </summary>
    public decimal CalculateLiquidityRatio()
    {
        return ShortTermLiabilities > 0 ? Math.Round((LiquidAssets / ShortTermLiabilities) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Liquidity Coverage Ratio (LCR)
    /// Basel III minimum: 100%
    /// </summary>
    public decimal CalculateLiquidityCoverageRatio()
    {
        // Simplified calculation - in practice this is more complex
        return ShortTermLiabilities > 0 ? Math.Round((LiquidAssets / ShortTermLiabilities) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Net Stable Funding Ratio (NSFR)
    /// Basel III minimum: 100%
    /// </summary>
    public decimal CalculateNetStableFundingRatio()
    {
        // Simplified calculation - in practice this requires stable funding analysis
        var stableFunding = TotalEquity + (CustomerDeposits * 0.85m); // Assume 85% of deposits are stable
        var requiredStableFunding = LoanPortfolio * 0.85m; // Assume 85% RSF factor for loans
        return requiredStableFunding > 0 ? Math.Round((stableFunding / requiredStableFunding) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Deposit to Loan Ratio
    /// </summary>
    public decimal CalculateDepositToLoanRatio()
    {
        return LoanPortfolio > 0 ? Math.Round((CustomerDeposits / LoanPortfolio) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Return on Assets (ROA) - Net Income / Total Assets
    /// </summary>
    public decimal CalculateReturnOnAssets()
    {
        return TotalAssets > 0 ? Math.Round((NetIncome / TotalAssets) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Return on Equity (ROE) - Net Income / Total Equity
    /// </summary>
    public decimal CalculateReturnOnEquity()
    {
        return TotalEquity > 0 ? Math.Round((NetIncome / TotalEquity) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Net Interest Margin (NIM) - Net Interest Income / Total Assets
    /// </summary>
    public decimal CalculateNetInterestMargin()
    {
        return TotalAssets > 0 ? Math.Round((NetInterestIncome / TotalAssets) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Cost to Income Ratio - Operating Expenses / Operating Income
    /// </summary>
    public decimal CalculateCostToIncomeRatio()
    {
        var totalIncome = InterestIncome + NonInterestIncome;
        return totalIncome > 0 ? Math.Round((OperatingExpenses / totalIncome) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Operating Margin - Operating Income / Total Income
    /// </summary>
    public decimal CalculateOperatingMargin()
    {
        var totalIncome = InterestIncome + NonInterestIncome;
        return totalIncome > 0 ? Math.Round((OperatingIncome / totalIncome) * 100, 2) : 0;
    }

    /// <summary>
    /// Calculate Efficiency Ratio - Operating Expenses / (Interest Income + Non-Interest Income)
    /// Lower is better
    /// </summary>
    public decimal CalculateEfficiencyRatio()
    {
        var totalRevenue = InterestIncome + NonInterestIncome;
        return totalRevenue > 0 ? Math.Round((OperatingExpenses / totalRevenue) * 100, 2) : 0;
    }

    /// <summary>
    /// Check if bank meets Basel III capital requirements
    /// </summary>
    public bool MeetsBaselIIIRequirements()
    {
        return CapitalAdequacyRatio >= 8.0m && 
               Tier1CapitalRatio >= 6.0m && 
               LeverageRatio >= 3.0m &&
               LiquidityCoverageRatio >= 100.0m &&
               NetStableFundingRatio >= 100.0m;
    }

    /// <summary>
    /// Get capital buffer above minimum requirements
    /// </summary>
    public decimal GetCapitalBuffer()
    {
        return Math.Max(0, CapitalAdequacyRatio - 8.0m);
    }

    /// <summary>
    /// Check if bank is well-capitalized (above regulatory minimums)
    /// </summary>
    public bool IsWellCapitalized()
    {
        return CapitalAdequacyRatio >= 10.0m && Tier1CapitalRatio >= 8.0m;
    }

    /// <summary>
    /// Get asset quality rating based on NPL ratio
    /// </summary>
    public string GetAssetQualityRating()
    {
        return GrossNPLRatio switch
        {
            <= 2.0m => "Excellent",
            <= 5.0m => "Good",
            <= 10.0m => "Satisfactory",
            <= 15.0m => "Needs Improvement",
            _ => "Poor"
        };
    }

    /// <summary>
    /// Get profitability rating based on ROE
    /// </summary>
    public string GetProfitabilityRating()
    {
        return ReturnOnEquity switch
        {
            >= 15.0m => "Excellent",
            >= 10.0m => "Good",
            >= 5.0m => "Satisfactory",
            >= 0.0m => "Needs Improvement",
            _ => "Poor"
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalAssets;
        yield return TotalLiabilities;
        yield return TotalEquity;
        yield return NetIncome;
        yield return NonPerformingLoans;
        yield return TotalCapital;
        yield return RiskWeightedAssets;
        yield return LiquidAssets;
        yield return CustomerDeposits;
        yield return LoanPortfolio;
    }

    public override string ToString()
    {
        return $"Assets: {TotalAssets:C0}, Equity: {TotalEquity:C0}, ROE: {ReturnOnEquity:F2}%, CAR: {CapitalAdequacyRatio:F2}%, NPL: {GrossNPLRatio:F2}%";
    }
}