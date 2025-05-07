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
        return await dbContext.Addresses.FirstOrDefaultAsync(f=>f.Id == addressId);
    }

    public void Update(Address address)
    {
        dbContext.Update(address);
    }
}
