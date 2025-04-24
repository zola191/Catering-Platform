using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories
{
    public interface ITenantRepository : IRepository<Tenant>
    {
        Task<Tenant> BlockAsync(Guid id, string blockReason);
        Task<Tenant> UnBlockAsync(Guid id);
    }
}
