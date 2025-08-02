using AutoFixture;
using Catering.Platform.API.Controllers;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels.Users;
using Catering.Platform.Domain.Requests.User;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Catering.Platform.UnitTests.User;

public class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly IValidator<CreateUserRequest> _mockCreateUserRequestValidator;
    private readonly IValidator<CreateSupplierRequest> _mockCreateSupplierRequestValidator;
    private readonly IValidator<CreateCustomerRequest> _mockCreateCustomerRequestValidator;
    private readonly IValidator<CreateBrokerRequest> _mockCreateBrokerRequestValidator;
    private readonly IUserService _mockUserService;
    private readonly ILogger<UsersController> _mockLogger;
    private readonly Fixture _fixture;
    public UsersControllerTests()
    {
        _mockCreateUserRequestValidator = Substitute.For<IValidator<CreateUserRequest>>();
        _mockCreateSupplierRequestValidator = Substitute.For<IValidator<CreateSupplierRequest>>();
        _mockCreateCustomerRequestValidator = Substitute.For<IValidator<CreateCustomerRequest>>();
        _mockCreateBrokerRequestValidator = Substitute.For<IValidator<CreateBrokerRequest>>();
        _mockUserService = Substitute.For<IUserService>();
        _mockLogger = NullLogger<UsersController>.Instance;

        _controller = new UsersController(
            _mockCreateUserRequestValidator,
            _mockCreateSupplierRequestValidator,
            _mockCreateCustomerRequestValidator,
            _mockCreateBrokerRequestValidator,
            _mockUserService,
            _mockLogger);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedResult_WhenDataIsValid()
    {
        // Arrange
        var request = _fixture.Create<CreateUserRequest>();
        var validationResult = new ValidationResult(); // Успешная валидация
        _mockCreateUserRequestValidator.ValidateAsync(request).Returns(validationResult);

        var userViewModel = _fixture.Create<UserViewModel>();
        _mockUserService.CreateUserAsync(request).Returns(userViewModel);

        // Act
        var result = await _controller.CreateUser(request);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        var returnedUser = Assert.IsType<UserViewModel>(createdResult.Value);
        Assert.Equal(userViewModel.Id, returnedUser.Id);
        Assert.Equal($"/api/addresses/{userViewModel.Id}", createdResult.Location);
    }

    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = _fixture.Create<CreateUserRequest>();

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("PropertyName", "Error message")
        };
        var validationResult = new ValidationResult(validationErrors);
        _mockCreateUserRequestValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var result = await _controller.CreateUser(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(validationErrors, badRequestResult.Value);
    }

}
