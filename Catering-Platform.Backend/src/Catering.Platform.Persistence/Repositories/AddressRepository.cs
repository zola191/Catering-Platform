using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catering.Platform.Persistence.Repositories
{
    public class AddressRepository(ApplicationDbContext dbContext) : IAddressRepository
    {
        public async Task AddAsync(Address entity)
        {
            var existingTenant = await dbContext.Tenants.FirstOrDefaultAsync(f => f.Id == entity.TenantId);

            if (existingTenant == null)
                throw new TenantNotFoundException();

            if (!existingTenant.IsActive)
                throw new TenantInactiveException();

            var result = await dbContext.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}
