namespace Catering.Platform.Domain.Models;

public class Company
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
    public string Name { get; set; }
    public string TaxNumber { get; set; }
    public Guid AddressId { get; set; }
    public Address Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsBlocked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
