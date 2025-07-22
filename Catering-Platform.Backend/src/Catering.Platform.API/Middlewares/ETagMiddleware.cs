using System.Security.Cryptography;

public class ETagMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ETagMiddleware> _logger;
    public ETagMiddleware(RequestDelegate next, ILogger<ETagMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var originalBody = context.Response.Body;

        using (var memoryStream = new MemoryStream())
        {
            try
            {
                context.Response.Body = memoryStream;
                await _next(context);

                if (!ShouldProcessResponse(context))
                {
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(originalBody);
                    return;
                }

                var bodyBytes = memoryStream.ToArray();

                if (context.Request.Headers.TryGetValue("If-None-Match", out var clientETag))
                {
                    var eTag = GenerateETag(bodyBytes);

                    if (string.Equals(clientETag, eTag, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.StatusCode = StatusCodes.Status304NotModified;
                        context.Response.ContentLength = 0;
                        context.Response.Body = originalBody;
                        return;
                    }
                }

                var finalETag = GenerateETag(bodyBytes);
                context.Response.Headers.Append("ETag", finalETag);

                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBody);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error in ETagMiddleware");
                memoryStream.Position = 0;
                await memoryStream.CopyToAsync(originalBody);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }

    private string GenerateETag(byte[] content)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(content);
            return '"' + BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant() + '"';
        }
    }

    private static bool ShouldProcessResponse(HttpContext context)
    {
        if (context.Response.StatusCode != StatusCodes.Status200OK)
            return false;

        if (context.Response.Body?.Length <= 0)
            return false;

        if (!context.Response.Headers.TryGetValue("Content-Type", out var contentTypeValues))
            return false;

        var contentType = contentTypeValues.FirstOrDefault();
        if (string.IsNullOrEmpty(contentType))
            return false;

        return true;
    }
}