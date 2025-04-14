using Catering.Platform.Domain.Models;
using MediatR;

namespace Catering.Platform.Applications.Features.Categories.Update;

public record UpdateCategoryCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    
    public static void UpdateEntity(Category entity, UpdateCategoryCommand request)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
    }
}