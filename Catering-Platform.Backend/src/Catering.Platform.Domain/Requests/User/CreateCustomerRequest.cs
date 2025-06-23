using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests.User;

public class CreateCustomerRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? AddressId { get; set; }
    public int TaxNumber { get; set; }
    public Guid TenantId { get; set; }

    public static Customer MapToDomain(CreateCustomerRequest request)
    {
        return new Customer()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email,
            Phone = request.Phone,
            CreatedAt = DateTime.UtcNow,
            CompanyId = request.CompanyId,
            AddressId = request.AddressId,
            TaxNumber = request.TaxNumber,
            TenantId = request.TenantId,
        };
    }
}
