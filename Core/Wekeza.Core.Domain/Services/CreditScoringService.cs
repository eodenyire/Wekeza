using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// Credit Scoring Service - Risk assessment and credit decision engine
/// Inspired by Finacle Credit Scoring and T24 Credit Risk Assessment
/// Provides automated credit scoring, risk grading, and pricing decisions
/// </summary>
public class CreditScoringService
{
    private readonly IPartyRepository _partyRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILoanRepository _loanRepository;

    public CreditScoringService(
        IPartyRepository partyRepository,
        IAccountRepository accountRepository,
        ILoanRepository loanRepository)
    {
        _partyRepository = partyRepository;
        _accountRepository = accountRepository;
        _loanRepository = loanRepository;
    }

    /// <summary>
    /// Calculate comprehensive credit score for a customer
    /// </summary>
    public async Task<CreditScoreResult> CalculateCreditScoreAsync(Guid customerId, Money loanAmount)
    {
        // Get customer information
        var customer = await _partyRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new ArgumentException("Customer not found");

        var creditScore = 0m;
        var scoreFactors = new List<CreditScoreFactor>();

        // 1. Customer Profile Score (25% weight)
        var profileScore = CalculateProfileScore(customer);
        creditScore += profileScore * 0.25m;
        scoreFactors.Add(new CreditScoreFactor("Customer Profile", profileScore, 0.25m));

        // 2. Banking Relationship Score (30% weight)
        var relationshipScore = await CalculateRelationshipScoreAsync(customerId);
        creditScore += relationshipScore * 0.30m;
        scoreFactors.Add(new CreditScoreFactor("Banking Relationship", relationshipScore, 0.30m));

        // 3. Credit History Score (35% weight)
        var creditHistoryScore = await CalculateCreditHistoryScoreAsync(customerId);
        creditScore += creditHistoryScore * 0.35m;
        scoreFactors.Add(new CreditScoreFactor("Credit History", creditHistoryScore, 0.35m));

        // 4. Loan-to-Income Ratio (10% weight)
        var incomeScore = CalculateIncomeScore(customer, loanAmount);
        creditScore += incomeScore * 0.10m;
        scoreFactors.Add(new CreditScoreFactor("Income Assessment", incomeScore, 0.10m));

        // Normalize score to 0-1000 range
        creditScore = Math.Max(0, Math.Min(1000, creditScore));

        // Determine risk grade
        var riskGrade = DetermineRiskGrade(creditScore);

        // Calculate risk premium
        var riskPremium = CalculateRiskPremium(riskGrade);

        return new CreditScoreResult
        {
            CustomerId = customerId,
            CreditScore = creditScore,
            RiskGrade = riskGrade,
            RiskPremium = riskPremium,
            ScoreFactors = scoreFactors,
            CalculationDate = DateTime.UtcNow,
            IsApprovalRecommended = creditScore >= 600, // Minimum score for approval
            MaxLoanAmount = CalculateMaxLoanAmount(customer, creditScore),
            RecommendedInterestRate = CalculateRecommendedRate(riskGrade, 12.0m) // Base rate 12%
        };
    }

    /// <summary>
    /// Calculate customer profile score based on demographics and KYC
    /// </summary>
    private decimal CalculateProfileScore(Party customer)
    {
        var score = 500m; // Base score

        // Age factor
        var age = customer.DateOfBirth.HasValue ? 
            DateTime.UtcNow.Year - customer.DateOfBirth.Value.Year : 30;
        
        score += age switch
        {
            >= 25 and <= 35 => 50,  // Prime age
            >= 36 and <= 50 => 75,  // Established age
            >= 51 and <= 65 => 60,  // Pre-retirement
            _ => 0
        };

        // Employment/Business type (if available in customer data)
        if (customer.PartyType == PartyType.Corporate)
        {
            score += 100; // Corporate customers generally lower risk
        }

        // KYC status
        score += customer.KYCStatus switch
        {
            KYCStatus.Verified => 100,
            KYCStatus.Pending => 50,
            KYCStatus.Rejected => -200,
            _ => 0
        };

        // AML risk rating
        score += customer.AMLRiskRating switch
        {
            RiskRating.Low => 50,
            RiskRating.Medium => 0,
            RiskRating.High => -100,
            _ => 0
        };

        return Math.Max(0, Math.Min(1000, score));
    }

    /// <summary>
    /// Calculate banking relationship score
    /// </summary>
    private async Task<decimal> CalculateRelationshipScoreAsync(Guid customerId)
    {
        var score = 300m; // Base score for existing customer

        // Get customer accounts
        var accounts = await _accountRepository.GetByCustomerIdAsync(customerId);
        
        // Account tenure (longer relationship = better score)
        if (accounts.Any())
        {
            var oldestAccount = accounts.OrderBy(a => a.OpenedDate).First();
            var relationshipMonths = (DateTime.UtcNow - oldestAccount.OpenedDate).Days / 30;
            
            score += relationshipMonths switch
            {
                >= 12 => 100,  // 1+ years
                >= 6 => 75,    // 6+ months
                >= 3 => 50,    // 3+ months
                _ => 25
            };
        }

        // Account balances (higher balances = better score)
        var totalBalance = accounts.Sum(a => a.Balance.Amount);
        score += totalBalance switch
        {
            >= 100000 => 150,  // 100K+
            >= 50000 => 100,   // 50K+
            >= 10000 => 75,    // 10K+
            >= 1000 => 50,     // 1K+
            _ => 0
        };

        // Number of products (cross-selling indicator)
        var productCount = accounts.Select(a => a.ProductId).Distinct().Count();
        score += productCount * 25;

        return Math.Max(0, Math.Min(1000, score));
    }

    /// <summary>
    /// Calculate credit history score based on loan performance
    /// </summary>
    private async Task<decimal> CalculateCreditHistoryScoreAsync(Guid customerId)
    {
        var score = 600m; // Base score for no history

        var loans = await _loanRepository.GetByCustomerIdAsync(customerId);
        
        if (!loans.Any())
            return score; // No credit history

        // Loan performance analysis
        var totalLoans = loans.Count();
        var paidInFullLoans = loans.Count(l => l.Status == LoanStatus.PaidInFull);
        var activeLoans = loans.Count(l => l.Status == LoanStatus.Active);
        var defaultedLoans = loans.Count(l => l.Status == LoanStatus.WrittenOff);

        // Payment performance
        var paymentPerformanceScore = totalLoans > 0 ? 
            (decimal)paidInFullLoans / totalLoans * 200 : 0;
        score += paymentPerformanceScore;

        // Penalty for defaults
        score -= defaultedLoans * 150;

        // Current loan performance
        foreach (var activeLoan in loans.Where(l => l.Status == LoanStatus.Active))
        {
            score += activeLoan.DaysPastDue switch
            {
                0 => 50,           // Current
                <= 30 => 25,      // Slightly past due
                <= 90 => -25,     // Past due
                _ => -100          // Seriously delinquent
            };
        }

        return Math.Max(0, Math.Min(1000, score));
    }

    /// <summary>
    /// Calculate income-based score
    /// </summary>
    private decimal CalculateIncomeScore(Party customer, Money loanAmount)
    {
        var score = 500m; // Base score

        // This would typically integrate with income verification systems
        // For now, we'll use estimated income based on customer type
        var estimatedMonthlyIncome = customer.PartyType switch
        {
            PartyType.Individual => 50000m, // Estimated individual income
            PartyType.Corporate => 200000m,  // Estimated corporate income
            _ => 30000m
        };

        // Calculate debt-to-income ratio (simplified)
        var monthlyLoanPayment = loanAmount.Amount * 0.1m; // Rough estimate
        var debtToIncomeRatio = monthlyLoanPayment / estimatedMonthlyIncome;

        score += debtToIncomeRatio switch
        {
            <= 0.2m => 200,   // Excellent DTI
            <= 0.3m => 150,   // Good DTI
            <= 0.4m => 100,   // Fair DTI
            <= 0.5m => 50,    // Poor DTI
            _ => -100         // Unacceptable DTI
        };

        return Math.Max(0, Math.Min(1000, score));
    }

    /// <summary>
    /// Determine risk grade based on credit score
    /// </summary>
    public Enums.CreditRiskGrade DetermineRiskGrade(decimal creditScore)
    {
        return creditScore switch
        {
            >= 900 => Enums.CreditRiskGrade.AAA,
            >= 850 => Enums.CreditRiskGrade.AA,
            >= 800 => Enums.CreditRiskGrade.A,
            >= 750 => Enums.CreditRiskGrade.BBB,
            >= 700 => Enums.CreditRiskGrade.BB,
            >= 650 => Enums.CreditRiskGrade.B,
            >= 600 => Enums.CreditRiskGrade.CCC,
            >= 550 => Enums.CreditRiskGrade.CC,
            >= 500 => Enums.CreditRiskGrade.C,
            _ => Enums.CreditRiskGrade.D
        };
    }

    /// <summary>
    /// Calculate risk premium based on risk grade
    /// </summary>
    public decimal CalculateRiskPremium(Enums.CreditRiskGrade riskGrade)
    {
        return riskGrade switch
        {
            Enums.CreditRiskGrade.AAA => 0.0m,    // No premium
            Enums.CreditRiskGrade.AA => 0.5m,     // 0.5% premium
            Enums.CreditRiskGrade.A => 1.0m,      // 1.0% premium
            Enums.CreditRiskGrade.BBB => 1.5m,    // 1.5% premium
            Enums.CreditRiskGrade.BB => 2.5m,     // 2.5% premium
            Enums.CreditRiskGrade.B => 4.0m,      // 4.0% premium
            Enums.CreditRiskGrade.CCC => 6.0m,    // 6.0% premium
            Enums.CreditRiskGrade.CC => 8.0m,     // 8.0% premium
            Enums.CreditRiskGrade.C => 12.0m,     // 12.0% premium
            Enums.CreditRiskGrade.D => 20.0m,     // 20.0% premium (likely declined)
            _ => 10.0m
        };
    }

    /// <summary>
    /// Calculate maximum loan amount based on customer profile and credit score
    /// </summary>
    private Money CalculateMaxLoanAmount(Party customer, decimal creditScore)
    {
        var baseAmount = customer.PartyType switch
        {
            PartyType.Individual => 500000m,   // 500K for individuals
            PartyType.Corporate => 5000000m,   // 5M for corporates
            _ => 100000m
        };

        // Adjust based on credit score
        var scoreMultiplier = creditScore / 1000m;
        var maxAmount = baseAmount * scoreMultiplier;

        return new Money(maxAmount, new Currency("KES"));
    }

    /// <summary>
    /// Calculate recommended interest rate
    /// </summary>
    private decimal CalculateRecommendedRate(Enums.CreditRiskGrade riskGrade, decimal baseRate)
    {
        var riskPremium = CalculateRiskPremium(riskGrade);
        return baseRate + riskPremium;
    }
}

/// <summary>
/// Credit score calculation result
/// </summary>
public class CreditScoreResult
{
    public Guid CustomerId { get; set; }
    public decimal CreditScore { get; set; }
    public Enums.CreditRiskGrade RiskGrade { get; set; }
    public decimal RiskPremium { get; set; }
    public List<CreditScoreFactor> ScoreFactors { get; set; } = new();
    public DateTime CalculationDate { get; set; }
    public bool IsApprovalRecommended { get; set; }
    public Money MaxLoanAmount { get; set; } = Money.Zero(new Currency("KES"));
    public decimal RecommendedInterestRate { get; set; }
    public string? Comments { get; set; }
}

/// <summary>
/// Individual credit score factor
/// </summary>
public record CreditScoreFactor(
    string FactorName,
    decimal Score,
    decimal Weight);

/// <summary>
/// Credit decision result
/// </summary>
public enum CreditDecision
{
    Approved,
    Declined,
    Referred,
    ConditionalApproval
}