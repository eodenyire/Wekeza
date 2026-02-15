using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccount;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using UserRole = Wekeza.Core.Domain.Enums.UserRole;

namespace Wekeza.Core.Application.Features.Accounts.Commands.OpenAccount;

/// <summary>
/// Enhanced Account Opening Command with CIF Integration and Maker-Checker Workflow
/// Supports both new customer onboarding and additional account opening for existing customers
/// </summary>
[Authorize(UserRole.Teller, UserRole.CustomerService, UserRole.Supervisor)]
public record OpenAccountCommand : ICommand<Result<OpenAccountResult>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    // Customer Information (for new customers)
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? IdentificationNumber { get; init; }
    public string? IdentificationType { get; init; }
    public string? Nationality { get; init; } = "KE";
    
    // Existing Customer (for additional accounts)
    public Guid? ExistingCustomerId { get; init; }
    public string? CIFNumber { get; init; }
    
    // Account Details
    public Guid ProductId { get; init; }
    public string AccountType { get; init; } = "SAVINGS";
    public string CurrencyCode { get; init; } = "KES";
    public decimal InitialDeposit { get; init; }
    public Guid BranchId { get; init; }
    
    // Joint Account Information
    public bool IsJointAccount { get; init; } = false;
    public List<JointAccountHolderDto>? JointAccountHolders { get; init; }
    
    // Business Account Information
    public bool IsBusinessAccount { get; init; } = false;
    public BusinessAccountDetailsDto? BusinessDetails { get; init; }
    
    // Signatory Information
    public List<SignatoryDto> Signatories { get; init; } = new();
    
    // Additional Information
    public string? Purpose { get; init; }
    public string? SourceOfFunds { get; init; }
    public bool RequiresApproval { get; init; } = true;
    public string? TellerComments { get; init; }
}

public record JointAccountHolderDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string IdentificationNumber { get; init; } = string.Empty;
    public string IdentificationType { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Relationship { get; init; } = string.Empty;
    public decimal OwnershipPercentage { get; init; } = 50.0m;
}

public record BusinessAccountDetailsDto
{
    public string CompanyName { get; init; } = string.Empty;
    public string RegistrationNumber { get; init; } = string.Empty;
    public DateTime IncorporationDate { get; init; }
    public string CompanyType { get; init; } = string.Empty;
    public string Industry { get; init; } = string.Empty;
    public string TaxNumber { get; init; } = string.Empty;
    public List<BusinessSignatoryDto> AuthorizedSignatories { get; init; } = new();
}

public record BusinessSignatoryDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Position { get; init; } = string.Empty;
    public string IdentificationNumber { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool IsPrimaryContact { get; init; } = false;
}

public record SignatoryDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string IdentificationNumber { get; init; } = string.Empty;
    public string SignatureImageBase64 { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
    public int SigningOrder { get; init; } = 1;
}

public record OpenAccountResult
{
    public Guid AccountId { get; init; }
    public string AccountNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CIFNumber { get; init; } = string.Empty;
    public Guid? WorkflowId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public bool RequiresApproval { get; init; }
}