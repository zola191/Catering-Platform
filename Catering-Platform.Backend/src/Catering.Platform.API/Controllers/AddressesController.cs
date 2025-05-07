using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Applications.Services;
using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Models;
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
    private readonly ILogger<AddressesController> _logger;

    public AddressesController(
        IAddressService addressService,
        IValidator<CreateAddressViewModel> createAdressViewModelValidator,
        ILogger<AddressesController> logger)
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
        var addressViewModel = await _addressService.CreateAddressAsync(request, tenantId);
        return Created(
            new Uri($"/api/addresses/{addressViewModel.Id}", UriKind.Relative),
            addressViewModel);
    }
    // GET /api/addresses/{addressId}
    [HttpGet("{addressId:guid}")]
    public async Task<ActionResult> GetById([FromRoute] Guid addressId, [FromBody] UpdateAddressViewModel viewModel)
    {
        var tenantId = Guid.Parse("0196763c-9106-7806-a03f-960a1dad80e7");
        await _addressService.UpdateAddressAsync(addressId, viewModel, tenantId);
        return Ok(viewModel);
    }
}
