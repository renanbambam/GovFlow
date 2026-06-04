using GovFlow.Application.Common.Interfaces;

namespace GovFlow.Infrastructure.Storage;

internal sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(string basePath) => _basePath = basePath;

    public async Task<StoredFile> SaveAsync(
        string folder,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        var safeFolder = Path.GetFileName(folder);
        var directory = Path.Combine(_basePath, safeFolder);
        Directory.CreateDirectory(directory);

        var extension = Path.GetExtension(fileName);
        var key = $"{Guid.NewGuid():N}{extension}";
        var absolutePath = Path.Combine(directory, key);

        await using (var target = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write))
        {
            await content.CopyToAsync(target, cancellationToken);
        }

        var size = new FileInfo(absolutePath).Length;

        return new StoredFile($"{safeFolder}/{key}", size);
    }
}
