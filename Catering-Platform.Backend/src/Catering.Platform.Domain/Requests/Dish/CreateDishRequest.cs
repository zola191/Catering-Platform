using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests.Dish;

public record CreateDishRequest
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

    public static Models.Dish MapToDomain(CreateDishRequest request)
    {
        return new Models.Dish()
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
    }
}