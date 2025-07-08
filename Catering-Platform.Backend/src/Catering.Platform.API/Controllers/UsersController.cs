using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels.Users;
using Catering.Platform.Domain.Requests.User;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

//стратегия
//команда
//прочитать наследование в EF CORE
//TODO
//401 Unauthorized для неаутентифицированных пользователей.
//403 Forbidden для не-администраторов.
namespace Catering.Platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IValidator<CreateUserRequest> _createUserRequestValidator;
        private readonly IValidator<CreateSupplierRequest> _createSupplierRequestValidator;
        private readonly IValidator<CreateCustomerRequest> _createCustomerRequestValidator;
        private readonly IValidator<CreateBrokerRequest> _createBrokerRequestValidator;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        public UsersController(
            IValidator<CreateUserRequest> createUserRequestValidator,
            IValidator<CreateSupplierRequest> createSupplierRequestValidator,
            IValidator<CreateCustomerRequest> createCustomerRequestValidator,
            IValidator<CreateBrokerRequest> createBrokerRequestValidator,
            IUserService userService, 
            ILogger<UsersController> logger)
        {
            _createUserRequestValidator = createUserRequestValidator;
            _createSupplierRequestValidator = createSupplierRequestValidator;
            _createCustomerRequestValidator = createCustomerRequestValidator;
            _createBrokerRequestValidator = createBrokerRequestValidator;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("user")]
        public async Task<ActionResult<UserViewModel>> CreateUser(CreateUserRequest request)
        {
            var validationResult = await _createUserRequestValidator.ValidateAsync(request);

            if (validationResult.IsValid == false)
            {
                _logger.LogInformation(
                "Validation failed for CreateUser. Errors: {@ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var userViewModel = await _userService.CreateUserAsync(request);
            return Created(
                new Uri($"/api/addresses/{userViewModel.Id}", UriKind.Relative),
                userViewModel);
        }

        [HttpPost("supplier")]
        public async Task<ActionResult<UserViewModel>> CreateSupplier(CreateSupplierRequest request)
        {
            var validationResult = await _createSupplierRequestValidator.ValidateAsync(request);

            if (validationResult.IsValid == false)
            {
                _logger.LogInformation(
                "Validation failed for CreateSupplier. Errors: {@ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var supplierViewModel = await _userService.CreateSupplierAsync(request);
            return Created(
                new Uri($"/api/addresses/{supplierViewModel.Id}", UriKind.Relative),
                supplierViewModel);
        }

        [HttpPost("customer")]
        public async Task<ActionResult<CustomerViewModel>> CreateCustomer(CreateCustomerRequest request)
        {
            var validationResult = await _createCustomerRequestValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                _logger.LogInformation(
                "Validation failed for CreateCustomer. Errors: {@ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var customerViewModel = await _userService.CreateCustomerAsync(request);
            return Created(
                new Uri($"/api/addresses/{customerViewModel.Id}", UriKind.Relative),
                customerViewModel);
        }

        [HttpPost("broker")]
        public async Task<ActionResult<BrokerViewModel>> CreateCustomer(CreateBrokerRequest request)
        {
            var validationResult = await _createBrokerRequestValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                _logger.LogInformation(
                "Validation failed for CreateCustomer. Errors: {@ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var brokerViewModel = await _userService.CreateBrokerAsync(request);
            return Created(
                new Uri($"/api/addresses/{brokerViewModel.Id}", UriKind.Relative),
                brokerViewModel);
        }
    }
}
