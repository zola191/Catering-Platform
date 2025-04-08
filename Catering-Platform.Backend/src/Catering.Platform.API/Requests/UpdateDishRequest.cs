namespace Catering.Platform.API.Requests;

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
}