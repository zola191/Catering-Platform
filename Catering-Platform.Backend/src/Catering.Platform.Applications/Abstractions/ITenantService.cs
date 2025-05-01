using Catering.Platform.Applications.ViewModels;

namespace Catering.Platform.Applications.Abstractions
{
    public interface ITenantService
    {
        Task<IEnumerable<TenantViewModel>> GetAllAsync();
        Task<TenantViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
