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
    }
}
