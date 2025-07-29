using AutoFixture;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Services;
using Catering.Platform.Applications.ViewModels.Company;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Company;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Catering.Platform.UnitTests.Company;

public class CompanyServiceTests
{
    private readonly ITenantRepository _mockTenantRepository;
    private readonly IAddressRepository _mockAddressRepository;
    private readonly ICompanyRepository _mockCompanyRepository;
    private readonly ILogger<ICompanyService> _mockLogger;
    private readonly CompanyService _service;
    private readonly Fixture _fixture;

    public CompanyServiceTests()
    {
        _mockTenantRepository = Substitute.For<ITenantRepository>();
        _mockAddressRepository = Substitute.For<IAddressRepository>();
        _mockCompanyRepository = Substitute.For<ICompanyRepository>();
        _mockLogger = Substitute.For<ILogger<ICompanyService>>();

        _service = new CompanyService(
            _mockTenantRepository,
            _mockAddressRepository,
            _mockCompanyRepository,
            _mockLogger);

        _fixture = new Fixture();
        // Решение проблемы циклических ссылок
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task CreateCompanyAsync_ReturnsViewModel_WhenAllConditionsMet()
    {
        // Arrange
        var request = _fixture.Create<CreateCompanyRequest>();
        var userId = Guid.NewGuid();

        var tenant = new Domain.Models.Tenant { Id = request.TenantId };
        var address = new Domain.Models.Address { Id = request.AddressId };
        var expectedCompany = _fixture.Create<Domain.Models.Company>();

        _mockTenantRepository.GetByIdAsync(request.TenantId).Returns(tenant);
        _mockAddressRepository.GetByIdAsync(request.AddressId).Returns(address);
        _mockCompanyRepository.AddAsync(Arg.Any<Domain.Models.Company>()).Returns(Task.CompletedTask)
            .AndDoes(x => x.Arg<Domain.Models.Company>().Id = expectedCompany.Id);

        // Act
        var result = await _service.CreateCompanyAsync(request, userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedCompany.Id);
        result.Name.Should().Be(request.Name);

        await _mockTenantRepository.Received(1).GetByIdAsync(request.TenantId);
        await _mockAddressRepository.Received(1).GetByIdAsync(request.AddressId);
        await _mockCompanyRepository.Received(1).AddAsync(Arg.Any<Domain.Models.Company>());
    }

    [Fact]
    public async Task CreateCompanyAsync_ThrowsTenantNotFoundException_WhenTenantNotFound()
    {
        // Arrange
        var request = new CreateCompanyRequest { TenantId = Guid.NewGuid() };
        var userId = Guid.NewGuid();

        _mockTenantRepository.GetByIdAsync(request.TenantId).Returns((Domain.Models.Tenant)null);

        // Act & Assert
        await Assert.ThrowsAsync<TenantNotFoundException>(
            () => _service.CreateCompanyAsync(request, userId));

        await _mockAddressRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
        await _mockCompanyRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Models.Company>());
    }

    [Fact]
    public async Task CreateCompanyAsync_ThrowsAddressNotFoundException_WhenAddressNotFound()
    {
        // Arrange
        var request = new CreateCompanyRequest
        {
            TenantId = Guid.NewGuid(),
            AddressId = Guid.NewGuid()
        };
        var userId = Guid.NewGuid();
        var tenant = new Domain.Models.Tenant { Id = request.TenantId };

        _mockTenantRepository.GetByIdAsync(request.TenantId).Returns(tenant);
        _mockAddressRepository.GetByIdAsync(request.AddressId).Returns((Domain.Models.Address)null);

        // Act & Assert
        await Assert.ThrowsAsync<AddressNotFoundException>(
            () => _service.CreateCompanyAsync(request, userId));

        await _mockCompanyRepository.DidNotReceive().AddAsync(Arg.Any<Domain.Models.Company>());
    }

    [Fact]
    public async Task UpdateCompanyAsync_ReturnsUpdatedViewModel_WhenUpdateIsSuccessful()
    {
        // Arrange
        var request = _fixture.Create<UpdateCompanyRequest>();
        var userId = Guid.NewGuid();

        var existingCompany = _fixture.Build<Domain.Models.Company>()
            .With(c => c.Id, request.CompanyId)
            .Create();

        var existingAddress = _fixture.Build<Domain.Models.Address>()
            .With(a => a.Id, request.AddressId)
            .Create();

        _mockCompanyRepository.GetByIdAsync(request.CompanyId).Returns(existingCompany);
        _mockAddressRepository.GetByIdAsync(request.AddressId).Returns(existingAddress);
        _mockCompanyRepository.UpdateAsync(Arg.Any<Domain.Models.Company>()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateCompanyAsync(request, userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(request.CompanyId);

        await _mockCompanyRepository.Received(1).GetByIdAsync(request.CompanyId);
        await _mockAddressRepository.Received(1).GetByIdAsync(request.AddressId);
        await _mockCompanyRepository.Received(1).UpdateAsync(Arg.Any<Domain.Models.Company>());
    }

    [Fact]
    public async Task UpdateCompanyAsync_ThrowsCompanyNotFoundException_WhenCompanyNotFound()
    {
        // Arrange
        var request = _fixture.Create<UpdateCompanyRequest>();
        var userId = Guid.NewGuid();

        _mockCompanyRepository.GetByIdAsync(request.CompanyId).Returns((Domain.Models.Company)null);

        // Act & Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(
            () => _service.UpdateCompanyAsync(request, userId));

        await _mockAddressRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>());
        await _mockCompanyRepository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Models.Company>());
    }

    [Fact]
    public async Task UpdateCompanyAsync_ThrowsAddressNotFoundException_WhenAddressNotFound()
    {
        // Arrange
        var request = _fixture.Create<UpdateCompanyRequest>();
        var userId = Guid.NewGuid();
        var existingCompany = _fixture.Build<Domain.Models.Company>()
            .With(c => c.Id, request.CompanyId)
            .Create();

        _mockCompanyRepository.GetByIdAsync(request.CompanyId).Returns(existingCompany);
        _mockAddressRepository.GetByIdAsync(request.AddressId).Returns((Domain.Models.Address)null);

        // Act & Assert
        await Assert.ThrowsAsync<AddressNotFoundException>(
            () => _service.UpdateCompanyAsync(request, userId));

        await _mockCompanyRepository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Models.Company>());
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ReturnsCompany_WhenExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingCompany = _fixture.Build<Domain.Models.Company>()
            .With(c => c.Id, companyId)
            .Create();

        _mockCompanyRepository.GetByIdAsync(companyId)
            .Returns(existingCompany);

        // Act
        var result = await _service.GetCompanyByIdAsync(companyId, userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(companyId);
        await _mockCompanyRepository.Received(1).GetByIdAsync(companyId);
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ThrowsNotFound_WhenCompanyNotExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockCompanyRepository.GetByIdAsync(companyId)
            .Returns((Domain.Models.Company)null);

        // Act & Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(
            () => _service.GetCompanyByIdAsync(companyId, userId));

    }

    [Fact]
    public async Task GetCompanyByTaxNumberAsync_ReturnsCompany_WhenExists()
    {
        // Arrange
        var taxNumber = "1234567890";
        var userId = Guid.NewGuid();
        var existingCompany = _fixture.Build<Domain.Models.Company>()
            .With(c => c.TaxNumber, taxNumber)
            .Create();

        _mockCompanyRepository.GetByTaxNumberAsync(taxNumber)
            .Returns(existingCompany);

        // Act
        var result = await _service.GetCompanyByTaxNumberAsync(taxNumber, userId);

        // Assert
        result.Should().NotBeNull();
        result.TaxNumber.Should().Be(taxNumber);
        await _mockCompanyRepository.Received(1).GetByTaxNumberAsync(taxNumber);
    }

    [Fact]
    public async Task GetCompanyByTaxNumberAsync_ThrowsNotFound_WhenCompanyNotExists()
    {
        // Arrange
        var taxNumber = "1234567890";
        var userId = Guid.NewGuid();

        _mockCompanyRepository.GetByTaxNumberAsync(taxNumber)
            .Returns((Domain.Models.Company)null);

        // Act & Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(
            () => _service.GetCompanyByTaxNumberAsync(taxNumber, userId));
    }

    [Fact]
    public async Task SearchCompaniesByNameAsync_ReturnsCompanies_WhenFound()
    {
        // Arrange
        var request = new SearchByNameRequest { Name = "Test" };
        var userId = Guid.NewGuid();
        var tenant = _fixture.Create<Domain.Models.Tenant>();
        var companies = _fixture.CreateMany<Domain.Models.Company>(2).ToList();

        _mockTenantRepository.GetByIdAsync(userId).Returns(tenant);
        _mockCompanyRepository.SearchByNameAsync(userId, request.Name).Returns(companies);

        // Act
        var result = await _service.SearchCompaniesByNameAsync(request, userId);

        // Assert
        result.Should().HaveCount(2);
        await _mockTenantRepository.Received(1).GetByIdAsync(userId);
        await _mockCompanyRepository.Received(1).SearchByNameAsync(userId, request.Name);
    }

    [Fact]
    public async Task SearchCompaniesByNameAsync_ThrowsTenantNotFound_WhenTenantNotExists()
    {
        // Arrange
        var request = new SearchByNameRequest { Name = "Test" };
        var userId = Guid.NewGuid();

        _mockTenantRepository.GetByIdAsync(userId).Returns((Domain.Models.Tenant)null);

        // Act & Assert
        await Assert.ThrowsAsync<TenantNotFoundException>(
            () => _service.SearchCompaniesByNameAsync(request, userId));
    }

    [Fact]
    public async Task SearchCompaniesByNameAsync_ThrowsCompanyNotFound_WhenNoCompaniesFound()
    {
        // Arrange
        var request = new SearchByNameRequest { Name = "Test" };
        var userId = Guid.NewGuid();
        var tenant = _fixture.Create<Domain.Models.Tenant>();

        _mockTenantRepository.GetByIdAsync(userId).Returns(tenant);
        _mockCompanyRepository.SearchByNameAsync(userId, request.Name).Returns((List<Domain.Models.Company>)null);

        // Act & Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(
            () => _service.SearchCompaniesByNameAsync(request, userId));
    }

    [Fact]
    public async Task UnblockCompanyAsync_ReturnsUnblockedCompany_WhenSuccess()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var company = _fixture.Build<Domain.Models.Company>()
            .With(c => c.Id, companyId)
            .With(c => c.TenantId, userId)
            .With(c => c.IsBlocked, true)
            .Create();

        _mockCompanyRepository.GetByIdAsync(companyId).Returns(company);
        _mockCompanyRepository.UpdateAsync(Arg.Any<Domain.Models.Company>()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UnblockCompanyAsync(companyId, userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(companyId);
        company.IsBlocked.Should().BeFalse();

        await _mockCompanyRepository.Received(1).GetByIdAsync(companyId);
        await _mockCompanyRepository.Received(1).UpdateAsync(company);
    }

    [Fact]
    public async Task UnblockCompanyAsync_ThrowsCompanyNotFound_WhenCompanyNotExists()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockCompanyRepository.GetByIdAsync(companyId).Returns((Domain.Models.Company)null);

        // Act & Assert
        await Assert.ThrowsAsync<CompanyNotFoundException>(
            () => _service.UnblockCompanyAsync(companyId, userId));

        await _mockCompanyRepository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Models.Company>());
    }

    [Fact]
    public async Task UnblockCompanyAsync_ThrowsUnauthorized_WhenCompanyBelongsToOtherTenant()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var company = _fixture.Build<Domain.Models.Company>()
            .With(c => c.Id, companyId)
            .With(c => c.TenantId, otherUserId)
            .Create();

        _mockCompanyRepository.GetByIdAsync(companyId).Returns(company);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.UnblockCompanyAsync(companyId, userId));

        ex.Message.Should().Be("Company does not belong to this tenant");
        await _mockCompanyRepository.DidNotReceive().UpdateAsync(Arg.Any<Domain.Models.Company>());
    }

    [Fact]
    public async Task UnblockCompanyAsync_ThrowsGenericException_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var company = _fixture.Build<Domain.Models.Company>()
            .With(c => c.Id, companyId)
            .With(c => c.TenantId, userId)
            .Create();

        _mockCompanyRepository.GetByIdAsync(companyId).Returns(company);
        _mockCompanyRepository.UpdateAsync(Arg.Any<Domain.Models.Company>())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _service.UnblockCompanyAsync(companyId, userId));
    }
}
