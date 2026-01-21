using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Branch aggregate - Complete branch operations management
/// Supports EOD/BOD processing, vault management, and operational controls
/// </summary>
public class Branch : AggregateRoot
{
    public string BranchCode { get; private set; }
    public string BranchName { get; private set; }
    public string Address { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Email { get; private set; }
    public BranchType BranchType { get; private set; }
    public BranchStatus Status { get; private set; }
    public DateTime OpeningDate { get; private set; }
    public DateTime? ClosingDate { get; private set; }
    public string TimeZone { get; private set; }
    public TimeSpan BusinessHoursStart { get; private set; }
    public TimeSpan BusinessHoursEnd { get; private set; }
    public string ManagerId { get; private set; }
    public string? DeputyManagerId { get; private set; }
    public Money CashLimit { get; private set; }
    public Money TransactionLimit { get; private set; }
    public bool IsEODCompleted { get; private set; }
    public DateTime? LastEODDate { get; private set; }
    public bool IsBODCompleted { get; private set; }
    public DateTime? LastBODDate { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    private readonly List<BranchVault> _vaults = new();
    public IReadOnlyList<BranchVault> Vaults => _vaults.AsReadOnly();

    private readonly List<BranchLimit> _limits = new();
    public IReadOnlyList<BranchLimit> Limits => _limits.AsReadOnly();

    private readonly List<BranchPerformance> _performanceMetrics = new();
    public IReadOnlyList<BranchPerformance> PerformanceMetrics => _performanceMetrics.AsReadOnly();

    private Branch() : base(Guid.NewGuid()) { } // EF Core

    public Branch(
        Guid id,
        string branchCode,
        string branchName,
        string address,
        string city,
        string country,
        string phoneNumber,
        string email,
        BranchType branchType,
        DateTime openingDate,
        string timeZone,
        TimeSpan businessHoursStart,
        TimeSpan businessHoursEnd,
        string managerId,
        Money cashLimit,
        Money transactionLimit,
        string createdBy) : base(id) {
        Id = id;
        BranchCode = branchCode;
        BranchName = branchName;
        Address = address;
        City = city;
        Country = country;
        PhoneNumber = phoneNumber;
        Email = email;
        BranchType = branchType;
        OpeningDate = openingDate;
        TimeZone = timeZone;
        BusinessHoursStart = businessHoursStart;
        BusinessHoursEnd = businessHoursEnd;
        ManagerId = managerId;
        CashLimit = cashLimit;
        TransactionLimit = transactionLimit;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        
        Status = BranchStatus.Active;
        IsEODCompleted = false;
        IsBODCompleted = false;

        // Create default main vault
        var mainVault = new BranchVault(
            Guid.NewGuid(),
            Id,
            "MAIN",
            "Main Vault",
            VaultType.Main,
            cashLimit,
            new Money(0, cashLimit.Currency),
            true,
            createdBy,
            DateTime.UtcNow);

        _vaults.Add(mainVault);

        AddDomainEvent(new BranchCreatedDomainEvent(Id, BranchCode, BranchName, ManagerId));
    }

    public void ProcessBOD(string processedBy)
    {
        if (IsBODCompleted && LastBODDate?.Date == DateTime.UtcNow.Date)
            throw new InvalidOperationException("BOD has already been completed for today");

        if (!IsEODCompleted || LastEODDate?.Date != DateTime.UtcNow.AddDays(-1).Date)
            throw new InvalidOperationException("Previous day's EOD must be completed before BOD");

        // Reset daily flags and counters
        IsBODCompleted = true;
        LastBODDate = DateTime.UtcNow;
        IsEODCompleted = false;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        // Reset vault daily limits
        foreach (var vault in _vaults)
        {
            vault.ResetDailyLimits();
        }

        AddDomainEvent(new BranchBODCompletedDomainEvent(Id, BranchCode, processedBy, DateTime.UtcNow));
    }

    public void ProcessEOD(string processedBy)
    {
        if (IsEODCompleted && LastEODDate?.Date == DateTime.UtcNow.Date)
            throw new InvalidOperationException("EOD has already been completed for today");

        if (!IsBODCompleted || LastBODDate?.Date != DateTime.UtcNow.Date)
            throw new InvalidOperationException("BOD must be completed before EOD");

        // Validate all teller sessions are closed
        ValidateAllTellerSessionsClosed();

        // Validate vault balances
        ValidateVaultBalances();

        // Calculate daily performance metrics
        CalculateDailyPerformance(processedBy);

        IsEODCompleted = true;
        LastEODDate = DateTime.UtcNow;
        ModifiedBy = processedBy;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new BranchEODCompletedDomainEvent(Id, BranchCode, processedBy, DateTime.UtcNow));
    }

    private void ValidateAllTellerSessionsClosed()
    {
        // This would check with teller session repository
        // For now, we'll assume validation passes
    }

    private void ValidateVaultBalances()
    {
        foreach (var vault in _vaults)
        {
            if (!vault.IsBalanced())
            {
                throw new InvalidOperationException($"Vault {vault.VaultCode} is not balanced");
            }
        }
    }

    private void CalculateDailyPerformance(string calculatedBy)
    {
        var performance = new BranchPerformance(
            Guid.NewGuid(),
            Id,
            DateTime.UtcNow.Date,
            0, // Would be calculated from actual transactions
            new Money(0, CashLimit.Currency), // Would be calculated from actual transactions
            0, // Would be calculated from actual customers
            0, // Would be calculated from actual accounts
            calculatedBy,
            DateTime.UtcNow);

        _performanceMetrics.Add(performance);
    }

    public void AddVault(string vaultCode, string vaultName, VaultType vaultType, Money capacity, string addedBy)
    {
        if (_vaults.Any(v => v.VaultCode == vaultCode))
            throw new InvalidOperationException("Vault with this code already exists");

        var vault = new BranchVault(
            Guid.NewGuid(),
            Id,
            vaultCode,
            vaultName,
            vaultType,
            capacity,
            new Money(0, capacity.Currency),
            true,
            addedBy,
            DateTime.UtcNow);

        _vaults.Add(vault);

        AddDomainEvent(new BranchVaultAddedDomainEvent(Id, BranchCode, vaultCode, capacity));
    }

    public void UpdateVaultBalance(string vaultCode, Money newBalance, string updatedBy)
    {
        var vault = _vaults.FirstOrDefault(v => v.VaultCode == vaultCode);
        if (vault == null)
            throw new InvalidOperationException("Vault not found");

        vault.UpdateBalance(newBalance, updatedBy);

        AddDomainEvent(new BranchVaultBalanceUpdatedDomainEvent(Id, BranchCode, vaultCode, newBalance));
    }

    public void AddLimit(string limitType, Money limitAmount, string addedBy)
    {
        var existingLimit = _limits.FirstOrDefault(l => l.LimitType == limitType);
        if (existingLimit != null)
        {
            existingLimit.UpdateLimit(limitAmount, addedBy);
        }
        else
        {
            var limit = new BranchLimit(
                Guid.NewGuid(),
                Id,
                limitType,
                limitAmount,
                addedBy,
                DateTime.UtcNow);

            _limits.Add(limit);
        }

        AddDomainEvent(new BranchLimitUpdatedDomainEvent(Id, BranchCode, limitType, limitAmount));
    }

    public void UpdateManager(string newManagerId, string updatedBy)
    {
        var previousManager = ManagerId;
        ManagerId = newManagerId;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new BranchManagerUpdatedDomainEvent(Id, BranchCode, previousManager, newManagerId));
    }

    public void UpdateStatus(BranchStatus newStatus, string updatedBy, string? reason = null)
    {
        if (Status == newStatus)
            return;

        var previousStatus = Status;
        Status = newStatus;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;

        if (newStatus == BranchStatus.Closed)
        {
            ClosingDate = DateTime.UtcNow;
        }

        AddDomainEvent(new BranchStatusUpdatedDomainEvent(Id, BranchCode, previousStatus, newStatus, reason));
    }

    public bool IsOperational()
    {
        return Status == BranchStatus.Active && IsBODCompleted && !IsEODCompleted;
    }

    public bool IsWithinBusinessHours()
    {
        var currentTime = DateTime.UtcNow.TimeOfDay;
        return currentTime >= BusinessHoursStart && currentTime <= BusinessHoursEnd;
    }

    public Money GetTotalVaultBalance()
    {
        if (!_vaults.Any())
            return new Money(0, CashLimit.Currency);

        var total = _vaults.Sum(v => v.CurrentBalance.Amount);
        return new Money(total, CashLimit.Currency);
    }

    public Money GetVaultCapacity()
    {
        if (!_vaults.Any())
            return new Money(0, CashLimit.Currency);

        var total = _vaults.Sum(v => v.Capacity.Amount);
        return new Money(total, CashLimit.Currency);
    }

    public decimal GetVaultUtilization()
    {
        var capacity = GetVaultCapacity();
        if (capacity.Amount == 0)
            return 0;

        var balance = GetTotalVaultBalance();
        return (balance.Amount / capacity.Amount) * 100;
    }

    public BranchVault? GetVault(string vaultCode)
    {
        return _vaults.FirstOrDefault(v => v.VaultCode == vaultCode);
    }

    public BranchVault GetMainVault()
    {
        return _vaults.First(v => v.VaultType == VaultType.Main);
    }

    public Money GetLimit(string limitType)
    {
        var limit = _limits.FirstOrDefault(l => l.LimitType == limitType);
        return limit?.LimitAmount ?? new Money(0, CashLimit.Currency);
    }

    public BranchPerformance? GetTodayPerformance()
    {
        return _performanceMetrics.FirstOrDefault(p => p.PerformanceDate.Date == DateTime.UtcNow.Date);
    }

    public IEnumerable<BranchPerformance> GetPerformanceHistory(DateTime fromDate, DateTime toDate)
    {
        return _performanceMetrics.Where(p => p.PerformanceDate >= fromDate && p.PerformanceDate <= toDate);
    }

    public bool CanProcessTransaction(Money amount)
    {
        return IsOperational() && amount.Amount <= TransactionLimit.Amount;
    }

    public bool HasSufficientCash(Money amount)
    {
        return GetTotalVaultBalance().Amount >= amount.Amount;
    }
}

/// <summary>
/// Branch Vault for cash management
/// </summary>
public class BranchVault
{
    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public string VaultCode { get; private set; }
    public string VaultName { get; private set; }
    public VaultType VaultType { get; private set; }
    public Money Capacity { get; private set; }
    public Money CurrentBalance { get; private set; }
    public Money DailyLimit { get; private set; }
    public Money DailyUsed { get; private set; }
    public bool IsActive { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    private BranchVault() { Id = Guid.NewGuid(); } // EF Core

    public BranchVault(Guid id, Guid branchId, string vaultCode, string vaultName, VaultType vaultType, Money capacity, Money currentBalance, bool isActive, string createdBy, DateTime createdAt)
    {
        Id = id;
        BranchId = branchId;
        VaultCode = vaultCode;
        VaultName = vaultName;
        VaultType = vaultType;
        Capacity = capacity;
        CurrentBalance = currentBalance;
        DailyLimit = new Money(capacity.Amount * 0.1m, capacity.Currency); // 10% of capacity as daily limit
        DailyUsed = new Money(0, capacity.Currency);
        IsActive = isActive;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public void UpdateBalance(Money newBalance, string updatedBy)
    {
        if (newBalance.Amount > Capacity.Amount)
            throw new InvalidOperationException("Balance cannot exceed vault capacity");

        CurrentBalance = newBalance;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void AddCash(Money amount, string addedBy)
    {
        var newBalance = new Money(CurrentBalance.Amount + amount.Amount, CurrentBalance.Currency);
        UpdateBalance(newBalance, addedBy);
    }

    public void RemoveCash(Money amount, string removedBy)
    {
        if (amount.Amount > CurrentBalance.Amount)
            throw new InvalidOperationException("Insufficient cash in vault");

        var newBalance = new Money(CurrentBalance.Amount - amount.Amount, CurrentBalance.Currency);
        var newDailyUsed = new Money(DailyUsed.Amount + amount.Amount, DailyUsed.Currency);

        if (newDailyUsed.Amount > DailyLimit.Amount)
            throw new InvalidOperationException("Daily limit exceeded");

        CurrentBalance = newBalance;
        DailyUsed = newDailyUsed;
        ModifiedBy = removedBy;
        ModifiedAt = DateTime.UtcNow;
    }

    public void ResetDailyLimits()
    {
        DailyUsed = new Money(0, DailyUsed.Currency);
    }

    public bool IsBalanced()
    {
        // This would include physical count validation
        return true; // Simplified for now
    }

    public decimal GetUtilization()
    {
        return Capacity.Amount > 0 ? (CurrentBalance.Amount / Capacity.Amount) * 100 : 0;
    }

    public Money GetAvailableCapacity()
    {
        return new Money(Capacity.Amount - CurrentBalance.Amount, Capacity.Currency);
    }

    public Money GetRemainingDailyLimit()
    {
        return new Money(DailyLimit.Amount - DailyUsed.Amount, DailyLimit.Currency);
    }
}

/// <summary>
/// Branch operational limits
/// </summary>
public class BranchLimit
{
    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public string LimitType { get; private set; }
    public Money LimitAmount { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    private BranchLimit() { Id = Guid.NewGuid(); } // EF Core

    public BranchLimit(Guid id, Guid branchId, string limitType, Money limitAmount, string createdBy, DateTime createdAt)
    {
        Id = id;
        BranchId = branchId;
        LimitType = limitType;
        LimitAmount = limitAmount;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
    }

    public void UpdateLimit(Money newLimitAmount, string updatedBy)
    {
        LimitAmount = newLimitAmount;
        ModifiedBy = updatedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Branch daily performance metrics
/// </summary>
public class BranchPerformance
{
    public Guid Id { get; private set; }
    public Guid BranchId { get; private set; }
    public DateTime PerformanceDate { get; private set; }
    public int TransactionCount { get; private set; }
    public Money TransactionVolume { get; private set; }
    public int NewCustomers { get; private set; }
    public int NewAccounts { get; private set; }
    public string CalculatedBy { get; private set; }
    public DateTime CalculatedAt { get; private set; }

    private BranchPerformance() { Id = Guid.NewGuid(); } // EF Core

    public BranchPerformance(Guid id, Guid branchId, DateTime performanceDate, int transactionCount, Money transactionVolume, int newCustomers, int newAccounts, string calculatedBy, DateTime calculatedAt)
    {
        Id = id;
        BranchId = branchId;
        PerformanceDate = performanceDate;
        TransactionCount = transactionCount;
        TransactionVolume = transactionVolume;
        NewCustomers = newCustomers;
        NewAccounts = newAccounts;
        CalculatedBy = calculatedBy;
        CalculatedAt = calculatedAt;
    }
}

// Enums
public enum BranchType
{
    Main = 1,
    Regional = 2,
    Urban = 3,
    Rural = 4,
    Corporate = 5,
    Retail = 6,
    Digital = 7,
    ATMOnly = 8
}

public enum BranchStatus
{
    Active = 1,
    Inactive = 2,
    Maintenance = 3,
    Closed = 4,
    Suspended = 5
}

public enum VaultType
{
    Main = 1,
    Secondary = 2,
    ATM = 3,
    Teller = 4,
    Night = 5
}

// Domain Events
public record BranchCreatedDomainEvent(
    Guid BranchId,
    string BranchCode,
    string BranchName,
    string ManagerId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BranchBODCompletedDomainEvent(
    Guid BranchId,
    string BranchCode,
    string ProcessedBy,
    DateTime ProcessedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BranchEODCompletedDomainEvent(
    Guid BranchId,
    string BranchCode,
    string ProcessedBy,
    DateTime ProcessedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BranchVaultAddedDomainEvent(
    Guid BranchId,
    string BranchCode,
    string VaultCode,
    Money Capacity) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BranchVaultBalanceUpdatedDomainEvent(
    Guid BranchId,
    string BranchCode,
    string VaultCode,
    Money NewBalance) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BranchLimitUpdatedDomainEvent(
    Guid BranchId,
    string BranchCode,
    string LimitType,
    Money LimitAmount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BranchManagerUpdatedDomainEvent(
    Guid BranchId,
    string BranchCode,
    string PreviousManager,
    string NewManager) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public record BranchStatusUpdatedDomainEvent(
    Guid BranchId,
    string BranchCode,
    BranchStatus PreviousStatus,
    BranchStatus NewStatus,
    string? Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

