namespace Catering.Platform.Domain.Requests.Tenant;

public class UpdateTenantRequest
{
    public string Name { get; set; }
    public static Models.Tenant MapToDomain(CreateTenantRequest request)
    {
        return new Models.Tenant()
        {
            Name = request.Name,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow,
        };
    }
}
