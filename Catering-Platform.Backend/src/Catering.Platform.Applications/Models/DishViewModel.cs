using Catering.Platform.Domain.Models;

namespace Catering.Platform.Applications.Models;

public record DishViewModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public List<string> Ingredients { get; set; }
    public List<string> Allergens { get; set; }
    public string PortionSize { get; set; }

    public static DishViewModel MapToViewModel(Dish dish)
    {
        return new DishViewModel()
        {
            Name = dish.Name,
            Description = dish.Description,
            Price = dish.Price,
            CategoryId = dish.CategoryId,
            ImageUrl = dish.ImageUrl,
            IsAvailable = dish.IsAvailable,
            Ingredients = dish.Ingredients.Items,
            Allergens = dish.Allergens.Items,
            PortionSize = dish.PortionSize
        };
    }
}