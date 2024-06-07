using System.Text;
using Maple.Application.Common.Interfaces;
using Maple.Infrastructure.DbContext;
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
        services.AddSupabase();
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

    private static IServiceCollection AddSupabase(this IServiceCollection services)
    {
        const string supabaseUrl = "https://timbkbpfurjfwtqnfrov.supabase.co";
        const string supabaseKey =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InRpbWJrYnBmdXJqZnd0cW5mcm92Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTU1MzQ5MzEsImV4cCI6MjAzMTExMDkzMX0.P_Nnd3zfLhwCdkYxNkMpMgdkRpAtjeTHp_berx1oBLI";

        services.AddSingleton(new SupabaseClient(supabaseUrl, supabaseKey));
        return services;
    }
}