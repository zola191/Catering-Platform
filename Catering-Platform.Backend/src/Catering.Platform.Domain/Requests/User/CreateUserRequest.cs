namespace Catering.Platform.Domain.Requests.User;

public record CreateUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }

    public static Models.User MapToDomain(CreateUserRequest request)
    {
        return new Models.User()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Email = request.Email,
            Phone = request.Phone,
            CreatedAt = DateTime.Now,
        };
    }
}
