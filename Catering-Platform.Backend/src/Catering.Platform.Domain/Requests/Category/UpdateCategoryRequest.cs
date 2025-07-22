namespace Catering.Platform.Domain.Requests.Category;

public record UpdateCategoryRequest
{
    public string Name { get; init; }
    public string? Description { get; init; }
}