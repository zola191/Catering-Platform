using AutoFixture;
using Catering.Platform.API.Controllers;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using FluentValidation.Results;
using FluentValidation;
using Catering.Platform.Domain.Requests.Tenant;
using Catering.Platform.Domain.Exceptions;
using NSubstitute.ExceptionExtensions;
using Microsoft.AspNetCore.Http;
using Catering.Platform.Domain.Shared;
using Catering.Platform.API.Validators.Tenants;

namespace Catering.Platform.UnitTests
{
    public class TenantsControllerTests
    {
        //TODO FluentAssertions попрактиковаться

        private readonly IValidator<CreateTenantRequest> _mockCreateTenantRequestValidatior;
        private readonly IValidator<UpdateTenantRequest> _mockUpdateTenantRequestValidatior;
        private readonly ITenantService _mockTenantService;
        private readonly ILogger<TenantsController> _mockLogger;
        private readonly TenantsController _controller;
        private readonly Fixture _fixture;

        public TenantsControllerTests()
        {
            _mockCreateTenantRequestValidatior = Substitute.For<IValidator<CreateTenantRequest>>();
            _mockUpdateTenantRequestValidatior = Substitute.For<IValidator<UpdateTenantRequest>>();
            _mockTenantService = Substitute.For<ITenantService>();
            _mockLogger = Substitute.For<ILogger<TenantsController>>();

            _controller = new TenantsController(
                _mockTenantService,
                _mockCreateTenantRequestValidatior,
                _mockUpdateTenantRequestValidatior,
                _mockLogger);

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
            var result = await _controller.GetById(expectedViewModel.Id);

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
            _mockTenantService.GetByIdAsync(tenantId)
                .Returns(Task.FromResult<TenantViewModel?>(null));

            // Act
            var result = await _controller.GetById(tenantId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsOkResult_WithGuid_WhenValidationSucceeds()
        {
            // Arrange
            var request = _fixture.Create<CreateTenantRequest>();
            var validationResult = new FluentValidation.Results.ValidationResult();
            var expectedResult = Guid.NewGuid();

            _mockCreateTenantRequestValidatior.ValidateAsync(request)
                .Returns(Task.FromResult(validationResult));

            _mockTenantService.AddAsync(request)
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.Create(request);

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
            var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            });

            _mockCreateTenantRequestValidatior.ValidateAsync(request)
                .Returns(Task.FromResult(validationResult));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);

            var errorMessages = errors.Select(e => e.ErrorMessage);
            Assert.Contains("Name is required", errorMessages);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WithCorrectData()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = _fixture.Create<UpdateTenantRequest>();
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockUpdateTenantRequestValidatior.ValidateAsync(request)
                .Returns(Task.FromResult(validationResult));

            _mockTenantService.UpdateAsync(requestId, request)
                .Returns(Task.FromResult(requestId));

            // Act
            var result = await _controller.Update(requestId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGuid = Assert.IsType<Guid>(okResult.Value);
            Assert.Equal(requestId, returnedGuid);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = _fixture.Build<UpdateTenantRequest>()
                .With(f => f.Name, string.Empty)
                .Create();
            var validationResult = new FluentValidation.Results.ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Name", "Name is required")
            });

            _mockUpdateTenantRequestValidatior.ValidateAsync(request)
                .Returns(Task.FromResult(validationResult));

            // Act
            var result = await _controller.Update(requestId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);

            var errorMessages = errors.Select(e => e.ErrorMessage);
            Assert.Contains("Name is required", errorMessages);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenTenantDoesNotExist()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = _fixture.Create<UpdateTenantRequest>();
            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockUpdateTenantRequestValidatior.ValidateAsync(request)
                .Returns(Task.FromResult(validationResult));

            _mockTenantService
                .UpdateAsync(requestId, request)
                .ThrowsAsync(new TenantNotFoundException());

            // Act
            var result = await _controller.Update(requestId, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);

            var problemDetails = Assert.IsType<ProblemDetails>(notFoundResult.Value);
            Assert.Equal("Tenant not found", problemDetails.Title);
            Assert.Equal(StatusCodes.Status404NotFound, problemDetails.Status);
        }

        [Fact]
        public async Task Delete_ExistingTenant_ReturnsNoContent()
        {
            // Arrange
            var tenantId = _fixture.Create<Guid>();
            _mockTenantService.DeleteAsync(tenantId).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(tenantId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Verify
            await _mockTenantService.Received(1).DeleteAsync(tenantId);
        }

        [Fact]
        public async Task Delete_NonExistentTenant_ThrowsTenantNotFoundException()
        {
            // Arrange
            var tenantId = _fixture.Create<Guid>();
            _mockTenantService
                .When(x => x.DeleteAsync(tenantId))
                .Throw(new TenantNotFoundException());

            // Act
            var exception = await Assert.ThrowsAsync<TenantNotFoundException>(
                () => _controller.Delete(tenantId));
            
            // Assert
            Assert.Equal(ErrorMessages.TenantNotFound, exception.Message);
            await _mockTenantService.Received(1).DeleteAsync(tenantId);
        }


        [Fact]
        public async Task Block_ValidRequest_ReturnsOk()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = new BlockTenantRequest { Reason = "Violation" };
            var expectedResult = _fixture.Create<TenantViewModel>();

            _mockTenantService.BlockTenantAsync(tenantId, request)
                .Returns(expectedResult);

            // Act
            var result = await _controller.Block(tenantId, request, new BlockTenantRequestValidator());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task Block_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = new BlockTenantRequest { Reason = "" };
            var validator = new BlockTenantRequestValidator();

            // Act
            var result = await _controller.Block(tenantId, request, validator);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotEmpty((IEnumerable<ValidationFailure>)badRequestResult.Value);
        }

        [Fact]
        public async Task Block_NonExistentTenant_ThrowsNotFoundException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = new BlockTenantRequest { Reason = "Valid reason" };

            _mockTenantService.BlockTenantAsync(tenantId, request)
                .Throws(new TenantNotFoundException());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TenantNotFoundException>(
                () => _controller.Block(tenantId, request, new BlockTenantRequestValidator()));
        }

        [Fact]
        public async Task Block_AlreadyBlocked_ThrowsBadRequest()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var request = new BlockTenantRequest { Reason = "Valid reason" };

            _mockTenantService.BlockTenantAsync(tenantId, request)
                .Throws(new TenantAlreadyBlockException());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TenantAlreadyBlockException>(
                () => _controller.Block(tenantId, request, new BlockTenantRequestValidator()));
        }

        [Fact]
        public async Task UnBlock_ValidRequest_ReturnsOk()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var expectedResult = _fixture.Create<TenantViewModel>();

            _mockTenantService.UnblockTenantAsync(tenantId)
                .Returns(expectedResult);

            // Act
            var result = await _controller.UnBlock(tenantId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
        }

        [Fact]
        public async Task UnBlock_NonExistentTenant_ThrowsNotFoundException()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            _mockTenantService.UnblockTenantAsync(tenantId)
                .Throws(new TenantNotFoundException());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TenantNotFoundException>(
                () => _controller.UnBlock(tenantId));
        }

        [Fact]
        public async Task UnBlock_TenantWithActiveData_ReturnsBadRequest()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            _mockTenantService.UnblockTenantAsync(tenantId)
                .Throws(new TenantHasActiveDataException());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TenantHasActiveDataException>(
                () => _controller.UnBlock(tenantId));
        }
    }
}