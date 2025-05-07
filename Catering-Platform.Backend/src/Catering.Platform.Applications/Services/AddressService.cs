using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Adress;
using Catering.Platform.Domain.Requests.Tenant;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _adressRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IAddressService> _logger;

    public AddressService(
        IAddressRepository adressRepository,
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork,
        ILogger<IAddressService> logger)
    {
        _adressRepository = adressRepository;
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AddressViewModel> CreateAddressAsync(CreateAddressViewModel request, Guid tenantId)
    {
        try
        {
            var existingTenant = await _tenantRepository.GetByIdAsync(tenantId);

            if (existingTenant == null)
                throw new TenantNotFoundException();

            if (!existingTenant.IsActive)
                throw new TenantInactiveException();

            var entity = CreateAddressViewModel.MapToDomain(request, tenantId);

            await _adressRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = AddressViewModel.MapToViewModel(entity);
            return result;
        }
        catch (TenantNotFoundException ex)
        // сделать generic exception для NotFoundException т.к. может быть и в других сервисах
        {
            _logger.LogError("Tenant not found. TenantId: {TenantId}", tenantId);
            throw;
        }
        catch (TenantInactiveException ex)
        {
            _logger.LogError("Attempt to use inactive tenant. TenantId: {TenantId}", tenantId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating address. TenantId: {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<AddressViewModel> UpdateAddressAsync(Guid addressId, UpdateAddressViewModel request, Guid tenantId)
    {
        try
        {
            var existingTenant = await _tenantRepository.GetByIdWithAddresses(tenantId);

            if (existingTenant == null)
                throw NotFoundException.For<Tenant>(tenantId);

            if (!existingTenant.IsActive)
                throw new TenantInactiveException();

            var address = existingTenant?.Addresses.FirstOrDefault(a => a.Id == addressId)
                              ?? throw NotFoundException.For<Address>(addressId);

            UpdateAddressViewModel.MapToDomain(address, request);

            _adressRepository.Update(address);
            _unitOfWork.SaveChanges();

            return AddressViewModel.MapToViewModel(address);

        }
        catch (NotFoundException ex)
        {
            _logger.LogError("{Name} not found. TenantId: {Id}", ex.EntityName, ex.EntityId);
            throw;
        }
        catch (TenantInactiveException ex)
        {
            _logger.LogError("Attempt to use inactive tenant. TenantId: {TenantId}", tenantId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating address. TenantId: {TenantId}", tenantId);
            throw;
        }
    }
}
