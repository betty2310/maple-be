using System.Text;
using Maple.Application.Common.Interfaces;
using Maple.Application.Common.Interfaces.Persistence;
using Maple.Infrastructure.Persistence;
using Maple.Infrastructure.Simulator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Maple.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddAuth();
        services.AddSingleton<ISimulatorRepository, SimulatorRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        const string supabaseJwtSecret =
            "dUk0UwQ+xdCeUBHkI9qP2sjsltqIJ+W8MVMJ/Vn7QC1THC/HX/p+9tOqaxMmSbBNgidGfPQ1H8JdzuhppIQ8zg==";
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJwtSecret))
                };
            });
        return services;
    }
}