using Catering.Platform.Applications.Abstractions;
using Catering.Platform.Domain.Requests.Dish;
using Microsoft.AspNetCore.Mvc;

namespace Catering.Platform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DishesController : ControllerBase
{
    private readonly IDishService _dishService;

    public DishesController(IDishService dishService)
    {
        _dishService = dishService;
    }

    [HttpGet]
    public async Task<ActionResult> Dishes()
    {
        var result = await _dishService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateDishRequest request,
        CancellationToken ct = default)
    {
        var result = await _dishService.AddAsync(request, ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDishRequest request,
        CancellationToken ct = default)
    {
        var result = await _dishService.UpdateAsync(id, request, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var result = await _dishService.DeleteAsync(id, ct);
        return Ok(result);
    }
}