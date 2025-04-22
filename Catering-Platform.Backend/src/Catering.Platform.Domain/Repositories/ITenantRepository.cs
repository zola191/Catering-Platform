using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories
{
    public interface ITenantRepository
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
    }
}
