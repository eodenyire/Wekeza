using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Product Template Aggregate - Template for rapid product creation
/// Enables Product Factory with pre-configured templates
/// </summary>
public class ProductTemplate : AggregateRoot
{
    public string TemplateName { get; private set; }
    public string TemplateCode { get; private set; }
    public string ProductType { get; private set; }
    public string Description { get; private set; }
    public string Status { get; private set; } // Draft, Active, Archived
    public string Category { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    
    // Template Configuration (stored as JSON)
    public string TemplateConfig { get; private set; }
    public int Version { get; private set; }
    
    // Product Features/Attributes
    public Dictionary<string, string> Attributes { get; private set; }

    private ProductTemplate() : base(Guid.NewGuid()) 
    { 
        Attributes = new Dictionary<string, string>();
    }

    public static ProductTemplate Create(
        string templateName,
        string templateCode,
        string productType,
        string category,
        string createdBy)
    {
        var template = new ProductTemplate
        {
            Id = Guid.NewGuid(),
            TemplateName = templateName,
            TemplateCode = templateCode,
            ProductType = productType,
            Category = category,
            Status = "Draft",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            Version = 1,
            TemplateConfig = "{}",
            Attributes = new Dictionary<string, string>()
        };

        return template;
    }

    public void UpdateTemplate(string templateConfig, string updatedBy)
    {
        TemplateConfig = templateConfig;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        Version++;
    }

    public void Publish(string publishedBy)
    {
        Status = "Active";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = publishedBy;
    }

    public void Archive(string reason, string archivedBy)
    {
        Status = "Archived";
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = archivedBy;
    }
}
