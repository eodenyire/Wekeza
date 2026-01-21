using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Exceptions;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Enhanced Loan Aggregate - Complete loan lifecycle management
/// Inspired by Finacle LMS and T24 AA LOANS
/// Manages the entire journey: Application â†’ Credit Scoring â†’ Approval â†’ Disbursement â†’ Servicing â†’ Closure
/// </summary>
public class Loan : AggregateRoot
{
    // Basic loan information
    public string LoanNumber { get; private set; } // Unique loan identifier
    public Guid CustomerId { get; private set; }
    public Guid ProductId { get; private set; } // Product Factory integration
    public Guid? DisbursementAccountId { get; private set; }
    
    // Loan amounts and terms
    public Money Principal { get; private set; }
    public Money OutstandingPrincipal { get; private set; }
    public decimal InterestRate { get; private set; } // Annual percentage rate
    public int TermInMonths { get; private set; }
    public DateTime? FirstPaymentDate { get; private set; }
    public DateTime? MaturityDate { get; private set; }
    
    // Computed properties for compatibility
    public string LoanType => Product?.Type.ToString() ?? "Unknown";
    public Money PrincipalAmount => Principal;
    public Money OutstandingBalance => OutstandingPrincipal;
    public DateTime? NextPaymentDate => _schedule.Where(s => !s.IsPaid).OrderBy(s => s.DueDate).FirstOrDefault()?.DueDate;
    
    // Loan status and lifecycle
    public LoanStatus Status { get; private set; }
    public LoanSubStatus SubStatus { get; private set; }
    public DateTime ApplicationDate { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public DateTime? DisbursementDate { get; private set; }
    public DateTime? ClosureDate { get; private set; }
    
    // Credit assessment
    public decimal? CreditScore { get; private set; }
    public CreditRiskGrade? RiskGrade { get; private set; }
    public decimal? RiskPremium { get; private set; } // Additional rate based on risk
    
    // Interest and fees
    public Money AccruedInterest { get; private set; }
    public Money TotalInterestPaid { get; private set; }
    public Money TotalFeesPaid { get; private set; }
    public DateTime LastInterestCalculationDate { get; private set; }
    
    // Payment tracking
    public Money TotalAmountPaid { get; private set; }
    public DateTime? LastPaymentDate { get; private set; }
    public int DaysPastDue { get; private set; }
    public Money PastDueAmount { get; private set; }
    
    // Collateral and guarantees
    private readonly List<LoanCollateral> _collaterals = new();
    public IReadOnlyCollection<LoanCollateral> Collaterals => _collaterals.AsReadOnly();
    
    private readonly List<LoanGuarantor> _guarantors = new();
    public IReadOnlyCollection<LoanGuarantor> Guarantors => _guarantors.AsReadOnly();
    
    // Repayment schedule
    private readonly List<LoanScheduleItem> _schedule = new();
    public IReadOnlyCollection<LoanScheduleItem> Schedule => _schedule.AsReadOnly();
    
    // Loan conditions and covenants
    private readonly List<LoanCondition> _conditions = new();
    public IReadOnlyCollection<LoanCondition> Conditions => _conditions.AsReadOnly();
    
    // GL Integration
    public string LoanGLCode { get; private set; } // GL account for this loan
    public string InterestReceivableGLCode { get; private set; }
    
    // Provisioning
    public decimal ProvisionRate { get; private set; }
    public Money ProvisionAmount { get; private set; }
    public DateTime? LastProvisionDate { get; private set; }
    
    // Audit trail
    public string CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? DisbursedBy { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }
    
    // Navigation properties
    public Customer? Customer { get; private set; }
    public Product? Product { get; private set; }
    public Account? DisbursementAccount { get; private set; }

    private Loan() : base(Guid.NewGuid()) { }

    public static Loan CreateApplication(
        Guid customerId,
        Guid productId,
        Money principal,
        int termInMonths,
        string createdBy,
        Product product)
    {
        var loanNumber = GenerateLoanNumber(product);
        var currency = principal.Currency;
        
        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            LoanNumber = loanNumber,
            CustomerId = customerId,
            ProductId = productId,
            Principal = principal,
            OutstandingPrincipal = principal,
            TermInMonths = termInMonths,
            Status = LoanStatus.Applied,
            SubStatus = LoanSubStatus.PendingDocuments,
            ApplicationDate = DateTime.UtcNow,
            AccruedInterest = Money.Zero(currency),
            TotalInterestPaid = Money.Zero(currency),
            TotalFeesPaid = Money.Zero(currency),
            TotalAmountPaid = Money.Zero(currency),
            PastDueAmount = Money.Zero(currency),
            ProvisionAmount = Money.Zero(currency),
            LastInterestCalculationDate = DateTime.UtcNow.Date,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow
        };

        // Apply product configuration
        loan.ApplyProductConfiguration(product);
        
        loan.AddDomainEvent(new LoanApplicationCreatedDomainEvent(loan.Id, loan.LoanNumber, customerId, principal));
        return loan;
    }

    private void ApplyProductConfiguration(Product product)
    {
        // Apply interest rate from product
        if (product.InterestConfig != null)
        {
            InterestRate = product.InterestConfig.Rate;
        }

        // Apply GL codes from product accounting configuration
        if (product.AccountingConfig != null)
        {
            LoanGLCode = product.AccountingConfig.AssetGLCode;
            InterestReceivableGLCode = product.AccountingConfig.InterestReceivableGLCode;
        }
    }

    public void UpdateCreditAssessment(decimal creditScore, CreditRiskGrade riskGrade, decimal riskPremium, string assessedBy)
    {
        if (Status != LoanStatus.Applied)
            throw new GenericDomainException("Credit assessment can only be updated for applied loans");

        CreditScore = creditScore;
        RiskGrade = riskGrade;
        RiskPremium = riskPremium;
        
        // Adjust interest rate based on risk
        InterestRate += riskPremium;
        
        SubStatus = LoanSubStatus.CreditAssessed;
        LastModifiedBy = assessedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new LoanCreditAssessedDomainEvent(Id, LoanNumber, creditScore, riskGrade));
    }

    public void Approve(string approvedBy, DateTime? firstPaymentDate = null, List<LoanCondition>? conditions = null)
    {
        if (Status != LoanStatus.Applied)
            throw new GenericDomainException("Only applied loans can be approved");

        if (!CreditScore.HasValue)
            throw new GenericDomainException("Loan must have credit assessment before approval");

        Status = LoanStatus.Approved;
        SubStatus = LoanSubStatus.AwaitingDisbursement;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        FirstPaymentDate = firstPaymentDate ?? DateTime.UtcNow.AddMonths(1).Date;
        MaturityDate = FirstPaymentDate.Value.AddMonths(TermInMonths - 1);
        LastModifiedBy = approvedBy;
        LastModifiedDate = DateTime.UtcNow;

        // Add conditions if provided
        if (conditions != null)
        {
            _conditions.AddRange(conditions);
        }

        // Generate repayment schedule
        GenerateRepaymentSchedule();

        AddDomainEvent(new LoanApprovedDomainEvent(Id, LoanNumber, Principal, approvedBy));
    }

    public void Reject(string rejectedBy, string reason)
    {
        if (Status != LoanStatus.Applied)
            throw new GenericDomainException("Only applied loans can be rejected");

        Status = LoanStatus.Rejected;
        SubStatus = LoanSubStatus.Closed;
        ClosureDate = DateTime.UtcNow;
        LastModifiedBy = rejectedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new LoanRejectedDomainEvent(Id, LoanNumber, reason, rejectedBy));
    }

    public void Disburse(Guid disbursementAccountId, string disbursedBy, DateTime? disbursementDate = null)
    {
        if (Status != LoanStatus.Approved)
            throw new GenericDomainException("Only approved loans can be disbursed");

        DisbursementAccountId = disbursementAccountId;
        Status = LoanStatus.Active;
        SubStatus = LoanSubStatus.Current;
        DisbursementDate = disbursementDate ?? DateTime.UtcNow;
        DisbursedBy = disbursedBy;
        LastModifiedBy = disbursedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new LoanDisbursedEvent(Id, LoanNumber, disbursementAccountId, Principal, DisbursementDate.Value));
    }

    public void ProcessRepayment(Money paymentAmount, DateTime paymentDate, string processedBy, string? paymentReference = null)
    {
        if (Status != LoanStatus.Active)
            throw new GenericDomainException("Can only process payments for active loans");

        if (paymentAmount.IsZero() || paymentAmount.IsNegative())
            throw new GenericDomainException("Payment amount must be positive");

        // Allocate payment: Interest first, then principal
        var interestPayment = Money.Min(paymentAmount, AccruedInterest);
        var principalPayment = paymentAmount - interestPayment;

        // Update balances
        AccruedInterest -= interestPayment;
        OutstandingPrincipal -= principalPayment;
        TotalAmountPaid += paymentAmount;
        TotalInterestPaid += interestPayment;
        LastPaymentDate = paymentDate;

        // Update schedule
        UpdateScheduleWithPayment(paymentAmount, paymentDate);

        // Update past due status
        UpdatePastDueStatus(paymentDate);

        // Check if loan is fully paid
        if (OutstandingPrincipal.IsZero() && AccruedInterest.IsZero())
        {
            Status = LoanStatus.PaidInFull;
            SubStatus = LoanSubStatus.Closed;
            ClosureDate = paymentDate;
        }

        LastModifiedBy = processedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new LoanRepaymentProcessedDomainEvent(Id, LoanNumber, paymentAmount, paymentDate, paymentReference));
    }

    public void AccrueInterest(DateTime calculationDate)
    {
        if (Status != LoanStatus.Active)
            return;

        var daysSinceLastCalculation = (calculationDate - LastInterestCalculationDate).Days;
        if (daysSinceLastCalculation <= 0)
            return;

        // Calculate daily interest
        var dailyRate = InterestRate / 100 / 365;
        var interestAmount = OutstandingPrincipal.Amount * (decimal)dailyRate * daysSinceLastCalculation;
        var interest = new Money(interestAmount, Principal.Currency);

        AccruedInterest += interest;
        LastInterestCalculationDate = calculationDate;

        if (interest.Amount > 0)
        {
            AddDomainEvent(new LoanInterestAccruedDomainEvent(Id, LoanNumber, interest, calculationDate));
        }
    }

    public void AddCollateral(LoanCollateral collateral)
    {
        _collaterals.Add(collateral);
        AddDomainEvent(new LoanCollateralAddedDomainEvent(Id, LoanNumber, collateral.CollateralType, collateral.Value));
    }

    public void AddGuarantor(LoanGuarantor guarantor)
    {
        _guarantors.Add(guarantor);
        AddDomainEvent(new LoanGuarantorAddedDomainEvent(Id, LoanNumber, guarantor.GuarantorId, guarantor.GuaranteeAmount));
    }

    public void Restructure(LoanRestructureRequest request, string restructuredBy)
    {
        if (Status != LoanStatus.Active)
            throw new GenericDomainException("Only active loans can be restructured");

        // Apply restructuring changes
        if (request.NewInterestRate.HasValue)
            InterestRate = request.NewInterestRate.Value;

        if (request.NewTermInMonths.HasValue)
        {
            TermInMonths = request.NewTermInMonths.Value;
            MaturityDate = FirstPaymentDate?.AddMonths(TermInMonths - 1);
        }

        if (request.PrincipalMoratorium.HasValue)
        {
            // Handle principal moratorium logic
        }

        SubStatus = LoanSubStatus.Restructured;
        LastModifiedBy = restructuredBy;
        LastModifiedDate = DateTime.UtcNow;

        // Regenerate schedule
        GenerateRepaymentSchedule();

        AddDomainEvent(new LoanRestructuredDomainEvent(Id, LoanNumber, request.Reason, restructuredBy));
    }

    public void CalculateProvision(DateTime calculationDate)
    {
        if (Status != LoanStatus.Active)
            return;

        // Calculate provision based on days past due
        var newProvisionRate = DaysPastDue switch
        {
            >= 0 and <= 30 => 0.01m,      // 1% for current
            >= 31 and <= 90 => 0.05m,     // 5% for 31-90 days
            >= 91 and <= 180 => 0.20m,    // 20% for 91-180 days
            >= 181 and <= 365 => 0.50m,   // 50% for 181-365 days
            > 365 => 1.00m                 // 100% for over 1 year
        };

        if (newProvisionRate != ProvisionRate)
        {
            ProvisionRate = newProvisionRate;
            var newProvisionAmount = new Money(OutstandingPrincipal.Amount * ProvisionRate, Principal.Currency);
            var provisionChange = newProvisionAmount - ProvisionAmount;
            
            ProvisionAmount = newProvisionAmount;
            LastProvisionDate = calculationDate;

            if (!provisionChange.IsZero())
            {
                AddDomainEvent(new LoanProvisionCalculatedDomainEvent(Id, LoanNumber, ProvisionAmount, provisionChange));
            }
        }
    }

    private void GenerateRepaymentSchedule()
    {
        _schedule.Clear();

        if (!FirstPaymentDate.HasValue)
            return;

        // Calculate monthly payment using amortization formula
        var monthlyRate = (double)(InterestRate / 100 / 12);
        var numPayments = TermInMonths;
        var loanAmount = (double)Principal.Amount;

        double monthlyPayment;
        if (monthlyRate == 0)
        {
            monthlyPayment = loanAmount / numPayments;
        }
        else
        {
            monthlyPayment = loanAmount * (monthlyRate * Math.Pow(1 + monthlyRate, numPayments)) / 
                           (Math.Pow(1 + monthlyRate, numPayments) - 1);
        }

        var currentDate = FirstPaymentDate.Value;
        var remainingPrincipal = (double)Principal.Amount;

        for (int i = 1; i <= TermInMonths; i++)
        {
            var interestPayment = remainingPrincipal * monthlyRate;
            var principalPayment = monthlyPayment - interestPayment;
            remainingPrincipal -= principalPayment;

            _schedule.Add(new LoanScheduleItem(
                ScheduleNumber: i,
                DueDate: currentDate,
                PrincipalAmount: new Money((decimal)principalPayment, Principal.Currency),
                InterestAmount: new Money((decimal)interestPayment, Principal.Currency),
                TotalAmount: new Money((decimal)monthlyPayment, Principal.Currency),
                OutstandingBalance: new Money(Math.Max(0, (decimal)remainingPrincipal), Principal.Currency),
                IsPaid: false,
                PaidDate: null,
                PaidAmount: Money.Zero(Principal.Currency)));

            currentDate = currentDate.AddMonths(1);
        }
    }

    private void UpdateScheduleWithPayment(Money paymentAmount, DateTime paymentDate)
    {
        var remainingPayment = paymentAmount;
        
        foreach (var scheduleItem in _schedule.Where(s => !s.IsPaid).OrderBy(s => s.DueDate))
        {
            if (remainingPayment.IsZero())
                break;

            var amountToPay = Money.Min(remainingPayment, scheduleItem.TotalAmount - scheduleItem.PaidAmount);
            
            // Update the schedule item (would need mutable version in real implementation)
            remainingPayment -= amountToPay;
        }
    }

    private void UpdatePastDueStatus(DateTime asOfDate)
    {
        var overdueSchedules = _schedule.Where(s => !s.IsPaid && s.DueDate < asOfDate).ToList();
        
        if (overdueSchedules.Any())
        {
            var oldestOverdue = overdueSchedules.OrderBy(s => s.DueDate).First();
            DaysPastDue = (asOfDate - oldestOverdue.DueDate).Days;
            PastDueAmount = new Money(overdueSchedules.Sum(s => s.TotalAmount.Amount - s.PaidAmount.Amount), Principal.Currency);
            
            // Update sub-status based on days past due
            SubStatus = DaysPastDue switch
            {
                >= 1 and <= 30 => LoanSubStatus.PastDue1to30,
                >= 31 and <= 60 => LoanSubStatus.PastDue31to60,
                >= 61 and <= 90 => LoanSubStatus.PastDue61to90,
                > 90 => LoanSubStatus.NonPerforming,
                _ => LoanSubStatus.Current
            };
        }
        else
        {
            DaysPastDue = 0;
            PastDueAmount = Money.Zero(Principal.Currency);
            SubStatus = LoanSubStatus.Current;
        }
    }

    private static string GenerateLoanNumber(Product product)
    {
        var prefix = product.Type switch
        {
            ProductType.PersonalLoan => "PL",
            ProductType.MortgageLoan => "ML",
            ProductType.AutoLoan => "AL",
            ProductType.BusinessLoan => "BL",
            _ => "LN"
        };

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd");
        var sequence = new Random().Next(100000, 999999);
        
        return $"{prefix}{timestamp}{sequence}";
    }
}

// Value objects
public record LoanScheduleItem(
    int ScheduleNumber,
    DateTime DueDate,
    Money PrincipalAmount,
    Money InterestAmount,
    Money TotalAmount,
    Money OutstandingBalance,
    bool IsPaid,
    DateTime? PaidDate,
    Money PaidAmount);

public record LoanCollateral(
    Guid CollateralId,
    string CollateralType,
    string Description,
    Money Value,
    DateTime ValuationDate,
    string? ValuedBy);

public record LoanGuarantor(
    Guid GuarantorId,
    string GuarantorName,
    Money GuaranteeAmount,
    DateTime GuaranteeDate,
    string? GuaranteeDocument);

public record LoanCondition(
    string ConditionType,
    string Description,
    bool IsMandatory,
    DateTime? DueDate,
    bool IsComplied);

public record LoanRestructureRequest(
    decimal? NewInterestRate,
    int? NewTermInMonths,
    int? PrincipalMoratorium,
    string Reason);



