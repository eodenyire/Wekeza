using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin;

/// <summary>
/// Product Admin Service - Product Definitions, Fee Structures, Rate Management, Posting Rules
/// Product management portal for defining and configuring banking products and their operational parameters
/// </summary>
public interface IProductAdminService
{
    // ===== Product Template Management =====
    Task<ProductTemplateDTO> GetProductTemplateAsync(Guid templateId);
    Task<List<ProductTemplateDTO>> SearchProductTemplatesAsync(string? productType = null, string? status = null, int page = 1, int pageSize = 50);
    Task<ProductTemplateDTO> CreateProductTemplateAsync(CreateProductTemplateRequest request, Guid createdByUserId);
    Task<ProductTemplateDTO> UpdateProductTemplateAsync(Guid templateId, UpdateProductTemplateRequest request, Guid updatedByUserId);
    Task PublishProductTemplateAsync(Guid templateId, Guid publishedByUserId);
    Task ArchiveProductTemplateAsync(Guid templateId, string reason, Guid archivedByUserId);
    Task<List<ProductVersionDTO>> GetProductVersionHistoryAsync(Guid templateId);

    // ===== Fee Structure Management =====
    Task<FeeStructureDTO> GetFeeStructureAsync(Guid feeStructureId);
    Task<List<FeeStructureDTO>> SearchFeeStructuresAsync(string? productCode = null, int page = 1, int pageSize = 50);
    Task<FeeStructureDTO> CreateFeeStructureAsync(CreateFeeStructureRequest request, Guid createdByUserId);
    Task<FeeStructureDTO> UpdateFeeStructureAsync(Guid feeStructureId, UpdateFeeStructureRequest request, Guid updatedByUserId);
    Task ApproveFeesAsync(Guid feeStructureId, Guid approverUserId);
    Task<FeeCalculationPreviewDTO> PreviewFeeCalculationAsync(Guid feeStructureId, FeeCalculationScenarioRequest scenario);

    // ===== Interest Rate Management =====
    Task<InterestRateTableDTO> GetInterestRateTableAsync(Guid tableId);
    Task<List<InterestRateTableDTO>> SearchInterestRateTablesAsync(string? productCode = null, int page = 1, int pageSize = 50);
    Task<InterestRateTableDTO> CreateInterestRateTableAsync(CreateInterestRateTableRequest request, Guid createdByUserId);
    Task<InterestRateTableDTO> UpdateInterestRateTableAsync(Guid tableId, UpdateInterestRateTableRequest request, Guid updatedByUserId);
    Task EffectuateRateChangeAsync(Guid tableId, DateTime effectiveDate, Guid effectuatedByUserId);
    Task<InterestCalculationPreviewDTO> PreviewInterestCalculationAsync(Guid tableId, InterestCalculationScenarioRequest scenario);

    // ===== Posting Rules & GL Mapping =====
    Task<PostingRuleDTO> GetPostingRuleAsync(Guid ruleId);
    Task<List<PostingRuleDTO>> SearchPostingRulesAsync(string? productCode = null, string? transactionType = null, int page = 1, int pageSize = 50);
    Task<PostingRuleDTO> CreatePostingRuleAsync(CreatePostingRuleRequest request, Guid createdByUserId);
    Task<PostingRuleDTO> UpdatePostingRuleAsync(Guid ruleId, UpdatePostingRuleRequest request, Guid updatedByUserId);
    Task ActivatePostingRuleAsync(Guid ruleId, Guid activatedByUserId);
    Task DeactivatePostingRuleAsync(Guid ruleId, string reason, Guid deactivatedByUserId);

    // ===== Product Simulation & Testing =====
    Task<ProductSimulationResultDTO> SimulateProductAsync(ProductSimulationRequest request, Guid simulatedByUserId);
    Task<AccountProjectionDTO> ProjectAccountBehaviorAsync(Guid productTemplateId, AccountProjectionRequest request);
    Task<RegulatoryComplianceCheckDTO> CheckRegulatoryComplianceAsync(Guid productTemplateId);
    Task<PricingCompetitivenessDTO> ComparePricingAsync(Guid productTemplateId, List<CompetitorProductDTO> competitors);

    // ===== Product Approval Workflows =====
    Task<ProductApprovalDTO> SubmitForApprovalAsync(Guid productTemplateId, string submissionNotes, Guid submittedByUserId);
    Task ApproveProductAsync(Guid approvalId, string approvalNotes, Guid approverUserId);
    Task RejectProductAsync(Guid approvalId, string rejectionReason, Guid rejectorUserId);
    Task<List<ApprovalRequirementDTO>> GetApprovalRequirementsAsync(Guid productTemplateId);

    // ===== Product Deployment =====
    Task DeployProductAsync(Guid productTemplateId, DateTime effectiveDate, Guid deployedByUserId);
    Task<ProductDeploymentStatusDTO> GetDeploymentStatusAsync(Guid productTemplateId);
    Task RollbackProductAsync(Guid productTemplateId, string rollbackReason, Guid rolledBackByUserId);

    // ===== Product Dashboard =====
    Task<ProductPortfolioDTO> GetProductPortfolioAsync();
    Task<ProductPerformanceDTO> GetProductPerformanceAsync(string productCode, DateTime? fromDate = null, DateTime? toDate = null);
    Task<List<ProductAlertDTO>> GetProductAlertsAsync();
}

// DTOs
public class ProductTemplateDTO
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductType { get; set; }
    public string Status { get; set; }
    public decimal MinimumBalance { get; set; }
    public decimal MaximumBalance { get; set; }
    public string CurrencyCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class CreateProductTemplateRequest
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string ProductType { get; set; }
    public string Description { get; set; }
    public decimal MinimumBalance { get; set; }
    public decimal MaximumBalance { get; set; }
    public string CurrencyCode { get; set; }
    public Dictionary<string, object> ProductDefinition { get; set; }
}

public class UpdateProductTemplateRequest
{
    public string ProductName { get; set; }
    public string Description { get; set; }
    public Dictionary<string, object> ProductDefinition { get; set; }
}

public class ProductVersionDTO
{
    public int VersionNumber { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class FeeStructureDTO
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public string FeeName { get; set; }
    public string FeeType { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal FeePercentage { get; set; }
    public string CalculationMethod { get; set; }
    public string Status { get; set; }
    public List<FeeRangeDTO> FeeRanges { get; set; }
}

public class FeeRangeDTO
{
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal Fee { get; set; }
}

public class CreateFeeStructureRequest
{
    public string ProductCode { get; set; }
    public string FeeName { get; set; }
    public string FeeType { get; set; }
    public decimal FeeAmount { get; set; }
    public decimal FeePercentage { get; set; }
    public string CalculationMethod { get; set; }
    public List<FeeRangeDTO> FeeRanges { get; set; }
}

public class UpdateFeeStructureRequest
{
    public decimal FeeAmount { get; set; }
    public decimal FeePercentage { get; set; }
    public List<FeeRangeDTO> FeeRanges { get; set; }
}

public class FeeCalculationPreviewDTO
{
    public decimal TransactionAmount { get; set; }
    public decimal CalculatedFee { get; set; }
    public string CalculationFormula { get; set; }
    public decimal TotalWithFee { get; set; }
}

public class FeeCalculationScenarioRequest
{
    public decimal TransactionAmount { get; set; }
    public string TransactionType { get; set; }
}

public class InterestRateTableDTO
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public string TableName { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string Status { get; set; }
    public List<InterestSlabDTO> InterestSlabs { get; set; }
}

public class InterestSlabDTO
{
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal InterestRate { get; set; }
    public string CompoundingFrequency { get; set; }
}

public class CreateInterestRateTableRequest
{
    public string ProductCode { get; set; }
    public string TableName { get; set; }
    public DateTime EffectiveDate { get; set; }
    public List<InterestSlabDTO> InterestSlabs { get; set; }
}

public class UpdateInterestRateTableRequest
{
    public List<InterestSlabDTO> InterestSlabs { get; set; }
}

public class InterestCalculationPreviewDTO
{
    public decimal Principal { get; set; }
    public decimal InterestRate { get; set; }
    public decimal CalculatedInterest { get; set; }
    public decimal MaturityAmount { get; set; }
    public string CalculationMethod { get; set; }
}

public class InterestCalculationScenarioRequest
{
    public decimal Principal { get; set; }
    public int DurationDays { get; set; }
    public string CompoundingFrequency { get; set; }
}

public class PostingRuleDTO
{
    public Guid Id { get; set; }
    public string ProductCode { get; set; }
    public string TransactionType { get; set; }
    public List<PostingLineDTO> PostingLines { get; set; }
    public string Status { get; set; }
}

public class PostingLineDTO
{
    public string GLCode { get; set; }
    public string DebitOrCredit { get; set; }
    public string Amount { get; set; }
    public int SequenceNumber { get; set; }
}

public class CreatePostingRuleRequest
{
    public string ProductCode { get; set; }
    public string TransactionType { get; set; }
    public List<PostingLineDTO> PostingLines { get; set; }
}

public class UpdatePostingRuleRequest
{
    public List<PostingLineDTO> PostingLines { get; set; }
}

public class ProductSimulationResultDTO
{
    public Guid SimulationId { get; set; }
    public string ProductCode { get; set; }
    public List<string> ValidationResults { get; set; }
    public List<string> WarningMessages { get; set; }
    public bool IsReady { get; set; }
    public DateTime SimulatedAt { get; set; }
}

public class ProductSimulationRequest
{
    public Guid ProductTemplateId { get; set; }
    public Dictionary<string, object> TestScenarios { get; set; }
}

public class AccountProjectionDTO
{
    public string ProductCode { get; set; }
    public DateTime ProjectionDate { get; set; }
    public decimal ProjectedBalance { get; set; }
    public decimal ProjectedInterest { get; set; }
    public List<ProjectionPeriodDTO> ProjectionPeriods { get; set; }
}

public class ProjectionPeriodDTO
{
    public DateTime PeriodDate { get; set; }
    public decimal Balance { get; set; }
    public decimal Interest { get; set; }
}

public class AccountProjectionRequest
{
    public decimal OpeningBalance { get; set; }
    public int DurationMonths { get; set; }
}

public class RegulatoryComplianceCheckDTO
{
    public Guid ProductId { get; set; }
    public string ComplianceStatus { get; set; }
    public List<ComplianceIssueDTO> Issues { get; set; }
    public DateTime CheckedAt { get; set; }
}

public class PricingCompetitivenessDTO
{
    public string ProductCode { get; set; }
    public decimal YourPrice { get; set; }
    public decimal MarketAverage { get; set; }
    public decimal MarketLowest { get; set; }
    public decimal MarketHighest { get; set; }
    public string Competitiveness { get; set; }
}

public class CompetitorProductDTO
{
    public string CompetitorName { get; set; }
    public decimal Price { get; set; }
}

public class ProductApprovalDTO
{
    public Guid Id { get; set; }
    public Guid ProductTemplateId { get; set; }
    public string Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string SubmittedBy { get; set; }
}

public class ApprovalRequirementDTO
{
    public string RequirementName { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
}

public class ProductDeploymentStatusDTO
{
    public Guid ProductTemplateId { get; set; }
    public string Status { get; set; }
    public DateTime DeploymentDate { get; set; }
    public List<string> DeploymentSteps { get; set; }
    public int ProgressPercentage { get; set; }
}

public class ProductPortfolioDTO
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public List<ProductMetricDTO> ProductMetrics { get; set; }
}

public class ProductMetricDTO
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public decimal TotalAccounts { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal AverageBalance { get; set; }
    public decimal MonthlyRevenue { get; set; }
}

public class ProductPerformanceDTO
{
    public string ProductCode { get; set; }
    public DateTime Period { get; set; }
    public decimal Growth { get; set; }
    public decimal Profitability { get; set; }
    public int AccountCount { get; set; }
}

public class ProductAlertDTO
{
    public Guid AlertId { get; set; }
    public string ProductCode { get; set; }
    public string AlertType { get; set; }
    public string Message { get; set; }
    public string Severity { get; set; }
    public DateTime CreatedAt { get; set; }
}
