using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Application.Common.Interfaces;
using DomainEnums = Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Loans.Commands.ApplyForLoan;

/// <summary>
/// Apply for Loan Handler - Processes loan applications with credit assessment
/// Integrates with Product Factory, Credit Scoring, and Workflow Engine
/// </summary>
public class ApplyForLoanHandler : IRequestHandler<ApplyForLoanCommand, ApplyForLoanResult>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly CreditScoringService _creditScoringService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyForLoanHandler(
        ILoanRepository loanRepository,
        IProductRepository productRepository,
        IPartyRepository partyRepository,
        IWorkflowRepository workflowRepository,
        CreditScoringService creditScoringService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _productRepository = productRepository;
        _partyRepository = partyRepository;
        _workflowRepository = workflowRepository;
        _creditScoringService = creditScoringService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplyForLoanResult> Handle(ApplyForLoanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate customer exists
            var customer = await _partyRepository.GetByIdAsync(request.CustomerId, cancellationToken);
            if (customer == null)
            {
                return ApplyForLoanResult.Failed("Customer not found");
            }

            // 2. Validate product exists and is active
            var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                return ApplyForLoanResult.Failed("Loan product not found");
            }

            if (!product.IsActive)
            {
                return ApplyForLoanResult.Failed("Loan product is not active");
            }

            // 3. Validate loan amount against product limits
            var loanAmount = new Money(request.Amount, Currency.FromCode(request.Currency));
            if (product.LimitConfig != null)
            {
                if (product.LimitConfig.MinTransactionAmount.HasValue && 
                    loanAmount.Amount < product.LimitConfig.MinTransactionAmount.Value)
                {
                    return ApplyForLoanResult.Failed($"Loan amount below minimum limit of {product.LimitConfig.MinTransactionAmount.Value}");
                }

                if (product.LimitConfig.MaxTransactionAmount.HasValue && 
                    loanAmount.Amount > product.LimitConfig.MaxTransactionAmount.Value)
                {
                    return ApplyForLoanResult.Failed($"Loan amount exceeds maximum limit of {product.LimitConfig.MaxTransactionAmount.Value}");
                }
            }

            // 4. Create loan application
            var currentUser = (_currentUserService.UserId ?? Guid.Empty).ToString();
            var loan = Loan.CreateApplication(
                request.CustomerId,
                request.ProductId,
                loanAmount,
                request.TermInMonths,
                currentUser,
                product);

            // 5. Add collaterals if provided
            if (request.Collaterals != null)
            {
                foreach (var collateralDto in request.Collaterals)
                {
                    var collateral = new LoanCollateral(
                        Guid.NewGuid(),
                        collateralDto.CollateralType,
                        collateralDto.Description,
                        new Money(collateralDto.Value, Currency.FromCode(collateralDto.Currency)),
                        collateralDto.ValuationDate,
                        collateralDto.ValuedBy);

                    loan.AddCollateral(collateral);
                }
            }

            // 6. Add guarantors if provided
            if (request.Guarantors != null)
            {
                foreach (var guarantorDto in request.Guarantors)
                {
                    var guarantor = new LoanGuarantor(
                        guarantorDto.GuarantorId,
                        guarantorDto.GuarantorName,
                        new Money(guarantorDto.GuaranteeAmount, Currency.FromCode(guarantorDto.Currency)),
                        DateTime.UtcNow,
                        guarantorDto.GuaranteeDocument);

                    loan.AddGuarantor(guarantor);
                }
            }

            // 7. Perform credit scoring
            var creditScoreResult = await _creditScoringService.CalculateCreditScoreAsync(
                request.CustomerId, loanAmount);

            // 8. Update loan with credit assessment
            loan.UpdateCreditAssessment(
                creditScoreResult.CreditScore,
                creditScoreResult.RiskGrade,
                creditScoreResult.RiskPremium,
                currentUser);

            // 9. Check for auto-approval
            var isAutoApproved = ShouldAutoApprove(creditScoreResult, loanAmount, product);
            
            if (isAutoApproved)
            {
                // Auto-approve the loan
                loan.Approve(currentUser, request.PreferredDisbursementDate);
            }
            else
            {
                // Create workflow for manual approval
                await CreateApprovalWorkflowAsync(loan, creditScoreResult, currentUser);
            }

            // 10. Save loan
            await _loanRepository.AddAsync(loan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApplyForLoanResult.Success(
                loan.Id,
                loan.LoanNumber,
                creditScoreResult.CreditScore,
                creditScoreResult.RiskGrade.ToString(),
                creditScoreResult.RecommendedInterestRate,
                isAutoApproved);
        }
        catch (Exception ex)
        {
            return ApplyForLoanResult.Failed($"Error processing loan application: {ex.Message}");
        }
    }

    private bool ShouldAutoApprove(CreditScoreResult creditScore, Money loanAmount, Product product)
    {
        // Auto-approval criteria
        // 1. Credit score above threshold
        if (creditScore.CreditScore < 750) return false;

        // 2. Loan amount within auto-approval limits (e.g., under 100K)
        if (loanAmount.Amount > 100000) return false;

        // 3. Customer has good banking relationship
        if (!creditScore.IsApprovalRecommended) return false;

        // 4. Product allows auto-approval
        // This would be configured in the product
        return true;
    }

    private async Task CreateApprovalWorkflowAsync(Loan loan, CreditScoreResult creditScore, string initiatedBy, CancellationToken cancellationToken = default)
    {
        // Determine approval level based on loan amount and risk
        var approvalLevel = DetermineApprovalLevel(loan.Principal, creditScore.RiskGrade);
        
        // Get approval matrix for loan approval
        var approvalMatrix = await _workflowRepository.GetApprovalMatrixAsync(
            "LoanApproval", 
            loan.Principal.Amount, 
            loan.Principal.Currency.Code,
            cancellationToken);
        
        if (approvalMatrix != null)
        {
            // Create workflow instance
            var workflow = WorkflowInstance.Create(
                "LOAN_APPROVAL",
                "Loan Approval",
                Wekeza.Core.Domain.Aggregates.WorkflowType.LoanApproval,
                "Loan",
                loan.Id,
                loan.LoanNumber,
                approvalMatrix.Rules?.Count ?? 1, // Use rule count as required levels
                initiatedBy,
                $"Loan approval request for {loan.LoanNumber} - Amount: {loan.Principal.Amount}, Risk Grade: {creditScore.RiskGrade}",
                24);

            await _workflowRepository.AddWorkflowAsync(workflow, cancellationToken);
        }
    }

    private string DetermineApprovalLevel(Money loanAmount, DomainEnums.CreditRiskGrade riskGrade)
    {
        // Determine approval level based on amount and risk
        return (loanAmount.Amount, riskGrade) switch
        {
            (< 50000, DomainEnums.CreditRiskGrade.AAA or DomainEnums.CreditRiskGrade.AA or DomainEnums.CreditRiskGrade.A) => "Level1", // Branch Manager
            (< 100000, DomainEnums.CreditRiskGrade.AAA or DomainEnums.CreditRiskGrade.AA or DomainEnums.CreditRiskGrade.A) => "Level2", // Regional Manager
            (< 500000, _) => "Level3", // Credit Committee
            (_, _) => "Level4" // Board Approval
        };
    }
}