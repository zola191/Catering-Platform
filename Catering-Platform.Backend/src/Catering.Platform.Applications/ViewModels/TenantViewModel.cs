using Catering.Platform.Domain.Models;

namespace Catering.Platform.Applications.ViewModels;

public class TenantViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public static TenantViewModel MapToViewModel(Tenant tenant)
    {
        return new TenantViewModel()
        {
            Id = tenant.Id,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
        };
    }
}
