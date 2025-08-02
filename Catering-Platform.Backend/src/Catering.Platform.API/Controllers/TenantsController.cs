using Catering.Platform.API.Validators.Tenants;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Domain.Exceptions;
using Catering.Platform.Domain.Requests.Tenant;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Tenants()
        {
            //чтение claim
            //чтение Token
            var context = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var token = ExtractJwtToken(HttpContext);
            var viewModels = await _tenantService.GetAllAsync();
            return Ok(viewModels);
        }

        private string ExtractJwtToken(HttpContext httpContext)
        {
            var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return null;
            }

            // Извлекаем токен из заголовка "Bearer <token>"
            return authorizationHeader.Substring("Bearer ".Length);
        }


        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById([FromRoute] Guid id)
        {
            var viewModel = await _tenantService.GetByIdAsync(id);
            if (viewModel == null)
            {
                _logger.LogWarning(
                "Tenant is not found {Id}", id);
                return NotFound();
            }
            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTenantRequest request)
        {
            var validationResult = await _createTenantRequestValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                _logger.LogWarning(
                "Validation failed for CreateTenantRequest. Errors: {@ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var result = await _tenantService.AddAsync(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateTenantRequest request)
        {
            var validationResult = await _updateTenantRequestValidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
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
                _logger.LogWarning(ex, "Tenant not found: {TenantId}", id);
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
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _tenantService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id:guid}/block")]
        public async Task<ActionResult> Block(
            [FromRoute] Guid id,
            [FromBody] BlockTenantRequest request,
            [FromServices] BlockTenantRequestValidator validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                _logger.LogWarning(
                "Validation failed for BlockTenantRequest. Errors: {@ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var result = await _tenantService.BlockTenantAsync(id, request);
            return Ok(result);
        }

        [HttpPut("{id:guid}/unblock")]
        public async Task<ActionResult> UnBlock([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Tenant ID cannot be empty.");
            }
            var result = await _tenantService.UnblockTenantAsync(id);
            return Ok(result);
        }
    }
}
