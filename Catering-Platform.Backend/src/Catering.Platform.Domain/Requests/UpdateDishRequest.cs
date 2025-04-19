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

    public static Dish UpdateFrom(Dish dish)
    {
        return new Dish()
        {
            Name = dish.Name,
            Description = dish.Description,
            Price = dish.Price,
            CategoryId = dish.CategoryId,
            ImageUrl = dish.ImageUrl,
            IsAvailable = dish.IsAvailable,
            Ingredients = new IngredientList(dish.Ingredients.Items),
            Allergens = new AllergenList(dish.Ingredients.Items),
            PortionSize = dish.PortionSize
        };
    }
}