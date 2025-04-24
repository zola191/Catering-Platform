namespace Catering.Platform.Domain.Requests.Dish;

public record CreateDishRequest
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