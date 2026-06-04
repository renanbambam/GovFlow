using MediatR;

namespace GovFlow.Application.Process.Commands.AddProcessComment;

public sealed record AddProcessCommentCommand(Guid ProcessInstanceId, string Content) : IRequest<Guid>;
