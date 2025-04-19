namespace Catering.Platform.Domain.Models;

public class Dish : Entity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string? ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public IngredientList Ingredients { get; set; }
    public AllergenList Allergens { get; set; }
    public string PortionSize { get; set; }
}