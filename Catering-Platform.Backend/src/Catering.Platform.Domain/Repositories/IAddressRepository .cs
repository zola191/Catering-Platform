using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Repositories
{
    public interface IAddressRepository
    {
        Task AddAsync(Address address);
        void Update(Address address);
        void Delete(Address address);
        Task<Address?> GetByIdAsync(Guid addressId);
        Task<IEnumerable<Address>> SearchByTextAsync(Guid? tenantId, string query);
    }
}
