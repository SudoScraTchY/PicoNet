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
        services.AddScoped<AuthHeaderHandler>();
        services.AddScoped<CurrentUserTokenAccessor>();
        
        services.AddHttpClient<IUrlApiClient, UrlApiClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        }).AddHttpMessageHandler<TokenForwardingHandler>();

        services.AddHttpClient<IRedirectClient, RedirectClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        }).AddHttpMessageHandler<TokenForwardingHandler>();
        
        services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://api");
        }).AddHttpMessageHandler<TokenForwardingHandler>();      
        
        return services;
    }
}