using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.CIF.Queries.GetCustomer360View;

/// <summary>
/// Query to get complete 360° view of a customer
/// Similar to Finacle Customer 360 and T24 Customer Overview
/// </summary>
[Authorize(UserRole.Teller, UserRole.RiskOfficer, UserRole.Administrator)]
public record GetCustomer360ViewQuery : IQuery<Customer360ViewDto>
{
    public string PartyNumber { get; init; } = string.Empty;
}

public record Customer360ViewDto
{
    // Party Information
    public string PartyNumber { get; init; } = string.Empty;
    public string PartyType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; }

    // Individual Details
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Nationality { get; init; }

    // Corporate Details
    public string? CompanyName { get; init; }
    public string? RegistrationNumber { get; init; }
    public string? Industry { get; init; }

    // Contact Information
    public string? PrimaryEmail { get; init; }
    public string? PrimaryPhone { get; init; }
    public List<AddressInfo> Addresses { get; init; } = new();

    // KYC & Risk
    public string KYCStatus { get; init; } = string.Empty;
    public DateTime? KYCCompletedDate { get; init; }
    public DateTime? KYCExpiryDate { get; init; }
    public string RiskRating { get; init; } = string.Empty;
    public bool IsPEP { get; init; }
    public bool IsSanctioned { get; init; }

    // Segmentation
    public string Segment { get; init; } = string.Empty;
    public string? SubSegment { get; init; }

    // Accounts Summary
    public List<AccountSummary> Accounts { get; init; } = new();
    public decimal TotalBalance { get; init; }

    // Loans Summary
    public List<LoanSummary> Loans { get; init; } = new();
    public decimal TotalLoanOutstanding { get; init; }

    // Cards Summary
    public List<CardSummary> Cards { get; init; } = new();

    // Recent Transactions
    public List<TransactionSummary> RecentTransactions { get; init; } = new();

    // Relationships
    public List<RelationshipInfo> Relationships { get; init; } = new();

    // Alerts & Flags
    public List<string> Alerts { get; init; } = new();
}

public record AddressInfo
{
    public string AddressType { get; init; } = string.Empty;
    public string FullAddress { get; init; } = string.Empty;
    public bool IsPrimary { get; init; }
}

public record AccountSummary
{
    public string AccountNumber { get; init; } = string.Empty;
    public string AccountType { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public decimal Balance { get; init; }
    public string Status { get; init; } = string.Empty;
}

public record LoanSummary
{
    public Guid LoanId { get; init; }
    public string LoanType { get; init; } = string.Empty;
    public decimal Principal { get; init; }
    public decimal Outstanding { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? NextPaymentDate { get; init; }
}

public record CardSummary
{
    public string CardNumber { get; init; } = string.Empty;
    public string CardType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime ExpiryDate { get; init; }
}

public record TransactionSummary
{
    public DateTime Date { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Description { get; init; } = string.Empty;
}

public record RelationshipInfo
{
    public string RelatedPartyNumber { get; init; } = string.Empty;
    public string RelatedPartyName { get; init; } = string.Empty;
    public string RelationshipType { get; init; } = string.Empty;
}
