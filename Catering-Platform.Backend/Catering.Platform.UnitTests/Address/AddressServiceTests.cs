using AutoFixture;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Services;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Adress;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Net;

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
        _logger = NullLogger<AddressService>.Instance;
        _fixture = new Fixture();

        _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

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

    [Fact]
    public async Task UpdateAddressAsync_ReturnsViewModel_WhenCorrectData()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        var request = _fixture.Build<UpdateAddressViewModel>()
            .With(x => x.Country, "Russia")
            .With(x => x.StreetAndBuilding, "Lenina st., 10")
            .With(x => x.Zip, "123456")
            .With(x => x.City, "Moscow")
            .With(x => x.Region, "Moscow Region")
            .With(x => x.Comment, "Back entrance")
            .With(x => x.Description, "Main office")
            .Create();

        var existingAddress = _fixture.Build<Domain.Models.Address>()
            .With(a => a.Id, addressId)
            .With(a => a.TenantId, tenantId)
            .Without(a => a.Tenant)
            .Create();

        var existingTenant = _fixture.Build<Domain.Models.Tenant>()
            .With(t => t.Id, tenantId)
            .With(t => t.IsActive, true)
            .With(t => t.Addresses, new List<Domain.Models.Address> { existingAddress })
            .Create();

        _tenantRepo.GetByIdWithAddresses(tenantId).Returns(existingTenant);

        // Act
        var result = await _service.UpdateAddressAsync(addressId, request, tenantId);

        // Assert
        existingAddress.Country.Should().Be(request.Country);
        existingAddress.StreetAndBuilding.Should().Be(request.StreetAndBuilding);
        existingAddress.Zip.Should().Be(request.Zip);
        existingAddress.City.Should().Be(request.City);
        existingAddress.Region.Should().Be(request.Region);
        existingAddress.Comment.Should().Be(request.Comment);
        existingAddress.Description.Should().Be(request.Description);

        result.Should().NotBeNull();
        result.Id.Should().Be(existingAddress.Id);
        result.TenantId.Should().Be(existingAddress.TenantId);
        result.Country.Should().Be(existingAddress.Country);
        result.StreetAndBuilding.Should().Be(existingAddress.StreetAndBuilding);
        result.Zip.Should().Be(existingAddress.Zip);
        result.City.Should().Be(existingAddress.City);
        result.Region.Should().Be(existingAddress.Region);
        result.Comment.Should().Be(existingAddress.Comment);
        result.Description.Should().Be(existingAddress.Description);
    }

    [Fact]
    public async Task UpdateAddressAsync_ThrowsAddressNotFoundException_WhenAddressDoesNotExist()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        var request = _fixture.Create<UpdateAddressViewModel>();

        var existingTenant = _fixture.Build<Domain.Models.Tenant>()
            .With(t => t.Id, tenantId)
            .With(t => t.IsActive, true)
            .With(t => t.Addresses, new List<Domain.Models.Address>())
            .Create();

        _tenantRepo.GetByIdWithAddresses(tenantId).Returns(existingTenant);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.UpdateAddressAsync(addressId, request, tenantId));

        Assert.Contains("Address", exception.Message);
        Assert.Equal(addressId, exception.EntityId);
    }
}
