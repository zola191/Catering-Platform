namespace Catering.Platform.Domain.Repositories;

public interface IUnidOfWork
{
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
