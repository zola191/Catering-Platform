using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catering.Platform.Persistence.Repositories;

internal abstract class Repository<T> : IRepository<T>
where T : Entity, new()
{
    protected ApplicationDbContext DbContext { get; set; }
    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<Guid> AddAsync(T entity, CancellationToken cancellationToken)
    {
        var result = await DbContext.Set<T>().AddAsync(entity, cancellationToken);
        return result.Entity.Id;
    }

    public void Delete(T entity)
    {
        DbContext.Set<T>().Remove(entity);
    }

    public Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        //отложенность переносим на более высокий уровень, решение для оптимизации расходования памяти
        return DbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return DbContext.Set<T>().FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public Guid Update(T entity)
    {
        var result = DbContext.Set<T>().Update(entity);
        return result.Entity.Id;
    }
}
