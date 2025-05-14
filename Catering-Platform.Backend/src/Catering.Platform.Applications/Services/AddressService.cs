using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Adress;
using Catering.Platform.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IAddressService> _logger;

    public AddressService(
        IAddressRepository adressRepository,
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork,
        ILogger<IAddressService> logger)
    {
        _addressRepository = adressRepository;
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

            await _addressRepository.AddAsync(entity);
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

    public async Task DeleteAddressAsync(Guid addressId, Guid tenantId)
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

            _addressRepository.Delete(address);
            _unitOfWork.SaveChanges();
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

    public async Task<AddressViewModel> GetAddressByIdAsync(Guid addressId, Guid tenantId)
    {
        try
        {
            var existingTenant = await _tenantRepository.GetByIdWithAddresses(tenantId);

            if (existingTenant == null)
                throw NotFoundException.For<Tenant>(tenantId);

            if (!existingTenant.IsActive)
                throw new TenantInactiveException();

            var address = await _addressRepository.GetByIdAsync(addressId);

            if (address == null)
                throw NotFoundException.For<Address>(addressId);

            if (address.TenantId != tenantId)
                throw NotFoundException.For<Address>(addressId);

            var addressViewModel = AddressViewModel.MapToViewModel(address);
            return addressViewModel;
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
            _logger.LogError(ex, "Unexpected error fetching address. TenantId: {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<IEnumerable<AddressViewModel>> SearchAddressesByTextAsync(SearchByTextViewModel viewModel, Guid? tenantId)
    {
        //TODO добавить проверку, что tenantId является админом
        try
        {
            if (string.IsNullOrWhiteSpace(viewModel.Query))
            {
                throw new SearchByTextException(ErrorMessages.SearchQueryEmpty);
            }

            var sanitizedQuery = SanitizeTsQueryInput(viewModel.Query);

            var textSearchQuery = PrepareTsQuery(sanitizedQuery);

            if (string.IsNullOrWhiteSpace(textSearchQuery))
            {
                throw new SearchByTextException(ErrorMessages.SearchQueryInvalidAfterSanitization);
            }

            var addresses = await _addressRepository.SearchByTextAsync(viewModel.Id, textSearchQuery);

            return addresses.Select(AddressViewModel.MapToViewModel).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching address. TenantId: {TenantId}", tenantId);
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

            _addressRepository.Update(address);
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

    public async Task<IEnumerable<AddressViewModel>> SearchAddressesByZipAsync(SearchByZipViewModel request, Guid? requestingUserId)
    {
        //TODO Проверки прав доступа:
        //пользователи могут искать только адреса своего арендатора (User.TenantId),
        //администраторы — любые.
        try
        {
            var addresses = await _addressRepository.SearchByZipAsync(request.Id, request.Zip);
            return addresses.Select(AddressViewModel.MapToViewModel);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetch address. zip: {TenantId}", request.Zip);
            throw;
        }
    }

    private static string SanitizeTsQueryInput(string input)
    {
        var invalidChars = new[] { '&', '|', '!', '(', ')', '\'' };
        return new string(input.Where(c => !invalidChars.Contains(c)).ToArray());
    }

    private string PrepareTsQuery(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" & ", words.Select(w => $"\"{w}\""));
    }
}
