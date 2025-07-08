namespace Catering.Platform.Domain.Requests.Company;

public class UpdateCompanyRequest
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; }
    public string TaxNumber { get; set; }
    public Guid AddressId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public static Models.Company MapToDomain(UpdateCompanyRequest request, Guid tenantId)
    {
        return new Models.Company()
        {
            Id = request.CompanyId,
            TenantId = tenantId,
            Name = request.Name,
            TaxNumber = request.TaxNumber,
            AddressId = request.AddressId,
            Phone = request.Phone,
            Email = request.Email,
            UpdatedAt = DateTime.UtcNow,
        };
    }
}
