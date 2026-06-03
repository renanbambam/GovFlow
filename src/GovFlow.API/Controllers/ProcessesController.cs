using GovFlow.API.Authentication;
using GovFlow.API.Contracts;
using GovFlow.API.Contracts.Processes;
using GovFlow.Application.Process.Dtos;
using GovFlow.Application.Process.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovFlow.API.Controllers;

/// <summary>Opens and advances running process instances.</summary>
[ApiController]
[Route("api/v1/processes")]
[Produces("application/json")]
[Authorize]
public sealed class ProcessesController : ControllerBase
{
    private readonly ISender _sender;

    public ProcessesController(ISender sender) => _sender = sender;

    /// <summary>Lists process instances (summary), optionally filtered by organization.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProcessSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProcessSummaryDto>>> List(
        [FromQuery] Guid? organizationId,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessesQuery(organizationId), cancellationToken));

    /// <summary>Gets a single process instance (with steps) by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProcessInstanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessInstanceDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessByIdQuery(id), cancellationToken));

    /// <summary>Opens a new process instance from a process type. Requires the Manager role.</summary>
    [HttpPost]
    [Authorize(Policy = GovFlowPolicies.RequireManager)]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Open([FromBody] OpenProcessInstanceRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(request.ToCommand(), cancellationToken);
        return Created($"/api/v1/processes/{id}", new CreatedResponse(id));
    }

    /// <summary>Completes the current step, advancing the workflow or resolving the process. Requires the Analyst role.</summary>
    [HttpPost("{id:guid}/complete-step")]
    [Authorize(Policy = GovFlowPolicies.RequireAnalyst)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CompleteStep(
        Guid id,
        [FromBody] CompleteProcessStepRequest request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(request.ToCommand(id), cancellationToken);
        return NoContent();
    }
}
