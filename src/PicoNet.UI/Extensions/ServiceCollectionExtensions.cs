using PicoNet.UI.ApiClients.Implementations;
using PicoNet.UI.ApiClients.Interfaces;

namespace PicoNet.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IUrlApiClient, UrlApiClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        });

        services.AddHttpClient<IRedirectClient, RedirectClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        });

        return services;
    }
}