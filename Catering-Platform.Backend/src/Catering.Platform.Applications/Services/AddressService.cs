using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Adress;
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating address. TenantId: {TenantId}", tenantId);
            throw;
        }
    }
}
