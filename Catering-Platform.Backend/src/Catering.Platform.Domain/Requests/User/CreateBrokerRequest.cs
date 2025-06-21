using Catering.Platform.Domain.Models;

namespace Catering.Platform.Domain.Requests.User;

public class CreateBrokerRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public RoleRequest RoleRequest { get; set; }
    public static Broker MapToDomain(CreateBrokerRequest request)
    {
        return new Broker()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email,
            Phone = request.Phone,
            CreatedAt = DateTime.Now,
            Role = (Role)request.RoleRequest,
        };
    }
}
