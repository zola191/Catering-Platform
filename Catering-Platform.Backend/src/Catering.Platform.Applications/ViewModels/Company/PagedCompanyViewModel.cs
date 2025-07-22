
namespace Catering.Platform.Applications.ViewModels.Company;

public class PagedCompanyViewModel
{
    public List<CompanyViewModel> CompanyViewModels { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public static PagedCompanyViewModel MapToViewModel(
    IEnumerable<Domain.Models.Company> companies,
    int totalCount,
    int page,
    int pageSize)
    {
        var companyList = companies?.Select(CompanyViewModel.MapToViewModel).ToList()
                          ?? new List<CompanyViewModel>();

        return new PagedCompanyViewModel
        {
            CompanyViewModels = companyList,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
