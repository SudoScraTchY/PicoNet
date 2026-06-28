using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.IServices;
using PicoNet.Infrastructure.Services;

namespace PicoNet.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var dbConnectionString = 
            configuration.GetConnectionString("piconet") ??
                                 configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<PicoNetDbContext>(options =>
        {
            options.UseNpgsql(
                dbConnectionString,
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
        
        services.AddOpenIddictConfig();
        
        services.AddSingleton<IShortCodeGenerator>(sp =>
        {
            var salt = configuration["ShortCode:Salt"] ?? "PicoNet-Default-Salt";
            return new ShortCodeGenerator(salt);
        });
        
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");
        
        services.AddScoped<IEmailService, ResendEmailService>();
        
        return services;
    }
}