using Wekeza.Core.Domain.Aggregates;

namespace Wekeza.Core.Domain.Interfaces;

/// <summary>
/// Repository contract for Card Application aggregate operations
/// </summary>
public interface ICardApplicationRepository
{
    Task<CardApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<CardApplication?> GetByApplicationNumberAsync(string applicationNumber, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetByStatusAsync(CardApplicationStatus status, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetByCardTypeAsync(CardType cardType, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetPendingApplicationsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetApplicationsRequiringDocumentsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetApplicationsUnderReviewAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetApplicationsPendingApprovalAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetApprovedApplicationsAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetApplicationsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetApplicationsByRiskCategoryAsync(string riskCategory, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetApplicationsRequiringManualReviewAsync(CancellationToken cancellationToken = default);
    
    Task<IEnumerable<CardApplication>> GetApplicationsByWorkflowInstanceAsync(Guid workflowInstanceId, CancellationToken cancellationToken = default);
    
    Task<int> GetApplicationCountByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    
    Task<int> GetApplicationCountByStatusAsync(CardApplicationStatus status, CancellationToken cancellationToken = default);
    
    Task AddAsync(CardApplication application, CancellationToken cancellationToken = default);
    
    void Update(CardApplication application);
    
    Task UpdateAsync(CardApplication application, CancellationToken cancellationToken = default);
}