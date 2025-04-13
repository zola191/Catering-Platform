using Catering.Platform.Domain.Models;
using MediatR;

namespace Catering.Platform.Applications.Features.Categories.Create;

public class CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public static Category MapToDomain(CreateCategoryCommand category)
    {
        return new Category()
        {
            Name = category.Name,
            Description = category.Description,
        };
    }
}
