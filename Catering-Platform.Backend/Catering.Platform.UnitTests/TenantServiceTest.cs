using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Services;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Catering.Platform.UnitTests
{
    public class TenantServiceTest
    {
        private readonly ITenantRepository _mockRepository;
        private readonly ITenantService _tenantService;
        private readonly Fixture _fixture;

        public TenantServiceTest()
        {
            _fixture = new Fixture();
            _fixture.Customize(new AutoNSubstituteCustomization());

            _mockRepository = Substitute.For<ITenantRepository>();
            _tenantService = new TenantService(_mockRepository, Substitute.For<ILogger<ITenantService>>());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedTenantViewModels()
        {
            // Arrange
            var tenants = _fixture.CreateMany<Tenant>(2).ToList();
            _mockRepository.GetAllAsync().Returns(Task.FromResult<IEnumerable<Tenant>>(tenants));

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
            _mockRepository.GetAllAsync().Returns(Task.FromResult<IEnumerable<Tenant>>(new List<Tenant>()));

            // Act
            var result = await _tenantService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
