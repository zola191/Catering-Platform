using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests;

public record UpdateCategoryRequest
{
    public string Name { get; init; }
    public string? Description { get; init; }
}