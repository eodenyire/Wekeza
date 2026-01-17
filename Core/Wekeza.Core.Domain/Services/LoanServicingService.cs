using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// Loan Servicing Service - Complete loan servicing operations
/// Handles disbursement, repayment processing, and GL integration
/// Inspired by Finacle LMS and T24 Loan Servicing
/// </summary>
public class LoanServicingService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IGLAccountRepository _glAccountRepository;

    public LoanServicingService(
        ILoanRepository loanRepository,
        IAccountRepository accountRepository,
        IJournalEntryRepository journalEntryRepository,
        IGLAccountRepository glAccountRepository)
    {
        _loanRepository = loanRepository;
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
        _glAccountRepository = glAccountRepository;
    }

    /// <summary>
    /// Disburse loan funds to customer account with GL posting
    /// </summary>
    public async Task<LoanServicingResult> DisburseLoanAsync(
        Loan loan,
        Guid disbursementAccountId,
        string disbursedBy)
    {
        try
        {
            // 1. Validate loan can be disbursed
            if (loan.Status != LoanStatus.Approved)
            {
                return LoanServicingResult.Failed("Loan must be approved before disbursement");
            }

            // 2. Get disbursement account
            var disbursementAccount = await _accountRepository.GetByIdAsync(disbursementAccountId);
            if (disbursementAccount == null)
            {
                return LoanServicingResult.Failed("Disbursement account not found");
            }

            // 3. Disburse the loan
            loan.Disburse(disbursementAccountId, disbursedBy);

            // 4. Credit customer account
            disbursementAccount.Credit(loan.Principal, loan.LoanNumber, $"Loan disbursement - {loan.LoanNumber}");

            // 5. Create GL entries for disbursement
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Standard);
            var journalEntry = CreateDisbursementGLEntry(loan, disbursementAccount, journalNumber, disbursedBy);

            journalEntry.Post(disbursedBy);
            _journalEntryRepository.Add(journalEntry);

            // 6. Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            return LoanServicingResult.Success(
                loan.LoanNumber,
                "Loan disbursed successfully",
                journalNumber);
        }
        catch (Exception ex)
        {
            return LoanServicingResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// Process loan repayment with GL posting
    /// </summary>
    public async Task<LoanServicingResult> ProcessRepaymentAsync(
        Loan loan,
        Money paymentAmount,
        Guid paymentAccountId,
        DateTime paymentDate,
        string processedBy,
        string? paymentReference = null)
    {
        try
        {
            // 1. Validate loan can receive payments
            if (loan.Status != LoanStatus.Active)
            {
                return LoanServicingResult.Failed("Can only process payments for active loans");
            }

            // 2. Get payment account
            var paymentAccount = await _accountRepository.GetByIdAsync(paymentAccountId);
            if (paymentAccount == null)
            {
                return LoanServicingResult.Failed("Payment account not found");
            }

            // 3. Validate account has sufficient funds
            if (paymentAmount.IsGreaterThan(paymentAccount.Balance))
            {
                return LoanServicingResult.Failed("Insufficient funds in payment account");
            }

            // 4. Calculate payment allocation
            var allocationResult = CalculatePaymentAllocation(loan, paymentAmount);

            // 5. Debit payment account
            paymentAccount.Debit(paymentAmount, paymentReference ?? loan.LoanNumber, 
                $"Loan repayment - {loan.LoanNumber}");

            // 6. Process repayment in loan
            loan.ProcessRepayment(paymentAmount, paymentDate, processedBy, paymentReference);

            // 7. Create GL entries for repayment
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Standard);
            var journalEntry = CreateRepaymentGLEntry(
                loan, paymentAccount, allocationResult, journalNumber, processedBy, paymentReference);

            journalEntry.Post(processedBy);
            _journalEntryRepository.Add(journalEntry);

            // 8. Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            return LoanServicingResult.Success(
                loan.LoanNumber,
                $"Repayment processed: Principal {allocationResult.PrincipalPayment.Amount}, Interest {allocationResult.InterestPayment.Amount}",
                journalNumber,
                allocationResult);
        }
        catch (Exception ex)
        {
            return LoanServicingResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// Accrue interest for a loan with GL posting
    /// </summary>
    public async Task<LoanServicingResult> AccrueInterestAsync(
        Loan loan,
        DateTime calculationDate,
        string processedBy)
    {
        try
        {
            if (loan.Status != LoanStatus.Active)
            {
                return LoanServicingResult.Success(loan.LoanNumber, "No interest accrual for inactive loan");
            }

            var previousAccruedInterest = loan.AccruedInterest;
            
            // Accrue interest in loan
            loan.AccrueInterest(calculationDate);

            var interestAccrued = loan.AccruedInterest - previousAccruedInterest;

            if (interestAccrued.IsZero())
            {
                return LoanServicingResult.Success(loan.LoanNumber, "No interest accrued");
            }

            // Create GL entries for interest accrual
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Accrual);
            var journalEntry = CreateInterestAccrualGLEntry(loan, interestAccrued, journalNumber, processedBy);

            journalEntry.Post(processedBy);
            _journalEntryRepository.Add(journalEntry);

            // Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            return LoanServicingResult.Success(
                loan.LoanNumber,
                $"Interest accrued: {interestAccrued.Amount}",
                journalNumber);
        }
        catch (Exception ex)
        {
            return LoanServicingResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// Calculate loan provision with GL posting
    /// </summary>
    public async Task<LoanServicingResult> CalculateProvisionAsync(
        Loan loan,
        DateTime calculationDate,
        string processedBy)
    {
        try
        {
            var previousProvision = loan.ProvisionAmount;
            
            // Calculate provision in loan
            loan.CalculateProvision(calculationDate);

            var provisionChange = loan.ProvisionAmount - previousProvision;

            if (provisionChange.IsZero())
            {
                return LoanServicingResult.Success(loan.LoanNumber, "No provision change");
            }

            // Create GL entries for provision
            var journalNumber = await _journalEntryRepository.GenerateJournalNumberAsync(JournalType.Provision);
            var journalEntry = CreateProvisionGLEntry(loan, provisionChange, journalNumber, processedBy);

            journalEntry.Post(processedBy);
            _journalEntryRepository.Add(journalEntry);

            // Update GL account balances
            await UpdateGLAccountBalancesAsync(journalEntry);

            return LoanServicingResult.Success(
                loan.LoanNumber,
                $"Provision calculated: {loan.ProvisionAmount.Amount} (Change: {provisionChange.Amount})",
                journalNumber);
        }
        catch (Exception ex)
        {
            return LoanServicingResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// Create GL entry for loan disbursement
    /// Dr. Loans and Advances (Asset)
    /// Cr. Customer Account (Liability)
    /// </summary>
    private JournalEntry CreateDisbursementGLEntry(
        Loan loan,
        Account disbursementAccount,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            loan.DisbursementDate ?? DateTime.UtcNow.Date,
            JournalType.Standard,
            "LoanDisbursement",
            loan.Id,
            loan.LoanNumber,
            loan.Principal.Currency.Code,
            createdBy,
            $"Loan disbursement - {loan.LoanNumber}");

        // Debit: Loans and Advances (Asset increases)
        journalEntry.AddDebitLine(
            loan.LoanGLCode,
            loan.Principal.Amount,
            description: $"Loan disbursement - {loan.LoanNumber}");

        // Credit: Customer Account (Liability increases)
        journalEntry.AddCreditLine(
            disbursementAccount.CustomerGLCode,
            loan.Principal.Amount,
            description: $"Loan disbursement to {disbursementAccount.AccountNumber}");

        return journalEntry;
    }

    /// <summary>
    /// Create GL entry for loan repayment
    /// Dr. Customer Account (Liability decreases)
    /// Cr. Loans and Advances (Asset decreases) - Principal portion
    /// Cr. Interest Income (Income increases) - Interest portion
    /// </summary>
    private JournalEntry CreateRepaymentGLEntry(
        Loan loan,
        Account paymentAccount,
        PaymentAllocationResult allocation,
        string journalNumber,
        string createdBy,
        string? paymentReference)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "LoanRepayment",
            loan.Id,
            paymentReference ?? loan.LoanNumber,
            allocation.TotalPayment.Currency.Code,
            createdBy,
            $"Loan repayment - {loan.LoanNumber}");

        // Debit: Customer Account (Liability decreases)
        journalEntry.AddDebitLine(
            paymentAccount.CustomerGLCode,
            allocation.TotalPayment.Amount,
            description: $"Loan repayment from {paymentAccount.AccountNumber}");

        // Credit: Loans and Advances (Asset decreases) - Principal portion
        if (!allocation.PrincipalPayment.IsZero())
        {
            journalEntry.AddCreditLine(
                loan.LoanGLCode,
                allocation.PrincipalPayment.Amount,
                description: $"Principal repayment - {loan.LoanNumber}");
        }

        // Credit: Interest Income (Income increases) - Interest portion
        if (!allocation.InterestPayment.IsZero())
        {
            var interestIncomeGLCode = GetInterestIncomeGLCode();
            journalEntry.AddCreditLine(
                interestIncomeGLCode,
                allocation.InterestPayment.Amount,
                description: $"Interest income - {loan.LoanNumber}");
        }

        return journalEntry;
    }

    /// <summary>
    /// Create GL entry for interest accrual
    /// Dr. Interest Receivable (Asset)
    /// Cr. Interest Income (Income)
    /// </summary>
    private JournalEntry CreateInterestAccrualGLEntry(
        Loan loan,
        Money interestAmount,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Accrual,
            "InterestAccrual",
            loan.Id,
            $"INT-{loan.LoanNumber}",
            interestAmount.Currency.Code,
            createdBy,
            $"Interest accrual - {loan.LoanNumber}");

        // Debit: Interest Receivable (Asset increases)
        journalEntry.AddDebitLine(
            loan.InterestReceivableGLCode,
            interestAmount.Amount,
            description: $"Interest receivable - {loan.LoanNumber}");

        // Credit: Interest Income (Income increases)
        var interestIncomeGLCode = GetInterestIncomeGLCode();
        journalEntry.AddCreditLine(
            interestIncomeGLCode,
            interestAmount.Amount,
            description: $"Interest income accrual - {loan.LoanNumber}");

        return journalEntry;
    }

    /// <summary>
    /// Create GL entry for loan provision
    /// Dr. Provision Expense (Expense)
    /// Cr. Loan Loss Provision (Contra Asset)
    /// </summary>
    private JournalEntry CreateProvisionGLEntry(
        Loan loan,
        Money provisionChange,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Provision,
            "LoanProvision",
            loan.Id,
            $"PROV-{loan.LoanNumber}",
            provisionChange.Currency.Code,
            createdBy,
            $"Loan provision - {loan.LoanNumber}");

        if (provisionChange.IsPositive())
        {
            // Increase provision
            // Debit: Provision Expense (Expense increases)
            journalEntry.AddDebitLine(
                GetProvisionExpenseGLCode(),
                provisionChange.Amount,
                description: $"Provision expense - {loan.LoanNumber}");

            // Credit: Loan Loss Provision (Contra Asset increases)
            journalEntry.AddCreditLine(
                GetLoanLossProvisionGLCode(),
                provisionChange.Amount,
                description: $"Loan loss provision - {loan.LoanNumber}");
        }
        else
        {
            // Decrease provision (reversal)
            var reversalAmount = provisionChange.Amount * -1;
            
            // Debit: Loan Loss Provision (Contra Asset decreases)
            journalEntry.AddDebitLine(
                GetLoanLossProvisionGLCode(),
                reversalAmount,
                description: $"Provision reversal - {loan.LoanNumber}");

            // Credit: Provision Expense (Expense decreases)
            journalEntry.AddCreditLine(
                GetProvisionExpenseGLCode(),
                reversalAmount,
                description: $"Provision expense reversal - {loan.LoanNumber}");
        }

        return journalEntry;
    }

    /// <summary>
    /// Calculate how payment should be allocated between principal and interest
    /// </summary>
    private PaymentAllocationResult CalculatePaymentAllocation(Loan loan, Money paymentAmount)
    {
        // Allocate to interest first, then principal
        var interestPayment = Money.Min(paymentAmount, loan.AccruedInterest);
        var principalPayment = paymentAmount - interestPayment;

        return new PaymentAllocationResult
        {
            TotalPayment = paymentAmount,
            InterestPayment = interestPayment,
            PrincipalPayment = principalPayment,
            RemainingBalance = loan.OutstandingPrincipal - principalPayment
        };
    }

    private async Task UpdateGLAccountBalancesAsync(JournalEntry journalEntry)
    {
        foreach (var line in journalEntry.Lines)
        {
            var glAccount = await _glAccountRepository.GetByGLCodeAsync(line.GLCode);
            if (glAccount != null)
            {
                if (line.DebitAmount > 0)
                    glAccount.PostDebit(line.DebitAmount);
                if (line.CreditAmount > 0)
                    glAccount.PostCredit(line.CreditAmount);
            }
        }
    }

    // GL Code helpers - these would typically come from configuration
    private string GetInterestIncomeGLCode() => "4101"; // Interest Income
    private string GetProvisionExpenseGLCode() => "5201"; // Provision Expense
    private string GetLoanLossProvisionGLCode() => "1901"; // Loan Loss Provision (Contra Asset)
}

/// <summary>
/// Loan servicing operation result
/// </summary>
public class LoanServicingResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? LoanNumber { get; private set; }
    public string? Message { get; private set; }
    public string? JournalNumber { get; private set; }
    public PaymentAllocationResult? PaymentAllocation { get; private set; }

    private LoanServicingResult() { }

    public static LoanServicingResult Success(
        string loanNumber, 
        string message, 
        string? journalNumber = null,
        PaymentAllocationResult? paymentAllocation = null)
    {
        return new LoanServicingResult
        {
            IsSuccess = true,
            LoanNumber = loanNumber,
            Message = message,
            JournalNumber = journalNumber,
            PaymentAllocation = paymentAllocation
        };
    }

    public static LoanServicingResult Failed(string errorMessage)
    {
        return new LoanServicingResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

/// <summary>
/// Payment allocation result
/// </summary>
public class PaymentAllocationResult
{
    public Money TotalPayment { get; set; } = Money.Zero(new Currency("KES"));
    public Money InterestPayment { get; set; } = Money.Zero(new Currency("KES"));
    public Money PrincipalPayment { get; set; } = Money.Zero(new Currency("KES"));
    public Money RemainingBalance { get; set; } = Money.Zero(new Currency("KES"));
}