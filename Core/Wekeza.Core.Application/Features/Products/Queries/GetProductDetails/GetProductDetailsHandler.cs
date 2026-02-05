using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Features.Products.Commands.CreateProduct;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Products.Queries.GetProductDetails;

public class GetProductDetailsHandler : IRequestHandler<GetProductDetailsQuery, ProductDetailsDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductDetailsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDetailsDto> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByProductCodeAsync(request.ProductCode, cancellationToken);
        
        if (product == null)
        {
            throw new NotFoundException("Product", request.ProductCode);
        }

        return new ProductDetailsDto
        {
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            Category = product.Category.ToString(),
            Type = product.Type.ToString(),
            Status = product.Status.ToString(),
            Currency = product.Currency,
            Description = product.Description,
            MarketingDescription = product.MarketingDescription,
            EffectiveDate = product.EffectiveDate,
            ExpiryDate = product.ExpiryDate,
            
            InterestConfig = product.InterestConfig != null ? new InterestConfigDto
            {
                Type = product.InterestConfig.Type,
                Rate = product.InterestConfig.Rate,
                CalculationMethod = product.InterestConfig.CalculationMethod,
                PostingFrequency = product.InterestConfig.PostingFrequency,
                IsTiered = product.InterestConfig.IsTiered,
                Tiers = product.InterestConfig.Tiers?.Select(t => new InterestTierDto
                {
                    MinBalance = t.MinBalance,
                    MaxBalance = t.MaxBalance,
                    Rate = t.Rate
                }).ToList()
            } : null,
            
            Fees = product.Fees.Select(f => new FeeConfigDto
            {
                FeeCode = f.FeeCode,
                FeeType = f.FeeType,
                FeeName = f.FeeName,
                CalculationType = f.CalculationType,
                Amount = f.Amount,
                Percentage = f.Percentage,
                MinAmount = f.MinAmount,
                MaxAmount = f.MaxAmount,
                IsWaivable = f.IsWaivable
            }).ToList(),
            
            Limits = product.Limits != null ? new LimitConfigDto
            {
                MinBalance = product.Limits.MinBalance,
                MaxBalance = product.Limits.MaxBalance,
                MinTransactionAmount = product.Limits.MinTransactionAmount,
                MaxTransactionAmount = product.Limits.MaxTransactionAmount,
                DailyTransactionLimit = product.Limits.DailyTransactionLimit,
                MaxTransactionsPerDay = product.Limits.MaxTransactionsPerDay,
                MonthlyTransactionLimit = product.Limits.MonthlyTransactionLimit
            } : null,
            
            EligibilityRules = product.EligibilityRules.Select(r => new EligibilityRuleDto
            {
                RuleType = r.RuleType,
                Operator = r.Operator,
                Value = r.Value
            }).ToList(),
            
            AccountingConfig = product.AccountingConfig != null ? new AccountingConfigDto
            {
                AssetGLCode = product.AccountingConfig.AssetGLCode,
                LiabilityGLCode = product.AccountingConfig.LiabilityGLCode,
                IncomeGLCode = product.AccountingConfig.IncomeGLCode,
                ExpenseGLCode = product.AccountingConfig.ExpenseGLCode,
                InterestPayableGLCode = product.AccountingConfig.InterestPayableGLCode,
                InterestReceivableGLCode = product.AccountingConfig.InterestReceivableGLCode
            } : null,
            
            Attributes = product.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }
}
