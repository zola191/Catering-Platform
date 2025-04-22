using Catering.Platform.API.Contracts;
using Catering.Platform.API.Mapping;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Models;

namespace Catering.Platform.API.Extensions;

public static class WebExtensions
{
    public static void AddWeb(
        this IServiceCollection services)
    {
        services.AddScoped<IMapper<Category, CategoryViewModel>, CategoryMapper>();
    }
}