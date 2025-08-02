using AutoFixture;
using Catering.Platform.API.Controllers;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels.Company;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Requests.Company;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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

    [Fact]
    public async Task Get_ReturnsCompany_WhenExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var expectedCompany = _fixture.Create<CompanyViewModel>();

        _mockCompanyService.GetCompanyByIdAsync(companyId, Arg.Any<Guid>())
            .Returns(expectedCompany);

        // Act
        var result = await _controller.Get(companyId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedCompany);
    }

    [Fact]
    public async Task GetByTaxNumber_ReturnsCompany_WhenExists()
    {
        // Arrange
        var taxNumber = "1234567890";
        var expectedCompany = _fixture.Create<CompanyViewModel>();

        _mockCompanyService.GetCompanyByTaxNumberAsync(taxNumber, Arg.Any<Guid>())
            .Returns(expectedCompany);

        // Act
        var result = await _controller.GetByTaxNumber(taxNumber);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expectedCompany);
    }

    [Fact]
    public async Task GetByTaxNumber_ThrowsCompanyNotFoundException_WhenCompanyNotExists()
    {
        // Arrange
        var taxNumber = "1234567890";
        var expectedException = CompanyNotFoundException.ByTaxNumber(taxNumber);

        _mockCompanyService.GetCompanyByTaxNumberAsync(taxNumber, Arg.Any<Guid>())
            .Throws(expectedException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CompanyNotFoundException>(
            () => _controller.GetByTaxNumber(taxNumber));

        exception.Should().BeEquivalentTo(expectedException);
        exception.Message.Should().Contain(taxNumber);
    }

    [Fact]
    public async Task SearchByName_ReturnsCompanies_WhenFound()
    {
        // Arrange
        var query = new SearchByNameRequest { Name = "Test" };
        var expectedCompanies = _fixture.CreateMany<CompanyViewModel>(2).ToList();

        _mockCompanyService.SearchCompaniesByNameAsync(query, Arg.Any<Guid>())
            .Returns(Task.FromResult<IEnumerable<CompanyViewModel>>(expectedCompanies));

        // Act
        var result = await _controller.SearchByName(query);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var actualCompanies = okResult.Value.Should().BeAssignableTo<IEnumerable<CompanyViewModel>>().Subject;
        actualCompanies.Should().BeEquivalentTo(expectedCompanies);
    }

    [Fact]
    public async Task Unblock_ReturnsOkWithCompany_WhenSuccess()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var expected = _fixture.Create<CompanyViewModel>();

        _mockCompanyService.UnblockCompanyAsync(companyId, Arg.Any<Guid>())
            .Returns(expected);

        // Act
        var result = await _controller.Unblock(companyId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Unblock_PropagatesException_WhenErrorOccurs()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        _mockCompanyService.UnblockCompanyAsync(companyId, Arg.Any<Guid>())
            .ThrowsAsync(CompanyNotFoundException.ById(companyId));

        // Act & Assert
        await _controller.Invoking(c => c.Unblock(companyId))
            .Should().ThrowAsync<CompanyNotFoundException>();
    }
}
