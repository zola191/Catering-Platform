using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catering.Platform.Persistence.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TenantRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _dbContext.Set<Tenant>().ToListAsync();
        }
    }
}
