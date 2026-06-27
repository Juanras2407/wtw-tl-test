using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RequestPlatform.Application.Services;

namespace RequestPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRequestService, RequestService>();
        services.AddValidatorsFromAssemblyContaining<Validators.CreateRequestValidator>();
        return services;
    }
}
