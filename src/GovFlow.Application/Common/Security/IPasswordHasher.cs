namespace GovFlow.Application.Common.Security;

/// <summary>Hashes and verifies user passwords. Implemented with BCrypt in Infrastructure.</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);
}
