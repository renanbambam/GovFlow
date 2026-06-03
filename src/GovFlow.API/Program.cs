using System.Text.Json.Serialization;
using GovFlow.API.Authentication;
using GovFlow.API.Extensions;
using GovFlow.API.Middleware;
using GovFlow.Application;
using GovFlow.Infrastructure;
using GovFlow.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddGovFlowSwagger();
builder.Services.AddGovFlowAuthentication(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<GovFlowDbContext>("database", tags: new[] { "ready" });

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "GovFlow API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Liveness: process is up. Readiness: dependencies (database) reachable.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });
app.MapHealthChecks("/health");

app.Run();

// Exposed so the integration test project can use WebApplicationFactory<Program>.
public partial class Program;
