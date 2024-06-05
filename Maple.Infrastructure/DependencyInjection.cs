using Maple.Application.Common.Interfaces;
using Maple.Application.Common.Interfaces.Persistence;
using Maple.Infrastructure.Persistence;
using Maple.Infrastructure.Simulator;
using Microsoft.Extensions.DependencyInjection;

namespace Maple.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISimulatorRepository, SimulatorRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        return services;
    }
}