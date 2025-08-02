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
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Catering.Platform.UnitTests.Addresses;

public class AddressesControllerTests
{
    private readonly IValidator<CreateAddressViewModel> _mockCreateAdressViewModelValidator;
    private readonly IValidator<UpdateAddressViewModel> _mockUpdateAdressViewModelValidator;
    private readonly IAddressService _mockAddressService;
    private readonly ILogger<AddressesController> _mockLogger;
    private readonly AddressesController _controller;
    private readonly Fixture _fixture;

    public AddressesControllerTests()
    {
        _mockCreateAdressViewModelValidator = Substitute.For<IValidator<CreateAddressViewModel>>();
        _mockUpdateAdressViewModelValidator = Substitute.For<IValidator<UpdateAddressViewModel>>();
        _mockAddressService = Substitute.For<IAddressService>();
        _mockLogger = NullLogger<AddressesController>.Instance;

        _controller = new AddressesController(
            _mockAddressService,
            _mockCreateAdressViewModelValidator,
            _mockUpdateAdressViewModelValidator,
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

        var failures = new List<FluentValidation.Results.ValidationFailure>
        {
        new FluentValidation.Results.ValidationFailure("Zip", "Zip code must contain only digits")
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

    [Fact]
    public async Task Update_ThrowsAddressNotFoundException_WhenAddressDoesNotExist()
    {
        // Arrange
        var expectedTenantId = Guid.Parse("0196763c-9106-7806-a03f-960a1dad80e7");
        var addressId = Guid.NewGuid();
        var request = new UpdateAddressViewModel();

        var validationResult = new ValidationResult();

        _mockUpdateAdressViewModelValidator.ValidateAsync(request).Returns(validationResult);

        _mockAddressService
            .UpdateAddressAsync(addressId, request, expectedTenantId)
            .ThrowsAsync(NotFoundException.For<Domain.Models.Address>(addressId));

        // Act
        Exception exception = null;
        IActionResult result = null;

        try
        {
            result = await _controller.Update(addressId, request);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        Assert.NotNull(exception);
        Assert.IsType<NotFoundException>(exception);

        Assert.Null(result);

        Assert.Contains(addressId.ToString(), exception.Message);
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WithCorrectData()
    {
        // Arrange
        var expectedTenantId = Guid.Parse("0196763c-9106-7806-a03f-960a1dad80e7");
        var addressId = Guid.NewGuid();
        var request = new UpdateAddressViewModel();

        var addressViewModel = _fixture.Build<AddressViewModel>()
            .With(f => f.Id, addressId)
            .With(f => f.TenantId, expectedTenantId)
            .With(f => f.Country, request.Country)
            .With(f => f.StreetAndBuilding, request.StreetAndBuilding)
            .With(f => f.Zip, request.Zip)
            .With(f => f.City, request.City)
            .With(f => f.Region, request.Region)
            .With(f => f.Comment, request.Comment)
            .With(f => f.Description, request.Description)
            .With(x => x.CreatedAt, () => DateTime.UtcNow)
            .With(x => x.UpdatedAt, () => DateTime.UtcNow)
            .Create();

        var validationResult = new ValidationResult();

        _mockUpdateAdressViewModelValidator.ValidateAsync(request).Returns(validationResult);
        _mockAddressService.UpdateAddressAsync(addressId, request, expectedTenantId).Returns(addressViewModel);

        // Act
        var result = await _controller.Update(addressId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var model = Assert.IsType<AddressViewModel>(okResult.Value);
        Assert.Equal(addressId, model.Id);
        Assert.Equal(expectedTenantId, model.TenantId);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var request = new UpdateAddressViewModel();

        var validationResult = new ValidationResult(new[]
        {
            new FluentValidation.Results.ValidationFailure("Country", "Country is required.")
        });

        _mockUpdateAdressViewModelValidator.ValidateAsync(request).Returns(validationResult);

        // Act
        var result = await _controller.Update(addressId, request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<List<FluentValidation.Results.ValidationFailure>>(badRequestResult.Value);
        Assert.Single(errors);
        Assert.Equal("Country", errors[0].PropertyName);
    }

    [Fact]
    public async Task Delete_ExistingAddress_ReturnsNoContent()
    {
        // Arrange
        var addressId = _fixture.Create<Guid>();

        _mockAddressService
            .DeleteAddressAsync(addressId, Arg.Any<Guid>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(addressId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_NonExistingAddress_ReturnsNotFound()
    {
        // Arrange
        var addressId = _fixture.Create<Guid>();
        var tenantId = Guid.Parse("0196763c-9106-7806-a03f-960a1dad80e7");

        _mockAddressService
            .DeleteAddressAsync(addressId, tenantId)
            .ThrowsAsync(NotFoundException.For<Domain.Models.Address>(addressId));

        // Act & Assert
        await _controller.Awaiting(c => c.Delete(addressId))
            .Should().ThrowAsync<NotFoundException>()
            .Where(ex => ex.EntityName == nameof(Domain.Models.Address) &&
                         ex.EntityId.ToString() == addressId.ToString());
    }

    [Fact]
    public async Task Get_ExistingAddress_ReturnsOkWithViewModel()
    {
        // Arrange
        var addressId = _fixture.Create<Guid>();
        var viewModel = new AddressViewModel
        {
            Id = addressId
        };

        _mockAddressService
            .GetAddressByIdAsync(addressId, Arg.Any<Guid>())
            .Returns(viewModel);

        // Act
        var result = await _controller.Get(addressId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(viewModel);
    }

    [Fact]
    public async Task Get_NonExistingAddress_ThrowsNotFoundException()
    {
        // Arrange
        var addressId = _fixture.Create<Guid>();

        _mockAddressService
            .GetAddressByIdAsync(addressId, Arg.Any<Guid>())
            .ThrowsAsync(NotFoundException.For<Domain.Models.Address>(addressId));

        // Act & Assert
        await _controller.Awaiting(c => c.Get(addressId))
            .Should().ThrowAsync<NotFoundException>()
            .Where(ex => ex.EntityName == nameof(Address) && ex.EntityId.ToString() == addressId.ToString());
    }
}
