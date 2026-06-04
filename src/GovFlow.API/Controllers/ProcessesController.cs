using GovFlow.API.Authentication;
using GovFlow.API.Contracts;
using GovFlow.API.Contracts.Processes;
using GovFlow.Application.Process.Commands.AddProcessComment;
using GovFlow.Application.Process.Commands.AttachProcessDocument;
using GovFlow.Application.Process.Dtos;
using GovFlow.Application.Process.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovFlow.API.Controllers;

[ApiController]
[Route("api/v1/processes")]
[Produces("application/json")]
[Authorize]
public sealed class ProcessesController : ControllerBase
{
    private readonly ISender _sender;

    public ProcessesController(ISender sender) => _sender = sender;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProcessSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProcessSummaryDto>>> List(
        [FromQuery] Guid? organizationId,
        CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessesQuery(organizationId), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProcessInstanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessInstanceDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessByIdQuery(id), cancellationToken));

    [HttpGet("{id:guid}/timeline")]
    [ProducesResponseType(typeof(IReadOnlyList<ProcessTimelineEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ProcessTimelineEntryDto>>> GetTimeline(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessTimelineQuery(id), cancellationToken));

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

    [HttpGet("{id:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyList<ProcessCommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ProcessCommentDto>>> GetComments(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessCommentsQuery(id), cancellationToken));

    [HttpPost("{id:guid}/comments")]
    [Authorize(Policy = GovFlowPolicies.RequireAnalyst)]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddComment(
        Guid id,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        var commentId = await _sender.Send(request.ToCommand(id), cancellationToken);
        return Created($"/api/v1/processes/{id}/comments/{commentId}", new CreatedResponse(commentId));
    }

    [HttpGet("{id:guid}/documents")]
    [ProducesResponseType(typeof(IReadOnlyList<ProcessDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ProcessDocumentDto>>> GetDocuments(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProcessDocumentsQuery(id), cancellationToken));

    [HttpPost("{id:guid}/documents")]
    [Authorize(Policy = GovFlowPolicies.RequireAnalyst)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadDocument(
        Guid id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ProblemDetails { Title = "A non-empty file is required.", Status = StatusCodes.Status400BadRequest });

        await using var stream = file.OpenReadStream();
        var command = new AttachProcessDocumentCommand(id, file.FileName, file.ContentType, file.Length, stream);
        var documentId = await _sender.Send(command, cancellationToken);

        return Created($"/api/v1/processes/{id}/documents/{documentId}", new CreatedResponse(documentId));
    }

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
