using Catering.Platform.Applications.Models;
using Catering.Platform.Domain.Requests;

namespace Catering.Platform.Applications.Abstractions;

public interface ICategoryService
{
    Task<List<CategoryViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> AddAsync(CreateCategoryCommand request, CancellationToken cancellationToken = default);
    Task<Guid> UpdateAsync(Guid id, UpdateCategoryCommand request, CancellationToken cancellationToken = default);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
