using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.ValueObjects;

/// <summary>
/// SecurityPolicy Value Object - Represents a security policy with rules and enforcement
/// Supports enterprise-grade security policy management and evaluation
/// </summary>
public class SecurityPolicy : ValueObject
{
    public string PolicyCode { get; private set; } = string.Empty;
    public string PolicyName { get; private set; } = string.Empty;
    public PolicyType Type { get; private set; }
    public Dictionary<string, object> Rules { get; private set; } = new();
    public SecurityLevel Level { get; private set; }
    public bool IsEnforced { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public List<string> ApplicableResources { get; private set; } = new();
    public Dictionary<string, object> Metadata { get; private set; } = new();

    // Parameterless constructor for EF Core
    private SecurityPolicy() { }

    public SecurityPolicy(
        string policyCode,
        string policyName,
        PolicyType type,
        Dictionary<string, object> rules,
        SecurityLevel level = SecurityLevel.Internal,
        bool isEnforced = true,
        DateTime? effectiveFrom = null,
        DateTime? effectiveTo = null,
        string description = "",
        List<string>? applicableResources = null)
    {
        if (string.IsNullOrWhiteSpace(policyCode))
            throw new ArgumentException("Policy code cannot be empty", nameof(policyCode));
        if (string.IsNullOrWhiteSpace(policyName))
            throw new ArgumentException("Policy name cannot be empty", nameof(policyName));

        PolicyCode = policyCode;
        PolicyName = policyName;
        Type = type;
        Rules = rules ?? new Dictionary<string, object>();
        Level = level;
        IsEnforced = isEnforced;
        EffectiveFrom = effectiveFrom ?? DateTime.UtcNow;
        EffectiveTo = effectiveTo;
        Description = description;
        ApplicableResources = applicableResources ?? new List<string>();
        Metadata = new Dictionary<string, object>();
    }

    // Factory Methods for Common Security Policies
    public static SecurityPolicy CreatePasswordPolicy(
        int minLength = 8,
        int maxLength = 128,
        bool requireUppercase = true,
        bool requireLowercase = true,
        bool requireNumbers = true,
        bool requireSpecialChars = true,
        int maxAge = 90,
        int historyCount = 12)
    {
        var rules = new Dictionary<string, object>
        {
            ["MinLength"] = minLength,
            ["MaxLength"] = maxLength,
            ["RequireUppercase"] = requireUppercase,
            ["RequireLowercase"] = requireLowercase,
            ["RequireNumbers"] = requireNumbers,
            ["RequireSpecialChars"] = requireSpecialChars,
            ["MaxAge"] = maxAge,
            ["HistoryCount"] = historyCount,
            ["AllowedSpecialChars"] = "!@#$%^&*()_+-=[]{}|;:,.<>?"
        };

        return new SecurityPolicy(
            "PWD_POLICY_001",
            "Standard Password Policy",
            PolicyType.Authentication,
            rules,
            SecurityLevel.Internal,
            true,
            description: "Standard password complexity and lifecycle policy");
    }

    public static SecurityPolicy CreateAccountLockoutPolicy(
        int maxFailedAttempts = 5,
        int lockoutDurationMinutes = 30,
        int resetCounterMinutes = 15)
    {
        var rules = new Dictionary<string, object>
        {
            ["MaxFailedAttempts"] = maxFailedAttempts,
            ["LockoutDurationMinutes"] = lockoutDurationMinutes,
            ["ResetCounterMinutes"] = resetCounterMinutes,
            ["NotifyOnLockout"] = true,
            ["LogFailedAttempts"] = true
        };

        return new SecurityPolicy(
            "LOCKOUT_POLICY_001",
            "Account Lockout Policy",
            PolicyType.Authentication,
            rules,
            SecurityLevel.Internal,
            true,
            description: "Account lockout policy for failed login attempts");
    }

    public static SecurityPolicy CreateSessionPolicy(
        int sessionTimeoutMinutes = 480,
        int idleTimeoutMinutes = 30,
        int maxConcurrentSessions = 3,
        bool requireReauthentication = false)
    {
        var rules = new Dictionary<string, object>
        {
            ["SessionTimeoutMinutes"] = sessionTimeoutMinutes,
            ["IdleTimeoutMinutes"] = idleTimeoutMinutes,
            ["MaxConcurrentSessions"] = maxConcurrentSessions,
            ["RequireReauthentication"] = requireReauthentication,
            ["TrackSessionActivity"] = true,
            ["LogSessionEvents"] = true
        };

        return new SecurityPolicy(
            "SESSION_POLICY_001",
            "Session Management Policy",
            PolicyType.Session,
            rules,
            SecurityLevel.Internal,
            true,
            description: "Session timeout and management policy");
    }

    public static SecurityPolicy CreateTransactionLimitPolicy(
        decimal dailyLimit = 100000,
        decimal transactionLimit = 50000,
        decimal monthlyLimit = 1000000,
        bool requireApprovalAboveLimit = true)
    {
        var rules = new Dictionary<string, object>
        {
            ["DailyLimit"] = dailyLimit,
            ["TransactionLimit"] = transactionLimit,
            ["MonthlyLimit"] = monthlyLimit,
            ["RequireApprovalAboveLimit"] = requireApprovalAboveLimit,
            ["Currency"] = "KES",
            ["ApplyToAllUsers"] = false,
            ["ExemptRoles"] = new List<string> { "Administrator", "SystemService" }
        };

        return new SecurityPolicy(
            "TXN_LIMIT_POLICY_001",
            "Transaction Limit Policy",
            PolicyType.Authorization,
            rules,
            SecurityLevel.Confidential,
            true,
            applicableResources: new List<string> { "Transaction", "Transfer", "Payment" },
            description: "Daily and transaction limits for financial operations");
    }

    public static SecurityPolicy CreateDataClassificationPolicy(
        List<string> publicData,
        List<string> internalData,
        List<string> confidentialData,
        List<string> restrictedData)
    {
        var rules = new Dictionary<string, object>
        {
            ["PublicData"] = publicData ?? new List<string>(),
            ["InternalData"] = internalData ?? new List<string>(),
            ["ConfidentialData"] = confidentialData ?? new List<string>(),
            ["RestrictedData"] = restrictedData ?? new List<string>(),
            ["DefaultClassification"] = "Internal",
            ["RequireClassificationLabels"] = true,
            ["AuditDataAccess"] = true
        };

        return new SecurityPolicy(
            "DATA_CLASS_POLICY_001",
            "Data Classification Policy",
            PolicyType.DataProtection,
            rules,
            SecurityLevel.Confidential,
            true,
            description: "Data classification and handling policy");
    }

    public static SecurityPolicy CreateMfaPolicy(
        List<string> requiredRoles,
        List<string> requiredResources,
        bool allowBackupCodes = true,
        int backupCodeCount = 10)
    {
        var rules = new Dictionary<string, object>
        {
            ["RequiredRoles"] = requiredRoles ?? new List<string>(),
            ["RequiredResources"] = requiredResources ?? new List<string>(),
            ["AllowBackupCodes"] = allowBackupCodes,
            ["BackupCodeCount"] = backupCodeCount,
            ["SupportedMethods"] = new List<string> { "TOTP", "SMS", "Email", "Push" },
            ["GracePeriodDays"] = 7,
            ["EnforceForHighRiskOperations"] = true
        };

        return new SecurityPolicy(
            "MFA_POLICY_001",
            "Multi-Factor Authentication Policy",
            PolicyType.Authentication,
            rules,
            SecurityLevel.Confidential,
            true,
            description: "Multi-factor authentication requirements");
    }

    // Business Methods
    public bool Evaluate(Dictionary<string, object> context)
    {
        if (!IsEnforced || !IsEffective())
            return true;

        try
        {
            return Type switch
            {
                PolicyType.Authentication => EvaluateAuthenticationPolicy(context),
                PolicyType.Authorization => EvaluateAuthorizationPolicy(context),
                PolicyType.Session => EvaluateSessionPolicy(context),
                PolicyType.DataProtection => EvaluateDataProtectionPolicy(context),
                PolicyType.Compliance => EvaluateCompliancePolicy(context),
                _ => true
            };
        }
        catch (Exception)
        {
            // If evaluation fails, default to deny for security
            return false;
        }
    }

    public PolicyResult Apply(object target)
    {
        if (!IsEnforced || !IsEffective())
            return PolicyResult.NotApplicable();

        var context = ExtractContext(target);
        var isCompliant = Evaluate(context);

        return new PolicyResult(
            PolicyCode,
            isCompliant,
            isCompliant ? "Policy compliance verified" : "Policy violation detected",
            context);
    }

    public bool IsApplicable(string resource)
    {
        if (!ApplicableResources.Any())
            return true; // Policy applies to all resources

        return ApplicableResources.Contains(resource) || ApplicableResources.Contains("*");
    }

    public bool IsEffective(DateTime? checkDate = null)
    {
        var date = checkDate ?? DateTime.UtcNow;
        
        if (date < EffectiveFrom)
            return false;

        if (EffectiveTo.HasValue && date > EffectiveTo.Value)
            return false;

        return true;
    }

    public T GetRuleValue<T>(string ruleName, T defaultValue = default)
    {
        if (Rules.TryGetValue(ruleName, out var value))
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
        return defaultValue;
    }

    public bool HasRule(string ruleName)
    {
        return Rules.ContainsKey(ruleName);
    }

    public List<string> GetViolations(Dictionary<string, object> context)
    {
        var violations = new List<string>();

        if (!IsEnforced || !IsEffective())
            return violations;

        // This would contain specific violation checking logic based on policy type
        // For now, we'll return a general violation if evaluation fails
        if (!Evaluate(context))
        {
            violations.Add($"Policy {PolicyCode} violation detected");
        }

        return violations;
    }

    public SecurityPolicy WithUpdatedRules(Dictionary<string, object> newRules)
    {
        var updatedRules = new Dictionary<string, object>(Rules);
        foreach (var rule in newRules)
        {
            updatedRules[rule.Key] = rule.Value;
        }

        return new SecurityPolicy(
            PolicyCode,
            PolicyName,
            Type,
            updatedRules,
            Level,
            IsEnforced,
            EffectiveFrom,
            EffectiveTo,
            Description,
            ApplicableResources);
    }

    public SecurityPolicy WithEnforcement(bool enforced)
    {
        return new SecurityPolicy(
            PolicyCode,
            PolicyName,
            Type,
            Rules,
            Level,
            enforced,
            EffectiveFrom,
            EffectiveTo,
            Description,
            ApplicableResources);
    }

    // Private Methods
    private bool EvaluateAuthenticationPolicy(Dictionary<string, object> context)
    {
        // Password policy evaluation
        if (PolicyCode.StartsWith("PWD_"))
        {
            return EvaluatePasswordPolicy(context);
        }

        // Lockout policy evaluation
        if (PolicyCode.StartsWith("LOCKOUT_"))
        {
            return EvaluateLockoutPolicy(context);
        }

        // MFA policy evaluation
        if (PolicyCode.StartsWith("MFA_"))
        {
            return EvaluateMfaPolicy(context);
        }

        return true;
    }

    private bool EvaluateAuthorizationPolicy(Dictionary<string, object> context)
    {
        // Transaction limit evaluation
        if (PolicyCode.StartsWith("TXN_LIMIT_"))
        {
            return EvaluateTransactionLimitPolicy(context);
        }

        return true;
    }

    private bool EvaluateSessionPolicy(Dictionary<string, object> context)
    {
        // Session timeout evaluation
        if (context.TryGetValue("SessionStartTime", out var startTimeObj) && 
            startTimeObj is DateTime startTime)
        {
            var sessionTimeout = GetRuleValue("SessionTimeoutMinutes", 480);
            var elapsed = DateTime.UtcNow - startTime;
            
            if (elapsed.TotalMinutes > sessionTimeout)
                return false;
        }

        return true;
    }

    private bool EvaluateDataProtectionPolicy(Dictionary<string, object> context)
    {
        // Data classification evaluation
        if (context.TryGetValue("DataClassification", out var classificationObj))
        {
            var classification = classificationObj?.ToString();
            var requiredLevel = GetRuleValue("MinimumClassification", "Internal");
            
            // This would need proper classification level comparison
            return !string.IsNullOrEmpty(classification);
        }

        return true;
    }

    private bool EvaluateCompliancePolicy(Dictionary<string, object> context)
    {
        // Compliance-specific evaluation logic
        return true;
    }

    private bool EvaluatePasswordPolicy(Dictionary<string, object> context)
    {
        if (!context.TryGetValue("Password", out var passwordObj) || 
            passwordObj is not string password)
            return true; // No password to evaluate

        var minLength = GetRuleValue("MinLength", 8);
        var maxLength = GetRuleValue("MaxLength", 128);
        var requireUppercase = GetRuleValue("RequireUppercase", true);
        var requireLowercase = GetRuleValue("RequireLowercase", true);
        var requireNumbers = GetRuleValue("RequireNumbers", true);
        var requireSpecialChars = GetRuleValue("RequireSpecialChars", true);

        if (password.Length < minLength || password.Length > maxLength)
            return false;

        if (requireUppercase && !password.Any(char.IsUpper))
            return false;

        if (requireLowercase && !password.Any(char.IsLower))
            return false;

        if (requireNumbers && !password.Any(char.IsDigit))
            return false;

        if (requireSpecialChars)
        {
            var specialChars = GetRuleValue("AllowedSpecialChars", "!@#$%^&*()_+-=[]{}|;:,.<>?");
            if (!password.Any(c => specialChars.Contains(c)))
                return false;
        }

        return true;
    }

    private bool EvaluateLockoutPolicy(Dictionary<string, object> context)
    {
        if (context.TryGetValue("FailedAttempts", out var attemptsObj) && 
            attemptsObj is int attempts)
        {
            var maxAttempts = GetRuleValue("MaxFailedAttempts", 5);
            return attempts < maxAttempts;
        }

        return true;
    }

    private bool EvaluateMfaPolicy(Dictionary<string, object> context)
    {
        var requiredRoles = GetRuleValue<List<string>>("RequiredRoles", new List<string>());
        var requiredResources = GetRuleValue<List<string>>("RequiredResources", new List<string>());

        if (context.TryGetValue("UserRoles", out var rolesObj) && 
            rolesObj is List<string> userRoles)
        {
            if (requiredRoles.Any(role => userRoles.Contains(role)))
            {
                return context.TryGetValue("MfaVerified", out var mfaObj) && 
                       mfaObj is bool mfaVerified && mfaVerified;
            }
        }

        if (context.TryGetValue("Resource", out var resourceObj) && 
            resourceObj is string resource)
        {
            if (requiredResources.Contains(resource))
            {
                return context.TryGetValue("MfaVerified", out var mfaObj) && 
                       mfaObj is bool mfaVerified && mfaVerified;
            }
        }

        return true;
    }

    private bool EvaluateTransactionLimitPolicy(Dictionary<string, object> context)
    {
        if (context.TryGetValue("Amount", out var amountObj) && 
            amountObj is decimal amount)
        {
            var transactionLimit = GetRuleValue("TransactionLimit", 50000m);
            return amount <= transactionLimit;
        }

        return true;
    }

    private Dictionary<string, object> ExtractContext(object target)
    {
        var context = new Dictionary<string, object>();
        
        if (target != null)
        {
            var properties = target.GetType().GetProperties();
            foreach (var prop in properties)
            {
                try
                {
                    var value = prop.GetValue(target);
                    if (value != null)
                    {
                        context[prop.Name] = value;
                    }
                }
                catch
                {
                    // Ignore property access errors
                }
            }
        }

        return context;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PolicyCode;
        yield return Type;
        yield return Level;
        yield return IsEnforced;
        yield return EffectiveFrom;
        yield return EffectiveTo ?? DateTime.MaxValue;
        
        foreach (var rule in Rules.OrderBy(r => r.Key))
        {
            yield return rule.Key;
            yield return rule.Value;
        }
    }

    public override string ToString()
    {
        var status = IsEnforced ? "Enforced" : "Not Enforced";
        var effective = IsEffective() ? "Active" : "Inactive";
        return $"{PolicyCode}: {PolicyName} ({Type}, {Level}, {status}, {effective})";
    }
}

// Supporting Classes
public class PolicyResult
{
    public string PolicyCode { get; set; }
    public bool IsCompliant { get; set; }
    public string Message { get; set; }
    public Dictionary<string, object> Context { get; set; }
    public DateTime EvaluatedAt { get; set; }

    public PolicyResult(string policyCode, bool isCompliant, string message, Dictionary<string, object> context)
    {
        PolicyCode = policyCode;
        IsCompliant = isCompliant;
        Message = message;
        Context = context ?? new Dictionary<string, object>();
        EvaluatedAt = DateTime.UtcNow;
    }

    public static PolicyResult NotApplicable()
    {
        return new PolicyResult("", true, "Policy not applicable", new Dictionary<string, object>());
    }
}

// Enumerations
public enum PolicyType
{
    Authentication,
    Authorization,
    Session,
    DataProtection,
    Compliance,
    Audit,
    Network,
    Application
}