using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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

    public async Task<(IEnumerable<Company>, int totalCount)> GetListAsync(Guid? tenantId, int page, int pageSize)
    {
        var query = _dbContext.Companies.AsNoTracking().AsQueryable();

        if (tenantId.HasValue)
        {
            query = query.Where(c => c.TenantId == tenantId.Value);
        }

        var totalCount = await query.CountAsync();

        var companies = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (companies, totalCount);
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
