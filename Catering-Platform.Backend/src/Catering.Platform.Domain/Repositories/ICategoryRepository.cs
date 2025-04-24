using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}
