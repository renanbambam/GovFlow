using GovFlow.Application.Dashboard.Dtos;
using GovFlow.Application.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovFlow.API.Controllers;

/// <summary>Executive dashboard counters.</summary>
[ApiController]
[Route("api/v1/dashboard")]
[Produces("application/json")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender) => _sender = sender;

    /// <summary>Returns totals for organizations, process types and processes.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> Get(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetDashboardQuery(), cancellationToken));
}
