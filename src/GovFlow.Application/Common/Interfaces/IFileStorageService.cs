namespace GovFlow.Application.Common.Interfaces;

public sealed record StoredFile(string StoragePath, long SizeBytes);

public interface IFileStorageService
{
    Task<StoredFile> SaveAsync(
        string folder,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default);
}
