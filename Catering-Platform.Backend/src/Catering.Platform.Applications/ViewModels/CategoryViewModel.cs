using Catering.Platform.Domain.Models;

namespace Catering.Platform.Applications.ViewModels;

public record CategoryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public static CategoryViewModel MapToViewModel(Category category)
    {
        return new CategoryViewModel()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
        };
    }
}