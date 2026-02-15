using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.TradeFinance.Commands.IssueBankGuarantee;

/// <summary>
/// Command to issue a Bank Guarantee
/// Supports various types of guarantees including performance, advance payment, and bid bonds
/// </summary>
[Authorize(UserRole.LoanOfficer, UserRole.BranchManager, UserRole.Administrator)]
public record IssueBankGuaranteeCommand : ICommand<Result<BankGuaranteeResult>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    
    // Basic Information
    public Guid CustomerId { get; init; }
    public string GuaranteeType { get; init; } = string.Empty; // PERFORMANCE, ADVANCE_PAYMENT, BID_BOND, FINANCIAL
    public string GuaranteeNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
    
    // Parties
    public BeneficiaryDto Beneficiary { get; init; } = new();
    public ApplicantDto Applicant { get; init; } = new();
    
    // Terms and Conditions
    public DateTime IssueDate { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public List<string> Terms { get; init; } = new();
    public List<string> Conditions { get; init; } = new();
    
    // Security and Collateral
    public decimal MarginPercentage { get; init; } = 100; // 100% means full cash cover
    public List<CollateralDto> Collaterals { get; init; } = new();
    public Guid? CashCoverAccountId { get; init; }
    
    // Fees
    public decimal CommissionRate { get; init; }
    public decimal CommissionAmount { get; init; }
    public List<FeeDto> AdditionalFees { get; init; } = new();
    
    // Documentation
    public List<DocumentDto> RequiredDocuments { get; init; } = new();
    public List<DocumentDto> SubmittedDocuments { get; init; } = new();
    
    // Approval
    public bool RequiresApproval { get; init; } = true;
    public string? Comments { get; init; }
}

public record BeneficiaryDto
{
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public record ApplicantDto
{
    public Guid CustomerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public record CollateralDto
{
    public string Type { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Currency { get; init; } = "KES";
    public string Location { get; init; } = string.Empty;
    public DateTime? ValuationDate { get; init; }
}

public record FeeDto
{
    public string FeeType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "KES";
}

public record DocumentDto
{
    public string DocumentType { get; init; } = string.Empty;
    public string DocumentName { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public DateTime? SubmittedDate { get; init; }
    public bool IsRequired { get; init; } = true;
    public bool IsSubmitted { get; init; } = false;
}

public record BankGuaranteeResult
{
    public Guid GuaranteeId { get; init; }
    public string GuaranteeNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime IssuedDate { get; init; }
    public DateTime ExpiryDate { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public Guid? WorkflowId { get; init; }
    public string Message { get; init; } = string.Empty;
}