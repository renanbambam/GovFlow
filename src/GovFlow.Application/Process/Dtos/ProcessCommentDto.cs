namespace GovFlow.Application.Process.Dtos;

public sealed record ProcessCommentDto(
    Guid Id,
    Guid ProcessInstanceId,
    Guid AuthorId,
    string Content,
    DateTime CreatedAt);
