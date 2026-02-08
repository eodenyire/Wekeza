using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Loans.Commands.ApproveLoan;

/// <summary>
/// Approve Loan Handler - Processes manual loan approvals
/// Integrates with workflow engine and applies approval conditions
/// </summary>
public class ApproveLoanHandler : IRequestHandler<ApproveLoanCommand, ApproveLoanResult>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveLoanHandler(
        ILoanRepository loanRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _workflowRepository = workflowRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApproveLoanResult> Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Get the loan
            var loan = await _loanRepository.GetByIdAsync(request.LoanId, cancellationToken);
            if (loan == null)
            {
                return ApproveLoanResult.Failed("Loan not found");
            }

            // 2. Validate loan can be approved
            if (loan.Status != LoanStatus.Applied)
            {
                return ApproveLoanResult.Failed("Only applied loans can be approved");
            }

            // 3. Apply any modifications from approval
            if (request.ApprovedAmount.HasValue)
            {
                // This would require modifying the loan principal - simplified for now
                // In a real system, you might create a new loan or modify the existing one
            }

            if (request.ApprovedInterestRate.HasValue)
            {
                // This would require updating the interest rate - simplified for now
            }

            // 4. Create conditions if provided
            List<LoanCondition>? conditions = null;
            if (request.Conditions != null && request.Conditions.Any())
            {
                conditions = request.Conditions.Select(c => new LoanCondition(
                    c.ConditionType,
                    c.Description,
                    c.IsMandatory,
                    c.DueDate,
                    false)).ToList();
            }

            // 5. Approve the loan
            var approvedBy = request.ApprovedBy ?? (_currentUserService.UserId ?? Guid.Empty).ToString();
            loan.Approve(approvedBy, request.FirstPaymentDate, conditions);

            // 6. Update any related workflow
            await UpdateWorkflowStatusAsync(loan.Id, approvedBy, request.Comments);

            // 7. Save changes
            _loanRepository.Update(loan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApproveLoanResult.Success(
                loan.LoanNumber,
                loan.Principal.Amount,
                loan.InterestRate,
                loan.FirstPaymentDate,
                loan.MaturityDate,
                "Loan approved successfully and ready for disbursement");
        }
        catch (Exception ex)
        {
            return ApproveLoanResult.Failed($"Error approving loan: {ex.Message}");
        }
    }

    private async Task UpdateWorkflowStatusAsync(Guid loanId, string approvedBy, string? comments)
    {
        // Find any pending workflow for this loan
        var workflows = await _workflowRepository.GetByEntityIdAsync(loanId);
        var pendingWorkflow = workflows.FirstOrDefault(w => w.Status == WorkflowStatus.Pending);

        if (pendingWorkflow != null)
        {
            // Approve the workflow
            pendingWorkflow.Approve(approvedBy, comments ?? "Loan approved");
            _workflowRepository.UpdateWorkflow(pendingWorkflow);
        }
    }
}
