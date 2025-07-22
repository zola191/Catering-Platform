using Catering.Platform.API.Validators.Tenants;
using Catering.Platform.Domain.Requests.Tenant;
using FluentValidation;
using Catering.Platform.Domain.Shared;

namespace Catering.Platform.UnitTests;

public class TenantValidationTests
{
    private readonly IValidator<BlockTenantRequest> _validator;

    public TenantValidationTests()
    {
        _validator = new BlockTenantRequestValidator();
    }

    [Fact]
    public async Task Validate_WithEmptyReason_ThrowsValidationException()
    {
        // Arrange
        var request = new BlockTenantRequest { Reason = string.Empty };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _validator.ValidateAndThrowAsync(request));

        Assert.Contains("The reason must be provided", exception.Message);
    }

    [Fact]
    public async Task Validate_WithNullReason_ThrowsValidationException()
    {
        // Arrange
        var request = new BlockTenantRequest { Reason = null };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _validator.ValidateAndThrowAsync(request));

        Assert.Contains("The reason must be provided", exception.Message);
    }

    [Fact]
    public async Task Validate_WithReasonExceedingMaxLength_ThrowsValidationException()
    {
        // Arrange
        var longReason = new string('A', Constants.MAX_TEXT_LENGTH_500 + 1);
        var request = new BlockTenantRequest { Reason = longReason };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _validator.ValidateAndThrowAsync(request));

        Assert.Contains("Maximum Length exceeded", exception.Message);
    }

    [Fact]
    public async Task Validate_WithValidReason_PassesValidation()
    {
        // Arrange
        var validReason = "This is a valid reason.";
        var request = new BlockTenantRequest { Reason = validReason };

        // Act
        await _validator.ValidateAndThrowAsync(request);

        // Assert
        Assert.True(true);
    }
}
