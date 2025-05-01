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
    }
}
