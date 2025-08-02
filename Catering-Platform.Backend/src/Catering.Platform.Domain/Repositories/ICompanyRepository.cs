using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories;

public interface ICompanyRepository
{
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task<Company?> GetByIdAsync(Guid id);
    Task<Company?> GetByTaxNumberAsync(string taxNumber);
    Task<IEnumerable<Company>?> SearchByNameAsync(Guid? tenantId, string query);
    Task<(IEnumerable<Company>, int totalCount)> GetListAsync(Guid? tenantId, int page, int pageSize);
}
