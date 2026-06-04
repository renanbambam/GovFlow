using GovFlow.Application.Common.Exceptions;
using GovFlow.Application.Common.Interfaces;
using GovFlow.Domain.Common;
using GovFlow.Domain.Process;
using MediatR;

namespace GovFlow.Application.Process.Commands.AddProcessComment;

public sealed class AddProcessCommentCommandHandler : IRequestHandler<AddProcessCommentCommand, Guid>
{
    private readonly IProcessInstanceRepository _instances;
    private readonly IProcessCommentRepository _comments;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public AddProcessCommentCommandHandler(
        IProcessInstanceRepository instances,
        IProcessCommentRepository comments,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _instances = instances;
        _comments = comments;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(AddProcessCommentCommand request, CancellationToken cancellationToken)
    {
        var authorId = _currentUser.UserId
            ?? throw new UnauthorizedException("An authenticated user is required to comment.");

        if (!await _instances.ExistsAsync(request.ProcessInstanceId, cancellationToken))
            throw NotFoundException.For("ProcessInstance", request.ProcessInstanceId);

        var comment = ProcessComment.Create(request.ProcessInstanceId, authorId, request.Content);

        await _comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}
