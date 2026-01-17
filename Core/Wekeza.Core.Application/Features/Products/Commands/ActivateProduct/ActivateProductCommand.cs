using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Products.Commands.ActivateProduct;

[Authorize(UserRole.Administrator)]
public record ActivateProductCommand : ICommand<bool>
{
    public string ProductCode { get; init; } = string.Empty;
}
