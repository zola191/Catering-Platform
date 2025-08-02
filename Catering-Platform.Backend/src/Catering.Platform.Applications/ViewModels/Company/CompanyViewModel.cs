
namespace Catering.Platform.Applications.ViewModels.Company;

public class CompanyViewModel
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public string TaxNumber { get; set; }
    public Guid AddressId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static CompanyViewModel MapToViewModel(Domain.Models.Company company)
    {
        return new CompanyViewModel()
        {
            Id = company.Id,
            TenantId = company.TenantId,
            Name = company.Name,
            TaxNumber = company.TaxNumber,
            AddressId = company.AddressId,
            Phone = company.Phone,
            Email = company.Email,
            IsBlocked = company.IsBlocked,
            CreatedAt = company.CreatedAt,
            UpdatedAt = company.UpdatedAt,
        };
    }
}
