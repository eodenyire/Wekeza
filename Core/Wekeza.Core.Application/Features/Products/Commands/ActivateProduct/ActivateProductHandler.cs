using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Products.Commands.ActivateProduct;

public class ActivateProductHandler : IRequestHandler<ActivateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ActivateProductHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByProductCodeAsync(request.ProductCode, cancellationToken);
        
        if (product == null)
        {
            throw new NotFoundException($"Product with code {request.ProductCode} not found.");
        }

        product.Activate(_currentUserService.UserId ?? "System");

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
