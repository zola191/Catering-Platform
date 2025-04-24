using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Services;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Catering.Platform.Domain.Requests.Tenant;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Catering.Platform.UnitTests
{
    public class TenantServiceTests
    {
        private readonly ITenantRepository _mockRepository;
        private readonly ITenantService _mockTenantService;
        private readonly IUnitOfWork _mockUnitOfWork;
        private readonly ILogger<ITenantService> _mockLogger;
        private readonly Fixture _fixture;

        public TenantServiceTests()
        {
            _fixture = new Fixture();
            //Настройка AutoFixture для работы с NSubstitute.
            _fixture.Customize(new AutoNSubstituteCustomization());
            _mockLogger = Substitute.For<ILogger<TenantService>>();
            _mockRepository = Substitute.For<ITenantRepository>();
            _mockUnitOfWork = Substitute.For<IUnitOfWork>();
            _mockTenantService = new TenantService(_mockRepository, _mockUnitOfWork, _mockLogger);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedTenantViewModels()
        {
            // Arrange
            var tenants = _fixture.CreateMany<Tenant>(2).ToList();
            _mockRepository.GetAllAsync().Returns(Task.FromResult(tenants));

            // Act
            var result = await _mockTenantService.GetAllAsync();

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
            _mockRepository.GetAllAsync().Returns(Task.FromResult<List<Tenant>>(new List<Tenant>()));

            // Act
            var result = await _mockTenantService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMappedTenantViewModel_WhenTenantExist()
        {
            // Arrange
            var tenant = _fixture.Create<Tenant>();
            var tenantId = tenant.Id;
            _mockRepository.GetByIdAsync(tenantId, CancellationToken.None).Returns(Task.FromResult(tenant));

            // Act
            var result = await _mockTenantService.GetByIdAsync(tenantId, CancellationToken.None);

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
            _mockRepository.GetByIdAsync(tenantId, CancellationToken.None).Returns(Task.FromResult<Tenant>(null));

            // Act
            var result = await _mockTenantService.GetByIdAsync(tenantId, CancellationToken.None);

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

            _mockRepository.AddAsync(Arg.Any<Tenant>(), CancellationToken.None)
                .Returns(Task.FromResult(expectedResult));

            _mockUnitOfWork.SaveChangesAsync(CancellationToken.None)
                .Returns(Task.FromResult(1));

            var service = new TenantService(_mockRepository, _mockUnitOfWork, _mockLogger);

            // Act
            var result = await service.AddAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResult, result);

            await _mockRepository.Received(1).AddAsync(Arg.Any<Tenant>(), Arg.Any<CancellationToken>());
            await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }
}
