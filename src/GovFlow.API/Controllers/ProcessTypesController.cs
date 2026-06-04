using GovFlow.API.Authentication;
using GovFlow.API.Contracts;
using GovFlow.API.Contracts.Processes;
using GovFlow.Application.Process.Dtos;
using GovFlow.Application.Process.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovFlow.API.Controllers;

[ApiController]
[Route("api/v1/process-types")]
[Produces("application/json")]
[Authorize]
public sealed class ProcessTypesController : ControllerBase
{
    private readonly ISender _sender;

    public ProcessTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProcessTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProcessTypeDto>>> List(
        [FromQuery] Guid? organizationId,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessTypesQuery(organizationId), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProcessTypeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessTypeDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessTypeByIdQuery(id), cancellationToken));

    [HttpPost]
    [Authorize(Policy = GovFlowPolicies.RequireManager)]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateProcessTypeRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(request.ToCommand(), cancellationToken);
        return Created($"/api/v1/process-types/{id}", new CreatedResponse(id));
    }
}
