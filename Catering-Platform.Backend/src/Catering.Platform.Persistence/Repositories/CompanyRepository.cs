using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;

namespace Catering.Platform.Persistence.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CompanyRepository(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task AddAsync(Company company)
    {
        var result = await _dbContext.AddAsync(company);
        await _dbContext.SaveChangesAsync();
    }
}
