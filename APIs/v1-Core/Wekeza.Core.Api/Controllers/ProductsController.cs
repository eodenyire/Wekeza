using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.Products.Commands.CreateProduct;
using Wekeza.Core.Application.Features.Products.Commands.ActivateProduct;
using Wekeza.Core.Application.Features.Products.Queries.GetProductCatalog;
using Wekeza.Core.Application.Features.Products.Queries.GetProductDetails;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Product Factory Controller
/// Enterprise product configuration similar to Finacle Product Factory and T24 ARRANGEMENT
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new banking product
    /// </summary>
    /// <param name="command">Product configuration</param>
    /// <returns>Product code</returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productCode = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetProductDetails),
            new { productCode },
            new { productCode, message = "Product created successfully" });
    }

    /// <summary>
    /// Get product catalog (all active products)
    /// </summary>
    /// <param name="category">Optional category filter</param>
    /// <param name="activeOnly">Show only active products</param>
    /// <returns>Product catalog</returns>
    [HttpGet("catalog")]
    [ProducesResponseType(typeof(ProductCatalogDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductCatalog(
        [FromQuery] ProductCategory? category = null,
        [FromQuery] bool activeOnly = true)
    {
        var query = new GetProductCatalogQuery
        {
            Category = category,
            ActiveOnly = activeOnly
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get detailed product configuration
    /// </summary>
    /// <param name="productCode">Product code</param>
    /// <returns>Complete product details</returns>
    [HttpGet("{productCode}")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductDetails(string productCode)
    {
        var query = new GetProductDetailsQuery { ProductCode = productCode };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Activate a product (make it available to customers)
    /// </summary>
    /// <param name="productCode">Product code</param>
    /// <returns>Success indicator</returns>
    [HttpPost("{productCode}/activate")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateProduct(string productCode)
    {
        var command = new ActivateProductCommand { ProductCode = productCode };
        var result = await _mediator.Send(command);
        return Ok(new { success = result, message = "Product activated successfully" });
    }

    /// <summary>
    /// Get active deposit products
    /// </summary>
    /// <returns>List of deposit products</returns>
    [HttpGet("deposits")]
    [ProducesResponseType(typeof(ProductCatalogDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDepositProducts()
    {
        var query = new GetProductCatalogQuery
        {
            Category = ProductCategory.Deposits,
            ActiveOnly = true
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get active loan products
    /// </summary>
    /// <returns>List of loan products</returns>
    [HttpGet("loans")]
    [ProducesResponseType(typeof(ProductCatalogDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoanProducts()
    {
        var query = new GetProductCatalogQuery
        {
            Category = ProductCategory.Loans,
            ActiveOnly = true
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
