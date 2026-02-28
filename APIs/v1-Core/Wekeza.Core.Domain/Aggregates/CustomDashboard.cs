using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Custom Dashboard Aggregate - User-created dashboard configurations
/// Enables personalized admin/management dashboards
/// </summary>
public class CustomDashboard : AggregateRoot
{
    public string DashboardName { get; private set; }
    public string DashboardCode { get; private set; }
    public Guid UserId { get; private set; }
    public string DashboardType { get; private set; } // Admin, Executive, Manager, etc.
    public string Status { get; private set; } // Active, Inactive, Archived
    
    public string WidgetConfiguration { get; private set; } // JSON: widget list and layout
    public string Filters { get; private set; } // JSON: default filters
    public bool IsDefault { get; private set; }
    public int RefreshIntervalSeconds { get; private set; } = 60;
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    private CustomDashboard() : base(Guid.NewGuid()) 
    { 
        Metadata = new Dictionary<string, object>();
    }

    public static CustomDashboard Create(
        string dashboardName,
        string dashboardCode,
        Guid userId,
        string dashboardType,
        string createdBy)
    {
        var dashboard = new CustomDashboard
        {
            Id = Guid.NewGuid(),
            DashboardName = dashboardName,
            DashboardCode = dashboardCode,
            UserId = userId,
            DashboardType = dashboardType,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy,
            WidgetConfiguration = "[]",
            Filters = "{}",
            IsDefault = false,
            Metadata = new Dictionary<string, object>()
        };

        return dashboard;
    }

    public void UpdateWidgets(string widgetConfig, string updatedBy)
    {
        WidgetConfiguration = widgetConfig;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateFilters(string filters, string updatedBy)
    {
        Filters = filters;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void SetAsDefault(string updatedBy)
    {
        IsDefault = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
