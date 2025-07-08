using Catering.Platform.API.Validators.Address;
using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Requests.Adress;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;
    private readonly IValidator<CreateAddressViewModel> _createAdressViewModelValidator;
    private readonly IValidator<UpdateAddressViewModel> _updateAdressViewModelValidator;
    private readonly ILogger<AddressesController> _logger;

    public AddressesController(
        IAddressService addressService,
        IValidator<CreateAddressViewModel> createAdressViewModelValidator,
        IValidator<UpdateAddressViewModel> updateAdressViewModelValidator,
        ILogger<AddressesController> logger)
    {
        _addressService = addressService;
        _createAdressViewModelValidator = createAdressViewModelValidator;
        _updateAdressViewModelValidator = updateAdressViewModelValidator;
        _logger = logger;
    }

    [HttpPost("{tenantId}")]
    public async Task<IActionResult> Create(
    [FromRoute] Guid tenantId,
    [FromBody] CreateAddressViewModel request)
    {
        var validationResult = await _createAdressViewModelValidator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            _logger.LogWarning(
            "Validation failed for CreateAddressViewModel. TenantId: {TenantId}, Errors: {@ValidationErrors}",
            tenantId,
            validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }
        var addressViewModel = await _addressService.CreateAddressAsync(request, tenantId);
        return Created(
            new Uri($"/api/addresses/{addressViewModel.Id}", UriKind.Relative),
            addressViewModel);
    }

    [HttpPut("{addressId:guid}")]
    public async Task<ActionResult> Update(
    [FromRoute] Guid addressId,
    [FromBody] UpdateAddressViewModel request)
    {
        var validationResult = await _updateAdressViewModelValidator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            _logger.LogWarning(
            "Validation failed for UpdateAddressViewModel. TenantId: {TenantId}, Errors: {@ValidationErrors}",
            addressId,
            validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }
        var tenantId = Guid.Parse("0196763c-9106-7806-a03f-960a1dad80e7");
        var result = await _addressService.UpdateAddressAsync(addressId, request, tenantId);
        return Ok(result);
    }

    [HttpDelete("{addressId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid addressId)
    {
        var tenantId = Guid.Parse("0196763c-9106-7806-a03f-960a1dad80e7");
        await _addressService.DeleteAddressAsync(addressId, tenantId);
        return NoContent();
    }

    [HttpGet("{addressId:guid}")]
    public async Task<ActionResult> Get([FromRoute] Guid addressId)
    {
        var tenantId = Guid.Parse("0196763c-9106-7806-a03f-960a1dad80e7");
        var result = await _addressService.GetAddressByIdAsync(addressId, tenantId);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
    [FromQuery] Guid? tenantId,
    [FromQuery] string query)
    {
        var request = new SearchByTextViewModel
        {
            Id = tenantId,
            Query = query
        };
        // временно заглушкой передал tenantId вторым параметров в SearchAddressesByTextAsync
        var result = await _addressService.SearchAddressesByTextAsync(request, tenantId);
        return Ok(result);
    }

    [HttpGet("search-by-zip")]
    public async Task<IActionResult> SearchByZip(
    [FromServices] SearchByZipViewModelValidator zipValidator,
    [FromQuery] Guid? tenantId,
    [FromQuery] string zip)
    {
        var request = new SearchByZipViewModel
        {
            Id = tenantId,
            Zip = zip
        };

        var validationResult = await zipValidator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            _logger.LogWarning(
            "Validation failed for SearchByZip. TenantId: {TenantId}, Errors: {@ValidationErrors}",
            tenantId,
            validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        // временно заглушкой передал tenantId вторым параметров в SearchByZip
        var result = await _addressService.SearchAddressesByZipAsync(request, tenantId);
        return Ok(result);
    }
}
