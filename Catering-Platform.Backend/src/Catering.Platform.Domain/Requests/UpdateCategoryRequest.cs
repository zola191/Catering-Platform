namespace Catering.Platform.Domain.Requests;

public class UpdateCategoryRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
}