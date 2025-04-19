using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;

namespace Catering.Platform.Persistence.Repositories;

internal sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        
    }
    
}
