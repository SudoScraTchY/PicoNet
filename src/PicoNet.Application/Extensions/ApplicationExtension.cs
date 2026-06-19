using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PicoNet.Contracts.DTOs.Requests;
using PicoNet.Contracts.DTOs.Requests.Shortener;
using PicoNet.Infrastructure.IServices;
using PicoNet.Infrastructure.Services;

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

        return services;
    }
}