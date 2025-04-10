using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Requests;

namespace Catering.Platform.Domain.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> AddAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
        Task<Guid> UpdateAsync(Category entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Category entity, CancellationToken cancellationToken = default);
    }
}
