using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Common.Security;
using GovFlow.Domain.Identity;

namespace GovFlow.Application.Tests.Fakes;

internal sealed class FakeCurrentUserService : ICurrentUserService
{
    public FakeCurrentUserService(Guid? userId = null) => UserId = userId;

    public Guid? UserId { get; set; }

    public string? Email { get; set; } = "tester@govflow.local";

    public bool IsAuthenticated => UserId is not null;
}

internal sealed class FakeFileStorageService : IFileStorageService
{
    public List<(string Folder, string FileName, long Size)> Saved { get; } = new();

    public async Task<StoredFile> SaveAsync(string folder, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, cancellationToken);
        var size = ms.Length;
        Saved.Add((folder, fileName, size));
        return new StoredFile($"{folder}/{Guid.NewGuid():N}{Path.GetExtension(fileName)}", size);
    }
}

internal sealed class FakeUserRepository : IUserRepository
{
    public List<User> Items { get; } = new();

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(u => u.Id == id));

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(u => u.Email == email));

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.Any(u => u.Email == email));

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        Items.Add(user);
        return Task.CompletedTask;
    }
}

internal sealed class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    public List<RefreshToken> Items { get; } = new();

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        => Task.FromResult(Items.FirstOrDefault(t => t.Token == token));

    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        Items.Add(refreshToken);
        return Task.CompletedTask;
    }
}

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string password, string passwordHash) => passwordHash == $"hashed:{password}";
}

internal sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public AuthTokens GenerateTokens(User user)
        => new(
            $"access-{user.Id}",
            DateTime.UtcNow.AddMinutes(15),
            $"refresh-{Guid.NewGuid()}",
            DateTime.UtcNow.AddDays(7));
}
