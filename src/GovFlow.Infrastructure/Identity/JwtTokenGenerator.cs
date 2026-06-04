using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GovFlow.Application.Common.Security;
using GovFlow.Domain.Identity;
using Microsoft.IdentityModel.Tokens;

namespace GovFlow.Infrastructure.Identity;

internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(JwtSettings settings) => _settings = settings;

    public AuthTokens GenerateTokens(User user)
    {
        var now = DateTime.UtcNow;
        var accessExpires = now.AddMinutes(_settings.AccessTokenExpiryMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey.PadRight(32, '0')));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("org", user.OrganizationId.ToString())
        };
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: accessExpires,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshExpires = now.AddDays(_settings.RefreshTokenExpiryDays);

        return new AuthTokens(accessToken, accessExpires, GenerateRefreshToken(), refreshExpires);
    }

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
