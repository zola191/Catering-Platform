using Catering.Platform.Applications.ViewModels;

namespace Catering.Platform.Applications.Abstractions
{
    public interface ITenantService
    {
        Task<List<TenantViewModel>> GetAllAsync();
    }
}
