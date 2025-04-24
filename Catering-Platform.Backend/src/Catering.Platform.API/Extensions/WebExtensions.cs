using Catering.Platform.API.Contracts;
using Catering.Platform.API.Mapping;
using Catering.Platform.API.Middlewares;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Models;
using FluentValidation;
using System.Reflection;

namespace Catering.Platform.API.Extensions;

public static class WebExtensions
{
    public static void AddWeb(
        this IServiceCollection services)
    {
        services.AddScoped<IMapper<Category, CategoryViewModel>, CategoryMapper>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient<ExceptionHandlerMiddleware>();
    }
}