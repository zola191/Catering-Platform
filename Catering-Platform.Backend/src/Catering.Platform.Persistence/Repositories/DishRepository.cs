using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;

namespace Catering.Platform.Persistence.Repositories;

internal sealed class DishRepository : Repository<Dish>, IDishRepository
{
    public DishRepository(ApplicationDbContext dbContext):base(dbContext)
    {
        
    }
}