///📂 Wekeza.Core.Api/Controllers/
///1. BaseApiController.cs
///This is the "Parent" of all endpoints. It injects MediatR once so that every other controller is clean and lean.
///

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Wekeza.Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    // Use Null-coalescing assignment to ensure mediator is only fetched once
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
