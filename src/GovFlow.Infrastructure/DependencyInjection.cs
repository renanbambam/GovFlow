using GovFlow.Application.Common.Interfaces;
using GovFlow.Application.Common.Security;
using GovFlow.Domain.Common;
using GovFlow.Domain.Identity;
using GovFlow.Domain.Organization;
using GovFlow.Domain.Process;
using GovFlow.Infrastructure.Identity;
using GovFlow.Infrastructure.Persistence;
using GovFlow.Infrastructure.Persistence.ReadRepositories;
using GovFlow.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GovFlow.Infrastructure;

/// <summary>Registers the Infrastructure layer: the EF Core context and repositories.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<GovFlowDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<GovFlowDbContext>());

        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IProcessTypeRepository, ProcessTypeRepository>();
        services.AddScoped<IProcessInstanceRepository, ProcessInstanceRepository>();

        // Read side (CQRS queries → DTOs)
        services.AddScoped<IOrganizationReadRepository, OrganizationReadRepository>();
        services.AddScoped<IProcessTypeReadRepository, ProcessTypeReadRepository>();
        services.AddScoped<IProcessReadRepository, ProcessReadRepository>();
        services.AddScoped<IDashboardReadRepository, DashboardReadRepository>();

        // Identity / auth
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
        services.AddSingleton(jwtSettings);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
