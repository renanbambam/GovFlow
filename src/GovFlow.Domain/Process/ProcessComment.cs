using GovFlow.Domain.Common;

namespace GovFlow.Domain.Process;

public sealed class ProcessComment : Entity
{
    public Guid ProcessInstanceId { get; private set; }

    public Guid AuthorId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    private ProcessComment()
    {
    }

    public static ProcessComment Create(Guid processInstanceId, Guid authorId, string content)
    {
        if (processInstanceId == Guid.Empty)
            throw new ArgumentException("Process instance id is required.", nameof(processInstanceId));
        if (authorId == Guid.Empty)
            throw new ArgumentException("Author id is required.", nameof(authorId));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content is required.", nameof(content));

        return new ProcessComment
        {
            ProcessInstanceId = processInstanceId,
            AuthorId = authorId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
