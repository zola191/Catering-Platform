using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories
{
    public interface IAddressRepository
    {
        Task AddAsync(Address address);
    }
}
