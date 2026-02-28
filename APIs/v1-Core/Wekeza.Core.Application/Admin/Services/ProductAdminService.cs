using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Wekeza.Core.Application.Admin.Services;

/// <summary>
/// Production implementation for Product Admin Service
/// Manages product templates, fee structures, interest rates, posting rules, and product deployment
/// </summary>
public class ProductAdminService : IProductAdminService
{
    private readonly ProductAdminRepository _repository;
    private readonly ILogger<ProductAdminService> _logger;

    public ProductAdminService(ProductAdminRepository repository, ILogger<ProductAdminService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ==================== PRODUCT TEMPLATES ====================

    public async Task<ProductTemplateDTO> GetProductTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _repository.GetProductTemplateByIdAsync(templateId, cancellationToken);
            if (template == null)
            {
                _logger.LogWarning($"Product template not found: {templateId}");
                return null;
            }
            return MapToProductTemplateDTO(template);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving product template {templateId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<ProductTemplateDTO>> SearchProductTemplatesAsync(string productType, string status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var templates = await _repository.SearchProductTemplatesAsync(productType, status, page, pageSize, cancellationToken);
            return templates.Select(MapToProductTemplateDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching product templates: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<ProductTemplateDTO> CreateProductTemplateAsync(CreateProductTemplateDTO createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = new ProductTemplate
            {
                Id = Guid.NewGuid(),
                ProductCode = createDto.ProductCode,
                ProductType = createDto.ProductType,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            var created = await _repository.AddProductTemplateAsync(template, cancellationToken);
            _logger.LogInformation($"Product template created: {created.ProductCode}");
            return MapToProductTemplateDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating product template: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<ProductTemplateDTO> UpdateProductTemplateAsync(Guid templateId, UpdateProductTemplateDTO updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _repository.GetProductTemplateByIdAsync(templateId, cancellationToken);
            if (template == null) throw new InvalidOperationException("Product template not found");

            template.ProductType = updateDto.ProductType ?? template.ProductType;
            template.ModifiedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateProductTemplateAsync(template, cancellationToken);
            _logger.LogInformation($"Product template updated: {updated.ProductCode}");
            return MapToProductTemplateDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating product template {templateId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<ProductTemplateDTO> PublishProductTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _repository.GetProductTemplateByIdAsync(templateId, cancellationToken);
            if (template == null) throw new InvalidOperationException("Product template not found");

            template.Status = "Published";
            template.ModifiedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateProductTemplateAsync(template, cancellationToken);
            _logger.LogInformation($"Product template published: {updated.ProductCode}");
            return MapToProductTemplateDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error publishing product template {templateId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<ProductTemplateDTO> ArchiveProductTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            var template = await _repository.GetProductTemplateByIdAsync(templateId, cancellationToken);
            if (template == null) throw new InvalidOperationException("Product template not found");

            template.Status = "Archived";
            template.ModifiedAt = DateTime.UtcNow;

            var updated = await _repository.UpdateProductTemplateAsync(template, cancellationToken);
            _logger.LogInformation($"Product template archived: {updated.ProductCode}");
            return MapToProductTemplateDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error archiving product template {templateId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<ProductTemplateVersionDTO>> GetProductVersionHistoryAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation($"Retrieving version history for template {templateId}");
            // Placeholder: In production, this would query version history from repository
            return new List<ProductTemplateVersionDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving version history: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== FEE STRUCTURES ====================

    public async Task<FeeStructureDTO> GetFeeStructureAsync(Guid feeStructureId, CancellationToken cancellationToken = default)
    {
        try
        {
            var feeStructure = await _repository.GetFeeStructureByIdAsync(feeStructureId, cancellationToken);
            if (feeStructure == null)
            {
                _logger.LogWarning($"Fee structure not found: {feeStructureId}");
                return null;
            }
            return MapToFeeStructureDTO(feeStructure);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving fee structure {feeStructureId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<FeeStructureDTO>> SearchFeeStructuresAsync(string productCode, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var feeStructures = await _repository.SearchFeeStructuresAsync(productCode, page, pageSize, cancellationToken);
            return feeStructures.Select(MapToFeeStructureDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching fee structures: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<FeeStructureDTO> CreateFeeStructureAsync(CreateFeeStructureDTO createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var feeStructure = new FeeStructure
            {
                Id = Guid.NewGuid(),
                ProductCode = createDto.ProductCode,
                FeeName = createDto.FeeName,
                FeeAmount = createDto.FeeAmount,
                FeePercentage = createDto.FeePercentage,
                Status = "Active"
            };

            var created = await _repository.AddFeeStructureAsync(feeStructure, cancellationToken);
            _logger.LogInformation($"Fee structure created: {created.FeeName}");
            return MapToFeeStructureDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating fee structure: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<FeeStructureDTO> UpdateFeeStructureAsync(Guid feeStructureId, UpdateFeeStructureDTO updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var feeStructure = await _repository.GetFeeStructureByIdAsync(feeStructureId, cancellationToken);
            if (feeStructure == null) throw new InvalidOperationException("Fee structure not found");

            feeStructure.FeeName = updateDto.FeeName ?? feeStructure.FeeName;
            feeStructure.FeeAmount = updateDto.FeeAmount ?? feeStructure.FeeAmount;
            feeStructure.FeePercentage = updateDto.FeePercentage ?? feeStructure.FeePercentage;

            var updated = await _repository.UpdateFeeStructureAsync(feeStructure, cancellationToken);
            _logger.LogInformation($"Fee structure updated: {updated.FeeName}");
            return MapToFeeStructureDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating fee structure {feeStructureId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<FeeApprovalDTO> ApproveFeesAsync(Guid feeStructureId, string approverComments, CancellationToken cancellationToken = default)
    {
        try
        {
            var feeStructure = await _repository.GetFeeStructureByIdAsync(feeStructureId, cancellationToken);
            if (feeStructure == null) throw new InvalidOperationException("Fee structure not found");

            feeStructure.Status = "Approved";
            await _repository.UpdateFeeStructureAsync(feeStructure, cancellationToken);

            _logger.LogInformation($"Fee structure approved: {feeStructure.FeeName}");
            return new FeeApprovalDTO { FeeStructureId = feeStructureId, ApprovalStatus = "Approved", ApprovedAt = DateTime.UtcNow };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error approving fees {feeStructureId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<FeeCalculationPreviewDTO> PreviewFeeCalculationAsync(Guid feeStructureId, decimal transactionAmount, CancellationToken cancellationToken = default)
    {
        try
        {
            var feeStructure = await _repository.GetFeeStructureByIdAsync(feeStructureId, cancellationToken);
            if (feeStructure == null) throw new InvalidOperationException("Fee structure not found");

            var calculatedFee = CalculateFeeAmount(feeStructure, transactionAmount);
            return new FeeCalculationPreviewDTO
            {
                FeeStructureId = feeStructureId,
                TransactionAmount = transactionAmount,
                CalculatedFee = calculatedFee,
                TotalAmount = transactionAmount + calculatedFee,
                CalculatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error previewing fee calculation: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== INTEREST RATES ====================

    public async Task<InterestRateTableDTO> GetInterestRateTableAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        try
        {
            var table = await _repository.GetInterestRateTableByIdAsync(tableId, cancellationToken);
            if (table == null)
            {
                _logger.LogWarning($"Interest rate table not found: {tableId}");
                return null;
            }
            return MapToInterestRateTableDTO(table);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving interest rate table {tableId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<InterestRateTableDTO>> SearchInterestRateTablesAsync(string productCode, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var tables = await _repository.SearchInterestRateTablesAsync(productCode, page, pageSize, cancellationToken);
            return tables.Select(MapToInterestRateTableDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching interest rate tables: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<InterestRateTableDTO> CreateInterestRateTableAsync(CreateInterestRateTableDTO createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var table = new InterestRateTable
            {
                Id = Guid.NewGuid(),
                ProductCode = createDto.ProductCode,
                EffectiveDate = createDto.EffectiveDate,
                BaseRate = createDto.BaseRate,
                Spread = createDto.Spread,
                Status = "Active"
            };

            var created = await _repository.AddInterestRateTableAsync(table, cancellationToken);
            _logger.LogInformation($"Interest rate table created for {created.ProductCode}");
            return MapToInterestRateTableDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating interest rate table: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<InterestRateTableDTO> UpdateInterestRateTableAsync(Guid tableId, UpdateInterestRateTableDTO updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var table = await _repository.GetInterestRateTableByIdAsync(tableId, cancellationToken);
            if (table == null) throw new InvalidOperationException("Interest rate table not found");

            table.BaseRate = updateDto.BaseRate ?? table.BaseRate;
            table.Spread = updateDto.Spread ?? table.Spread;

            var updated = await _repository.UpdateInterestRateTableAsync(table, cancellationToken);
            _logger.LogInformation($"Interest rate table updated for {updated.ProductCode}");
            return MapToInterestRateTableDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating interest rate table {tableId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<InterestRateChangeDTO> EffectuateRateChangeAsync(Guid tableId, DateTime effectiveDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var table = await _repository.GetInterestRateTableByIdAsync(tableId, cancellationToken);
            if (table == null) throw new InvalidOperationException("Interest rate table not found");

            var oldRate = table.BaseRate;
            table.EffectiveDate = effectiveDate;

            await _repository.UpdateInterestRateTableAsync(table, cancellationToken);
            _logger.LogInformation($"Interest rate change effectuated for {table.ProductCode}");

            return new InterestRateChangeDTO
            {
                TableId = tableId,
                OldRate = oldRate,
                NewRate = table.BaseRate,
                EffectiveDate = effectiveDate,
                ChangeAppliedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error effectuating rate change: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<InterestCalculationPreviewDTO> PreviewInterestCalculationAsync(Guid tableId, decimal principalAmount, int daysCount, CancellationToken cancellationToken = default)
    {
        try
        {
            var table = await _repository.GetInterestRateTableByIdAsync(tableId, cancellationToken);
            if (table == null) throw new InvalidOperationException("Interest rate table not found");

            var interestAmount = CalculateInterestAmount(table, principalAmount, daysCount);
            return new InterestCalculationPreviewDTO
            {
                TableId = tableId,
                PrincipalAmount = principalAmount,
                DayCount = daysCount,
                CalculatedInterest = interestAmount,
                MaturityAmount = principalAmount + interestAmount,
                CalculatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error previewing interest calculation: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== POSTING RULES ====================

    public async Task<PostingRuleDTO> GetPostingRuleAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetPostingRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null)
            {
                _logger.LogWarning($"Posting rule not found: {ruleId}");
                return null;
            }
            return MapToPostingRuleDTO(rule);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving posting rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<List<PostingRuleDTO>> SearchPostingRulesAsync(string productCode, string transactionType, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var rules = await _repository.SearchPostingRulesAsync(productCode, transactionType, page, pageSize, cancellationToken);
            return rules.Select(MapToPostingRuleDTO).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error searching posting rules: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<PostingRuleDTO> CreatePostingRuleAsync(CreatePostingRuleDTO createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = new PostingRule
            {
                Id = Guid.NewGuid(),
                ProductCode = createDto.ProductCode,
                TransactionType = createDto.TransactionType,
                DebitAccount = createDto.DebitAccount,
                CreditAccount = createDto.CreditAccount,
                Status = "Active"
            };

            var created = await _repository.AddPostingRuleAsync(rule, cancellationToken);
            _logger.LogInformation($"Posting rule created: {created.ProductCode}-{created.TransactionType}");
            return MapToPostingRuleDTO(created);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating posting rule: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<PostingRuleDTO> UpdatePostingRuleAsync(Guid ruleId, UpdatePostingRuleDTO updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetPostingRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null) throw new InvalidOperationException("Posting rule not found");

            rule.DebitAccount = updateDto.DebitAccount ?? rule.DebitAccount;
            rule.CreditAccount = updateDto.CreditAccount ?? rule.CreditAccount;

            var updated = await _repository.UpdatePostingRuleAsync(rule, cancellationToken);
            _logger.LogInformation($"Posting rule updated: {updated.ProductCode}-{updated.TransactionType}");
            return MapToPostingRuleDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating posting rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<PostingRuleDTO> ActivatePostingRuleAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetPostingRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null) throw new InvalidOperationException("Posting rule not found");

            rule.Status = "Active";
            var updated = await _repository.UpdatePostingRuleAsync(rule, cancellationToken);
            _logger.LogInformation($"Posting rule activated: {updated.ProductCode}");
            return MapToPostingRuleDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error activating posting rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<PostingRuleDTO> DeactivatePostingRuleAsync(Guid ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rule = await _repository.GetPostingRuleByIdAsync(ruleId, cancellationToken);
            if (rule == null) throw new InvalidOperationException("Posting rule not found");

            rule.Status = "Inactive";
            var updated = await _repository.UpdatePostingRuleAsync(rule, cancellationToken);
            _logger.LogInformation($"Posting rule deactivated: {updated.ProductCode}");
            return MapToPostingRuleDTO(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deactivating posting rule {ruleId}: {ex.Message}", ex);
            throw;
        }
    }

    // ==================== HELPER METHODS ====================

    private decimal CalculateFeeAmount(FeeStructure feeStructure, decimal transactionAmount)
    {
        if (feeStructure.FeePercentage > 0)
            return (transactionAmount * feeStructure.FeePercentage) / 100;
        return feeStructure.FeeAmount;
    }

    private decimal CalculateInterestAmount(InterestRateTable table, decimal principal, int days)
    {
        var annualRate = (table.BaseRate + table.Spread) / 100;
        return (principal * annualRate * days) / 365;
    }

    private ProductTemplateDTO MapToProductTemplateDTO(ProductTemplate template) =>
        new ProductTemplateDTO { Id = template.Id, ProductCode = template.ProductCode, ProductType = template.ProductType, Status = template.Status };

    private FeeStructureDTO MapToFeeStructureDTO(FeeStructure fee) =>
        new FeeStructureDTO { Id = fee.Id, ProductCode = fee.ProductCode, FeeName = fee.FeeName, FeeAmount = fee.FeeAmount, FeePercentage = fee.FeePercentage };

    private InterestRateTableDTO MapToInterestRateTableDTO(InterestRateTable table) =>
        new InterestRateTableDTO { Id = table.Id, ProductCode = table.ProductCode, EffectiveDate = table.EffectiveDate, BaseRate = table.BaseRate, Spread = table.Spread };

    private PostingRuleDTO MapToPostingRuleDTO(PostingRule rule) =>
        new PostingRuleDTO { Id = rule.Id, ProductCode = rule.ProductCode, TransactionType = rule.TransactionType, DebitAccount = rule.DebitAccount, CreditAccount = rule.CreditAccount };
}

// DTOs (simplified versions)
public class ProductTemplateDTO { public Guid Id { get; set; } public string ProductCode { get; set; } public string ProductType { get; set; } public string Status { get; set; } }
public class CreateProductTemplateDTO { public string ProductCode { get; set; } public string ProductType { get; set; } }
public class UpdateProductTemplateDTO { public string ProductType { get; set; } }
public class ProductTemplateVersionDTO { public Guid TemplateId { get; set; } public int VersionNumber { get; set; } public DateTime CreatedAt { get; set; } }
public class FeeStructureDTO { public Guid Id { get; set; } public string ProductCode { get; set; } public string FeeName { get; set; } public decimal FeeAmount { get; set; } public decimal FeePercentage { get; set; } }
public class CreateFeeStructureDTO { public string ProductCode { get; set; } public string FeeName { get; set; } public decimal FeeAmount { get; set; } public decimal FeePercentage { get; set; } }
public class UpdateFeeStructureDTO { public string FeeName { get; set; } public decimal? FeeAmount { get; set; } public decimal? FeePercentage { get; set; } }
public class FeeApprovalDTO { public Guid FeeStructureId { get; set; } public string ApprovalStatus { get; set; } public DateTime ApprovedAt { get; set; } }
public class FeeCalculationPreviewDTO { public Guid FeeStructureId { get; set; } public decimal TransactionAmount { get; set; } public decimal CalculatedFee { get; set; } public decimal TotalAmount { get; set; } public DateTime CalculatedAt { get; set; } }
public class InterestRateTableDTO { public Guid Id { get; set; } public string ProductCode { get; set; } public DateTime EffectiveDate { get; set; } public decimal BaseRate { get; set; } public decimal Spread { get; set; } }
public class CreateInterestRateTableDTO { public string ProductCode { get; set; } public DateTime EffectiveDate { get; set; } public decimal BaseRate { get; set; } public decimal Spread { get; set; } }
public class UpdateInterestRateTableDTO { public decimal? BaseRate { get; set; } public decimal? Spread { get; set; } }
public class InterestRateChangeDTO { public Guid TableId { get; set; } public decimal OldRate { get; set; } public decimal NewRate { get; set; } public DateTime EffectiveDate { get; set; } public DateTime ChangeAppliedAt { get; set; } }
public class InterestCalculationPreviewDTO { public Guid TableId { get; set; } public decimal PrincipalAmount { get; set; } public int DayCount { get; set; } public decimal CalculatedInterest { get; set; } public decimal MaturityAmount { get; set; } public DateTime CalculatedAt { get; set; } }
public class PostingRuleDTO { public Guid Id { get; set; } public string ProductCode { get; set; } public string TransactionType { get; set; } public string DebitAccount { get; set; } public string CreditAccount { get; set; } }
public class CreatePostingRuleDTO { public string ProductCode { get; set; } public string TransactionType { get; set; } public string DebitAccount { get; set; } public string CreditAccount { get; set; } }
public class UpdatePostingRuleDTO { public string DebitAccount { get; set; } public string CreditAccount { get; set; } }
