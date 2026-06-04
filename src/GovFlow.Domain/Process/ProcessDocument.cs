using GovFlow.Domain.Common;

namespace GovFlow.Domain.Process;

public sealed class ProcessDocument : Entity
{
    public Guid ProcessInstanceId { get; private set; }

    public Guid UploadedByUserId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long SizeBytes { get; private set; }

    public string StoragePath { get; private set; } = string.Empty;

    public DateTime UploadedAt { get; private set; }

    private ProcessDocument()
    {
    }

    public static ProcessDocument Create(
        Guid processInstanceId,
        Guid uploadedByUserId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath)
    {
        if (processInstanceId == Guid.Empty)
            throw new ArgumentException("Process instance id is required.", nameof(processInstanceId));
        if (uploadedByUserId == Guid.Empty)
            throw new ArgumentException("Uploader id is required.", nameof(uploadedByUserId));
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));
        if (sizeBytes <= 0)
            throw new ArgumentException("File size must be positive.", nameof(sizeBytes));
        if (string.IsNullOrWhiteSpace(storagePath))
            throw new ArgumentException("Storage path is required.", nameof(storagePath));

        return new ProcessDocument
        {
            ProcessInstanceId = processInstanceId,
            UploadedByUserId = uploadedByUserId,
            FileName = fileName.Trim(),
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StoragePath = storagePath,
            UploadedAt = DateTime.UtcNow
        };
    }
}
