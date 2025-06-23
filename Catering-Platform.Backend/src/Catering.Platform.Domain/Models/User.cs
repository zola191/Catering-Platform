namespace Catering.Platform.Domain.Models;

public class User : Entity
{
    public Tenant Tenant { get; set; }
    public Guid TenantId { get; set; }
    public string FirstName {  get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Email { get; set; }
    public string Phone {  get; set; }
    public string PasswordHash { get; set; }
    public bool IsBlocked { get; set; } = false;
    public string? BlockReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt  { get; set; }
}
