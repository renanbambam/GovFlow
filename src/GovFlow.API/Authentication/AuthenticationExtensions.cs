using System.Text;
using GovFlow.Application.Common.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GovFlow.API.Authentication;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddGovFlowAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
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
