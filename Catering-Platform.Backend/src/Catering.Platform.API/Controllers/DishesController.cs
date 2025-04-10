using Catering.Platform.API.Models;
using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Requests;
using Catering.Platform.Domain.Services;
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
    public async Task<ActionResult> Dishes(CancellationToken ct = default)
    {
        var result = await _dishService.GetAllAsync(ct);
        var dishViewModels = result.Select(d => new DishViewModel()
        {
            Name = d.Name,
            Description = d.Description,
            Price = d.Price,
            CategoryId = d.CategoryId,
            ImageUrl = d.ImageUrl,
            IsAvailable = d.IsAvailable,
            Ingredients = d.Ingredients.Items,
            Allergens = d.Allergens.Items,
            PortionSize = d.PortionSize
        });
        return Ok(dishViewModels);
    }
    

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody]CreateDishRequest request, 
        CancellationToken ct = default)
    {
        var dish = new Dish()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            ImageUrl = request.ImageUrl,
            IsAvailable = request.IsAvailable,
            Ingredients = new IngredientList(request.Ingredients),
            Allergens = new AllergenList(request.Allergens),
            PortionSize = request.PortionSize
        };
        var result = await _dishService.AddAsync(dish, ct);
        return result;
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> Update(
        [FromRoute] Guid id,
        UpdateDishRequest request, 
        CancellationToken ct = default)
    {
        var existingDish = await _dishService.GetByIdAsync(id, ct);
        if (existingDish != null)
        {
            existingDish.Name = request.Name;
            existingDish.Description = request.Description;
            existingDish.Price = request.Price;
            existingDish.CategoryId = request.CategoryId;
            existingDish.ImageUrl = request.ImageUrl;
            existingDish.IsAvailable = request.IsAvailable;
            existingDish.Ingredients = existingDish.Ingredients.UpdateItems(request.Ingredients);
            existingDish.Allergens = existingDish.Allergens.UpdateItems(request.Allergens);;
            existingDish.PortionSize = request.PortionSize;

            await _dishService.UpdateAsync(existingDish, ct);

            return Ok(existingDish.Id);
        }

        return BadRequest();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> Delete(
        [FromRoute] Guid id,
        CancellationToken ct = default)
    {
        var existingDish = await _dishService.GetByIdAsync(id, ct);
        if (existingDish != null)
        {
            await _dishService.DeleteAsync(existingDish, ct);
            return Ok(existingDish.CategoryId);
        }

        return BadRequest();
    }
}