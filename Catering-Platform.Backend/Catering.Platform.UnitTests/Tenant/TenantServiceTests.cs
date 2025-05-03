using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Castle.Core.Logging;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Services;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Tenant;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Catering.Platform.UnitTests.Tenant
{
    public class TenantServiceTests
    {
        //TODO FluentAssertions попрактиковаться

        private readonly ITenantRepository _mockRepository;
        private readonly ITenantService _tenantService;
        private readonly IUnitOfWork _mockUnitOfWork;
        private readonly IDistributedCache _mockCache;
        private readonly ILogger<ITenantService> _mockLogger;
        private readonly Fixture _fixture;

        public TenantServiceTests()
        {
            _fixture = new Fixture();
            //Настройка AutoFixture для работы с NSubstitute.
            _fixture.Customize(new AutoNSubstituteCustomization());
            _mockLogger = Substitute.For<ILogger<TenantService>>();
            _mockRepository = Substitute.For<ITenantRepository>();
            _mockCache = Substitute.For<IDistributedCache>();
            _mockUnitOfWork = Substitute.For<IUnitOfWork>();
            _tenantService = new TenantService(_mockRepository, _mockUnitOfWork, _mockCache, _mockLogger);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedTenantViewModels()
        {
            // Arrange
            var tenants = _fixture.CreateMany<Domain.Models.Tenant>(2).ToList();
            _mockRepository.GetAllAsync().Returns(Task.FromResult(tenants));

            // Act
            var result = await _tenantService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tenants.Count, result.Count());

            var resultList = result.ToList();
            for (int i = 0; i < tenants.Count; i++)
            {
                Assert.Equal(tenants[i].Id, resultList[i].Id);
                Assert.Equal(tenants[i].Name, resultList[i].Name);
                Assert.Equal(tenants[i].IsActive, resultList[i].IsActive);
                Assert.Equal(tenants[i].CreatedAt, resultList[i].CreatedAt);
            }
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoTenantsExist()
        {
            // Arrange
            _mockRepository.GetAllAsync().Returns(Task.FromResult(new List<Domain.Models.Tenant>()));

            // Act
            var result = await _tenantService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMappedTenantViewModel_WhenTenantExist()
        {
            // Arrange
            var tenant = _fixture.Create<Domain.Models.Tenant>();
            var tenantId = tenant.Id;
            _mockRepository.GetByIdAsync(tenantId).Returns(Task.FromResult(tenant));

            // Act
            var result = await _tenantService.GetByIdAsync(tenantId);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(tenant.Id, result.Id);
            Assert.Equal(tenant.Name, result.Name);
            Assert.Equal(tenant.IsActive, result.IsActive);
            Assert.Equal(tenant.CreatedAt, result.CreatedAt);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenTenantDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _mockRepository.GetByIdAsync(tenantId).Returns(Task.FromResult<Domain.Models.Tenant>(null));

            // Act
            var result = await _tenantService.GetByIdAsync(tenantId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ReturnsGuid_WhenTenantIsAddedSuccessfully()
        {
            // Arrange
            var request = _fixture.Create<CreateTenantRequest>();
            var tenant = CreateTenantRequest.MapToDomain(request);
            var expectedResult = Guid.NewGuid();

            _mockRepository.AddAsync(Arg.Any<Domain.Models.Tenant>())
                .Returns(Task.FromResult(expectedResult));

            _mockUnitOfWork.SaveChangesAsync()
                .Returns(Task.FromResult(1));

            var service = new TenantService(_mockRepository, _mockUnitOfWork, _mockCache, _mockLogger);

            // Act
            var result = await service.AddAsync(request);

            // Assert
            Assert.Equal(expectedResult, result);

            await _mockRepository.Received(1).AddAsync(Arg.Any<Domain.Models.Tenant>());
            await _mockUnitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateAsync_ReturnsGuid_WhenTenantIsUpdatedSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = _fixture.Create<UpdateTenantRequest>();
            var existingTenant = _fixture.Build<Domain.Models.Tenant>()
                .With(t => t.Id, id)
                .Create();

            _mockRepository.GetByIdAsync(id)
                .Returns(Task.FromResult(existingTenant));

            _mockRepository.Update(existingTenant)
                .Returns(existingTenant.Id);

            _mockUnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(1));

            var service = new TenantService(_mockRepository, _mockUnitOfWork, _mockCache, _mockLogger);

            // Act
            var result = await service.UpdateAsync(id, request);

            // Assert
            Assert.Equal(existingTenant.Id, result);

            await _mockRepository.Received(1).GetByIdAsync(id);
            _mockRepository.Received(1).Update(Arg.Is<Domain.Models.Tenant>(t => t.Id == id));
            await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_ThrowsTenantNotFoundException_WhenTenantDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = _fixture.Create<UpdateTenantRequest>();

            _mockRepository.GetByIdAsync(id)
                .Returns(Task.FromResult<Domain.Models.Tenant>(null));

            var service = new TenantService(_mockRepository, _mockUnitOfWork, _mockCache, _mockLogger);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TenantNotFoundException>(
                () => service.UpdateAsync(id, request));

            Assert.Contains("does not exist", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = _fixture.Create<UpdateTenantRequest>();
            var existingTenant = _fixture.Build<Domain.Models.Tenant>()
                .With(t => t.Id, id)
                .Create();

            _mockRepository.GetByIdAsync(id)
                .Returns(Task.FromResult(existingTenant));

            _mockRepository.Update(existingTenant)
                .Throws(new InvalidOperationException("Database error"));

            var service = new TenantService(_mockRepository, _mockUnitOfWork, _mockCache, _mockLogger);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.UpdateAsync(id, request));

            Assert.Contains("Database error", exception.Message);
        }
        [Fact]
        public async Task DeleteAsync_ExistingTenant_DeletesSuccessfully()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var existingTenant = _fixture.Create<Domain.Models.Tenant>();

            _mockRepository.GetByIdAsync(tenantId).Returns(Task.FromResult(existingTenant));
            _mockUnitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
                         .Returns(Task.FromResult(1));

            // Act
            await _tenantService.DeleteAsync(tenantId);

            // Assert
            _mockRepository.Received(1).Delete(existingTenant);
            await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_NonExistentTenant_ThrowsAndLogs()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _mockRepository.GetByIdAsync(tenantId).Returns((Domain.Models.Tenant?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TenantNotFoundException>(
                () => _tenantService.DeleteAsync(tenantId));

            // Assert
            _mockRepository.DidNotReceiveWithAnyArgs().Delete(Arg.Any<Domain.Models.Tenant>());
            await _mockUnitOfWork.DidNotReceive().SaveChangesAsync();
        }

        [Fact]
        public async Task BlockTenantAsync_ValidRequest_ReturnsViewModel()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = _fixture.Create<BlockTenantRequest>();
            var tenant = _fixture.Build<Domain.Models.Tenant>()
                .With(t => t.Id, tenantId)
                .With(t => t.IsActive, true)
                .Create();

            _mockRepository.BlockAsync(tenantId, request.Reason)
                .Returns(tenant);

            // Act
            var result = await _tenantService.BlockTenantAsync(tenantId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tenant.Id, result.Id);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task BlockTenantAsync_NonExistentTenant_ThrowsNotFoundException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = _fixture.Create<BlockTenantRequest>();

            _mockRepository.BlockAsync(tenantId, request.Reason)
                .Throws(new TenantNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<TenantNotFoundException>(
                () => _tenantService.BlockTenantAsync(tenantId, request));
        }

        [Fact]
        public async Task BlockTenantAsync_RepositoryError_ThrowsException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = _fixture.Create<BlockTenantRequest>();

            _mockRepository.BlockAsync(tenantId, request.Reason)
                .Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _tenantService.BlockTenantAsync(tenantId, request));
        }

        [Fact]
        public async Task UnblockTenantAsync_ValidRequest_ReturnsViewModel()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var tenant = _fixture.Build<Domain.Models.Tenant>()
                .With(t => t.Id, tenantId)
                .With(t => t.IsActive, false)
                .Create();

            _mockRepository.UnBlockAsync(tenantId)
                .Returns(Task.FromResult(tenant))
                .AndDoes(x =>
                {
                    tenant.IsActive = true;
                    tenant.BlockReason = string.Empty;
                    tenant.UpdatedAt = DateTime.UtcNow;
                });

            // Act
            var result = await _tenantService.UnblockTenantAsync(tenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tenantId, result.Id);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task UnblockTenantAsync_NonExistentTenant_ThrowsNotFoundException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            _mockRepository.UnBlockAsync(tenantId)
                .Throws(new TenantNotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<TenantNotFoundException>(
                () => _tenantService.UnblockTenantAsync(tenantId));
        }

        [Fact]
        public async Task UnblockTenantAsync_RepositoryError_ThrowsException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            _mockRepository.UnBlockAsync(tenantId)
                .Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _tenantService.UnblockTenantAsync(tenantId));
        }
    }
}
