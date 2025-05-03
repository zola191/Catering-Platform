using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Domain.Requests.Adress;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;
    private readonly IValidator<CreateAddressViewModel> createAdressViewModelValidator;
    private readonly ILogger<TenantsController> _logger;

    public AddressesController(
        IAddressService addressService,
        IValidator<CreateAddressViewModel> createAdressViewModelValidator,
        ILogger<TenantsController> logger)
    {
        _addressService = addressService;
        this.createAdressViewModelValidator = createAdressViewModelValidator;
        _logger = logger;
    }

    [HttpPost("{tenantId}")]
    public async Task<IActionResult> Create(
    [FromRoute] Guid tenantId,
    [FromBody] CreateAddressViewModel request)
    {
        var validationResult = await createAdressViewModelValidator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            _logger.LogInformation(
            "Validation failed for CreateAddressViewModel. Errors: {ValidationErrors}",
            validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }
        try
        {
            var addressId = await _addressService.CreateAddressAsync(request, tenantId);
            return Created(new Uri($"/api/addresses/{addressId}", UriKind.Relative), addressId);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
