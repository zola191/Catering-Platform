using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catering.Platform.Persistence.Repositories;

public class AddressRepository(ApplicationDbContext dbContext) : IAddressRepository
{
    public async Task AddAsync(Address entity)
    {
        await dbContext.AddAsync(entity);
    }

    public void Delete(Address address)
    {
        dbContext.Addresses.Remove(address);
    }

    public async Task<Address?> GetByIdAsync(Guid addressId)
    {
        return await dbContext.Addresses.FirstOrDefaultAsync(f => f.Id == addressId);
    }

    public async Task<IEnumerable<Address>> SearchByTextAsync(Guid? tenantId, string query)
    {
        var baseQuery = dbContext.Addresses
            .Where(f => f.SearchVector.Matches(EF.Functions.PhraseToTsQuery("russian", query)))
            .Select(f => new
            {
                Address = f,
                Rank = f.SearchVector.Rank(EF.Functions.PhraseToTsQuery("russian", query)),
            })
            .OrderByDescending(x => x.Rank)
            .Select(x => x.Address)
            .AsNoTracking();

        if (tenantId == null)
        {
            return await baseQuery.ToListAsync();
        }

        return await baseQuery
            .Where(a => a.TenantId == tenantId)
            .ToListAsync();
    }

    public void Update(Address address)
    {
        dbContext.Update(address);
    }
}
