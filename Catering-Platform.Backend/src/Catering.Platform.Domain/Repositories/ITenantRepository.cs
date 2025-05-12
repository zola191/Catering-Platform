using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant> BlockAsync(Guid tenantId, string blockReason);
    }
}
