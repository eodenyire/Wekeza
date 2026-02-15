using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Events;

public record LoanCreditAssessedDomainEvent(
    Guid LoanId,
    string LoanNumber,
    decimal CreditScore,
    CreditRiskGrade RiskGrade) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}