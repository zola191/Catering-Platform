using AutoFixture;
using Catering.Platform.API.Validators.Address;
using Catering.Platform.Domain.Requests.Adress;
using Catering.Platform.Domain.Shared;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;

namespace Catering.Platform.UnitTests.Address;

public class CreateAddressViewModelValidatorTests
{
    private readonly IValidator<CreateAddressViewModel> _validator;
    private readonly Fixture _fixture;

    public CreateAddressViewModelValidatorTests()
    {
        _validator = new CreateAddressViewModelValidator();
        _fixture = new Fixture();
    }

    [Fact]
    public void Validate_WhenCountryIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var model = _fixture.Build<CreateAddressViewModel>()
            .With(x => x.Country, string.Empty)
            .Create();

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateAddressViewModel.Country) &&
            e.ErrorMessage == "Country is required");
    }

    [Fact]
    public void Validate_WhenCountryIsTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var longValue = new string('A', Constants.MAX_TEXT_LENGTH_64 + 1);
        var model = _fixture.Build<CreateAddressViewModel>()
            .With(x => x.Country, longValue)
            .Create();

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateAddressViewModel.Country) &&
            e.ErrorMessage == $"Country must not exceed {Constants.MAX_TEXT_LENGTH_64} characters");
    }

    [Fact]
    public void Validate_WhenZipIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var model = _fixture.Build<CreateAddressViewModel>()
            .With(x => x.Zip, string.Empty)
            .Create();

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateAddressViewModel.Zip) &&
            e.ErrorMessage == "Zip code is required");
    }

    [Fact]
    public void Validate_WhenZipHasLetters_ShouldReturnValidationError()
    {
        // Arrange
        var model = _fixture.Build<CreateAddressViewModel>()
            .With(x => x.Zip, "ABC123")
            .Create();

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateAddressViewModel.Zip) &&
            e.ErrorMessage == "Zip code must contain only digits");
    }

    [Fact]
    public void Validate_WhenZipIsTooLong_ShouldReturnValidationError()
    {
        // Arrange
        var model = _fixture.Build<CreateAddressViewModel>()
            .With(x => x.Zip, "1234567")
            .Create();

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(CreateAddressViewModel.Zip) &&
            e.ErrorMessage == $"Zip code must not exceed {Constants.MAX_TEXT_LENGTH_6} characters");
    }

    [Fact]
    public void Validate_WhenZipIsValid_ShouldNotReturnError()
    {
        // Arrange
        var model = _fixture.Build<CreateAddressViewModel>()
            .With(x => x.Zip, "123456")
            .Create();

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(CreateAddressViewModel.Zip));
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldReturnSuccess()
    {
        // Arrange
        var model = _fixture.Build<CreateAddressViewModel>()
            .With(x => x.Country, "Russia")
            .With(x => x.City, "Moscow")
            .With(x => x.StreetAndBuilding, "Red Square 1")
            .With(x => x.Zip, "123456")
            .With(x => x.Region, "Central")
            .Without(x => x.Comment)
            .Without(x => x.Description)
            .Create();

        // Act
        ValidationResult result = _validator.Validate(model);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
