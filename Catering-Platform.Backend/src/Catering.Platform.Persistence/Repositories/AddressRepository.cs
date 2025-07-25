﻿using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;

namespace Catering.Platform.Persistence.Repositories;

public class AddressRepository(ApplicationDbContext dbContext) : IAddressRepository
{
    public async Task AddAsync(Address entity)
    {
        await dbContext.AddAsync(entity);
    }
}
