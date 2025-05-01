using AutoFixture;
using Catering.Platform.API.Controllers;
using Catering.Platform.API.Validators.Tenants;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using FluentValidation.Results;
using FluentValidation;

namespace Catering.Platform.UnitTests
{
    public class TenantsControllerTests
    {
        private readonly IValidator<CreateTenantRequest> _mockCreateTenantRequestValidatior;
        private readonly ITenantService _mockTenantService;
        private readonly ILogger<TenantsController> _mockLogger;
        private readonly TenantsController _controller;
        private readonly Fixture _fixture;

        public TenantsControllerTests()
        {
            _mockCreateTenantRequestValidatior = Substitute.For<IValidator<CreateTenantRequest>>();
            _mockTenantService = Substitute.For<ITenantService>();
            _mockLogger = Substitute.For<ILogger<TenantsController>>();
            _controller = new TenantsController(_mockTenantService, _mockCreateTenantRequestValidatior, _mockLogger);
            _fixture = new Fixture();
        }

        [Fact]
        public async Task Tenants_ReturnsOkResult_WithCorrectData()
        {
            // Arrange
            var expectedViewModels = _fixture.CreateMany<TenantViewModel>(2).AsEnumerable();

            _mockTenantService.GetAllAsync().Returns(Task.FromResult(expectedViewModels));

            // Act
            var result = await _controller.Tenants();

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
            IEnumerable<TenantViewModel> emptyViewModels = Enumerable.Empty<TenantViewModel>();

            _mockTenantService.GetAllAsync().Returns(Task.FromResult(emptyViewModels));

            // Act
            var result = await _controller.Tenants();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedViewModels = Assert.IsAssignableFrom<IEnumerable<TenantViewModel>>(okResult.Value);
            Assert.Empty(returnedViewModels);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithCorrectData()
        {
            // Arrange
            var expectedViewModel = _fixture.Create<TenantViewModel>();

            _mockTenantService.GetByIdAsync(expectedViewModel.Id)
                .Returns(Task.FromResult<TenantViewModel?>(expectedViewModel));

            // Act
            var result = await _controller.GetById(expectedViewModel.Id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedViewModel = Assert.IsAssignableFrom<TenantViewModel>(okResult.Value);

            Assert.Equal(expectedViewModel.Id, returnedViewModel.Id);
            Assert.Equal(expectedViewModel.Name, returnedViewModel.Name);
            Assert.Equal(expectedViewModel.IsActive, returnedViewModel.IsActive);
            Assert.Equal(expectedViewModel.CreatedAt, returnedViewModel.CreatedAt);
        }

        [Fact]
        public async Task GetById_ReturnsNotFoundResult_WhenTenantDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _mockTenantService.GetByIdAsync(tenantId, CancellationToken.None)
                .Returns(Task.FromResult<TenantViewModel?>(null));

            // Act
            var result = await _controller.GetById(tenantId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WithGuid_WhenValidationSucceeds()
        {
            // Arrange
            var request = _fixture.Create<CreateTenantRequest>();
            var validationResult = new ValidationResult();
            var expectedResult = Guid.NewGuid();

            _mockCreateTenantRequestValidatior.ValidateAsync(request, CancellationToken.None)
                .Returns(Task.FromResult(validationResult));

            _mockTenantService.AddAsync(request, CancellationToken.None)
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedGuid = Assert.IsType<Guid>(okResult.Value);
            Assert.Equal(expectedResult, returnedGuid);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = _fixture.Build<CreateTenantRequest>()
                .With(f => f.Name, string.Empty)
                .Create();
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            });

            _mockCreateTenantRequestValidatior.ValidateAsync(request, CancellationToken.None)
                .Returns(Task.FromResult(validationResult));

            // Act
            var result = await _controller.Create(request, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);

            var errorMessages = errors.Select(e => e.ErrorMessage);
            Assert.Contains("Name is required", errorMessages);
        }
    }
}