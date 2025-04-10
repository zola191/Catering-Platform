using Catering.Platform.Domain.Models;

namespace Catering.Platform.API.Models;

public class CategoryViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }


    public static CategoryViewModel MapFrom(Category category)
    {
        return new CategoryViewModel()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
        };
    }

    //extensions для dto лучше не использовать
    //extensions для legacy
}