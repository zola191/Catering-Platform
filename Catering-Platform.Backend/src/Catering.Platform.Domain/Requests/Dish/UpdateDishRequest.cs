using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests.Dish;

public record UpdateDishRequest
{
    public string Name { get; init; }
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsAvailable { get; init; }
    public List<string> Ingredients { get; init; }
    public List<string> Allergens { get; init; }
    public string? PortionSize { get; init; }

    public static void MapToDomain(Models.Dish entity, UpdateDishRequest request)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.CategoryId = request.CategoryId;
        entity.ImageUrl = request.ImageUrl;
        entity.IsAvailable = request.IsAvailable;
        entity.Ingredients = new IngredientList(request.Ingredients);
        entity.Allergens = new AllergenList(request.Allergens);
        entity.PortionSize = request.PortionSize;
    }
}