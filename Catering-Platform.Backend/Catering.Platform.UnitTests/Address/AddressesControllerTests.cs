using AutoFixture;
using Catering.Platform.API.Controllers;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Requests.Adress;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Catering.Platform.UnitTests.Addresses;

public class AddressesControllerTests
{
    private readonly IValidator<CreateAddressViewModel> _mockCreateAdressViewModelValidator;
    private readonly IAddressService _mockAddressService;
    private readonly ILogger<AddressesController> _mockLogger;
    private readonly AddressesController _controller;
    private readonly Fixture _fixture;

    public AddressesControllerTests()
    {
        _mockCreateAdressViewModelValidator = Substitute.For<IValidator<CreateAddressViewModel>>();
        _mockAddressService = Substitute.For<IAddressService>();
        _mockLogger = Substitute.For<ILogger<AddressesController>>();

        _controller = new AddressesController(
            _mockAddressService,
            _mockCreateAdressViewModelValidator,
            _mockLogger);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_ReturnsCorrectResponse_WhenServiceReturnsViewModel()
    {
        var tenantId = Guid.NewGuid();
        var request = _fixture.Create<CreateAddressViewModel>();
        var addressId = Guid.NewGuid();
        // можно перенести в общий класс для переиспользования ObjectCreator, но необязательно
        var addressViewModel = _fixture.Build<AddressViewModel>()
                                       .With(f => f.Id, addressId)
                                       .With(f => f.TenantId, tenantId)
                                       .With(f => f.Country, request.Country)
                                       .With(f => f.StreetAndBuilding, request.StreetAndBuilding)
                                       .With(f => f.Zip, request.Zip)
                                       .With(f => f.City, request.City)
                                       .With(f => f.Region, request.Region)
                                       .With(f => f.Comment, request.Comment)
                                       .With(f => f.Description, request.Description)
                                       .With(x => x.CreatedAt, () => DateTime.UtcNow)
                                       .With(x => x.UpdatedAt, () => null)
                                       .Create();

        var validationResult = new ValidationResult();

        _mockCreateAdressViewModelValidator.ValidateAsync(request).Returns(Task.FromResult(validationResult));
        _mockAddressService.CreateAddressAsync(request, tenantId).Returns(addressViewModel);

        var result = await _controller.Create(tenantId, request);

        result.Should().BeOfType<CreatedResult>();
        var createdResult = result as CreatedResult;
        createdResult!.Location.Should().Be($"/api/addresses/{addressViewModel.Id}");

        createdResult.Value.Should().Be(addressViewModel);

        await _mockCreateAdressViewModelValidator.Received(1).ValidateAsync(request);
        await _mockAddressService.Received(1).CreateAddressAsync(request, tenantId);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = _fixture.Create<CreateAddressViewModel>();

        var failures = new List<ValidationFailure>
        {
        new ValidationFailure("Zip", "Zip code must contain only digits")
        };

        var validationResult = new ValidationResult(failures);

        _mockCreateAdressViewModelValidator.ValidateAsync(request).Returns(Task.FromResult(validationResult));

        // Act
        var result = await _controller.Create(tenantId, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeEquivalentTo(validationResult.Errors);

        await _mockCreateAdressViewModelValidator.Received(1).ValidateAsync(request);
        await _mockAddressService.DidNotReceive().CreateAddressAsync(Arg.Any<CreateAddressViewModel>(), Arg.Any<Guid>());
    }

    [Fact]
    public async Task Create_ReturnsNotFound_WhenTenantNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateAddressViewModel();

        _mockCreateAdressViewModelValidator.ValidateAsync(request).Returns(new ValidationResult());
        _mockAddressService.When(x => x.CreateAddressAsync(request, tenantId))
                    .Do(_ => throw new TenantNotFoundException());

        // Act
        var act = async () => await _controller.Create(tenantId, request);

        // Assert
        await act.Should().ThrowAsync<TenantNotFoundException>();
    }

    [Fact]
    public async Task Create_ReturnsForbidden_WhenTenantInactive()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateAddressViewModel();

        _mockCreateAdressViewModelValidator.ValidateAsync(request).Returns(new ValidationResult());
        _mockAddressService.When(x => x.CreateAddressAsync(request, tenantId))
                    .Do(_ => throw new TenantInactiveException());

        // Act
        var act = async () => await _controller.Create(tenantId, request);

        // Assert
        await act.Should().ThrowAsync<TenantInactiveException>();
    }

    [Fact]
    public async Task Create_ThrowsException_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateAddressViewModel();

        _mockCreateAdressViewModelValidator.ValidateAsync(request).Returns(new ValidationResult());
        _mockAddressService.When(x => x.CreateAddressAsync(request, tenantId))
                           .Do(_ => throw new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _controller.Create(tenantId, request);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}
