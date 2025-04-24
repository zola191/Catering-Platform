namespace Catering.Platform.Domain.Requests.Tenant;

public record CreateTenantRequest
{
    public string Name { get; set; }
    public static Models.Tenant MapToDomain(CreateTenantRequest request)
    {
        return new Models.Tenant()
        {
            Name = request.Name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
