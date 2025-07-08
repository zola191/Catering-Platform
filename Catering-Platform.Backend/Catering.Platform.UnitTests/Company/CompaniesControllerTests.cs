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
    private readonly IValidator<UpdateCompanyRequest> _mockUpdateCompanyvalidator;
    private readonly ICompanyService _mockCompanyService;
    private readonly ILogger<CompaniesController> _mockLogger;
    private readonly CompaniesController _controller;
    private readonly Fixture _fixture;

    public CompaniesControllerTests()
    {
        _mockCreateCompanyvalidator = Substitute.For<IValidator<CreateCompanyRequest>>();
        _mockUpdateCompanyvalidator = Substitute.For<IValidator<UpdateCompanyRequest>>();
        _mockCompanyService = Substitute.For<ICompanyService>();
        _mockLogger = NullLogger<CompaniesController>.Instance;

        _controller = new CompaniesController(
            _mockCreateCompanyvalidator,
            _mockUpdateCompanyvalidator,
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
    public async Task Update_ReturnsOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var request = _fixture.Build<UpdateCompanyRequest>()
                            .With(x => x.CompanyId, companyId)
                            .Create();

        var companyViewModel = _fixture.Create<CompanyViewModel>();
        var validationResult = new ValidationResult();

        _mockUpdateCompanyvalidator.ValidateAsync(request)
            .Returns(Task.FromResult(validationResult));

        _mockCompanyService.UpdateCompanyAsync(request, Arg.Any<Guid>())
            .Returns(companyViewModel);

        // Act
        var result = await _controller.Update(request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(companyViewModel);

        await _mockUpdateCompanyvalidator.Received(1).ValidateAsync(request);
        await _mockCompanyService.Received(1).UpdateCompanyAsync(request, Arg.Any<Guid>());
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = _fixture.Create<UpdateCompanyRequest>();
        var validationErrors = new List<ValidationFailure>
        {
        new ValidationFailure("Name", "Name is required")
        };
        var validationResult = new ValidationResult(validationErrors);

        _mockUpdateCompanyvalidator.ValidateAsync(request)
            .Returns(Task.FromResult(validationResult));

        // Act
        var result = await _controller.Update(request);

        // Assert
        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().BeEquivalentTo(validationResult.Errors);

        await _mockCompanyService.DidNotReceive().UpdateCompanyAsync(Arg.Any<UpdateCompanyRequest>(), Arg.Any<Guid>());
    }
}
