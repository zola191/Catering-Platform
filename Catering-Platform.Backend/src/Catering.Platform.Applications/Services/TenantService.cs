using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Tenant;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ITenantService> _logger;

        public TenantService(ITenantRepository repository, IUnitOfWork unitOfWork, ILogger<ITenantService> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
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
                var result = await _repository.BlockAsync(id, request.Reason);
                return TenantViewModel.MapToViewModel(result);
            }
            catch (TenantNotFoundException ex)
            {
                _logger.LogWarning(ex, "Attempt to block non-existent tenant: {TenantId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking tenant {TenantId}", id);
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

            catch (TenantNotFoundException ex)
            {
                _logger.LogError(
                "Tenant is not found {Id}. See Details: {Details}", id, ex.Message);
                throw;
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
                var existingTenantViewmodels = existingTenants.Select(TenantViewModel.MapToViewModel).ToList();
                return existingTenantViewmodels;
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
            try
            {
                var existingTenant = await _repository.GetByIdAsync(id);
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
