using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Requests.Adress;

namespace Catering.Platform.Applications.Abstractions
{
    public interface IAddressService
    {
        Task<AddressViewModel> CreateAddressAsync(CreateAddressViewModel request, Guid tenantId);
        Task<AddressViewModel> UpdateAddressAsync(Guid addressId, UpdateAddressViewModel request, Guid requestingUserId);
    }
}
