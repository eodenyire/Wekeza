using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Events;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Dashboard Aggregate - Manages interactive dashboards and business intelligence
/// Supports executive, operational, risk, and custom dashboards
/// Industry Standard: Finacle MIS & T24 Business Intelligence
/// </summary>
public class Dashboard : AggregateRoot
{
    // Core Properties
    public string DashboardCode { get; private set; }
    public string DashboardName { get; private set; }
    public string Description { get; private set; }
    public DashboardType Type { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // Layout & Configuration
    public List<DashboardWidget> Widgets { get; private set; }
    public DashboardLayout Layout { get; private set; }
    public Dictionary<string, object> Configuration { get; private set; }
    public string Theme { get; private set; }
    
    // Access & Security
    public bool IsPublic { get; private set; }
    public List<string> AllowedRoles { get; private set; }
    public List<string> AllowedUsers { get; private set; }
    public DashboardVisibility Visibility { get; private set; }
    
    // Refresh & Caching
    public DateTime LastRefreshed { get; private set; }
    public int RefreshIntervalMinutes { get; private set; }
    public bool AutoRefresh { get; private set; }
    public DateTime? NextRefreshAt { get; private set; }
    
    // Status & Metadata
    public DashboardStatus Status { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }
    public int ViewCount { get; private set; }
    public DateTime? LastViewedAt { get; private set; }
    public string LastViewedBy { get; private set; }

    // Private constructor for EF Core
    private Dashboard() : base(Guid.NewGuid()) {
        Widgets = new List<DashboardWidget>();
        Configuration = new Dictionary<string, object>();
        AllowedRoles = new List<string>();
        AllowedUsers = new List<string>();
        Metadata = new Dictionary<string, object>();
    }

    // Factory method for creating new dashboard
    public static Dashboard Create(
        string dashboardCode,
        string dashboardName,
        DashboardType type,
        string createdBy,
        string description = null,
        bool isPublic = false,
        int refreshIntervalMinutes = 15)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(dashboardCode))
            throw new ArgumentException("Dashboard code cannot be empty", nameof(dashboardCode));
        
        if (string.IsNullOrWhiteSpace(dashboardName))
            throw new ArgumentException("Dashboard name cannot be empty", nameof(dashboardName));
        
        if (string.IsNullOrWhiteSpace(createdBy))
            throw new ArgumentException("Created by cannot be empty", nameof(createdBy));
        
        if (refreshIntervalMinutes < 1)
            throw new ArgumentException("Refresh interval must be at least 1 minute", nameof(refreshIntervalMinutes));

        var dashboard = new Dashboard
        {
            Id = Guid.NewGuid(),
            DashboardCode = dashboardCode,
            DashboardName = dashboardName,
            Description = description,
            Type = type,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            Widgets = new List<DashboardWidget>(),
            Layout = DashboardLayout.Grid,
            Configuration = new Dictionary<string, object>(),
            Theme = "default",
            IsPublic = isPublic,
            AllowedRoles = new List<string>(),
            AllowedUsers = new List<string>(),
            Visibility = isPublic ? DashboardVisibility.Public : DashboardVisibility.Private,
            LastRefreshed = DateTime.UtcNow,
            RefreshIntervalMinutes = refreshIntervalMinutes,
            AutoRefresh = true,
            NextRefreshAt = DateTime.UtcNow.AddMinutes(refreshIntervalMinutes),
            Status = DashboardStatus.Active,
            Metadata = new Dictionary<string, object>(),
            ViewCount = 0
        };

        // Add creation event
        dashboard.AddDomainEvent(new DashboardCreatedDomainEvent(
            dashboard.Id,
            dashboard.DashboardCode,
            dashboard.DashboardName,
            dashboard.Type,
            dashboard.CreatedBy));

        return dashboard;
    }

    // Add widget to dashboard
    public void AddWidget(DashboardWidget widget)
    {
        if (widget == null)
            throw new ArgumentNullException(nameof(widget));

        if (Status != DashboardStatus.Active)
            throw new InvalidOperationException($"Cannot add widget to dashboard in status: {Status}");

        // Check for duplicate widget IDs
        if (Widgets.Any(w => w.Id == widget.Id))
            throw new InvalidOperationException($"Widget with ID {widget.Id} already exists");

        Widgets.Add(widget);
        
        // Update metadata
        Metadata["LastModifiedAt"] = DateTime.UtcNow;
        Metadata["WidgetCount"] = Widgets.Count;

        AddDomainEvent(new DashboardWidgetAddedDomainEvent(
            Id,
            DashboardCode,
            widget.Id,
            widget.Type,
            widget.Title));
    }

    // Remove widget from dashboard
    public void RemoveWidget(Guid widgetId)
    {
        if (Status != DashboardStatus.Active)
            throw new InvalidOperationException($"Cannot remove widget from dashboard in status: {Status}");

        var widget = Widgets.FirstOrDefault(w => w.Id == widgetId);
        if (widget == null)
            throw new InvalidOperationException($"Widget with ID {widgetId} not found");

        Widgets.Remove(widget);
        
        // Update metadata
        Metadata["LastModifiedAt"] = DateTime.UtcNow;
        Metadata["WidgetCount"] = Widgets.Count;

        AddDomainEvent(new DashboardWidgetRemovedDomainEvent(
            Id,
            DashboardCode,
            widgetId,
            widget.Title));
    }

    // Update widget configuration
    public void UpdateWidget(Guid widgetId, Dictionary<string, object> configuration)
    {
        if (Status != DashboardStatus.Active)
            throw new InvalidOperationException($"Cannot update widget in dashboard with status: {Status}");

        var widget = Widgets.FirstOrDefault(w => w.Id == widgetId);
        if (widget == null)
            throw new InvalidOperationException($"Widget with ID {widgetId} not found");

        widget.UpdateConfiguration(configuration);
        
        // Update metadata
        Metadata["LastModifiedAt"] = DateTime.UtcNow;

        AddDomainEvent(new DashboardWidgetUpdatedDomainEvent(
            Id,
            DashboardCode,
            widgetId,
            widget.Title));
    }

    // Update dashboard layout
    public void UpdateLayout(DashboardLayout layout)
    {
        if (Status != DashboardStatus.Active)
            throw new InvalidOperationException($"Cannot update layout for dashboard in status: {Status}");

        Layout = layout;
        
        // Update metadata
        Metadata["LastModifiedAt"] = DateTime.UtcNow;
        Metadata["LayoutChangedAt"] = DateTime.UtcNow;

        AddDomainEvent(new DashboardLayoutUpdatedDomainEvent(
            Id,
            DashboardCode,
            layout));
    }

    // Update dashboard configuration
    public void UpdateConfiguration(Dictionary<string, object> configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        if (Status != DashboardStatus.Active)
            throw new InvalidOperationException($"Cannot update configuration for dashboard in status: {Status}");

        Configuration = new Dictionary<string, object>(configuration);
        
        // Update metadata
        Metadata["LastModifiedAt"] = DateTime.UtcNow;
        Metadata["ConfigurationChangedAt"] = DateTime.UtcNow;

        AddDomainEvent(new DashboardConfigurationUpdatedDomainEvent(
            Id,
            DashboardCode));
    }

    // Refresh dashboard data
    public void RefreshData(string refreshedBy = null)
    {
        if (Status != DashboardStatus.Active)
            throw new InvalidOperationException($"Cannot refresh dashboard in status: {Status}");

        LastRefreshed = DateTime.UtcNow;
        NextRefreshAt = AutoRefresh ? DateTime.UtcNow.AddMinutes(RefreshIntervalMinutes) : null;
        
        // Update metadata
        Metadata["LastRefreshedAt"] = LastRefreshed;
        Metadata["RefreshedBy"] = refreshedBy;
        Metadata["RefreshCount"] = Metadata.ContainsKey("RefreshCount") 
            ? (int)Metadata["RefreshCount"] + 1 
            : 1;

        AddDomainEvent(new DashboardRefreshedDomainEvent(
            Id,
            DashboardCode,
            refreshedBy,
            LastRefreshed));
    }

    // Share dashboard with user
    public void ShareWithUser(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (!AllowedUsers.Contains(userId))
        {
            AllowedUsers.Add(userId);
            
            // Update metadata
            Metadata["LastModifiedAt"] = DateTime.UtcNow;
            Metadata["SharedUserCount"] = AllowedUsers.Count;

            AddDomainEvent(new DashboardSharedWithUserDomainEvent(
                Id,
                DashboardCode,
                userId));
        }
    }

    // Share dashboard with role
    public void ShareWithRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        if (!AllowedRoles.Contains(role))
        {
            AllowedRoles.Add(role);
            
            // Update metadata
            Metadata["LastModifiedAt"] = DateTime.UtcNow;
            Metadata["SharedRoleCount"] = AllowedRoles.Count;

            AddDomainEvent(new DashboardSharedWithRoleDomainEvent(
                Id,
                DashboardCode,
                role));
        }
    }

    // Remove user access
    public void RemoveUserAccess(string userId)
    {
        if (AllowedUsers.Contains(userId))
        {
            AllowedUsers.Remove(userId);
            
            // Update metadata
            Metadata["LastModifiedAt"] = DateTime.UtcNow;
            Metadata["SharedUserCount"] = AllowedUsers.Count;

            AddDomainEvent(new DashboardUserAccessRemovedDomainEvent(
                Id,
                DashboardCode,
                userId));
        }
    }

    // Remove role access
    public void RemoveRoleAccess(string role)
    {
        if (AllowedRoles.Contains(role))
        {
            AllowedRoles.Remove(role);
            
            // Update metadata
            Metadata["LastModifiedAt"] = DateTime.UtcNow;
            Metadata["SharedRoleCount"] = AllowedRoles.Count;

            AddDomainEvent(new DashboardRoleAccessRemovedDomainEvent(
                Id,
                DashboardCode,
                role));
        }
    }

    // Record dashboard view
    public void RecordView(string viewedBy)
    {
        ViewCount++;
        LastViewedAt = DateTime.UtcNow;
        LastViewedBy = viewedBy;
        
        // Update metadata
        Metadata["TotalViews"] = ViewCount;
        Metadata["LastViewedAt"] = LastViewedAt;
        Metadata["LastViewedBy"] = viewedBy;

        AddDomainEvent(new DashboardViewedDomainEvent(
            Id,
            DashboardCode,
            viewedBy,
            LastViewedAt.Value));
    }

    // Set auto refresh
    public void SetAutoRefresh(bool autoRefresh, int intervalMinutes = 15)
    {
        if (intervalMinutes < 1)
            throw new ArgumentException("Refresh interval must be at least 1 minute", nameof(intervalMinutes));

        AutoRefresh = autoRefresh;
        RefreshIntervalMinutes = intervalMinutes;
        NextRefreshAt = autoRefresh ? DateTime.UtcNow.AddMinutes(intervalMinutes) : null;
        
        // Update metadata
        Metadata["LastModifiedAt"] = DateTime.UtcNow;
        Metadata["AutoRefreshChangedAt"] = DateTime.UtcNow;

        AddDomainEvent(new DashboardAutoRefreshUpdatedDomainEvent(
            Id,
            DashboardCode,
            autoRefresh,
            intervalMinutes));
    }

    // Set dashboard theme
    public void SetTheme(string theme)
    {
        if (string.IsNullOrWhiteSpace(theme))
            throw new ArgumentException("Theme cannot be empty", nameof(theme));

        Theme = theme;
        
        // Update metadata
        Metadata["LastModifiedAt"] = DateTime.UtcNow;
        Metadata["ThemeChangedAt"] = DateTime.UtcNow;

        AddDomainEvent(new DashboardThemeUpdatedDomainEvent(
            Id,
            DashboardCode,
            theme));
    }

    // Activate dashboard
    public void Activate()
    {
        if (Status == DashboardStatus.Active)
            return;

        Status = DashboardStatus.Active;
        
        // Update metadata
        Metadata["ActivatedAt"] = DateTime.UtcNow;

        AddDomainEvent(new DashboardActivatedDomainEvent(
            Id,
            DashboardCode));
    }

    // Deactivate dashboard
    public void Deactivate()
    {
        if (Status == DashboardStatus.Inactive)
            return;

        Status = DashboardStatus.Inactive;
        
        // Update metadata
        Metadata["DeactivatedAt"] = DateTime.UtcNow;

        AddDomainEvent(new DashboardDeactivatedDomainEvent(
            Id,
            DashboardCode));
    }

    // Check if user has access
    public bool HasUserAccess(string userId, List<string> userRoles = null)
    {
        if (IsPublic)
            return true;

        if (CreatedBy == userId)
            return true;

        if (AllowedUsers.Contains(userId))
            return true;

        if (userRoles != null && AllowedRoles.Any(role => userRoles.Contains(role)))
            return true;

        return false;
    }

    // Check if dashboard needs refresh
    public bool NeedsRefresh(DateTime currentTime)
    {
        if (!AutoRefresh || NextRefreshAt == null)
            return false;

        return currentTime >= NextRefreshAt.Value;
    }

    // Get widget by ID
    public DashboardWidget GetWidget(Guid widgetId)
    {
        return Widgets.FirstOrDefault(w => w.Id == widgetId);
    }

    // Get widgets by type
    public List<DashboardWidget> GetWidgetsByType(WidgetType type)
    {
        return Widgets.Where(w => w.Type == type).ToList();
    }

    // Add configuration value
    public void AddConfiguration(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Configuration key cannot be empty", nameof(key));

        Configuration[key] = value;
    }

    // Get configuration value
    public T GetConfiguration<T>(string key, T defaultValue = default)
    {
        if (Configuration.ContainsKey(key) && Configuration[key] is T value)
            return value;
        
        return defaultValue;
    }
}

/// <summary>
/// Dashboard Widget - Represents individual components within a dashboard
/// </summary>
public class DashboardWidget
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public WidgetType Type { get; private set; }
    public Dictionary<string, object> Configuration { get; private set; }
    public WidgetSize Size { get; private set; }
    public WidgetPosition Position { get; private set; }
    public bool IsVisible { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastUpdatedAt { get; private set; }

    private DashboardWidget() {
        Id = Guid.NewGuid();
        Configuration = new Dictionary<string, object>();
    }

    public static DashboardWidget Create(
        string title,
        WidgetType type,
        WidgetSize size = WidgetSize.Medium,
        string description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Widget title cannot be empty", nameof(title));

        return new DashboardWidget
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Type = type,
            Configuration = new Dictionary<string, object>(),
            Size = size,
            Position = new WidgetPosition { Row = 0, Column = 0 },
            IsVisible = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateConfiguration(Dictionary<string, object> configuration)
    {
        Configuration = new Dictionary<string, object>(configuration ?? new Dictionary<string, object>());
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePosition(WidgetPosition position)
    {
        Position = position ?? throw new ArgumentNullException(nameof(position));
        LastUpdatedAt = DateTime.UtcNow;
    }

    public void SetVisibility(bool isVisible)
    {
        IsVisible = isVisible;
        LastUpdatedAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Widget Position - Represents the position of a widget in the dashboard grid
/// </summary>
public class WidgetPosition
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int RowSpan { get; set; } = 1;
    public int ColumnSpan { get; set; } = 1;
}

