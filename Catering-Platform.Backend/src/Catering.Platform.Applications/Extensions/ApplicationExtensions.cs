using Catering.Platform.Applications.Services;
using Catering.Platform.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Catering.Platform.Applications.Extensions;

public static class ApplicationExtensions
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IDishService, DishService>();
    }
}