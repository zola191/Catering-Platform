
using Catering.Platform.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.API.Middlewares
{
    public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> _logger)
        : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case TenantNotFoundException tenantNotFoundException:
                        _logger.LogError("Attempt to delete a non-existent tenant");
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync(tenantNotFoundException.Message);
                        break;
                    case TenantAlreadyBlockException tenantAlreadyBlockException:
                        _logger.LogError("Tenant already blocked.");
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync(tenantAlreadyBlockException.Message);
                        break;
                    case TenantInactiveException tenantInactiveException:
                        _logger.LogError(tenantInactiveException.Message);
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync(tenantInactiveException.Message);
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        _logger.LogError("Something went wrong");
                        await context.Response.WriteAsync("Something went wrong");
                        break;
                }
            }
        }
    }
}
