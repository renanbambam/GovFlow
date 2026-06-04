namespace GovFlow.Application.Common.Security;

public sealed class JwtSettings
{
    public string Issuer { get; init; } = "govflow";
    public string Audience { get; init; } = "govflow-clients";
    public string SecretKey { get; init; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; init; } = 15;
    public int RefreshTokenExpiryDays { get; init; } = 7;
}
