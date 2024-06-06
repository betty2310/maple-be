using Maple.API.Common.Mapping;
using Maple.Application.Common.Interfaces;
using Maple.Application.Common.Interfaces.Persistence;
using Maple.Infrastructure.Persistence;
using Maple.Infrastructure.Simulator;

namespace Maple.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings();
        return services;
    }
}