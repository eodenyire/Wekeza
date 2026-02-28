using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Customer Service Admin Service - Customer Management, Complaints, Support, Service Requests
/// Customer service portal for managing customer relationships, complaints, requests, and feedback
/// </summary>
public interface ICustomerServiceAdminService
{
    // ===== Customer Management =====
    Task<CustomerServiceDTO> GetCustomerAsync(Guid customerId);
    Task<List<CustomerServiceDTO>> SearchCustomersAsync(string? searchTerm = null, string? status = null, int page = 1, int pageSize = 50);
    Task<CustomerServiceDTO> UpdateCustomerAsync(Guid customerId, UpdateCustomerRequest request, Guid updatedByUserId);
    Task<CustomerProfileDTO> GetCustomerProfileAsync(Guid customerId);
    Task UpdateCustomerProfileAsync(Guid customerId, UpdateCustomerProfileRequest request, Guid updatedByUserId);
    Task<CustomerPreferencesDTO> GetCustomerPreferencesAsync(Guid customerId);
    Task UpdateCustomerPreferencesAsync(Guid customerId, UpdateCustomerPreferencesRequest request);

    // ===== Complaint Management =====
    Task<ComplaintDTO> GetComplaintAsync(Guid complaintId);
    Task<List<ComplaintDTO>> SearchComplaintsAsync(string? status = null, string? priority = null, int page = 1, int pageSize = 50);
    Task<ComplaintDTO> CreateComplaintAsync(CreateComplaintRequest request, Guid createdByUserId);
    Task<ComplaintDTO> UpdateComplaintAsync(Guid complaintId, UpdateComplaintRequest request, Guid updatedByUserId);
    Task AssignComplaintAsync(Guid complaintId, Guid assigneeUserId, string reason, Guid assignedByUserId);
    Task ReassignComplaintAsync(Guid complaintId, Guid newAssigneeUserId, string reason, Guid reassignedByUserId);
    Task AddComplaintNoteAsync(Guid complaintId, string note, Guid addedByUserId);
    Task ResolveComplaintAsync(Guid complaintId, string resolution, string resolutionCategory, Guid resolvedByUserId);
    Task CloseComplaintAsync(Guid complaintId, Guid closedByUserId);
    Task EscalateComplaintAsync(Guid complaintId, string escalationReason, Guid escalatedByUserId);
    Task<ComplaintResolutionSummaryDTO> GetResolutionSummaryAsync(Guid complaintId);

    // ===== Customer Service Requests =====
    Task<ServiceRequestDTO> GetServiceRequestAsync(Guid requestId);
    Task<List<ServiceRequestDTO>> SearchServiceRequestsAsync(string? status = null, string? requestType = null, int page = 1, int pageSize = 50);
    Task<ServiceRequestDTO> CreateServiceRequestAsync(CreateServiceRequestRequest request, Guid createdByUserId);
    Task<ServiceRequestDTO> UpdateServiceRequestAsync(Guid requestId, UpdateServiceRequestRequest request, Guid updatedByUserId);
    Task FulfillServiceRequestAsync(Guid requestId, ServiceRequestFulfillmentRequest request, Guid fulfilledByUserId);
    Task<List<ServiceRequestDTO>> GetPendingRequestsAsync(Guid? customerId = null, int pageSize = 50);
    Task<ServiceRequestPerformanceDTO> GetServiceRequestPerformanceAsync(DateTime? fromDate = null, DateTime? toDate = null);

    // ===== Customer Feedback =====
    Task<FeedbackDTO> GetFeedbackAsync(Guid feedbackId);
    Task<List<FeedbackDTO>> SearchFeedbackAsync(string? category = null, int? rating = null, int page = 1, int pageSize = 50);
    Task<FeedbackDTO> CreateFeedbackAsync(CreateFeedbackRequest request, Guid customerId);
    Task AnalyzeFeedbackAsync(Guid feedbackId, Guid analyzedByUserId);
    Task<FeedbackAnalysisDTO> GetFeedbackAnalysisAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<FeedbackTrendDTO>> GetFeedbackTrendsAsync(int periodMonths = 12);

    // ===== Customer Communication =====
    Task<CommunicationRecordDTO> GetCommunicationRecordAsync(Guid recordId);
    Task<List<CommunicationRecordDTO>> GetCustomerCommunicationAsync(Guid customerId, int page = 1, int pageSize = 50);
    Task LogCommunicationAsync(Guid customerId, LogCommunicationRequest request, Guid loggedByUserId);
    Task SendNotificationAsync(Guid customerId, SendNotificationRequest request, Guid sentByUserId);
    Task<CommunicationPreferenceDTO> GetCommunicationPreferenceAsync(Guid customerId);
    Task UpdateCommunicationPreferenceAsync(Guid customerId, UpdateCommunicationPreferenceRequest request);

    // ===== Customer Relationship Management =====
    Task<CustomerSegmentDTO> GetCustomerSegmentAsync(Guid customerId);
    Task<List<CustomerDTO>> GetSegmentCustomersAsync(string segmentCode, int page = 1, int pageSize = 50);
    Task<CustomerLifetimeValueDTO> GetLifetimeValueAsync(Guid customerId);
    Task<List<CustomerOfferDTO>> RecommendOfferAsync(Guid customerId);
    Task<CustomerRiskAssessmentDTO> AssessCustomerRiskAsync(Guid customerId);

    // ===== Customer Service Dashboard =====
    Task<CustomerServiceDashboardDTO> GetDashboardAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<CustomerServiceMetricsDTO> GetMetricsAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<CustomerServiceAlertDTO>> GetAlertsAsync();
    Task<CustomerSatisfactionDTO> GetSatisfactionScoresAsync(DateTime? fromDate = null, DateTime? toDate = null);
}

// DTOs
public class CustomerServiceDTO
{
    public Guid Id { get; set; }
    public string CustomerCode { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Status { get; set; }
    public string Segment { get; set; }
    public DateTime CustomerSince { get; set; }
    public decimal LifetimeValue { get; set; }
}

public class UpdateCustomerRequest
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Dictionary<string, object> ContactDetails { get; set; }
}

public class CustomerProfileDTO
{
    public Guid CustomerId { get; set; }
    public string KYCStatus { get; set; }
    public DateTime KYCApprovedDate { get; set; }
    public string RiskLevel { get; set; }
    public List<string> AccountTypes { get; set; }
    public decimal TotalBalance { get; set; }
    public Dictionary<string, object> ProfileData { get; set; }
}

public class UpdateCustomerProfileRequest
{
    public Dictionary<string, object> ProfileData { get; set; }
    public List<string> UpdatedFields { get; set; }
}

public class CustomerPreferencesDTO
{
    public Guid CustomerId { get; set; }
    public string CommunicationPreference { get; set; }
    public bool ReceiveMarketingCommunications { get; set; }
    public List<string> PreferredChannels { get; set; }
    public string Language { get; set; }
}

public class UpdateCustomerPreferencesRequest
{
    public string CommunicationPreference { get; set; }
    public bool ReceiveMarketingCommunications { get; set; }
    public List<string> PreferredChannels { get; set; }
}

public class ComplaintDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string ComplaintNumber { get; set; }
    public string Category { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string AssignedToUser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string ResolutionNotes { get; set; }
}

public class CreateComplaintRequest
{
    public Guid CustomerId { get; set; }
    public string Category { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public List<string> Attachments { get; set; }
}

public class UpdateComplaintRequest
{
    public string Subject { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
}

public class ComplaintResolutionSummaryDTO
{
    public Guid ComplaintId { get; set; }
    public string Category { get; set; }
    public string ResolutionCategory { get; set; }
    public string Resolution { get; set; }
    public int DaysToResolve { get; set; }
    public bool CustomerSatisfied { get; set; }
}

public class ServiceRequestDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string RequestNumber { get; set; }
    public string RequestType { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CreateServiceRequestRequest
{
    public Guid CustomerId { get; set; }
    public string RequestType { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> RequestDetails { get; set; }
}

public class UpdateServiceRequestRequest
{
    public string Description { get; set; }
    public Dictionary<string, object> RequestDetails { get; set; }
}

public class ServiceRequestFulfillmentRequest
{
    public string FulfillmentNotes { get; set; }
    public Dictionary<string, object> FulfillmentDetails { get; set; }
}

public class ServiceRequestPerformanceDTO
{
    public int TotalRequests { get; set; }
    public int CompletedRequests { get; set; }
    public double AverageCompletionTime { get; set; }
    public int PendingRequests { get; set; }
}

public class FeedbackDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Category { get; set; }
    public int Rating { get; set; }
    public string Comments { get; set; }
    public DateTime ProvidedAt { get; set; }
    public string AnalysisStatus { get; set; }
}

public class CreateFeedbackRequest
{
    public string Category { get; set; }
    public int Rating { get; set; }
    public string Comments { get; set; }
}

public class FeedbackAnalysisDTO
{
    public double AverageRating { get; set; }
    public int TotalResponses { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; }
    public List<string> KeyThemes { get; set; }
}

public class FeedbackTrendDTO
{
    public DateTime PeriodDate { get; set; }
    public double AverageRating { get; set; }
    public int ResponseCount { get; set; }
    public string Trend { get; set; }
}

public class CommunicationRecordDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Channel { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public DateTime CommunicatedAt { get; set; }
    public string CommunicatedBy { get; set; }
}

public class LogCommunicationRequest
{
    public string Channel { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public string Notes { get; set; }
}

public class SendNotificationRequest
{
    public string NotificationType { get; set; }
    public string Channel { get; set; }
    public string Message { get; set; }
    public DateTime ScheduledFor { get; set; }
}

public class CommunicationPreferenceDTO
{
    public Guid CustomerId { get; set; }
    public List<string> PreferredChannels { get; set; }
    public string BestTimeToContact { get; set; }
    public bool OptInToMarketing { get; set; }
}

public class UpdateCommunicationPreferenceRequest
{
    public List<string> PreferredChannels { get; set; }
    public string BestTimeToContact { get; set; }
    public bool OptInToMarketing { get; set; }
}

public class CustomerSegmentDTO
{
    public Guid CustomerId { get; set; }
    public string SegmentCode { get; set; }
    public string SegmentName { get; set; }
    public string CharacteristicsProfile { get; set; }
}

public class CustomerDTO
{
    public Guid Id { get; set; }
    public string CustomerCode { get; set; }
    public string FullName { get; set; }
}

public class CustomerLifetimeValueDTO
{
    public Guid CustomerId { get; set; }
    public decimal TotalValue { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public DateTime CalculatedAt { get; set; }
}

public class CustomerOfferDTO
{
    public string OfferId { get; set; }
    public string OfferName { get; set; }
    public string Description { get; set; }
    public double RelevanceScore { get; set; }
}

public class CustomerRiskAssessmentDTO
{
    public Guid CustomerId { get; set; }
    public string RiskLevel { get; set; }
    public List<string> RiskFactors { get; set; }
    public double RiskScore { get; set; }
}

public class CustomerServiceDashboardDTO
{
    public int OpenComplaints { get; set; }
    public int PendingServiceRequests { get; set; }
    public double AverageResolutionTime { get; set; }
    public double CustomerSatisfactionScore { get; set; }
    public int NewCustomers { get; set; }
    public decimal CustomerLifetimeValue { get; set; }
    public List<CustomerServiceAlertDTO> Alerts { get; set; }
}

public class CustomerServiceMetricsDTO
{
    public int TotalComplaints { get; set; }
    public int ResolvedComplaints { get; set; }
    public int TotalServiceRequests { get; set; }
    public int ApprovedRequests { get; set; }
    public double CustomerSatisfaction { get; set; }
}

public class CustomerServiceAlertDTO
{
    public Guid AlertId { get; set; }
    public string AlertType { get; set; }
    public string Message { get; set; }
    public string Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerSatisfactionDTO
{
    public DateTime Period { get; set; }
    public double SatisfactionScore { get; set; }
    public int RespondentCount { get; set; }
    public Dictionary<string, double> CategoryScores { get; set; }
}
