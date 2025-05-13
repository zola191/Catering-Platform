namespace Catering.Platform.Applications.ViewModels;

public record SearchByTextViewModel
{
    public Guid? Id { get; init; }
    public string Query { get; init; }
}
