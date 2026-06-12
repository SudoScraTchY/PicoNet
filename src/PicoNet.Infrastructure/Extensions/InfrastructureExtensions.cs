using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PicoNet.Infrastructure.Data;

namespace PicoNet.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<PicoNetDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("piconet"), //DefaultConnection for local instance
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(3);
                    npgsqlOptions.CommandTimeout(30);
                    npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
                });
            
            // Enable sensitive data logging only in development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
            }
        });
        
        // OpenIddict stores will also use the same context
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<PicoNetDbContext>();
            });
        // ... other OpenIddict configuration
        
        // Health checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");
        
        return services;
    }
}