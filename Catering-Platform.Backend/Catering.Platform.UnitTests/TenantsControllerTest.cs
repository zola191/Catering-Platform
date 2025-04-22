using AutoFixture;
using Catering.Platform.API.Controllers;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Catering.Platform.UnitTests
{
    public class TenantsControllerTest
    {
        [Fact]
        public async Task Tenants_ReturnsOkResult_WithCorrectData()
        {
            // Arrange
            var fixture = new Fixture();
            var mockTenantService = Substitute.For<ITenantService>();

            var expectedViewModels = fixture.CreateMany<TenantViewModel>(2).AsEnumerable();

            mockTenantService.GetAllAsync().Returns(Task.FromResult(expectedViewModels));

            var controller = new TenantsController(mockTenantService);

            // Act
            var result = await controller.Tenants();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedViewModels = Assert.IsAssignableFrom<IEnumerable<TenantViewModel>>(okResult.Value);

            Assert.Equal(expectedViewModels.Count(), returnedViewModels.Count());
            for (int i = 0; i < expectedViewModels.Count(); i++)
            {
                Assert.Equal(expectedViewModels.ElementAt(i).Id, returnedViewModels.ElementAt(i).Id);
                Assert.Equal(expectedViewModels.ElementAt(i).Name, returnedViewModels.ElementAt(i).Name);
                Assert.Equal(expectedViewModels.ElementAt(i).IsActive, returnedViewModels.ElementAt(i).IsActive);
                Assert.Equal(expectedViewModels.ElementAt(i).CreatedAt, returnedViewModels.ElementAt(i).CreatedAt);
            }
        }

        [Fact]
        public async Task Tenants_ReturnsOkResult_WithEmptyData()
        {
            // Arrange
            var mockTenantService = Substitute.For<ITenantService>();
            IEnumerable<TenantViewModel> emptyViewModels = Enumerable.Empty<TenantViewModel>();

            mockTenantService.GetAllAsync().Returns(Task.FromResult(emptyViewModels));

            var controller = new TenantsController(mockTenantService);

            // Act
            var result = await controller.Tenants();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedViewModels = Assert.IsAssignableFrom<IEnumerable<TenantViewModel>>(okResult.Value);
            Assert.Empty(returnedViewModels);
        }
    }
}