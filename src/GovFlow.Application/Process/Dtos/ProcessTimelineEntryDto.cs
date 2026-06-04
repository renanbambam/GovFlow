namespace GovFlow.Application.Process.Dtos;

public sealed record ProcessTimelineEntryDto(
    Guid Id,
    int Sequence,
    string EventType,
    string Description,
    Guid? StepId,
    DateTime OccurredAt);
