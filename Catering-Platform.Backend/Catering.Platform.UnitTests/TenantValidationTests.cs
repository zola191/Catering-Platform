using AutoFixture.AutoNSubstitute;
using AutoFixture;
using Catering.Platform.API.Validators.Tenants;
using Catering.Platform.Domain.Requests.Tenant;
using FluentValidation;
using Catering.Platform.Domain.Shared;

namespace Catering.Platform.UnitTests;

public class TenantValidationTests
{
    private readonly IFixture _fixture;

    public TenantValidationTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
    }

    [Fact]
    public async Task CreateTenantRequestValidator_WithEmptyName_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateTenantRequest { Name = "" };

        var validator = new CreateTenantRequestValidatior();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => validator.ValidateAndThrowAsync(request));

        Assert.Contains("Name is required", exception.Message);
    }

    [Fact]
    public async Task CreateTenantRequestValidator_WithNameOverMaxLength_ThrowsValidationException()
    {
        // Arrange
        var name = new string('A', Constants.MAX_TEXT_LENGTH_200 + 1);
        var request = new CreateTenantRequest { Name = name };

        var validator = new CreateTenantRequestValidatior();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => validator.ValidateAndThrowAsync(request));

        Assert.Contains("Maximum Length exceeded", exception.Message);
    }

    [Fact]
    public async Task CreateTenantRequestValidator_WithNameWithinLimits_PassesValidation()
    {
        // Arrange
        var request = new CreateTenantRequest { Name = "Valid Name" };
        var validator = new CreateTenantRequestValidatior();

        // Act & Assert
        await validator.ValidateAndThrowAsync(request);
        Assert.True(true);
    }

    [Fact]
    public async Task UpdateTenantRequestValidator_WithEmptyName_ThrowsValidationException()
    {
        // Arrange
        var request = new UpdateTenantRequest { Name = "" };
        var validator = new UpdateTenantRequestValidatior();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => validator.ValidateAndThrowAsync(request));

        Assert.Contains("Name is required", exception.Message);
    }

    [Fact]
    public async Task UpdateTenantRequestValidator_WithNameOverMaxLength_ThrowsValidationException()
    {
        // Arrange
        var name = new string('A', Constants.MAX_TEXT_LENGTH_200 + 1);
        var request = new UpdateTenantRequest { Name = name };
        var validator = new UpdateTenantRequestValidatior();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => validator.ValidateAndThrowAsync(request));

        Assert.Contains("Maximum Length exceeded", exception.Message);
    }
}
