using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests;

public record UpdateDishRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public List<string> Ingredients { get; set; }
    public List<string> Allergens { get; set; }
    public string PortionSize { get; set; }

    public static void UpdateEntity(Dish entity, UpdateDishRequest request)
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