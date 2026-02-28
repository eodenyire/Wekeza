using MediatR;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Loans.Queries.GetLoanDetails;

/// <summary>
/// Get Loan Details Handler - Retrieves comprehensive loan information
/// Returns complete loan details with related data based on query parameters
/// </summary>
public class GetLoanDetailsHandler : IRequestHandler<GetLoanDetailsQuery, LoanDetailsDto?>
{
    private readonly ILoanRepository _loanRepository;

    public GetLoanDetailsHandler(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }

    public async Task<LoanDetailsDto?> Handle(GetLoanDetailsQuery request, CancellationToken cancellationToken)
    {
        // Get loan by ID or loan number
        var loan = request.LoanId.HasValue
            ? await _loanRepository.GetByIdAsync(request.LoanId.Value, cancellationToken)
            : !string.IsNullOrEmpty(request.LoanNumber)
                ? await _loanRepository.GetByLoanNumberAsync(request.LoanNumber, cancellationToken)
                : null;

        if (loan == null)
            return null;

        // Map to DTO
        var loanDetails = new LoanDetailsDto
        {
            Id = loan.Id,
            LoanNumber = loan.LoanNumber,
            CustomerId = loan.CustomerId,
            CustomerName = loan.Customer?.FullName ?? "Unknown",
            ProductId = loan.ProductId,
            ProductName = loan.Product?.Name ?? "Unknown",
            
            // Loan amounts and terms
            PrincipalAmount = loan.Principal.Amount,
            OutstandingPrincipal = loan.OutstandingPrincipal.Amount,
            Currency = loan.Principal.Currency.Code,
            InterestRate = loan.InterestRate,
            TermInMonths = loan.TermInMonths,
            FirstPaymentDate = loan.FirstPaymentDate,
            MaturityDate = loan.MaturityDate,
            
            // Loan status
            Status = loan.Status.ToString(),
            SubStatus = loan.SubStatus.ToString(),
            ApplicationDate = loan.ApplicationDate,
            ApprovalDate = loan.ApprovalDate,
            DisbursementDate = loan.DisbursementDate,
            ClosureDate = loan.ClosureDate,
            
            // Credit assessment
            CreditScore = loan.CreditScore,
            RiskGrade = loan.RiskGrade?.ToString(),
            RiskPremium = loan.RiskPremium,
            
            // Interest and payments
            AccruedInterest = loan.AccruedInterest.Amount,
            TotalInterestPaid = loan.TotalInterestPaid.Amount,
            TotalAmountPaid = loan.TotalAmountPaid.Amount,
            LastPaymentDate = loan.LastPaymentDate,
            DaysPastDue = loan.DaysPastDue,
            PastDueAmount = loan.PastDueAmount.Amount,
            
            // Provisioning
            ProvisionRate = loan.ProvisionRate,
            ProvisionAmount = loan.ProvisionAmount.Amount,
            
            // Disbursement account
            DisbursementAccountId = loan.DisbursementAccountId,
            DisbursementAccountNumber = loan.DisbursementAccount?.AccountNumber.Value,
            
            // Audit information
            CreatedBy = loan.CreatedBy,
            CreatedDate = loan.CreatedDate,
            ApprovedBy = loan.ApprovedBy,
            DisbursedBy = loan.DisbursedBy
        };

        // Include schedule if requested
        if (request.IncludeSchedule && loan.Schedule.Any())
        {
            loanDetails = loanDetails with
            {
                Schedule = loan.Schedule.Select(s => new LoanScheduleItemDto
                {
                    ScheduleNumber = s.ScheduleNumber,
                    DueDate = s.DueDate,
                    PrincipalAmount = s.PrincipalAmount.Amount,
                    InterestAmount = s.InterestAmount.Amount,
                    TotalAmount = s.TotalAmount.Amount,
                    OutstandingBalance = s.OutstandingBalance.Amount,
                    IsPaid = s.IsPaid,
                    PaidDate = s.PaidDate,
                    PaidAmount = s.PaidAmount.Amount
                }).ToList()
            };
        }

        // Include collaterals if requested
        if (request.IncludeCollaterals && loan.Collaterals.Any())
        {
            loanDetails = loanDetails with
            {
                Collaterals = loan.Collaterals.Select(c => new LoanCollateralDto
                {
                    CollateralId = c.CollateralId,
                    CollateralType = c.CollateralType,
                    Description = c.Description,
                    Value = c.Value.Amount,
                    Currency = c.Value.Currency.Code,
                    ValuationDate = c.ValuationDate,
                    ValuedBy = c.ValuedBy
                }).ToList()
            };
        }

        // Include guarantors if requested
        if (request.IncludeGuarantors && loan.Guarantors.Any())
        {
            loanDetails = loanDetails with
            {
                Guarantors = loan.Guarantors.Select(g => new LoanGuarantorDto
                {
                    GuarantorId = g.GuarantorId,
                    GuarantorName = g.GuarantorName,
                    GuaranteeAmount = g.GuaranteeAmount.Amount,
                    Currency = g.GuaranteeAmount.Currency.Code,
                    GuaranteeDate = g.GuaranteeDate,
                    GuaranteeDocument = g.GuaranteeDocument
                }).ToList()
            };
        }

        // Include conditions
        if (loan.Conditions.Any())
        {
            loanDetails = loanDetails with
            {
                Conditions = loan.Conditions.Select(c => new LoanConditionDto
                {
                    ConditionType = c.ConditionType,
                    Description = c.Description,
                    IsMandatory = c.IsMandatory,
                    DueDate = c.DueDate,
                    IsComplied = c.IsComplied
                }).ToList()
            };
        }

        return loanDetails;
    }
}