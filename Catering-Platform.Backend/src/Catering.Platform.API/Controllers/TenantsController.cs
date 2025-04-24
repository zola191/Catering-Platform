using Catering.Platform.Applications.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet]
        public async Task<ActionResult> Tenants()
        {
            var viewModels = await _tenantService.GetAllAsync();
            return Ok(viewModels);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var viewModel = await _tenantService.GetByIdAsync(id, ct);
            if (viewModel == null)
            {
                return NotFound();
            }
            return Ok(viewModel);
        }
    }
}
