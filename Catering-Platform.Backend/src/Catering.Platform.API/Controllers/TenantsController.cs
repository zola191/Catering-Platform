using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Domain.Requests;
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
        private readonly ILogger<TenantsController> _logger;

        public TenantsController(
            ITenantService tenantService, 
            IValidator<CreateTenantRequest> createTenantRequest, 
            ILogger<TenantsController> logger)
        {
            _tenantService = tenantService;
            _createTenantRequestValidator = createTenantRequest;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Tenants()
        {
            var viewModels = await _tenantService.GetAllAsync();
            return Ok(viewModels);
        }

        [HttpGet("{tenantId}")]
        public async Task<ActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var viewModel = await _tenantService.GetByIdAsync(id, ct);
            if (viewModel == null)
            {
                return NotFound();
            }
            return Ok(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateTenantRequest request,
        CancellationToken ct = default)
        {
            var validationResult = await _createTenantRequestValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation(
                "Validation failed for CreateTenantRequest. Errors: {ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var result = await _tenantService.AddAsync(request, ct);
            return Ok(result);
        }
    }
}
