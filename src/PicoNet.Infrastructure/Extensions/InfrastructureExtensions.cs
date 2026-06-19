using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.Identity;
using PicoNet.Infrastructure.Identity.Implementations;
using PicoNet.Infrastructure.Identity.Interfaces;
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
                options.UseEntityFrameworkCore().UseDbContext<PicoNetDbContext>());
        
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false; // tune later
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<PicoNetDbContext>();
        
        services.AddScoped<ITokenService, TokenService>();
        
        // Services
        services.AddSingleton<IShortCodeGenerator>(sp =>
        {
            var salt = configuration["ShortCode:Salt"] ?? "PicoNet-Default-Salt";
            return new ShortCodeGenerator(salt);
        });
        
        // Health checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");
        
        return services;
    }
}