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
                Description = "Enterprise process and workflow management platform built with Clean Architecture, "
                            + "DDD and CQRS. Features: JWT auth with refresh tokens and role-based policies "
                            + "(Admin/Manager/Analyst), organizations & departments, configurable process types, "
                            + "running process instances with a full state machine, a per-process timeline (audit "
                            + "history), comments, PDF document attachments, real-time updates over SignalR "
                            + "(hub at /hubs/processes) and SLA monitoring via a recurring Hangfire job."
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
