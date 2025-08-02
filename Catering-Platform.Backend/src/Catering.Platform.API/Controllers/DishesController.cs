using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Domain.Requests.Dish;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DishesController : ControllerBase
{
    private readonly IDishService _dishService;
    private readonly IValidator<CreateDishRequest> _createDishRequestValidator;
    private readonly IValidator<UpdateDishRequest> _updateDishRequestValidator;
    private readonly ILogger<DishesController> _logger;

    public DishesController(
        IDishService dishService,
        IValidator<CreateDishRequest> createDishRequestValidator,
        IValidator<UpdateDishRequest> updateDishRequestValidator,
        ILogger<DishesController> logger)
    {
        _dishService = dishService;
        _createDishRequestValidator = createDishRequestValidator;
        _updateDishRequestValidator = updateDishRequestValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> Dishes()
    {
        var result = await _dishService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateDishRequest request)
    {
        var validationResult = await _createDishRequestValidator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            _logger.LogWarning(
            "Validation failed for CreateDishRequest. Errors: {@ValidationErrors}",
            validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }
        var result = await _dishService.AddAsync(request);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDishRequest request)
    {
        var validationResult = await _updateDishRequestValidator.ValidateAsync(request);
        if (validationResult.IsValid == false)
        {
            _logger.LogWarning(
            "Validation failed for UpdateDishRequest. Errors: {@ValidationErrors}",
            validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }
        var result = await _dishService.UpdateAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid id)
    {
        var result = await _dishService.DeleteAsync(id);
        return Ok(result);
    }
}