using Catering.Platform.Applications.ViewModels.Users;
using Catering.Platform.Domain.Requests.User;

namespace Catering.Platform.Applications.Abstractions;

public interface IUserService
{
    Task<UserViewModel> CreateUserAsync(CreateUserRequest request);
    Task<SupplierViewModel> CreateSupplierAsync(CreateSupplierRequest request);
    Task<CustomerViewModel> CreateCustomerAsync(CreateCustomerRequest request);
    Task<BrokerViewModel> CreateBrokerAsync(CreateBrokerRequest request);
}
