using PicoNet.UI.ApiClients.Implementations;
using PicoNet.UI.ApiClients.Interfaces;
using PicoNet.UI.Services;

namespace PicoNet.UI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<AuthHeaderHandler>();
        
        services.AddHttpClient<IUrlApiClient, UrlApiClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IRedirectClient, RedirectClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        }).AddHttpMessageHandler<AuthHeaderHandler>();
        
        services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        }).AddHttpMessageHandler<AuthHeaderHandler>();      
        
        return services;
    }
}