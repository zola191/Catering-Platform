using Catering.Platform.Applications.Models;
using Catering.Platform.Domain.Requests;

namespace Catering.Platform.Applications.Abstractions;

public interface IDishService
{
    Task<List<DishViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DishViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(CreateDishRequest request, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(Guid id, UpdateDishRequest request, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}