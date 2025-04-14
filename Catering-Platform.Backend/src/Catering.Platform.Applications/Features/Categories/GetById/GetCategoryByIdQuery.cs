using Catering.Platform.Domain.Models;
using MediatR;

namespace Catering.Platform.Applications.Features.Categories.GetById;

public record GetCategoryByIdQuery : IRequest<Category>
{
    public Guid Id { get; init; }
}