using Catering.Platform.Applications.ViewModels.Company;
using Catering.Platform.Domain.Requests.Company;

namespace Catering.Platform.Applications.Abstractions;

public interface ICompanyService
{
    Task<CompanyViewModel> CreateCompanyAsync(CreateCompanyRequest request, Guid userId);
    Task<CompanyViewModel> UpdateCompanyAsync(UpdateCompanyRequest request, Guid userId);
    Task<CompanyViewModel> GetCompanyByIdAsync(Guid companyId, Guid userId);
    Task<CompanyViewModel> GetCompanyByTaxNumberAsync(string taxNumber, Guid userId);
    Task<IEnumerable<CompanyViewModel>> SearchCompaniesByNameAsync(SearchByNameRequest request, Guid userId);
    Task<CompanyViewModel> UnblockCompanyAsync(Guid companyId, Guid userId);
}
