using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Tenant;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Catering.Platform.Applications.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<ITenantService> _logger;

        public TenantService(
            ITenantRepository repository,
            IUnitOfWork unitOfWork,
            IDistributedCache distributedCache,
            ILogger<ITenantService> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<Guid> AddAsync(CreateTenantRequest request)
        {
            try
            {
                var tenant = CreateTenantRequest.MapToDomain(request);
                var result = await _repository.AddAsync(tenant);
                await _unitOfWork.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Unable to save tenant {Name}. See Details: {Details}",
                    request.Name,
                    ex.Message);
                throw;
            }
        }

        public async Task<TenantViewModel> BlockTenantAsync(Guid id, BlockTenantRequest request)
        {
            try
            {
                var existingTenant = await _repository.GetByIdAsync(id);
                if (existingTenant == null)
                    throw new TenantNotFoundException();

                if (existingTenant.IsActive == true)
                    throw new TenantHasActiveDataException();

                var result = await _repository.BlockAsync(existingTenant, request.Reason);
                return TenantViewModel.MapToViewModel(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking tenant {TenantId}", id);
                throw;
            }
        }

        public async Task<TenantViewModel> UnblockTenantAsync(Guid id)
        {
            try
            {
                var existingTenant = await _repository.GetByIdAsync(id);
                if (existingTenant == null)
                    throw new TenantNotFoundException();

                if (existingTenant.IsActive == true)
                    throw new TenantHasActiveDataException();

                await _repository.UnBlockAsync(existingTenant);

                return TenantViewModel.MapToViewModel(existingTenant);
            }
            catch (TenantNotFoundException ex) 
            {
                _logger.LogWarning(ex, "Attempt to unblock non-existent tenant: {TenantId}", id);
                throw;
            }
            catch (TenantHasActiveDataException ex)
            {
                _logger.LogWarning(ex, "Attempt to unblock a tenant that is already unblocked: {TenantId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Unable to unblock tenant. See Details: {Details}", ex.Message);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            //TODO обработать случай - Нельзя удалить tenant'а, если он связан с активными заказами
            try
            {
                var existingTenant = await _repository.GetByIdAsync(id);
                if (existingTenant == null)
                {
                    throw new TenantNotFoundException();
                }
                _repository.Delete(existingTenant);
                await _unitOfWork.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(
                    "Unable to delete tenant. See Details: {Details}", ex.Message);
                throw;
            }
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

        public async Task<TenantViewModel?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"tenant_{id}";
            var json = await _distributedCache.GetStringAsync(cacheKey);

            if (string.IsNullOrWhiteSpace(json) is false)
            {
                return JsonSerializer.Deserialize<TenantViewModel>(json);
            }

            try
            {
                var existingTenant = await _repository.GetByIdAsync(id);
                if (existingTenant == null)
                {
                    return null;
                }
                var tempViewModel = TenantViewModel.MapToViewModel(existingTenant);
                var tempJson = JsonSerializer.Serialize(tempViewModel);
                _distributedCache.SetString(cacheKey, tempJson);
                // TODO добавить DistributedCacheEntryOptions и потестить AbsoluteExpirationRelativeToNow
                Task.Delay(1000).Wait();
                return tempViewModel;
            }

            catch (Exception ex)
            {
                _logger.LogError(
                    "Unable to fetch tenant by id {Id}. See Details: {Details}", id, ex.Message);
                throw;
            }
        }

        public async Task<Guid> UpdateAsync(Guid id, UpdateTenantRequest request)
        {
            try
            {
                var existingTenant = await _repository.GetByIdAsync(id);
                if (existingTenant == null)
                {
                    throw new TenantNotFoundException();
                }
                UpdateTenantRequest.MapToDomain(existingTenant, request);

                var result = _repository.Update(existingTenant);
                await _unitOfWork.SaveChangesAsync();
                return result;
            }

            catch (TenantNotFoundException ex)
            {
                _logger.LogError(
                    "Tenant is not found {Id}. See Details: {Details}", id, ex.Message);
                throw;
            }

            catch (Exception ex)
            {
                _logger.LogError(
                    "Unable to update tenant {Name}. See Details: {Details}",
                    request.Name,
                    ex.Message);
                throw;
            }
        }

    }
}
