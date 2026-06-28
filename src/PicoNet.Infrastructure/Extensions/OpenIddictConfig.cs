using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PicoNet.Infrastructure.Data;
using PicoNet.Infrastructure.Identity.Entities;
using PicoNet.Infrastructure.Identity.Implementations;
using PicoNet.Infrastructure.Identity.Interfaces;

namespace PicoNet.Infrastructure.Extensions;

public static class  OpenIddictConfig
{
    public static IServiceCollection AddOpenIddictConfig(this IServiceCollection services)
    {
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
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<PicoNetDbContext>();
        
        services.AddScoped<ITokenService, TokenService>();
        
        return services;
    }
}