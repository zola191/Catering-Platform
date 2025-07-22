using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
namespace Catering.Platform.Persistence.Repositories
{
    internal class TenantRepository : Repository<Tenant>, ITenantRepository
    {
        public TenantRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<Tenant> BlockAsync(Tenant tenant, string blockReason)
        {
            tenant.IsActive = false;
            tenant.BlockReason = blockReason;
            tenant.UpdatedAt = DateTime.UtcNow;

            await DbContext.SaveChangesAsync();
            return tenant;
        }

        public async Task<Tenant> UnBlockAsync(Tenant tenant)
        {
            tenant.IsActive = true;
            tenant.BlockReason = string.Empty;
            tenant.UpdatedAt = DateTime.UtcNow;
            await DbContext.SaveChangesAsync();
            return tenant;
        }
    }
}
