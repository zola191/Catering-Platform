using AutoFixture;
using Catering.Platform.API.Controllers;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels.Company;
using Catering.Platform.Domain.Requests.Company;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Catering.Platform.UnitTests.Company;

public class CompaniesControllerTests
{
    private readonly IValidator<CreateCompanyRequest> _mockCreateCompanyvalidator;
    private readonly ICompanyService _mockCompanyService;
    private readonly ILogger<CompaniesController> _mockLogger;
    private readonly CompaniesController _controller;
    private readonly Fixture _fixture;

    public CompaniesControllerTests()
    {
        _mockCreateCompanyvalidator = Substitute.For<IValidator<CreateCompanyRequest>>();
        _mockCompanyService = Substitute.For<ICompanyService>();
        _mockLogger = NullLogger<CompaniesController>.Instance;

        _controller = new CompaniesController(
            _mockCreateCompanyvalidator,
            _mockCompanyService,
            _mockLogger);

        _fixture = new Fixture();
    }

    [Fact]
    public async Task Create_ReturnsCorrectResponse_WhenServiceReturnsViewModel()
    {
        // Arrange
        var request = _fixture.Create<CreateCompanyRequest>();
        var companyViewModel = _fixture.Create<CompanyViewModel>();
        var validationResult = new ValidationResult();

        _mockCreateCompanyvalidator.ValidateAsync(request)
            .Returns(Task.FromResult(validationResult));

        _mockCompanyService.CreateCompanyAsync(request, Arg.Any<Guid>())
            .Returns(companyViewModel);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Location.Should().Be($"/api/company/{companyViewModel.Id}");
        createdResult.Value.Should().Be(companyViewModel);

        await _mockCreateCompanyvalidator.Received(1).ValidateAsync(request);
        await _mockCompanyService.Received(1).CreateCompanyAsync(request, Arg.Any<Guid>());
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = _fixture.Create<CreateCompanyRequest>();

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("TaxNumber", "TaxNumber is invalid")
        };
        var validationResult = new ValidationResult(validationErrors);

        _mockCreateCompanyvalidator.ValidateAsync(request)
            .Returns(Task.FromResult(validationResult));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;

        badRequestResult.Value.Should().BeEquivalentTo(validationResult.Errors);

        await _mockCompanyService.DidNotReceive().CreateCompanyAsync(Arg.Any<CreateCompanyRequest>(), Arg.Any<Guid>());
        await _mockCreateCompanyvalidator.Received(1).ValidateAsync(request);
    }
}
