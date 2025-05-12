using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests
{
    public record CreateTenantRequest
    {
        public string Name { get; set; }
        public static Tenant MapToDomain(CreateTenantRequest request)
        {
            return new Tenant()
            {
                Name = request.Name,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}
