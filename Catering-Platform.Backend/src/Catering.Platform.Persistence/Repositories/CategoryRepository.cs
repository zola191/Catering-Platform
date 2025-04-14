using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catering.Platform.Persistence.Repositories;

internal sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        
    }

    public Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return DbContext.Categories.FirstOrDefaultAsync(f => f.Name == name, cancellationToken);
    }
}
