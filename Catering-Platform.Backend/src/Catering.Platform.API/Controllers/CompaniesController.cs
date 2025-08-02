using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Domain.Requests.Company;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IValidator<CreateCompanyRequest> _createCompanyvalidator;
        private readonly IValidator<UpdateCompanyRequest> _updateCompanyvalidator;
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        public CompaniesController(
            IValidator<CreateCompanyRequest> createCompanyvalidator,
            IValidator<UpdateCompanyRequest> updateCompanyvalidator,
            ICompanyService companyService,
            ILogger<CompaniesController> logger)
        {
            _createCompanyvalidator = createCompanyvalidator;
            _updateCompanyvalidator = updateCompanyvalidator;
            _companyService = companyService;
            _logger = logger;
            _updateCompanyvalidator = updateCompanyvalidator;
        }

        [HttpGet("{companyId:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var tenantId = Guid.NewGuid();
            var companyViewModel = await _companyService.GetCompanyByIdAsync(id, tenantId);
            return Ok(companyViewModel);
        }

        [HttpGet("by-tax-number")]
        public async Task<IActionResult> GetByTaxNumber([FromQuery] string taxNumber)
        {
            var guid = Guid.NewGuid();
            var company = await _companyService.GetCompanyByTaxNumberAsync(taxNumber, guid);
            return Ok(company);
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

        [HttpPut("{companyId:guid}")]
        public async Task<IActionResult> Update([FromBody] UpdateCompanyRequest request)
        {
            var tenantId = Guid.NewGuid();
            var validationResult = await _updateCompanyvalidator.ValidateAsync(request);
            if (validationResult.IsValid == false)
            {
                _logger.LogInformation(
                "Validation failed for UpdateCompany. Errors: {@ValidationErrors}",
                validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var companyViewModel = await _companyService.UpdateCompanyAsync(request, tenantId);
            return Ok(companyViewModel);
        }
    }
}
