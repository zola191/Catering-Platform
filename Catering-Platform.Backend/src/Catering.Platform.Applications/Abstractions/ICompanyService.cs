using Catering.Platform.Applications.ViewModels.Company;
using Catering.Platform.Domain.Requests.Company;

namespace Catering.Platform.Applications.Abstractions;

public interface ICompanyService
{
    Task<CompanyViewModel> CreateCompanyAsync(CreateCompanyRequest request, Guid userId);
    Task<CompanyViewModel> UpdateCompanyAsync(UpdateCompanyRequest request, Guid userId);
    Task<CompanyViewModel> GetCompanyByIdAsync(Guid companyId, Guid userId);
}
