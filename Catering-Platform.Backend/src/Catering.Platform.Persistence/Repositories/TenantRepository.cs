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
    }
}
