using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Call Deposit aggregate - On-demand deposits with flexible withdrawal terms
/// Supports instant access, tiered interest rates, and notice periods
/// </summary>
public class CallDeposit : AggregateRoot<Guid>
{
    public Guid AccountId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string DepositNumber { get; private set; }
    public Money Balance { get; private set; }
    public InterestRate CurrentInterestRate { get; private set; }
    public int NoticePeriodDays { get; private set; }
    public Money MinimumBalance { get; private set; }
    public Money MaximumBalance { get; private set; }
    public DepositStatus Status { get; private set; }
    public InterestPaymentFrequency InterestFrequency { get; private set; }
    public bool InstantAccess { get; private set; }
    public Money AccruedInterest { get; private set; }
    public DateTime LastInterestPostingDate { get; private set; }
    public DateTime OpeningDate { get; private set; }
    public DateTime? ClosureDate { get; private set; }
    public string? ClosureReason { get; private set; }
    public string BranchCode { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    // Navigation properties
    public virtual Account Account { get; private set; } = null!;
    public virtual Customer Customer { get; private set; } = null!;

    private readonly List<CallDepositTransaction> _transactions = new();
    public IReadOnlyList<CallDepositTransaction> Transactions => _transactions.AsReadOnly();

    private readonly List<WithdrawalNotice> _withdrawalNotices = new();
    public IReadOnlyList<WithdrawalNotice> WithdrawalNotices => _withdrawalNotices.AsReadOnly();

    private CallDeposit() { } // EF Core

    public CallDeposit(
        Guid id,
        Guid accountId,
        Guid customerId,
        string depositNumber,
        Money initialDeposit,
        InterestRate interestRate,
        int noticePeriodDays,
        Money minimumBalance,
        Money maximumBalance,
        InterestPaymentFrequency interestFrequency,
        bool instantAccess,
        string branchCode,
        string createdBy)
    {
        Id = id;
        AccountId = accountId;
        CustomerId = customerId;
        DepositNumber = depositNumber;
        Balance = initialDeposit;
        CurrentInterestRate = interestRate;
        NoticePeriodDays = noticePeriodDays;
        MinimumBalance = minimumBalance;
        MaximumBalance = maximumBalance;
        InterestFrequency = interestFrequency;
        InstantAccess = instantAccess;
        BranchCode = branchCode;
        CreatedBy = createdBy;
        
        Status = DepositStatus.Active;
        AccruedInterest = new Money(0, initialDeposit.Currency);
        OpeningDate = DateTime.UtcNow;
        LastInterestPostingDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        // Record opening transaction
        var openingTransaction = new CallDepositTransaction(
            Guid.NewGuid(),
            Id,
            CallDepositTransactionType.Opening,
            initialDeposit,
            Balance,
            "Account opening deposit",
            DateTime.UtcNow);

        _transactions.Add(openingTransaction);

        AddDomainEvent(new CallDepositOpenedDomainEvent(Id, AccountId, CustomerId, initialDeposit));
    }

    public void Deposit(Money amount, string description, string processedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        var newBalance = new Money(Balance.Amount + amount.Amount, Balance.Currency);

        if (newBalance.Amount > MaximumBalance.Amount)
            throw new InvalidOperationException("Deposit would exceed maximum balance limit");

        Balance = newBalance;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        // Record transaction
        var transaction = new CallDepositTransaction(
            Guid.NewGuid(),
            Id,
            CallDepositTransactionType.Deposit,
            amount,
            Balance,
            description,
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new CallDepositDepositMadeDomainEvent(Id, AccountId, amount, Balance));
    }

    public void SubmitWithdrawalNotice(Money amount, DateTime requestedWithdrawalDate, string reason, string submittedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        if (!InstantAccess && requestedWithdrawalDate < DateTime.UtcNow.AddDays(NoticePeriodDays))
            throw new InvalidOperationException($"Withdrawal requires {NoticePeriodDays} days notice");

        var balanceAfterWithdrawal = new Money(Balance.Amount - amount.Amount, Balance.Currency);
        if (balanceAfterWithdrawal.Amount < MinimumBalance.Amount)
            throw new InvalidOperationException("Withdrawal would result in balance below minimum required");

        var notice = new WithdrawalNotice(
            Guid.NewGuid(),
            Id,
            amount,
            requestedWithdrawalDate,
            reason,
            WithdrawalNoticeStatus.Pending,
            submittedBy,
            DateTime.UtcNow);

        _withdrawalNotices.Add(notice);

        AddDomainEvent(new CallDepositWithdrawalNoticeSubmittedDomainEvent(Id, AccountId, amount, requestedWithdrawalDate));
    }

    public void ProcessWithdrawal(Guid noticeId, string processedBy)
    {
        var notice = _withdrawalNotices.FirstOrDefault(n => n.Id == noticeId);
        if (notice == null)
            throw new InvalidOperationException("Withdrawal notice not found");

        if (notice.Status != WithdrawalNoticeStatus.Pending)
            throw new InvalidOperationException("Withdrawal notice is not pending");

        if (!InstantAccess && DateTime.UtcNow < notice.RequestedWithdrawalDate)
            throw new InvalidOperationException("Notice period has not elapsed");

        var balanceAfterWithdrawal = new Money(Balance.Amount - notice.Amount.Amount, Balance.Currency);
        if (balanceAfterWithdrawal.Amount < MinimumBalance.Amount)
            throw new InvalidOperationException("Withdrawal would result in balance below minimum required");

        Balance = balanceAfterWithdrawal;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        // Update notice status
        notice.Process(processedBy);

        // Record transaction
        var transaction = new CallDepositTransaction(
            Guid.NewGuid(),
            Id,
            CallDepositTransactionType.Withdrawal,
            notice.Amount,
            Balance,
            notice.Reason,
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new CallDepositWithdrawalProcessedDomainEvent(Id, AccountId, notice.Amount, Balance));
    }

    public void CancelWithdrawalNotice(Guid noticeId, string reason, string cancelledBy)
    {
        var notice = _withdrawalNotices.FirstOrDefault(n => n.Id == noticeId);
        if (notice == null)
            throw new InvalidOperationException("Withdrawal notice not found");

        if (notice.Status != WithdrawalNoticeStatus.Pending)
            throw new InvalidOperationException("Only pending notices can be cancelled");

        notice.Cancel(reason, cancelledBy);

        AddDomainEvent(new CallDepositWithdrawalNoticeCancelledDomainEvent(Id, AccountId, notice.Amount, reason));
    }

    public void AccrueInterest(Money interestAmount, DateTime accrualDate)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Cannot accrue interest on inactive deposit");

        AccruedInterest = new Money(AccruedInterest.Amount + interestAmount.Amount, AccruedInterest.Currency);
        LastInterestPostingDate = accrualDate;
        ModifiedAt = DateTime.UtcNow;

        // Record transaction
        var transaction = new CallDepositTransaction(
            Guid.NewGuid(),
            Id,
            CallDepositTransactionType.InterestAccrual,
            interestAmount,
            Balance,
            "Interest accrual",
            accrualDate);

        _transactions.Add(transaction);

        AddDomainEvent(new InterestAccruedDomainEvent(Id, AccountId, interestAmount, accrualDate));
    }

    public void PostInterest(string postedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Cannot post interest on inactive deposit");

        if (AccruedInterest.Amount <= 0)
            throw new InvalidOperationException("No accrued interest to post");

        Balance = new Money(Balance.Amount + AccruedInterest.Amount, Balance.Currency);
        var postedInterest = AccruedInterest;
        AccruedInterest = new Money(0, AccruedInterest.Currency);
        ModifiedBy = postedBy;
        ModifiedAt = DateTime.UtcNow;

        // Record transaction
        var transaction = new CallDepositTransaction(
            Guid.NewGuid(),
            Id,
            CallDepositTransactionType.InterestPosting,
            postedInterest,
            Balance,
            "Interest posting",
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new InterestPostedDomainEvent(Id, AccountId, postedInterest));
    }

    public void UpdateInterestRate(InterestRate newRate, string updatedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Cannot update interest rate on inactive deposit");

        var oldRate = CurrentInterestRate;
        CurrentInterestRate = newRate;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new CallDepositInterestRateUpdatedDomainEvent(Id, AccountId, oldRate, newRate));
    }

    public void CloseDeposit(string reason, string closedBy)
    {
        if (Status != DepositStatus.Active)
            throw new InvalidOperationException("Deposit is not active");

        // Check for pending withdrawal notices
        var pendingNotices = _withdrawalNotices.Where(n => n.Status == WithdrawalNoticeStatus.Pending);
        if (pendingNotices.Any())
            throw new InvalidOperationException("Cannot close deposit with pending withdrawal notices");

        Status = DepositStatus.Closed;
        ClosureDate = DateTime.UtcNow;
        ClosureReason = reason;
        ModifiedBy = closedBy;
        ModifiedAt = DateTime.UtcNow;

        var finalBalance = GetTotalBalance();

        // Record transaction
        var transaction = new CallDepositTransaction(
            Guid.NewGuid(),
            Id,
            CallDepositTransactionType.Closure,
            finalBalance,
            new Money(0, finalBalance.Currency),
            reason,
            DateTime.UtcNow);

        _transactions.Add(transaction);

        AddDomainEvent(new CallDepositClosedDomainEvent(Id, AccountId, CustomerId, finalBalance));
    }

    public Money GetTotalBalance()
    {
        return new Money(Balance.Amount + AccruedInterest.Amount, Balance.Currency);
    }

    public Money GetAvailableBalance()
    {
        var pendingWithdrawals = _withdrawalNotices
            .Where(n => n.Status == WithdrawalNoticeStatus.Pending)
            .Sum(n => n.Amount.Amount);

        return new Money(Balance.Amount - pendingWithdrawals, Balance.Currency);
    }

    public bool CanWithdraw(Money amount)
    {
        if (Status != DepositStatus.Active)
            return false;

        var availableBalance = GetAvailableBalance();
        var balanceAfterWithdrawal = availableBalance.Amount - amount.Amount;
        
        return balanceAfterWithdrawal >= MinimumBalance.Amount;
    }

    public int GetDaysToWithdrawal(DateTime requestedDate)
    {
        if (InstantAccess)
            return 0;

        var noticeDays = (requestedDate - DateTime.UtcNow).Days;
        return Math.Max(0, NoticePeriodDays - noticeDays);
    }

    public decimal CalculateDailyInterest()
    {
        return Balance.Amount * (CurrentInterestRate.Rate / 100) / 365;
    }

    public IEnumerable<WithdrawalNotice> GetPendingNotices()
    {
        return _withdrawalNotices.Where(n => n.Status == WithdrawalNoticeStatus.Pending);
    }

    public IEnumerable<CallDepositTransaction> GetTransactionsByType(CallDepositTransactionType type)
    {
        return _transactions.Where(t => t.TransactionType == type);
    }
}

/// <summary>
/// Individual transaction within a call deposit
/// </summary>
public class CallDepositTransaction
{
    public Guid Id { get; private set; }
    public Guid CallDepositId { get; private set; }
    public CallDepositTransactionType TransactionType { get; private set; }
    public Money Amount { get; private set; }
    public Money BalanceAfter { get; private set; }
    public string Description { get; private set; }
    public DateTime TransactionDate { get; private set; }

    private CallDepositTransaction() { } // EF Core

    public CallDepositTransaction(
        Guid id,
        Guid callDepositId,
        CallDepositTransactionType transactionType,
        Money amount,
        Money balanceAfter,
        string description,
        DateTime transactionDate)
    {
        Id = id;
        CallDepositId = callDepositId;
        TransactionType = transactionType;
        Amount = amount;
        BalanceAfter = balanceAfter;
        Description = description;
        TransactionDate = transactionDate;
    }
}

/// <summary>
/// Withdrawal notice for call deposits
/// </summary>
public class WithdrawalNotice
{
    public Guid Id { get; private set; }
    public Guid CallDepositId { get; private set; }
    public Money Amount { get; private set; }
    public DateTime RequestedWithdrawalDate { get; private set; }
    public string Reason { get; private set; }
    public WithdrawalNoticeStatus Status { get; private set; }
    public string SubmittedBy { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public string? ProcessedBy { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public string? CancellationReason { get; private set; }

    private WithdrawalNotice() { } // EF Core

    public WithdrawalNotice(
        Guid id,
        Guid callDepositId,
        Money amount,
        DateTime requestedWithdrawalDate,
        string reason,
        WithdrawalNoticeStatus status,
        string submittedBy,
        DateTime submittedAt)
    {
        Id = id;
        CallDepositId = callDepositId;
        Amount = amount;
        RequestedWithdrawalDate = requestedWithdrawalDate;
        Reason = reason;
        Status = status;
        SubmittedBy = submittedBy;
        SubmittedAt = submittedAt;
    }

    public void Process(string processedBy)
    {
        Status = WithdrawalNoticeStatus.Processed;
        ProcessedBy = processedBy;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason, string cancelledBy)
    {
        Status = WithdrawalNoticeStatus.Cancelled;
        CancellationReason = reason;
        ProcessedBy = cancelledBy;
        ProcessedAt = DateTime.UtcNow;
    }

    public bool IsExpired()
    {
        return Status == WithdrawalNoticeStatus.Pending && DateTime.UtcNow > RequestedWithdrawalDate.AddDays(7);
    }
}

// Enums
public enum CallDepositTransactionType
{
    Opening = 1,
    Deposit = 2,
    Withdrawal = 3,
    InterestAccrual = 4,
    InterestPosting = 5,
    Closure = 6
}

public enum WithdrawalNoticeStatus
{
    Pending = 1,
    Processed = 2,
    Cancelled = 3,
    Expired = 4
}

// Domain Events
public record CallDepositOpenedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money InitialDeposit) : IDomainEvent;

public record CallDepositDepositMadeDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Money DepositAmount,
    Money NewBalance) : IDomainEvent;

public record CallDepositWithdrawalNoticeSubmittedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Money Amount,
    DateTime RequestedDate) : IDomainEvent;

public record CallDepositWithdrawalProcessedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Money WithdrawalAmount,
    Money RemainingBalance) : IDomainEvent;

public record CallDepositWithdrawalNoticeCancelledDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Money Amount,
    string Reason) : IDomainEvent;

public record CallDepositInterestRateUpdatedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    InterestRate OldRate,
    InterestRate NewRate) : IDomainEvent;

public record CallDepositClosedDomainEvent(
    Guid DepositId,
    Guid AccountId,
    Guid CustomerId,
    Money FinalBalance) : IDomainEvent;