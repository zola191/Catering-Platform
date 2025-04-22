using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Pipelines;
using Catering.Platform.Applications.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Catering.Platform.Applications.Extensions;

public static class ApplicationExtensions
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IDishService, DishService>();
        services.AddScoped<ITenantService, TenantService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}