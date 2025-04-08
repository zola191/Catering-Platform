using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Services;

public interface IDishService
{
    Task<List<Dish>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Dish?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(Dish entity, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(Dish entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Dish entity, CancellationToken cancellationToken = default);
}