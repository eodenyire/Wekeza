using Wekeza.Core.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Infrastructure.Repositories.Admin;

public class CustomerServiceRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerServiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // ===== Customer Management =====
    public async Task<Customer> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
    }

    public async Task<List<Customer>> SearchCustomersAsync(string searchTerm, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.AsNoTracking();

        if (!string.IsNullOrEmpty(searchTerm))
            query = query.Where(c => EF.Functions.ILike(c.FullName, $"%{searchTerm}%") || 
                                     EF.Functions.ILike(c.Email, $"%{searchTerm}%") ||
                                     EF.Functions.ILike(c.PhoneNumber, $"%{searchTerm}%"));

        if (!string.IsNullOrEmpty(status))
            query = query.Where(c => c.Status == status);

        return await query
            .OrderByDescending(c => c.CustomerSince)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    // ===== Complaint Management =====
    public async Task<Complaint> GetComplaintByIdAsync(Guid complaintId, CancellationToken cancellationToken = default)
    {
        return await _context.Complaints
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == complaintId, cancellationToken);
    }

    public async Task<List<Complaint>> SearchComplaintsAsync(string status, string priority, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Complaints.AsNoTracking();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(c => c.Status == status);

        if (!string.IsNullOrEmpty(priority))
            query = query.Where(c => c.Priority == priority);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Complaint> AddComplaintAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        await _context.Complaints.AddAsync(complaint, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return complaint;
    }

    public async Task<Complaint> UpdateComplaintAsync(Complaint complaint, CancellationToken cancellationToken = default)
    {
        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync(cancellationToken);
        return complaint;
    }

    public async Task<int> GetOpenComplaintsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Complaints.CountAsync(c => c.Status == "Open", cancellationToken);
    }

    // ===== Service Request Operations =====
    public async Task<ServiceRequest> GetServiceRequestByIdAsync(Guid requestId, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == requestId, cancellationToken);
    }

    public async Task<List<ServiceRequest>> SearchServiceRequestsAsync(string status, string requestType, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.ServiceRequests.AsNoTracking();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(s => s.Status == status);

        if (!string.IsNullOrEmpty(requestType))
            query = query.Where(s => s.RequestType == requestType);

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceRequest> AddServiceRequestAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        await _context.ServiceRequests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request, CancellationToken cancellationToken = default)
    {
        _context.ServiceRequests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<int> GetPendingRequestsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ServiceRequests.CountAsync(s => s.Status == "Pending", cancellationToken);
    }

    // ===== Customer Feedback Operations =====
    public async Task<Feedback> GetFeedbackByIdAsync(Guid feedbackId, CancellationToken cancellationToken = default)
    {
        return await _context.Feedback
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == feedbackId, cancellationToken);
    }

    public async Task<List<Feedback>> SearchFeedbackAsync(string category, int? rating, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Feedback.AsNoTracking();

        if (!string.IsNullOrEmpty(category))
            query = query.Where(f => f.Category == category);

        if (rating.HasValue)
            query = query.Where(f => f.Rating == rating.Value);

        return await query
            .OrderByDescending(f => f.ProvidedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Feedback> AddFeedbackAsync(Feedback feedback, CancellationToken cancellationToken = default)
    {
        await _context.Feedback.AddAsync(feedback, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return feedback;
    }

    public async Task<double> GetAverageFeedbackRatingAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = _context.Feedback.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(f => f.ProvidedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(f => f.ProvidedAt <= toDate.Value);

        return await query.AverageAsync(f => (double)f.Rating, cancellationToken);
    }

    // ===== Communication Records =====
    public async Task<CommunicationRecord> GetCommunicationByIdAsync(Guid recordId, CancellationToken cancellationToken = default)
    {
        return await _context.CommunicationRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == recordId, cancellationToken);
    }

    public async Task<List<CommunicationRecord>> GetCustomerCommunicationAsync(Guid customerId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.CommunicationRecords
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.CommunicatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<CommunicationRecord> AddCommunicationAsync(CommunicationRecord record, CancellationToken cancellationToken = default)
    {
        await _context.CommunicationRecords.AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return record;
    }

    // ===== Customer Segment & Relationship =====
    public async Task<List<Customer>> GetSegmentCustomersAsync(string segmentCode, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .Where(c => c.Segment == segmentCode && c.Status == "Active")
            .OrderBy(c => c.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCustomerSegmentCountAsync(string segmentCode, CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .CountAsync(c => c.Segment == segmentCode && c.Status == "Active", cancellationToken);
    }

    // ===== Dashboard Metrics =====
    public async Task<int> GetActiveCustomersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers.CountAsync(c => c.Status == "Active", cancellationToken);
    }

    public async Task<int> GetNewCustomersCountAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(c => c.CustomerSince >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(c => c.CustomerSince <= toDate.Value);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<double> GetAverageCustomerLifetimeValueAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Customers
            .Where(c => c.Status == "Active")
            .AverageAsync(c => c.LifetimeValue, cancellationToken);
    }
}

// Placeholder domain entities
public class Customer { public Guid Id { get; set; } public string FullName { get; set; } public string Email { get; set; } public string PhoneNumber { get; set; } public string Status { get; set; } public string Segment { get; set; } public DateTime CustomerSince { get; set; } public decimal LifetimeValue { get; set; } }
public class Complaint { public Guid Id { get; set; } public Guid CustomerId { get; set; } public string ComplaintNumber { get; set; } public string Status { get; set; } public string Priority { get; set; } public DateTime CreatedAt { get; set; } }
public class ServiceRequest { public Guid Id { get; set; } public Guid CustomerId { get; set; } public string RequestNumber { get; set; } public string Status { get; set; } public string RequestType { get; set; } public DateTime CreatedAt { get; set; } }
public class Feedback { public Guid Id { get; set; } public Guid CustomerId { get; set; } public string Category { get; set; } public int Rating { get; set; } public DateTime ProvidedAt { get; set; } }
public class CommunicationRecord { public Guid Id { get; set; } public Guid CustomerId { get; set; } public string Channel { get; set; } public DateTime CommunicatedAt { get; set; } }
