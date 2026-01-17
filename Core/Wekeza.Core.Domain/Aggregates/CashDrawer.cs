using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Cash Drawer Aggregate - Individual teller cash position management
/// Inspired by Finacle and T24 cash drawer functionality
/// Manages multi-currency cash positions with real-time tracking
/// </summary>
public class CashDrawer : AggregateRoot
{
    // Drawer identification
    public string DrawerId { get; private set; } // Physical drawer identifier
    public Guid TellerId { get; private set; }
    public string TellerCode { get; private set; }
    public Guid BranchId { get; private set; }
    public string BranchCode { get; private set; }
    
    // Drawer status
    public CashDrawerStatus Status { get; private set; }
    public DateTime? LastOpenedDate { get; private set; }
    public DateTime? LastClosedDate { get; private set; }
    public string? CurrentSessionId { get; private set; }
    
    // Cash positions by currency
    private readonly Dictionary<string, CashPosition> _cashPositions = new();
    public IReadOnlyDictionary<string, CashPosition> CashPositions => _cashPositions.AsReadOnly();
    
    // Daily totals
    public Money TotalCashIn { get; private set; }
    public Money TotalCashOut { get; private set; }
    public Money NetCashMovement { get; private set; }
    
    // Limits and controls
    public Money MaxCashLimit { get; private set; }
    public Money MinCashLimit { get; private set; }
    public bool RequiresDualControl { get; private set; }
    
    // Reconciliation tracking
    public DateTime? LastReconciliationDate { get; private set; }
    public string? LastReconciledBy { get; private set; }
    public Money? LastReconciliationDifference { get; private set; }
    
    // Transaction tracking
    public int DailyTransactionCount { get; private set; }
    public DateTime LastTransactionDate { get; private set; }
    
    // Audit trail
    public string CreatedBy { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }

    private CashDrawer() : base(Guid.NewGuid()) { }

    public static CashDrawer Create(
        string drawerId,
        Guid tellerId,
        string tellerCode,
        Guid branchId,
        string branchCode,
        Money maxCashLimit,
        Money minCashLimit,
        bool requiresDualControl,
        string createdBy)
    {
        var drawer = new CashDrawer
        {
            Id = Guid.NewGuid(),
            DrawerId = drawerId,
            TellerId = tellerId,
            TellerCode = tellerCode,
            BranchId = branchId,
            BranchCode = branchCode,
            Status = CashDrawerStatus.Closed,
            MaxCashLimit = maxCashLimit,
            MinCashLimit = minCashLimit,
            RequiresDualControl = requiresDualControl,
            TotalCashIn = Money.Zero(maxCashLimit.Currency),
            TotalCashOut = Money.Zero(maxCashLimit.Currency),
            NetCashMovement = Money.Zero(maxCashLimit.Currency),
            DailyTransactionCount = 0,
            LastTransactionDate = DateTime.UtcNow.Date,
            CreatedBy = createdBy,
            CreatedDate = DateTime.UtcNow
        };

        drawer.AddDomainEvent(new CashDrawerCreatedDomainEvent(drawer.Id, drawerId, tellerId, branchId));
        return drawer;
    }

    public void Open(string sessionId, Money initialCash, string openedBy)
    {
        if (Status == CashDrawerStatus.Open)
            throw new DomainException("Cash drawer is already open");

        Status = CashDrawerStatus.Open;
        CurrentSessionId = sessionId;
        LastOpenedDate = DateTime.UtcNow;

        // Initialize or update cash position
        SetCashPosition(initialCash.Currency.Code, initialCash, openedBy);

        // Reset daily counters if new day
        if (LastTransactionDate.Date < DateTime.UtcNow.Date)
        {
            ResetDailyCounters();
        }

        LastModifiedBy = openedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashDrawerOpenedDomainEvent(Id, DrawerId, sessionId, initialCash));
    }

    public void Close(string closedBy, string? notes = null)
    {
        if (Status != CashDrawerStatus.Open)
            throw new DomainException("Cash drawer is not open");

        Status = CashDrawerStatus.Closed;
        CurrentSessionId = null;
        LastClosedDate = DateTime.UtcNow;
        LastModifiedBy = closedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashDrawerClosedDomainEvent(Id, DrawerId, GetTotalCashBalance(), notes));
    }

    public void AddCash(Money amount, CashSource source, string addedBy, string? reference = null)
    {
        ValidateDrawerOpen();
        ValidateCashLimits(amount, isAddition: true);

        var currencyCode = amount.Currency.Code;
        var currentPosition = GetOrCreateCashPosition(currencyCode, amount.Currency);
        
        currentPosition.AddCash(amount, source, addedBy, reference);
        _cashPositions[currencyCode] = currentPosition;

        TotalCashIn += amount;
        NetCashMovement += amount;
        DailyTransactionCount++;
        LastTransactionDate = DateTime.UtcNow;

        LastModifiedBy = addedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashAddedToDr awerDomainEvent(Id, DrawerId, amount, source, reference));
    }

    public void RemoveCash(Money amount, CashDestination destination, string removedBy, string? reference = null)
    {
        ValidateDrawerOpen();
        ValidateCashAvailability(amount);
        ValidateCashLimits(amount, isAddition: false);

        var currencyCode = amount.Currency.Code;
        var currentPosition = GetCashPosition(currencyCode);
        
        currentPosition.RemoveCash(amount, destination, removedBy, reference);
        _cashPositions[currencyCode] = currentPosition;

        TotalCashOut += amount;
        NetCashMovement -= amount;
        DailyTransactionCount++;
        LastTransactionDate = DateTime.UtcNow;

        LastModifiedBy = removedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashRemovedFromDrawerDomainEvent(Id, DrawerId, amount, destination, reference));
    }

    public void TransferCash(CashDrawer targetDrawer, Money amount, string transferredBy, string? reference = null)
    {
        ValidateDrawerOpen();
        ValidateCashAvailability(amount);

        if (targetDrawer.Status != CashDrawerStatus.Open)
            throw new DomainException("Target cash drawer is not open");

        // Remove from this drawer
        RemoveCash(amount, CashDestination.AnotherTeller, transferredBy, reference);

        // Add to target drawer
        targetDrawer.AddCash(amount, CashSource.AnotherTeller, transferredBy, reference);

        AddDomainEvent(new CashTransferredBetweenDrawersDomainEvent(
            Id, DrawerId, targetDrawer.Id, targetDrawer.DrawerId, amount, reference));
    }

    public void ReconcileCash(Dictionary<string, Money> actualCashByCurrency, string reconciledBy, string? notes = null)
    {
        ValidateDrawerOpen();

        var totalDifference = Money.Zero(TotalCashIn.Currency);
        var hasDiscrepancies = false;

        foreach (var (currencyCode, actualAmount) in actualCashByCurrency)
        {
            var currentPosition = GetCashPosition(currencyCode);
            var expectedAmount = currentPosition.CurrentBalance;
            var difference = actualAmount - expectedAmount;

            if (!difference.IsZero())
            {
                hasDiscrepancies = true;
                totalDifference += difference;
                
                // Update position with actual amount
                currentPosition.Reconcile(actualAmount, difference, reconciledBy, notes);
                _cashPositions[currencyCode] = currentPosition;
            }
        }

        LastReconciliationDate = DateTime.UtcNow;
        LastReconciledBy = reconciledBy;
        LastReconciliationDifference = hasDiscrepancies ? totalDifference : null;

        LastModifiedBy = reconciledBy;
        LastModifiedDate = DateTime.UtcNow;

        if (hasDiscrepancies)
        {
            AddDomainEvent(new CashReconciliationCompletedDomainEvent(
                Id, DrawerId, totalDifference, actualCashByCurrency, notes));
        }
    }

    public Money GetCashBalance(string currencyCode)
    {
        return _cashPositions.TryGetValue(currencyCode, out var position) 
            ? position.CurrentBalance 
            : Money.Zero(new Currency(currencyCode));
    }

    public Money GetTotalCashBalance()
    {
        // Return total in base currency (assuming first currency is base)
        var baseCurrency = TotalCashIn.Currency;
        return _cashPositions.Values
            .Where(p => p.CurrentBalance.Currency.Code == baseCurrency.Code)
            .Sum(p => p.CurrentBalance.Amount, baseCurrency);
    }

    public bool HasSufficientCash(Money amount)
    {
        var position = _cashPositions.GetValueOrDefault(amount.Currency.Code);
        return position != null && position.CurrentBalance.IsGreaterThanOrEqual(amount);
    }

    public void Lock(string reason, string lockedBy)
    {
        Status = CashDrawerStatus.Locked;
        LastModifiedBy = lockedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashDrawerLockedDomainEvent(Id, DrawerId, reason, lockedBy));
    }

    public void Unlock(string unlockedBy, string? notes = null)
    {
        if (Status != CashDrawerStatus.Locked)
            throw new DomainException("Cash drawer is not locked");

        Status = CashDrawerStatus.Closed;
        LastModifiedBy = unlockedBy;
        LastModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new CashDrawerUnlockedDomainEvent(Id, DrawerId, unlockedBy, notes));
    }

    private void ValidateDrawerOpen()
    {
        if (Status != CashDrawerStatus.Open)
            throw new DomainException($"Cash drawer is not open. Current status: {Status}");
    }

    private void ValidateCashAvailability(Money amount)
    {
        if (!HasSufficientCash(amount))
        {
            var available = GetCashBalance(amount.Currency.Code);
            throw new DomainException($"Insufficient cash in {amount.Currency.Code}. Available: {available.Amount}, Required: {amount.Amount}");
        }
    }

    private void ValidateCashLimits(Money amount, bool isAddition)
    {
        if (isAddition)
        {
            var totalAfterAddition = GetTotalCashBalance() + amount;
            if (totalAfterAddition.IsGreaterThan(MaxCashLimit))
                throw new DomainException($"Adding {amount.Amount} would exceed maximum cash limit of {MaxCashLimit.Amount}");
        }
        else
        {
            var totalAfterRemoval = GetTotalCashBalance() - amount;
            if (totalAfterRemoval.IsLessThan(MinCashLimit))
                throw new DomainException($"Removing {amount.Amount} would go below minimum cash limit of {MinCashLimit.Amount}");
        }
    }

    private CashPosition GetOrCreateCashPosition(string currencyCode, Currency currency)
    {
        if (!_cashPositions.TryGetValue(currencyCode, out var position))
        {
            position = new CashPosition(currencyCode, Money.Zero(currency));
            _cashPositions[currencyCode] = position;
        }
        return position;
    }

    private CashPosition GetCashPosition(string currencyCode)
    {
        if (!_cashPositions.TryGetValue(currencyCode, out var position))
            throw new DomainException($"No cash position found for currency {currencyCode}");
        return position;
    }

    private void SetCashPosition(string currencyCode, Money amount, string setBy)
    {
        _cashPositions[currencyCode] = new CashPosition(currencyCode, amount);
    }

    private void ResetDailyCounters()
    {
        DailyTransactionCount = 0;
        TotalCashIn = Money.Zero(TotalCashIn.Currency);
        TotalCashOut = Money.Zero(TotalCashOut.Currency);
        NetCashMovement = Money.Zero(NetCashMovement.Currency);
    }
}

// Supporting enums and value objects
public enum CashDrawerStatus
{
    Closed,
    Open,
    Locked,
    Maintenance
}

public record CashPosition
{
    public string CurrencyCode { get; init; }
    public Money CurrentBalance { get; private set; }
    public Money DailyAdditions { get; private set; }
    public Money DailyRemovals { get; private set; }
    public DateTime LastUpdated { get; private set; }
    public Money? LastReconciliationDifference { get; private set; }

    public CashPosition(string currencyCode, Money initialBalance)
    {
        CurrencyCode = currencyCode;
        CurrentBalance = initialBalance;
        DailyAdditions = Money.Zero(initialBalance.Currency);
        DailyRemovals = Money.Zero(initialBalance.Currency);
        LastUpdated = DateTime.UtcNow;
    }

    public void AddCash(Money amount, CashSource source, string addedBy, string? reference)
    {
        CurrentBalance += amount;
        DailyAdditions += amount;
        LastUpdated = DateTime.UtcNow;
    }

    public void RemoveCash(Money amount, CashDestination destination, string removedBy, string? reference)
    {
        CurrentBalance -= amount;
        DailyRemovals += amount;
        LastUpdated = DateTime.UtcNow;
    }

    public void Reconcile(Money actualAmount, Money difference, string reconciledBy, string? notes)
    {
        CurrentBalance = actualAmount;
        LastReconciliationDifference = difference;
        LastUpdated = DateTime.UtcNow;
    }
}

// Domain Events
public record CashDrawerCreatedDomainEvent(
    Guid DrawerId,
    string DrawerNumber,
    Guid TellerId,
    Guid BranchId) : IDomainEvent;

public record CashDrawerOpenedDomainEvent(
    Guid DrawerId,
    string DrawerNumber,
    string SessionId,
    Money InitialCash) : IDomainEvent;

public record CashDrawerClosedDomainEvent(
    Guid DrawerId,
    string DrawerNumber,
    Money FinalCash,
    string? Notes) : IDomainEvent;

public record CashAddedToDrawerDomainEvent(
    Guid DrawerId,
    string DrawerNumber,
    Money Amount,
    CashSource Source,
    string? Reference) : IDomainEvent;

public record CashRemovedFromDrawerDomainEvent(
    Guid DrawerId,
    string DrawerNumber,
    Money Amount,
    CashDestination Destination,
    string? Reference) : IDomainEvent;

public record CashTransferredBetweenDrawersDomainEvent(
    Guid SourceDrawerId,
    string SourceDrawerNumber,
    Guid TargetDrawerId,
    string TargetDrawerNumber,
    Money Amount,
    string? Reference) : IDomainEvent;

public record CashReconciliationCompletedDomainEvent(
    Guid DrawerId,
    string DrawerNumber,
    Money TotalDifference,
    Dictionary<string, Money> ActualCashByCurrency,
    string? Notes) : IDomainEvent;

public record CashDrawerLockedDomainEvent(
    Guid DrawerId,
    string DrawerNumber,
    string Reason,
    string LockedBy) : IDomainEvent;

public record CashDrawerUnlockedDomainEvent(
    Guid DrawerId,
    string DrawerNumber,
    string UnlockedBy,
    string? Notes) : IDomainEvent;