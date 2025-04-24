using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Requests.Tenant;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IValidator<CreateTenantRequest> _createTenantRequestValidator;
        private readonly IValidator<UpdateTenantRequest> _updateTenantRequestValidator;
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(
            ITenantService tenantService,
            IValidator<CreateTenantRequest> createTenantRequest,
            IValidator<UpdateTenantRequest> updateTenantRequestValidator,
            ILogger<TenantsController> logger)
        {
            _tenantService = tenantService;
            _createTenantRequestValidator = createTenantRequest;
            _updateTenantRequestValidator = updateTenantRequestValidator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Tenants()
        {
            var viewModels = await _tenantService.GetAllAsync();
            return Ok(viewModels);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById([FromRoute] Guid id)
        {
            var viewModel = await _tenantService.GetByIdAsync(id);
            if (viewModel == null)
            {
                return NotFound();
            }
            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTenantRequest request)
        {
            var validationResult = await _createTenantRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation(
                "Validation failed for CreateTenantRequest. Errors: {ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var result = await _tenantService.AddAsync(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody] UpdateTenantRequest request)
        {
            var validationResult = await _updateTenantRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                var result = await _tenantService.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (TenantNotFoundException ex)
            {
                _logger.LogError(ex, "Tenant not found: {TenantId}", id);
                return NotFound(new ProblemDetails
                {
                    Title = "Tenant not found",
                    Detail = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tenant {TenantId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid id)
        {
            var result = await _tenantService.DeleteAsync(id);
        }
    }
}
