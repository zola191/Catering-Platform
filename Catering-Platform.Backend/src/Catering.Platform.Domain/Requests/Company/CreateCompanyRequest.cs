namespace Catering.Platform.Domain.Requests.Company;

public class CreateCompanyRequest
{
    public Guid TenantId { get; set; }
    public string Name { get; set; }
    public string TaxNumber { get; set; }
    public Guid AddressId { get; set; }
    public string? Phone {  get; set; }
    public string? Email { get; set; }

    public static Models.Company MapToDomain(CreateCompanyRequest request, Guid tenantId)
    {
        return new Models.Company()
        {
            TenantId = tenantId,
            Name = request.Name,
            TaxNumber = request.TaxNumber,
            AddressId = request.AddressId,
            Phone = request.Phone,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
