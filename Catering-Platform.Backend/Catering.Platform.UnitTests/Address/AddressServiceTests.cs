using AutoFixture;
using Catering.Platform.Applications.Services;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Adress;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Catering.Platform.UnitTests.Addresses;

public class AddressServiceTests
{
    private readonly ITenantRepository _tenantRepo;
    private readonly IAddressRepository _addressRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddressService> _logger;
    private readonly AddressService _service;
    private readonly Fixture _fixture;
    public AddressServiceTests()
    {
        _tenantRepo = Substitute.For<ITenantRepository>();
        _addressRepo = Substitute.For<IAddressRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<AddressService>>();
        _fixture = new Fixture();

        _service = new AddressService(
            _addressRepo,
            _tenantRepo,
            _unitOfWork,
            _logger);
    }

    [Fact]
    public async Task CreateAddressAsync_ReturnsViewModel_WhenTenantIsActive()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = _fixture.Create<CreateAddressViewModel>();
        var expectedAddressId = Guid.NewGuid();

        var tenant = new Domain.Models.Tenant { Id = tenantId, IsActive = true };

        _tenantRepo.GetByIdAsync(tenantId).Returns(tenant);

        _addressRepo
            .AddAsync(Arg.Do<Domain.Models.Address>(a =>
            {
                a.Id = expectedAddressId;
                a.TenantId = tenantId;
            }))
            .Returns(Task.CompletedTask);

        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        var result = await _service.CreateAddressAsync(request, tenantId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedAddressId);
        result.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task CreateAddressAsync_ThrowsTenantNotFoundException_WhenTenantIsNull()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateAddressViewModel();

        _tenantRepo.GetByIdAsync(tenantId).Returns((Domain.Models.Tenant)null!);

        // Act
        Func<Task> act = () => _service.CreateAddressAsync(request, tenantId);

        // Assert
        await act.Should().ThrowAsync<TenantNotFoundException>();
    }

    [Fact]
    public async Task CreateAddressAsync_ThrowsTenantInactiveException_WhenTenantIsInactive()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateAddressViewModel();
        var tenant = new Domain.Models.Tenant { Id = tenantId, IsActive = false };

        _tenantRepo.GetByIdAsync(tenantId).Returns(tenant);

        // Act
        Func<Task> act = () => _service.CreateAddressAsync(request, tenantId);

        // Assert
        await act.Should().ThrowAsync<TenantInactiveException>();
    }

    [Fact]
    public async Task CreateAddressAsync_CallsSaveChangesOnce()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var request = new CreateAddressViewModel();
        var tenant = new Domain.Models.Tenant { Id = tenantId, IsActive = true };

        _tenantRepo.GetByIdAsync(tenantId).Returns(tenant);
        _addressRepo.AddAsync(Arg.Any<Domain.Models.Address>()).Returns(Task.CompletedTask);
        _unitOfWork.SaveChangesAsync().Returns(1);

        // Act
        await _service.CreateAddressAsync(request, tenantId);

        // Assert
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}
