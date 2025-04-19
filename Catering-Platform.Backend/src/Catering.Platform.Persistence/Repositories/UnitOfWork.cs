using Catering.Platform.Domain.Repositories;

namespace Catering.Platform.Persistence.Repositories
{
    internal sealed class UnitOfWork(ApplicationDbContext dbContext) : IUnitOfWork
    {
        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
