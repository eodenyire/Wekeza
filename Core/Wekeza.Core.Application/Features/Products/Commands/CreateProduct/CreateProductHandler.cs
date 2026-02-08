using MediatR;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Products.Commands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, string>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateProductHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate product code
        var exists = await _productRepository.ExistsByProductCodeAsync(request.ProductCode, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"Product with code {request.ProductCode} already exists.");
        }

        // Create product based on category
        var product = request.Category == Domain.Enums.ProductCategory.Deposits
            ? Product.CreateDepositProduct(
                request.ProductCode,
                request.ProductName,
                request.Type,
                request.Currency,
                (_currentUserService.UserId ?? Guid.Empty).ToString())
            : Product.CreateLoanProduct(
                request.ProductCode,
                request.ProductName,
                request.Type,
                request.Currency,
                (_currentUserService.UserId ?? Guid.Empty).ToString());

        // Set description
        product.UpdateDescription(request.Description, request.MarketingDescription);

        // Configure interest if provided
        if (request.InterestConfig != null)
        {
            var interestConfig = new InterestConfiguration(
                request.InterestConfig.Type,
                request.InterestConfig.Rate,
                request.InterestConfig.CalculationMethod,
                request.InterestConfig.PostingFrequency,
                request.InterestConfig.IsTiered,
                request.InterestConfig.Tiers?.Select(t => new InterestTier(
                    t.MinBalance,
                    t.MaxBalance,
                    t.Rate)).ToList());

            product.ConfigureInterest(interestConfig);
        }

        // Configure fees if provided
        if (request.Fees != null)
        {
            foreach (var feeDto in request.Fees)
            {
                var fee = new FeeConfiguration(
                    feeDto.FeeCode,
                    feeDto.FeeType,
                    feeDto.FeeName,
                    feeDto.CalculationType,
                    feeDto.Amount,
                    feeDto.Percentage,
                    feeDto.MinAmount,
                    feeDto.MaxAmount,
                    feeDto.IsWaivable);

                product.AddFee(fee);
            }
        }

        // Configure limits if provided
        if (request.Limits != null)
        {
            var limits = new LimitConfiguration(
                request.Limits.MinBalance,
                request.Limits.MaxBalance,
                request.Limits.MinTransactionAmount,
                request.Limits.MaxTransactionAmount,
                request.Limits.DailyTransactionLimit,
                request.Limits.MaxTransactionsPerDay,
                request.Limits.MonthlyTransactionLimit);

            product.ConfigureLimits(limits);
        }

        // Configure eligibility rules if provided
        if (request.EligibilityRules != null)
        {
            foreach (var ruleDto in request.EligibilityRules)
            {
                var rule = new EligibilityRule(
                    ruleDto.RuleType,
                    ruleDto.Operator,
                    ruleDto.Value);

                product.AddEligibilityRule(rule);
            }
        }

        // Configure accounting if provided
        if (request.AccountingConfig != null)
        {
            var accountingConfig = new AccountingConfiguration(
                request.AccountingConfig.AssetGLCode,
                request.AccountingConfig.LiabilityGLCode,
                request.AccountingConfig.IncomeGLCode,
                request.AccountingConfig.ExpenseGLCode,
                request.AccountingConfig.InterestPayableGLCode,
                request.AccountingConfig.InterestReceivableGLCode);

            product.ConfigureAccounting(accountingConfig);
        }

        // Save product
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.ProductCode;
    }
}
