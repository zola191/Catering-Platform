using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories;

public interface ICompanyRepository
{
    Task AddAsync(Company company);
}
