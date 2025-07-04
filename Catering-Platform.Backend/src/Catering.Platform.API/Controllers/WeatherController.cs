using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/weather")]
public class WeatherController : ControllerBase
{
    [HttpGet("old-forecast")]
    [MapToApiVersion("1.0")]
    [Obsolete("This endpoint is deprecated and will be removed in v3.0. Please use '/new-forecast' instead.")]
    public IActionResult GetOldForecast()
    {
        return Ok(new
        {
            Version = "1.0 (Deprecated)",
            Forecast = "Sunny",
            Warning = "This endpoint will be removed in v3.0",
            MigrationGuide = "Use /new-forecast instead"
        });
    }

    [HttpGet("new-forecast")]
    [MapToApiVersion("1.0")]
    [MapToApiVersion("2.0")]
    public IActionResult GetNewForecast()
    {
        var version = HttpContext.GetRequestedApiVersion();
        return Ok(new
        {
            Version = version?.ToString(),
            Forecast = new
            {
                Temperature = 25,
                Humidity = 60,
                Units = version?.MajorVersion >= 2 ? "metric" : "imperial"
            },
            Documentation = "https://api.docs/weather-forecast"
        });
    }
}
