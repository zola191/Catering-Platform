using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Catering.Platform.API.Extensions;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = $"Catering API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = description.IsDeprecated
                        ? "This API version is deprecated!"
                        : "Current stable API version"
                });
        }

        options.OperationFilter<DeprecatedOperationFilter>();
    }
}

public class DeprecatedOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var obsoleteAttribute = context.MethodInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false)
            .FirstOrDefault() as ObsoleteAttribute;

        if (obsoleteAttribute != null)
        {
            operation.Deprecated = true;
            operation.Description = $"DEPRECATED: {obsoleteAttribute.Message}";
        }
    }
}
