using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels.Users;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.User;
using Microsoft.Extensions.Logging;

namespace Catering.Platform.Applications.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;
    private readonly ILogger<IUserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IHashService hashService,
        ILogger<IUserService> logger)
    {
        _userRepository = userRepository;
        _hashService = hashService;
        _logger = logger;
    }

    public async Task<UserViewModel> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            var user = CreateUserRequest.MapToDomain(request);
            user.PasswordHash = _hashService.HashPassword(request.Password);
            await _userRepository.AddAsync(user);

            return UserViewModel.MapToViewModel(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to save user {FirstName} {MiddleName} {LastName}. See Details: {Details}",
                request.FirstName,
                request.MiddleName,
                request.LastName,
                ex.Message);
            throw;
        }
    }

    public async Task<SupplierViewModel> CreateSupplierAsync(CreateSupplierRequest request)
    {
        try
        {
            var supplier = CreateSupplierRequest.MapToDomain(request);
            supplier.PasswordHash = _hashService.HashPassword(request.Password);
            await _userRepository.AddAsync(supplier);

            return SupplierViewModel.MapToViewModel(supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to save supplier {FirstName} {MiddleName} {LastName}. See Details: {Details}",
                request.FirstName,
                request.MiddleName,
                request.LastName,
                ex.Message);
            throw;
        }
    }

    public async Task<CustomerViewModel> CreateCustomerAsync(CreateCustomerRequest request)
    {
        try
        {
            var supplier = CreateCustomerRequest.MapToDomain(request);
            supplier.PasswordHash = _hashService.HashPassword(request.Password);
            await _userRepository.AddAsync(supplier);

            return CustomerViewModel.MapToViewModel(supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to save customer {FirstName} {MiddleName} {LastName}. See Details: {Details}",
                request.FirstName,
                request.MiddleName,
                request.LastName,
                ex.Message);
            throw;
        }
    }

    public async Task<BrokerViewModel> CreateBrokerAsync(CreateBrokerRequest request)
    {
        try
        {
            var broker = CreateBrokerRequest.MapToDomain(request);
            broker.PasswordHash = _hashService.HashPassword(request.Password);
            await _userRepository.AddAsync(broker);

            return BrokerViewModel.MapToViewModel(broker);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Unable to save broker {FirstName} {MiddleName} {LastName}. See Details: {Details}",
                request.FirstName,
                request.MiddleName,
                request.LastName,
                ex.Message);
            throw;
        }
    }
}
