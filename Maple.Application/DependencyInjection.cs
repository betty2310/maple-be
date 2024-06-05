using Maple.Application.Services.Simulator;
using Maple.Application.Services.Simulator.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Maple.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ISimulatorQueryService, SimulatorQueryService>();
        return services;
    }
}