using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels.Company;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Company;
using MediatR;
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

    public async Task<CompanyViewModel> BlockCompanyAsync(Guid companyId, Guid userId)
    {
        try
        {
            var existingCompany = await _companyRepository.GetByIdAsync(companyId);
            if (existingCompany == null)
                throw CompanyNotFoundException.ById(companyId);

            if (existingCompany.TenantId != userId)
                throw new UnauthorizedAccessException("Company does not belong to this tenant");

            if (existingCompany.IsBlocked)
                throw new InvalidOperationException("Company is already blocked");

            existingCompany.IsBlocked = true;
            await _companyRepository.UpdateAsync(existingCompany);
            return CompanyViewModel.MapToViewModel(existingCompany);
        }
        catch (CompanyNotFoundException ex)
        {
            _logger.LogError("Company not found. CompanyId: {CompanyId}", companyId);
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized attempt to unblock company {CompanyId} by user {UserId}", companyId, userId);
            throw;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Attempt to block already blocked company. CompanyId: {CompanyId}",
                companyId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetch Company. CompanyId: {CompanyId}", companyId);
            throw;
        }
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

    public async Task<CompanyViewModel> GetCompanyByIdAsync(Guid companyId, Guid userId)
    {
        try
        {
            var existingCompany = await _companyRepository.GetByIdAsync(companyId);
            if (existingCompany == null)
                throw CompanyNotFoundException.ById(companyId);
            return CompanyViewModel.MapToViewModel(existingCompany);
        }
        catch (CompanyNotFoundException ex)
        {
            _logger.LogError("Company not found. CompanyId: {CompanyId}", companyId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetch Company. CompanyId: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<CompanyViewModel> GetCompanyByTaxNumberAsync(string taxNumber, Guid userId)
    {
        try
        {
            var existingCompany = await _companyRepository.GetByTaxNumberAsync(taxNumber);

            if (existingCompany == null)
                throw CompanyNotFoundException.ByTaxNumber(taxNumber);

            return CompanyViewModel.MapToViewModel(existingCompany);
        }
        catch (CompanyNotFoundException ex)
        {
            _logger.LogError("Company by TaxNumber not found. TaxNumber: {TaxNumber}", taxNumber);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetch company by taxNumber. TaxNumber: {TaxNumber}", taxNumber);
            throw;
        }
    }

    public async Task<IEnumerable<CompanyViewModel>> SearchCompaniesByNameAsync(SearchByNameRequest request, Guid userId)
    {
        try
        {
            var existingTenant = await _tenantRepository.GetByIdAsync(userId);

            if (existingTenant == null)
                throw new TenantNotFoundException();

            var existingCompanies = await _companyRepository.SearchByNameAsync(userId, request.Name);

            if (existingCompanies == null)
                throw CompanyNotFoundException.ByName(request.Name);

            return existingCompanies.Select(CompanyViewModel.MapToViewModel);
        }
        catch (TenantNotFoundException ex)
        {
            _logger.LogError("Tenant not found. TenantId: {TenantId}", userId);
            throw;
        }

        catch (CompanyNotFoundException ex)
        {
            _logger.LogError("Company not found by name Name: {Name}", request.Name);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetch company by name. CompanyName: {CompanyName}", request.Name);
            throw;
        }
    }

    public async Task<CompanyViewModel> UnblockCompanyAsync(Guid companyId, Guid userId)
    {
        try
        {
            var existingCompany = await _companyRepository.GetByIdAsync(companyId);
            if (existingCompany == null)
                throw CompanyNotFoundException.ById(companyId);

            if (existingCompany.TenantId != userId)
                throw new UnauthorizedAccessException("Company does not belong to this tenant");

            if (existingCompany.IsBlocked == false)
                throw new InvalidOperationException("Company is already Unblocked");

            existingCompany.IsBlocked = false;
            await _companyRepository.UpdateAsync(existingCompany);
            return CompanyViewModel.MapToViewModel(existingCompany);
        }
        catch (CompanyNotFoundException ex)
        {
            _logger.LogError("Company not found. CompanyId: {CompanyId}", companyId);
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized attempt to unblock company {CompanyId} by user {UserId}", companyId, userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetch Company. CompanyId: {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<CompanyViewModel> UpdateCompanyAsync(UpdateCompanyRequest request, Guid userId)
    {
        try
        {
            var existingCompany = await _companyRepository.GetByIdAsync(request.CompanyId);

            if (existingCompany == null)
                throw CompanyNotFoundException.ById(request.CompanyId);

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
