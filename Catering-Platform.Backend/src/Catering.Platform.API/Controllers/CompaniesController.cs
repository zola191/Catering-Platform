using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Domain.Requests.Company;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IValidator<CreateCompanyRequest> _createCompanyvalidator;
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(
            IValidator<CreateCompanyRequest> createCompanyvalidator,
            ICompanyService companyService,
            ILogger<CompaniesController> logger)
        {
            _createCompanyvalidator = createCompanyvalidator;
            _companyService = companyService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCompanyRequest request)
        {
            var tenantId = Guid.NewGuid();

            var validationResult = await _createCompanyvalidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                _logger.LogInformation(
                "Validation failed for CreateCompany. Errors: {@ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var companyViewModel = await _companyService.CreateCompanyAsync(request, tenantId);
            return Created(
                new Uri($"/api/company/{companyViewModel.Id}", UriKind.Relative),
                companyViewModel);
        }
    }
}
