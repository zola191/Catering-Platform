using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests;

public record CreateCategoryRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public static Category MapToDomain(CreateCategoryRequest category)
    {
        return new Category()
        {
            Name = category.Name,
            Description = category.Description,
        };
    }
}