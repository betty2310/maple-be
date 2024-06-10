using Maple.API.Common.Mapping;
using Maple.Application.Common.Interfaces;
using Maple.Infrastructure.Persistence;
using Maple.Infrastructure.Simulator;
using Newtonsoft.Json;

namespace Maple.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings();
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling =
                ReferenceLoopHandling.Ignore; // This may be needed for your relationships.
        });
        return services;
    }
}