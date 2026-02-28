using Wekeza.Core.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Infrastructure.Repositories.Admin;

public class BranchOperationsRepository
{
    private readonly ApplicationDbContext _context;

    public BranchOperationsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ===== Branch Operations =====
    public async Task<Branch> GetBranchByIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken);
    }

    public async Task<List<Branch>> GetAllBranchesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Branches
            .AsNoTracking()
            .OrderBy(b => b.BranchName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Branch> UpdateBranchAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        _context.Branches.Update(branch);
        await _context.SaveChangesAsync(cancellationToken);
        return branch;
    }

    // ===== Teller Operations =====
    public async Task<Teller> GetTellerByIdAsync(Guid tellerId, CancellationToken cancellationToken = default)
    {
        return await _context.Tellers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tellerId, cancellationToken);
    }

    public async Task<List<Teller>> GetBranchTellersAsync(Guid branchId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Tellers
            .AsNoTracking()
            .Where(t => t.BranchId == branchId)
            .OrderBy(t => t.TellerCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Teller> UpdateTellerAsync(Teller teller, CancellationToken cancellationToken = default)
    {
        _context.Tellers.Update(teller);
        await _context.SaveChangesAsync(cancellationToken);
        return teller;
    }

    public async Task<int> GetActiveTellerCountAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.Tellers
            .CountAsync(t => t.BranchId == branchId && t.Status == "Active", cancellationToken);
    }

    // ===== Teller Session Operations =====
    public async Task<TellerSession> GetSessionByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.TellerSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessionId, cancellationToken);
    }

    public async Task<TellerSession> AddSessionAsync(TellerSession session, CancellationToken cancellationToken = default)
    {
        await _context.TellerSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task<TellerSession> UpdateSessionAsync(TellerSession session, CancellationToken cancellationToken = default)
    {
        _context.TellerSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
        return session;
    }

    public async Task<List<TellerSession>> GetTellerSessionHistoryAsync(Guid tellerId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = _context.TellerSessions
            .AsNoTracking()
            .Where(s => s.TellerId == tellerId);

        if (fromDate.HasValue)
            query = query.Where(s => s.StartedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.StartedAt <= toDate.Value);

        return await query
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync(cancellationToken);
    }

    // ===== Cash Drawer Operations =====
    public async Task<CashDrawer> GetCashDrawerByIdAsync(Guid drawerId, CancellationToken cancellationToken = default)
    {
        return await _context.CashDrawers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == drawerId, cancellationToken);
    }

    public async Task<List<CashDrawer>> GetBranchCashDrawersAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.CashDrawers
            .AsNoTracking()
            .Where(c => c.BranchId == branchId)
            .OrderBy(c => c.DrawerCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<CashDrawer> UpdateCashDrawerAsync(CashDrawer drawer, CancellationToken cancellationToken = default)
    {
        _context.CashDrawers.Update(drawer);
        await _context.SaveChangesAsync(cancellationToken);
        return drawer;
    }

    public async Task<decimal> GetBranchTotalCashAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.CashDrawers
            .Where(c => c.BranchId == branchId && c.Status == "Open")
            .SumAsync(c => c.CurrentBalance, cancellationToken);
    }

    // ===== Teller Transaction Operations =====
    public async Task<TellerTransaction> GetTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _context.TellerTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);
    }

    public async Task<List<TellerTransaction>> SearchTransactionsAsync(Guid? tellerId, DateTime? fromDate, DateTime? toDate, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.TellerTransactions.AsNoTracking();

        if (tellerId.HasValue)
            query = query.Where(t => t.TellerId == tellerId.Value);

        if (fromDate.HasValue)
            query = query.Where(t => t.TransactionDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.TransactionDate <= toDate.Value);

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<TellerTransaction> UpdateTransactionAsync(TellerTransaction transaction, CancellationToken cancellationToken = default)
    {
        _context.TellerTransactions.Update(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    public async Task<int> GetTellerTransactionCountAsync(Guid tellerId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.TellerTransactions
            .CountAsync(t => t.TellerId == tellerId && t.TransactionDate >= fromDate && t.TransactionDate <= toDate, cancellationToken);
    }

    // ===== Branch User Management =====
    public async Task<BranchUser> GetBranchUserAsync(Guid userId, Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.BranchUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BranchId == branchId, cancellationToken);
    }

    public async Task<List<BranchUser>> GetBranchUsersAsync(Guid branchId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.BranchUsers
            .AsNoTracking()
            .Where(b => b.BranchId == branchId)
            .OrderBy(b => b.UserName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<BranchUser> AddBranchUserAsync(BranchUser branchUser, CancellationToken cancellationToken = default)
    {
        await _context.BranchUsers.AddAsync(branchUser, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return branchUser;
    }

    public async Task<BranchUser> UpdateBranchUserAsync(BranchUser branchUser, CancellationToken cancellationToken = default)
    {
        _context.BranchUsers.Update(branchUser);
        await _context.SaveChangesAsync(cancellationToken);
        return branchUser;
    }

    public async Task<int> GetBranchStaffCountAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _context.BranchUsers
            .CountAsync(b => b.BranchId == branchId && b.Status == "Active", cancellationToken);
    }

    // ===== Branch Reporting =====
    public async Task<int> GetDailyTransactionCountAsync(Guid branchId, DateTime reportDate, CancellationToken cancellationToken = default)
    {
        return await _context.TellerTransactions
            .CountAsync(t => t.TransactionDate.Date == reportDate.Date && EF.Functions.JsonContains(
                EF.Property<string>(t, "Details"), "{\"branchId\":\"" + branchId + "\"}"), cancellationToken);
    }
}

// Placeholder domain entities
public class Branch { public Guid Id { get; set; } public string BranchCode { get; set; } public string BranchName { get; set; } public string Status { get; set; } }
public class Teller { public Guid Id { get; set; } public Guid BranchId { get; set; } public string TellerCode { get; set; } public string Status { get; set; } }
public class TellerSession { public Guid Id { get; set; } public Guid TellerId { get; set; } public DateTime StartedAt { get; set; } public DateTime? EndedAt { get; set; } }
public class CashDrawer { public Guid Id { get; set; } public Guid BranchId { get; set; } public string DrawerCode { get; set; } public string Status { get; set; } public decimal CurrentBalance { get; set; } }
public class TellerTransaction { public Guid Id { get; set; } public Guid TellerId { get; set; } public DateTime TransactionDate { get; set; } public string Details { get; set; } }
public class BranchUser { public Guid UserId { get; set; } public Guid BranchId { get; set; } public string UserName { get; set; } public string Status { get; set; } }
