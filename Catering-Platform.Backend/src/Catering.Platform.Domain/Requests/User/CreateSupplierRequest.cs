using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests.User;

public record CreateSupplierRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public Guid CompanyId { get; set; }
    public string Position { get; set; }

    public static Supplier MapToDomain(CreateSupplierRequest request)
    {
        return new Supplier()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email,
            Phone = request.Phone,
            CreatedAt = DateTime.Now,
            CompanyId = request.CompanyId,
            Position = request.Position,
        };
    }
}
