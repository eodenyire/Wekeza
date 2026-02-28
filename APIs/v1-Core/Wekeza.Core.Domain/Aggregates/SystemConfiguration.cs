using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// System Configuration - Centralized system parameterization for banking operations
/// Supports product definitions, policies, and operational parameters
/// Industry standard: Finacle Configuration Framework, T24 Parameter Management
/// </summary>
public class SystemConfiguration : AggregateRoot
{
    public string ConfigCode { get; private set; }
    public string ConfigName { get; private set; }
    public string Category { get; private set; } // Product, Security, Operations, Integration, Compliance
    public string ConfigType { get; private set; } // Product, Parameter, Policy, Formula, Integration
    
    // Configuration Data
    public Dictionary<string, object> ConfigurationData { get; private set; }
    public string Version { get; private set; }
    public int VersionNumber { get; private set; }
    
    // Status & Lifecycle
    public ConfigurationStatus Status { get; private set; } // Draft, Approved, Active, Suspended, Archived
    public bool IsProductionReady { get; private set; }
    public DateTime? ActivationDate { get; private set; }
    public DateTime? SuspensionDate { get; private set; }
    
    // Approval Workflow
    public List<ConfigurationApproval> ApprovalHistory { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    
    // Impact & Dependencies
    public List<string> AffectedModules { get; private set; }
    public List<string> Dependencies { get; private set; }
    public List<string> ReferencedConfigs { get; private set; }
    public bool RequiresRestart { get; private set; }
    
    // Testing & Validation
    public string? TestResult { get; private set; }
    public DateTime? LastTestedAt { get; private set; }
    public List<string> ValidationRules { get; private set; }
    public Dictionary<string, object> TestData { get; private set; }
    
    // Audit
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public string? ChangeReason { get; private set; }
    public List<ConfigurationChangeLog> ChangeHistory { get; private set; }

    private SystemConfiguration() : base(Guid.NewGuid())
    {
        ConfigurationData = new Dictionary<string, object>();
        ApprovalHistory = new List<ConfigurationApproval>();
        AffectedModules = new List<string>();
        Dependencies = new List<string>();
        ReferencedConfigs = new List<string>();
        ValidationRules = new List<string>();
        TestData = new Dictionary<string, object>();
        ChangeHistory = new List<ConfigurationChangeLog>();
    }

    public SystemConfiguration(
        string configCode,
        string configName,
        string category,
        string configType,
        string createdBy,
        Dictionary<string, object> configData) : this()
    {
        if (string.IsNullOrWhiteSpace(configCode))
            throw new ArgumentException("Config code cannot be empty", nameof(configCode));
        if (string.IsNullOrWhiteSpace(configName))
            throw new ArgumentException("Config name cannot be empty", nameof(configName));

        Id = Guid.NewGuid();
        ConfigCode = configCode;
        ConfigName = configName;
        Category = category;
        ConfigType = configType;
        CreatedBy = createdBy;
        ConfigurationData = configData ?? new Dictionary<string, object>();
        Version = "1.0";
        VersionNumber = 1;
        Status = ConfigurationStatus.Draft;
        IsProductionReady = false;
    }

    public void UpdateConfiguration(Dictionary<string, object> newData, string modifiedBy, string changeReason)
    {
        if (Status == ConfigurationStatus.Active)
            throw new InvalidOperationException("Cannot modify active configuration. Create a new version.");

        var changeLog = new ConfigurationChangeLog
        {
            Version = Version,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = modifiedBy,
            ChangeReason = changeReason,
            PreviousData = new Dictionary<string, object>(ConfigurationData),
            NewData = newData
        };

        ChangeHistory.Add(changeLog);
        ConfigurationData = newData;
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }

    public void SubmitForApproval(string submittedBy, List<string> approvers)
    {
        if (Status != ConfigurationStatus.Draft)
            throw new InvalidOperationException("Only draft configurations can be submitted for approval");

        Status = ConfigurationStatus.PendingApproval;
        ApprovalHistory.Add(new ConfigurationApproval
        {
            RequestedAt = DateTime.UtcNow,
            RequestedBy = submittedBy,
            Approvers = approvers,
            Status = ApprovalStatus.Pending
        });
    }

    public void Approve(Guid approvedByUserId, string approverName, string comments = "")
    {
        if (Status != ConfigurationStatus.PendingApproval)
            throw new InvalidOperationException("Configuration is not pending approval");

        ApprovedBy = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        Status = ConfigurationStatus.Approved;

        var approval = ApprovalHistory.OrderByDescending(a => a.RequestedAt).FirstOrDefault();
        if (approval != null)
        {
            approval.Status = ApprovalStatus.Approved;
            approval.ApprovedAt = DateTime.UtcNow;
            approval.ApprovedBy = approverName;
            approval.Comments = comments;
        }
    }

    public void Reject(string rejectionReason)
    {
        Status = ConfigurationStatus.Draft;
        var approval = ApprovalHistory.OrderByDescending(a => a.RequestedAt).FirstOrDefault();
        if (approval != null)
        {
            approval.Status = ApprovalStatus.Rejected;
            approval.Comments = rejectionReason;
        }
    }

    public void Activate(DateTime? activationDate = null)
    {
        if (Status != ConfigurationStatus.Approved)
            throw new InvalidOperationException("Only approved configurations can be activated");

        Status = ConfigurationStatus.Active;
        IsProductionReady = true;
        ActivationDate = activationDate ?? DateTime.UtcNow;
    }

    public void Suspend(string reason)
    {
        if (Status != ConfigurationStatus.Active)
            throw new InvalidOperationException("Can only suspend active configurations");

        Status = ConfigurationStatus.Suspended;
        SuspensionDate = DateTime.UtcNow;
        ChangeHistory.Add(new ConfigurationChangeLog
        {
            Version = Version,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = "SYSTEM",
            ChangeReason = $"Configuration suspended: {reason}"
        });
    }

    public void TestConfiguration(string testResult, bool passed)
    {
        TestResult = testResult;
        LastTestedAt = DateTime.UtcNow;
        if (!passed)
            IsProductionReady = false;
    }

    public void CreateNewVersion(string modifiedBy)
    {
        VersionNumber++;
        Version = $"{VersionNumber / 10}.{VersionNumber % 10}";
        Status = ConfigurationStatus.Draft;
        LastModifiedBy = modifiedBy;
        LastModifiedAt = DateTime.UtcNow;
    }
}

public class ConfigurationApproval
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; }
    public List<string> Approvers { get; set; }
    public ApprovalStatus Status { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Comments { get; set; }
}

public class ConfigurationChangeLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Version { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; }
    public string ChangeReason { get; set; }
    public Dictionary<string, object> PreviousData { get; set; }
    public Dictionary<string, object> NewData { get; set; }
}

public enum ConfigurationStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Active = 4,
    Suspended = 5,
    Archived = 6,
    Deprecated = 7
}

public enum ApprovalStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Escalated = 4
}
