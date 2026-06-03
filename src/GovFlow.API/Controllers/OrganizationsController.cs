using GovFlow.API.Authentication;
using GovFlow.API.Contracts;
using GovFlow.API.Contracts.Organizations;
using GovFlow.Application.Organization.Dtos;
using GovFlow.Application.Organization.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovFlow.API.Controllers;

/// <summary>Manages organizations (tenants) and their departments.</summary>
[ApiController]
[Route("api/v1/organizations")]
[Produces("application/json")]
[Authorize]
public sealed class OrganizationsController : ControllerBase
{
    private readonly ISender _sender;

    public OrganizationsController(ISender sender) => _sender = sender;

    /// <summary>Lists all organizations.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrganizationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrganizationDto>>> List(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetOrganizationsQuery(), cancellationToken));

    /// <summary>Gets a single organization by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetOrganizationByIdQuery(id), cancellationToken));

    /// <summary>Creates a new organization. Requires the Admin role.</summary>
    [HttpPost]
    [Authorize(Policy = GovFlowPolicies.RequireAdmin)]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var id = await _sender.Send(request.ToCommand(), cancellationToken);
        return Created($"/api/v1/organizations/{id}", new CreatedResponse(id));
    }

    /// <summary>Creates a department within the organization. Requires the Manager role.</summary>
    [HttpPost("{organizationId:guid}/departments")]
    [Authorize(Policy = GovFlowPolicies.RequireManager)]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateDepartment(
        Guid organizationId,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _sender.Send(request.ToCommand(organizationId), cancellationToken);
        return Created($"/api/v1/organizations/{organizationId}/departments/{id}", new CreatedResponse(id));
    }
}
