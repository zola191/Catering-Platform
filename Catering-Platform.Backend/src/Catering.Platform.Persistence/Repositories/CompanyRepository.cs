using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

    public async Task<Company?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<Company>().FindAsync(id);
    }

    public async Task<Company?> GetByTaxNumberAsync(string taxNumber)
    {
        return await _dbContext.Set<Company>().FirstOrDefaultAsync(c => c.TaxNumber == taxNumber);
    }

    public async Task<IEnumerable<Company>?> SearchByNameAsync(Guid? tenantId, string query)
    {
        var normalizedQuery = query.Trim().ToLower();
        return await _dbContext.Set<Company>()
            .Where(c => c.Name.ToLower().Contains(normalizedQuery))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        _dbContext.Update(company);
        await _dbContext.SaveChangesAsync();
    }
}
