namespace GovFlow.Application.Process.Dtos;

public sealed record ProcessDocumentDto(
    Guid Id,
    Guid ProcessInstanceId,
    Guid UploadedByUserId,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime UploadedAt);
