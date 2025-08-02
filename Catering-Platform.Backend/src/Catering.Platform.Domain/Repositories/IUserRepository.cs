using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
}
