using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests;

public record UpdateCategoryCommand
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public static Category UpdateFrom(Category category)
    {
        return new Category()
        {
            Name = category.Name,
            Description = category.Description,
        };
    }
}