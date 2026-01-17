using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// GL Posting Service - Handles automatic GL entries for banking transactions
/// This is the heart of the accounting integration - every transaction creates balanced GL entries
/// </summary>
public class GLPostingService
{
    /// <summary>
    /// Creates GL entries for account deposit
    /// Dr. Cash/Bank Account
    /// Cr. Customer Deposit Account
    /// </summary>
    public static JournalEntry CreateDepositEntry(
        Account account,
        Money amount,
        string transactionReference,
        string description,
        string cashGLCode,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "AccountTransaction",
            account.Id,
            transactionReference,
            amount.Currency.Code,
            createdBy,
            $"Deposit: {description}");

        // Debit: Cash/Bank (Asset increases)
        journalEntry.AddDebitLine(
            cashGLCode,
            amount.Amount,
            description: $"Cash received - {account.AccountNumber}");

        // Credit: Customer Deposits (Liability increases)
        journalEntry.AddCreditLine(
            account.CustomerGLCode,
            amount.Amount,
            description: $"Customer deposit - {account.AccountNumber}");

        return journalEntry;
    }

    /// <summary>
    /// Creates GL entries for account withdrawal
    /// Dr. Customer Deposit Account
    /// Cr. Cash/Bank Account
    /// </summary>
    public static JournalEntry CreateWithdrawalEntry(
        Account account,
        Money amount,
        string transactionReference,
        string description,
        string cashGLCode,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "AccountTransaction",
            account.Id,
            transactionReference,
            amount.Currency.Code,
            createdBy,
            $"Withdrawal: {description}");

        // Debit: Customer Deposits (Liability decreases)
        journalEntry.AddDebitLine(
            account.CustomerGLCode,
            amount.Amount,
            description: $"Customer withdrawal - {account.AccountNumber}");

        // Credit: Cash/Bank (Asset decreases)
        journalEntry.AddCreditLine(
            cashGLCode,
            amount.Amount,
            description: $"Cash paid - {account.AccountNumber}");

        return journalEntry;
    }

    /// <summary>
    /// Creates GL entries for fund transfer between accounts
    /// Dr. From Customer Account
    /// Cr. To Customer Account
    /// </summary>
    public static JournalEntry CreateTransferEntry(
        Account fromAccount,
        Account toAccount,
        Money amount,
        string transactionReference,
        string description,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "AccountTransfer",
            fromAccount.Id,
            transactionReference,
            amount.Currency.Code,
            createdBy,
            $"Transfer: {description}");

        // Debit: From Customer Account (Liability decreases)
        journalEntry.AddDebitLine(
            fromAccount.CustomerGLCode,
            amount.Amount,
            description: $"Transfer from {fromAccount.AccountNumber}");

        // Credit: To Customer Account (Liability increases)
        journalEntry.AddCreditLine(
            toAccount.CustomerGLCode,
            amount.Amount,
            description: $"Transfer to {toAccount.AccountNumber}");

        return journalEntry;
    }

    /// <summary>
    /// Creates GL entries for interest accrual
    /// Dr. Interest Expense
    /// Cr. Interest Payable
    /// </summary>
    public static JournalEntry CreateInterestAccrualEntry(
        Account account,
        Money interestAmount,
        string interestExpenseGLCode,
        string interestPayableGLCode,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Accrual,
            "InterestAccrual",
            account.Id,
            $"INT-{account.AccountNumber}-{DateTime.UtcNow:yyyyMMdd}",
            interestAmount.Currency.Code,
            createdBy,
            $"Interest accrual for {account.AccountNumber}");

        // Debit: Interest Expense (Expense increases)
        journalEntry.AddDebitLine(
            interestExpenseGLCode,
            interestAmount.Amount,
            description: $"Interest expense - {account.AccountNumber}");

        // Credit: Interest Payable (Liability increases)
        journalEntry.AddCreditLine(
            interestPayableGLCode,
            interestAmount.Amount,
            description: $"Interest payable - {account.AccountNumber}");

        return journalEntry;
    }

    /// <summary>
    /// Creates GL entries for interest payment
    /// Dr. Interest Payable
    /// Cr. Customer Deposit Account
    /// </summary>
    public static JournalEntry CreateInterestPaymentEntry(
        Account account,
        Money interestAmount,
        string interestPayableGLCode,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "InterestPayment",
            account.Id,
            $"INTPAY-{account.AccountNumber}-{DateTime.UtcNow:yyyyMMdd}",
            interestAmount.Currency.Code,
            createdBy,
            $"Interest payment for {account.AccountNumber}");

        // Debit: Interest Payable (Liability decreases)
        journalEntry.AddDebitLine(
            interestPayableGLCode,
            interestAmount.Amount,
            description: $"Interest payment - {account.AccountNumber}");

        // Credit: Customer Deposits (Liability increases)
        journalEntry.AddCreditLine(
            account.CustomerGLCode,
            interestAmount.Amount,
            description: $"Interest credited - {account.AccountNumber}");

        return journalEntry;
    }

    /// <summary>
    /// Creates GL entries for fee collection
    /// Dr. Customer Deposit Account
    /// Cr. Fee Income
    /// </summary>
    public static JournalEntry CreateFeeEntry(
        Account account,
        Money feeAmount,
        string feeType,
        string feeIncomeGLCode,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "FeeCollection",
            account.Id,
            $"FEE-{account.AccountNumber}-{DateTime.UtcNow:yyyyMMdd}",
            feeAmount.Currency.Code,
            createdBy,
            $"{feeType} fee for {account.AccountNumber}");

        // Debit: Customer Deposits (Liability decreases)
        journalEntry.AddDebitLine(
            account.CustomerGLCode,
            feeAmount.Amount,
            description: $"{feeType} fee - {account.AccountNumber}");

        // Credit: Fee Income (Income increases)
        journalEntry.AddCreditLine(
            feeIncomeGLCode,
            feeAmount.Amount,
            description: $"{feeType} fee income - {account.AccountNumber}");

        return journalEntry;
    }

    /// <summary>
    /// Creates GL entries for loan disbursement
    /// Dr. Customer Account (or Cash)
    /// Cr. Loan Account
    /// </summary>
    public static JournalEntry CreateLoanDisbursementEntry(
        Loan loan,
        Account disbursementAccount,
        string loanGLCode,
        string journalNumber,
        string createdBy)
    {
        var amount = new Money(loan.Principal, disbursementAccount.Balance.Currency);
        
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "LoanDisbursement",
            loan.Id,
            $"LOAN-DISB-{loan.Id}",
            amount.Currency.Code,
            createdBy,
            $"Loan disbursement - Principal: {loan.Principal}");

        // Debit: Customer Account (Asset/Liability increases)
        journalEntry.AddDebitLine(
            disbursementAccount.CustomerGLCode,
            amount.Amount,
            description: $"Loan disbursement to {disbursementAccount.AccountNumber}");

        // Credit: Loans and Advances (Asset increases)
        journalEntry.AddCreditLine(
            loanGLCode,
            amount.Amount,
            description: $"Loan principal - {loan.Id}");

        return journalEntry;
    }

    /// <summary>
    /// Creates GL entries for loan repayment
    /// Dr. Loan Account
    /// Cr. Customer Account
    /// </summary>
    public static JournalEntry CreateLoanRepaymentEntry(
        Loan loan,
        Account repaymentAccount,
        Money repaymentAmount,
        string loanGLCode,
        string journalNumber,
        string createdBy)
    {
        var journalEntry = JournalEntry.Create(
            journalNumber,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date,
            JournalType.Standard,
            "LoanRepayment",
            loan.Id,
            $"LOAN-REPAY-{loan.Id}",
            repaymentAmount.Currency.Code,
            createdBy,
            $"Loan repayment - Amount: {repaymentAmount.Amount}");

        // Debit: Loans and Advances (Asset decreases)
        journalEntry.AddDebitLine(
            loanGLCode,
            repaymentAmount.Amount,
            description: $"Loan repayment - {loan.Id}");

        // Credit: Customer Account (Liability decreases)
        journalEntry.AddCreditLine(
            repaymentAccount.CustomerGLCode,
            repaymentAmount.Amount,
            description: $"Loan repayment from {repaymentAccount.AccountNumber}");

        return journalEntry;
    }
}