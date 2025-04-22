using Catering.Platform.Domain.Models;
using MediatR;

namespace Catering.Platform.Applications.Features.Categories.Create;

public record CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; init; }
    public string? Description { get; init; }

    public static Category MapToDomain(CreateCategoryCommand category)
    {
        return new Category()
        {
            Name = category.Name,
            Description = category.Description,
        };
    }
}
