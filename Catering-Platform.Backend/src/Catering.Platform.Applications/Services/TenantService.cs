using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _repository;
        private readonly ILogger<ITenantService> _logger;

        public TenantService(ITenantRepository repository, ILogger<ITenantService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<TenantViewModel>> GetAllAsync()
        {
            try
            {
                var existingTenants = await _repository.GetAllAsync();
                var existingTenantViewModels = existingTenants.Select(TenantViewModel.MapToViewModel).ToList();
                return existingTenantViewModels;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Unable to fetch all Tenants. See Details: {Details}", ex.Message);
                throw;
            }
        }

        public async Task<TenantViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingTenant = await _repository.GetByIdAsync(id, cancellationToken);
                if (existingTenant == null)
                {
                    return null;
                }
                return TenantViewModel.MapToViewModel(existingTenant);
            }

            catch (Exception ex)
            {
                _logger.LogError(
                    "Unable to fetch tenant by id {Id}. See Details: {Details}", id, ex.Message);
                throw;
            }
        }
    }
}
