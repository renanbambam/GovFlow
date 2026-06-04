using GovFlow.Application.Process.Commands.AddProcessComment;

namespace GovFlow.API.Contracts.Processes;

public sealed record AddCommentRequest(string Content)
{
    public AddProcessCommentCommand ToCommand(Guid processInstanceId) => new(processInstanceId, Content);
}
