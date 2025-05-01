using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Requests;

namespace Catering.Platform.Applications.Abstractions
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantViewModel>> GetAllAsync();
        Task<TenantViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> AddAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
    }
}
