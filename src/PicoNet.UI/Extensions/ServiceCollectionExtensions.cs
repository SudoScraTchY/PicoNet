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
        var apiUrl =
            configuration["Api:BaseUrl"]
            ?? "https+http://api";
        
        services.AddTransient<AuthHeaderHandler>();
        
        services.AddHttpClient<IUrlApiClient, UrlApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IRedirectClient, RedirectClient>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();      
        
        return services;
    }
}