using System.Reflection;
using Microsoft.OpenApi.Models;

namespace GovFlow.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddGovFlowSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GovFlow API",
                Version = "v1",
                Description = "Enterprise process and workflow management platform. "
                            + "Phase 1 endpoints: organizations, departments, process types and process instances."
            });

            var bearer = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste a JWT access token (without the 'Bearer ' prefix).",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };
            options.AddSecurityDefinition("Bearer", bearer);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { [bearer] = Array.Empty<string>() });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        });

        return services;
    }
}
