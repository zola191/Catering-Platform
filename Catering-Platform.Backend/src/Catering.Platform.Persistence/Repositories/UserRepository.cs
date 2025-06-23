using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;

namespace Catering.Platform.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;
    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(User user)
    {
        var result = await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
}
