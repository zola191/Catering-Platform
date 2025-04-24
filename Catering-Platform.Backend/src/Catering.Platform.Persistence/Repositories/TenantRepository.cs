using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catering.Platform.Persistence.Repositories
{
    internal class TenantRepository : Repository<Tenant>, ITenantRepository
    {
        public TenantRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<Tenant> BlockAsync(Guid id, string blockReason)
        {
            var tenant = await DbContext.Set<Tenant>().FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (tenant == null)
                throw new TenantNotFoundException();

            if (tenant.IsActive == false)
                throw new TenantAlreadyBlockException();

            tenant.IsActive = false;
            tenant.BlockReason = blockReason;
            tenant.UpdatedAt = DateTime.UtcNow;

            await DbContext.SaveChangesAsync();
            return tenant;
        }

        public async Task<Tenant> UnBlockAsync(Guid id)
        {
            var tenant = await DbContext.Set<Tenant>().FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (tenant == null)
                throw new TenantNotFoundException();

            if (tenant.IsActive == true)
                throw new TenantHasActiveDataException();

            tenant.IsActive = true;
            tenant.BlockReason = string.Empty;
            tenant.UpdatedAt = DateTime.UtcNow;

            await DbContext.SaveChangesAsync();
            return tenant;
        }
    }
}
