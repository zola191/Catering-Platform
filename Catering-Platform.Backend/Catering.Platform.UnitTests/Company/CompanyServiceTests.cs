using AutoFixture;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Services;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Company;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

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
}
