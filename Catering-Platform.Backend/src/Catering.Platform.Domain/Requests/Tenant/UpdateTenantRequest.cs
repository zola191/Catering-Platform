namespace Catering.Platform.Domain.Requests.Tenant;

public class UpdateTenantRequest
{
    public string Name { get; set; }
    public static void MapToDomain(Models.Tenant entity, UpdateTenantRequest request)
    {
        entity.Name = request.Name;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
