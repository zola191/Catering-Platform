using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant> BlockAsync(Tenant tenant, string blockReason);
        Task<Tenant> UnBlockAsync(Tenant tenant);
        Task<Tenant?> GetByIdWithAddresses(Guid tenantId);
    }
}
