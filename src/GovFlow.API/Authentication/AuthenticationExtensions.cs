using System.Text;
using GovFlow.Application.Common.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GovFlow.API.Authentication;

/// <summary>
/// Wires JWT bearer authentication and the RBAC authorization policies. The validation
/// pipeline is fully configured so that protecting an endpoint later is a one-attribute
/// change; no endpoint requires a token yet.
/// </summary>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddGovFlowAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // JwtSettings is registered as a singleton by the Infrastructure layer; here we only
        // need the values to configure token validation.
        var settings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

        // Guarantee a key of sufficient length even with placeholder configuration.
        var key = Encoding.UTF8.GetBytes(settings.SecretKey.PadRight(32, '0'));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(GovFlowPolicies.RequireAdmin, policy => policy.RequireRole("Admin"));
            options.AddPolicy(GovFlowPolicies.RequireManager, policy => policy.RequireRole("Manager", "Admin"));
            options.AddPolicy(GovFlowPolicies.RequireAnalyst, policy => policy.RequireRole("Analyst", "Manager", "Admin"));
        });

        return services;
    }
}
