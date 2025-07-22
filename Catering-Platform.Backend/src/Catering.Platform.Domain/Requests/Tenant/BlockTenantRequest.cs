namespace Catering.Platform.Domain.Requests.Tenant;

public record BlockTenantRequest
{
    public string Reason { get; init; }
}
