using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Teller Session Aggregate - Complete teller session management
/// Inspired by Finacle Teller and T24 TELLER session handling
/// Manages teller login, cash position, transactions, and session closure
/// </summary>
public class TellerSession : AggregateRoot
{
    // Session identification
    public string SessionId { get; private set; } // Unique session identifier
    public Guid TellerId { get; private set; }
    public string TellerCode { get; private set; }
    public string TellerName { get; private set; }
    public Guid BranchId { get; private set; }
    public string BranchCode { get; private set; }
    
    // Session timing
    public DateTime SessionStartTime { get; private set; }
    public DateTime? SessionEndTime { get; private set; }
    public TellerSessionStatus Status { get; private set; }
    
    // Cash management
    public Money OpeningCashBalance { get; private set; }
    public Money CurrentCashBalance { get; private set; }
    public Money? ClosingCashBalance { get; private set; }
    public Money? CashDifference { get; private set; }
    
    // Transaction tracking
    public int TransactionCount { get; private set; }
    public Money TotalDeposits { get; private set; }
    public Money TotalWithdrawals { get; private set; }
    public Money TotalTransfers { get; private set; }
    
    // Multi-currency support
    private readonly Dictionary<string, Money> _currencyBalances = new();
    public IReadOnlyDictionary<string, Money> CurrencyBalances => _currencyBalances.AsReadOnly();
    
    // Session limits and controls
    public Money DailyTransactionLimit { get; private set; }
    public Money SingleTransactionLimit { get; private set; }
    public Money CashWithdrawalLimit { get; private set; }
    
    // Supervisor and approval tracking
    public string? SupervisorId { get; private set; }
    public DateTime? LastSupervisorApproval { get; private set; }
    public int SupervisorApprovalsCount { get; private set; }
    
    // Session notes and exceptions
    private readonly List<TellerSessionNote> _sessionNotes = new();
    public IReadOnlyCollection<TellerSessionNote> SessionNotes => _sessionNotes.AsReadOnly();
    
    // Audit trail
    public string CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }

    private TellerSession() : base(Guid.NewGuid()) { }

    public static TellerSession Start(
        Guid tellerId,
        string tellerCode,
        string tellerName,
        Guid branchId,
        string branchCode,
        Money openingCashBalance,
        TellerLimits limits,
        string createdBy)
    {
        var sessionId = GenerateSessionId(tellerCode, branchCode);
        var currency = openingCashBalance.Currency;
        
        var session = new TellerSession
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            TellerId = tellerId,
            TellerCode = tellerCode,
            TellerName = tellerName,
            BranchId = branchId,
            BranchCode = branchCode,
            SessionStartTime = DateTime.UtcNow,
            Status = TellerSessionStatus.Active,
            OpeningCashBalance = openingCashBalance,
            CurrentCashBalance = openingCashBalance,
            TransactionCount = 0,
            TotalDeposits = Money.Zero(currency),
            TotalWithdrawals = Money.Zero(currency),
            TotalTransfers = Money.Zero(currency),
            DailyTransactionLimit = limits.DailyTransactionLimit,
            SingleTransactionLimit = limits.SingleTransactionLimit,
            CashWithdrawalLimit = limits.CashWithdrawalLimit,
            SupervisorApprovalsCount = 0,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow
        };

        // Initialize currency balances
        session._currencyBalances[currency.Code] = openingCashBalance;

        session.AddDomainEvent(new TellerSessionStartedDomainEvent(
            session.Id, session.SessionId, tellerId, branchId, openingCashBalance));

        return session;
    }

    public void ProcessCashDeposit(Money amount, string accountNumber, string processedBy, string? reference = null)
    {
        ValidateSessionActive();
        ValidateTransactionLimits(amount);

        // Update cash position
        CurrentCashBalance += amount;
        TotalDeposits += amount;
        TransactionCount++;

        // Update currency balance
        UpdateCurrencyBalance(amount.Currency.Code, amount);

        LastModifiedBy = processedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashDepositProcessedDomainEvent(
            Id, SessionId, amount, accountNumber, reference));
    }

    public void ProcessCashWithdrawal(Money amount, string accountNumber, string processedBy, string? reference = null)
    {
        ValidateSessionActive();
        ValidateTransactionLimits(amount);
        ValidateCashAvailability(amount);

        // Update cash position
        CurrentCashBalance -= amount;
        TotalWithdrawals += amount;
        TransactionCount++;

        // Update currency balance
        UpdateCurrencyBalance(amount.Currency.Code, -amount);

        LastModifiedBy = processedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashWithdrawalProcessedDomainEvent(
            Id, SessionId, amount, accountNumber, reference));
    }

    public void ProcessTransfer(Money amount, string fromAccount, string toAccount, string processedBy, string? reference = null)
    {
        ValidateSessionActive();
        ValidateTransactionLimits(amount);

        // Transfers don't affect cash position but count towards transaction limits
        TotalTransfers += amount;
        TransactionCount++;

        LastModifiedBy = processedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TransferProcessedDomainEvent(
            Id, SessionId, amount, fromAccount, toAccount, reference));
    }

    public void AddCash(Money amount, CashSource source, string addedBy, string? reference = null)
    {
        ValidateSessionActive();

        CurrentCashBalance += amount;
        UpdateCurrencyBalance(amount.Currency.Code, amount);

        AddSessionNote($"Cash added: {amount.Amount} {amount.Currency.Code} from {source}", addedBy);

        LastModifiedBy = addedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashAddedToSessionDomainEvent(Id, SessionId, amount, source, reference));
    }

    public void RemoveCash(Money amount, CashDestination destination, string removedBy, string? reference = null)
    {
        ValidateSessionActive();
        ValidateCashAvailability(amount);

        CurrentCashBalance -= amount;
        UpdateCurrencyBalance(amount.Currency.Code, -amount);

        AddSessionNote($"Cash removed: {amount.Amount} {amount.Currency.Code} to {destination}", removedBy);

        LastModifiedBy = removedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashRemovedFromSessionDomainEvent(Id, SessionId, amount, destination, reference));
    }

    public void RequestSupervisorApproval(string supervisorId, string reason, string requestedBy)
    {
        ValidateSessionActive();

        SupervisorId = supervisorId;
        LastSupervisorApproval = DateTime.UtcNow;
        SupervisorApprovalsCount++;

        AddSessionNote($"Supervisor approval requested: {reason}", requestedBy);

        AddDomainEvent(new SupervisorApprovalRequestedDomainEvent(Id, SessionId, supervisorId, reason));
    }

    public void ReconcileCash(Money actualCashBalance, string reconciledBy, string? notes = null)
    {
        ValidateSessionActive();

        var difference = actualCashBalance - CurrentCashBalance;
        
        if (!difference.IsZero())
        {
            CashDifference = difference;
            AddSessionNote($"Cash difference found: {difference.Amount} {difference.Currency.Code}. {notes}", reconciledBy);
            
            AddDomainEvent(new CashReconciliationDomainEvent(Id, SessionId, CurrentCashBalance, actualCashBalance, difference));
        }

        CurrentCashBalance = actualCashBalance;
        LastModifiedBy = reconciledBy;
        LastModifiedDate = DateTime.UtcNow;
    }

    public void EndSession(Money closingCashBalance, string supervisorId, string endedBy, string? notes = null)
    {
        ValidateSessionActive();

        ClosingCashBalance = closingCashBalance;
        CashDifference = closingCashBalance - CurrentCashBalance;
        SessionEndTime = DateTime.UtcNow;
        Status = TellerSessionStatus.Closed;
        SupervisorId = supervisorId;

        if (notes != null)
        {
            AddSessionNote($"Session ended: {notes}", endedBy);
        }

        LastModifiedBy = endedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerSessionEndedDomainEvent(
            Id, SessionId, closingCashBalance, CashDifference, TransactionCount));
    }

    public void SuspendSession(string reason, string suspendedBy)
    {
        ValidateSessionActive();

        Status = TellerSessionStatus.Suspended;
        AddSessionNote($"Session suspended: {reason}", suspendedBy);

        LastModifiedBy = suspendedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerSessionSuspendedDomainEvent(Id, SessionId, reason));
    }

    public void ResumeSession(string resumedBy, string? notes = null)
    {
        if (Status != TellerSessionStatus.Suspended)
            throw new DomainException("Only suspended sessions can be resumed");

        Status = TellerSessionStatus.Active;
        
        if (notes != null)
        {
            AddSessionNote($"Session resumed: {notes}", resumedBy);
        }

        LastModifiedBy = resumedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new TellerSessionResumedDomainEvent(Id, SessionId));
    }

    private void ValidateSessionActive()
    {
        if (Status != TellerSessionStatus.Active)
            throw new DomainException($"Session is not active. Current status: {Status}");
    }

    private void ValidateTransactionLimits(Money amount)
    {
        if (amount.IsGreaterThan(SingleTransactionLimit))
            throw new DomainException($"Transaction amount {amount.Amount} exceeds single transaction limit {SingleTransactionLimit.Amount}");

        var dailyTotal = TotalDeposits + TotalWithdrawals + amount;
        if (dailyTotal.IsGreaterThan(DailyTransactionLimit))
            throw new DomainException($"Daily transaction limit {DailyTransactionLimit.Amount} would be exceeded");
    }

    private void ValidateCashAvailability(Money amount)
    {
        var currencyBalance = _currencyBalances.GetValueOrDefault(amount.Currency.Code, Money.Zero(amount.Currency));
        if (amount.IsGreaterThan(currencyBalance))
            throw new DomainException($"Insufficient cash in {amount.Currency.Code}. Available: {currencyBalance.Amount}, Required: {amount.Amount}");
    }

    private void UpdateCurrencyBalance(string currencyCode, Money amount)
    {
        if (_currencyBalances.ContainsKey(currencyCode))
        {
            _currencyBalances[currencyCode] += amount;
        }
        else
        {
            _currencyBalances[currencyCode] = amount;
        }
    }

    private void AddSessionNote(string note, string addedBy)
    {
        _sessionNotes.Add(new TellerSessionNote(
            DateTime.UtcNow,
            note,
            addedBy));
    }

    private static string GenerateSessionId(string tellerCode, string branchCode)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return $"{branchCode}-{tellerCode}-{timestamp}";
    }
}

// Supporting enums and value objects
public enum TellerSessionStatus
{
    Active,
    Suspended,
    Closed,
    Terminated
}

public enum CashSource
{
    Vault,
    AnotherTeller,
    CashDelivery,
    ATMReplenishment,
    BranchTransfer
}

public enum CashDestination
{
    Vault,
    AnotherTeller,
    CashPickup,
    ATMReplenishment,
    BranchTransfer
}

public record TellerLimits(
    Money DailyTransactionLimit,
    Money SingleTransactionLimit,
    Money CashWithdrawalLimit);

public record TellerSessionNote(
    DateTime Timestamp,
    string Note,
    string AddedBy);

// Domain Events
public record TellerSessionStartedDomainEvent(
    Guid SessionId,
    string SessionNumber,
    Guid TellerId,
    Guid BranchId,
    Money OpeningCash) : IDomainEvent;

public record CashDepositProcessedDomainEvent(
    Guid SessionId,
    string SessionNumber,
    Money Amount,
    string AccountNumber,
    string? Reference) : IDomainEvent;

public record CashWithdrawalProcessedDomainEvent(
    Guid SessionId,
    string SessionNumber,
    Money Amount,
    string AccountNumber,
    string? Reference) : IDomainEvent;

public record TransferProcessedDomainEvent(
    Guid SessionId,
    string SessionNumber,
    Money Amount,
    string FromAccount,
    string ToAccount,
    string? Reference) : IDomainEvent;

public record CashAddedToSessionDomainEvent(
    Guid SessionId,
    string SessionNumber,
    Money Amount,
    CashSource Source,
    string? Reference) : IDomainEvent;

public record CashRemovedFromSessionDomainEvent(
    Guid SessionId,
    string SessionNumber,
    Money Amount,
    CashDestination Destination,
    string? Reference) : IDomainEvent;

public record SupervisorApprovalRequestedDomainEvent(
    Guid SessionId,
    string SessionNumber,
    string SupervisorId,
    string Reason) : IDomainEvent;

public record CashReconciliationDomainEvent(
    Guid SessionId,
    string SessionNumber,
    Money ExpectedCash,
    Money ActualCash,
    Money Difference) : IDomainEvent;

public record TellerSessionEndedDomainEvent(
    Guid SessionId,
    string SessionNumber,
    Money ClosingCash,
    Money? CashDifference,
    int TransactionCount) : IDomainEvent;

public record TellerSessionSuspendedDomainEvent(
    Guid SessionId,
    string SessionNumber,
    string Reason) : IDomainEvent;

public record TellerSessionResumedDomainEvent(
    Guid SessionId,
    string SessionNumber) : IDomainEvent;