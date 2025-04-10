namespace Catering.Platform.Domain.Requests;

public class CreateCategoryRequest
{
    public string Name { get; set; }
    public string? Description { get; set; }
}