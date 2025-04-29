using System.Security.Cryptography;
using System.Text;

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

                if (context.Response.StatusCode == StatusCodes.Status200OK && context.Response.Body.Length > 0)
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var eTag = GenerateETag(responseBody);

                    if (context.Request.Headers.ContainsKey("If-None-Match"))
                    {
                        var clientETag = context.Request.Headers["If-None-Match"].ToString();

                        if (clientETag == eTag)
                        {
                            context.Response.StatusCode = StatusCodes.Status304NotModified;
                            context.Response.Body = originalBody;
                            return;
                        }
                    }

                    context.Response.Headers.Append("ETag", eTag);

                    await memoryStream.CopyToAsync(originalBody);
                }
                else
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(originalBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error in ETagMiddleware:");
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBody);
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }

    private string GenerateETag(string content)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}