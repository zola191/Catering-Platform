namespace Catering.Platform.API.ViewModels;

public class TenantViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
