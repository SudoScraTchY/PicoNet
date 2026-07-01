using PicoNet.UI.ApiClients.Interfaces;
using PicoNet.UI.Services;

namespace PicoNet.UI.Extensions;

/// <summary>
/// Extension methods for configuring admin services
/// </summary>
public static class AdminServiceExtensions
{
    /// <summary>
    /// Add admin API clients and authorization services
    /// </summary>
    public static IServiceCollection AddAdminServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiUrl = configuration["Api:BaseUrl"] ?? "https+http://api";

        services.AddHttpClient<IAdminApiClient, PicoNet.UI.ApiClients.Implementations.AdminApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddScoped<IAdminAuthorizationService, AdminAuthorizationService>();

        return services;
    }
}
