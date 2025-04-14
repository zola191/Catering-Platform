using MediatR;

namespace Catering.Platform.Applications.Features.Categories.Delete;

public record DeleteCategoryCommand : IRequest<Guid>
{
    public Guid Id { get; init; }
}