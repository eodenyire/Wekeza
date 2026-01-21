using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.Interfaces;
using UserRole = Wekeza.Core.Domain.Enums.UserRole;

namespace Wekeza.Core.Domain.Services;

/// <summary>
/// Service for determining approval routing and steps
/// </summary>
public class ApprovalRoutingService
{
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;

    public ApprovalRoutingService(
        IUserRepository userRepository,
        IBranchRepository branchRepository)
    {
        _userRepository = userRepository;
        _branchRepository = branchRepository;
    }

    /// <summary>
    /// Determine approval steps based on approval matrix and context
    /// </summary>
    public async Task<List<ApprovalStep>> DetermineApprovalStepsAsync(
        ApprovalMatrix approvalMatrix,
        decimal? amount,
        string? branchCode,
        string? department,
        CancellationToken cancellationToken = default)
    {
        var steps = new List<ApprovalStep>();

        foreach (var rule in approvalMatrix.Rules.OrderBy(r => r.Level))
        {
            // Check if this level applies based on amount
            if (amount.HasValue && rule.MinAmount.HasValue && amount < rule.MinAmount)
                continue;
            
            if (amount.HasValue && rule.MaxAmount.HasValue && amount > rule.MaxAmount)
                continue;

            steps.Add(new ApprovalStep
            {
                Level = rule.Level,
                RequiredRole = rule.ApproverRoles.FirstOrDefault(),
                SpecificApprover = null,
                IsRequired = true,
                TimeoutHours = rule.SlaHours
            });
        }

        return steps;
    }

    /// <summary>
    /// Get available approvers for a specific level
    /// </summary>
    public async Task<List<User>> GetApproversForLevelAsync(
        Guid workflowId,
        int level,
        CancellationToken cancellationToken = default)
    {
        // This would implement logic to find users with appropriate roles
        // For now, return empty list
        return new List<User>();
    }
}

/// <summary>
/// Represents an approval step in a workflow
/// </summary>
public class ApprovalStep
{
    public int Level { get; set; }
    public UserRole RequiredRole { get; set; }
    public string? SpecificApprover { get; set; }
    public bool IsRequired { get; set; } = true;
    public int TimeoutHours { get; set; } = 24;
}