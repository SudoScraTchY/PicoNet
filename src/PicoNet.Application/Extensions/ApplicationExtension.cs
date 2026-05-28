using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Domain.IServices;
using PicoNet.Domain.Service;

namespace PicoNet.Application.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database

        services.AddValidatorsFromAssemblyContaining<CreateShortUrlRequest>();
        // Health checks
        
        // Services
        services.AddSingleton<IShortCodeGenerator>(sp =>
        {
            var salt = configuration["ShortCode:Salt"] ?? "PicoNet-Default-Salt";
            return new ShortCodeGenerator(salt);
        });

        return services;
    }
}