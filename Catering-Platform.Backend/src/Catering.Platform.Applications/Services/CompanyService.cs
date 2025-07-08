using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels.Company;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Company;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services;

public class CompanyService : ICompanyService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<ICompanyService> _logger;

    public CompanyService(
        ITenantRepository tenantRepository,
        IAddressRepository addressRepository,
        ICompanyRepository companyRepository,
        ILogger<ICompanyService> logger)
    {
        _tenantRepository = tenantRepository;
        _addressRepository = addressRepository;
        _companyRepository = companyRepository;
        _logger = logger;
    }

    public async Task<CompanyViewModel> CreateCompanyAsync(CreateCompanyRequest request, Guid userId)
    {
        try
        {
            var existingTenant = await _tenantRepository.GetByIdAsync(request.TenantId);

            if (existingTenant == null)
                throw new TenantNotFoundException();

            var existingAddress = await _addressRepository.GetByIdAsync(request.AddressId);
            if (existingAddress == null)
                throw new AddressNotFoundException(request.AddressId);

            var company = CreateCompanyRequest.MapToDomain(request, userId);
            company.Tenant = existingTenant;
            company.Address = existingAddress;

            await _companyRepository.AddAsync(company);

            return CompanyViewModel.MapToViewModel(company);
        }
        catch (TenantNotFoundException ex)
        {
            _logger.LogError("Tenant not found. TenantId: {TenantId}", request.TenantId);
            throw;
        }
        catch (AddressNotFoundException ex)
        {
            _logger.LogError("Address for TenantId: {TenantId} not found. AddressId: {AddressId}", userId, request.AddressId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating Company. TenantId: {TenantId}", request.TenantId);
            throw;
        }
    }

    public async Task<CompanyViewModel> UpdateCompanyAsync(UpdateCompanyRequest request, Guid userId)
    {
        try
        {
            var existingCompany = await _companyRepository.GetByIdAsync(request.CompanyId);

            if (existingCompany == null)
                throw new CompanyNotFoundException(request.CompanyId);

            var existingAddress = await _addressRepository.GetByIdAsync(request.AddressId);

            if (existingAddress == null)
                throw new AddressNotFoundException(request.AddressId);

            var company = UpdateCompanyRequest.MapToDomain(request, userId);
            company.Address = existingAddress;

            await _companyRepository.UpdateAsync(company);

            return CompanyViewModel.MapToViewModel(company);
        }
        catch (CompanyNotFoundException ex)
        {
            _logger.LogError("Company not found. CompanyId: {CompanyId}", request.CompanyId);
            throw;
        }
        catch (AddressNotFoundException ex)
        {
            _logger.LogError("Address for CompanyId: {CompanyId} not found. AddressId: {AddressId}", userId, request.AddressId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating company. CompanyId: {CompanyId}", request.CompanyId);
            throw;
        }
    }
}
