using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Events;

/// <summary>
/// Domain event raised when a loan is approved
/// </summary>
public class LoanApprovedEvent : DomainEvent
{
    public Guid LoanId { get; }
    public Guid CustomerId { get; }
    public decimal ApprovedAmount { get; }
    public string Currency { get; }
    public decimal InterestRate { get; }
    public int TermInMonths { get; }
    public string ApprovedBy { get; }
    public DateTime ApprovedAt { get; }
    public string? ApprovalComments { get; }

    public LoanApprovedEvent(
        Guid loanId,
        Guid customerId,
        decimal approvedAmount,
        string currency,
        decimal interestRate,
        int termInMonths,
        string approvedBy,
        DateTime approvedAt,
        string? approvalComments = null)
    {
        LoanId = loanId;
        CustomerId = customerId;
        ApprovedAmount = approvedAmount;
        Currency = currency;
        InterestRate = interestRate;
        TermInMonths = termInMonths;
        ApprovedBy = approvedBy;
        ApprovedAt = approvedAt;
        ApprovalComments = approvalComments;
    }
}