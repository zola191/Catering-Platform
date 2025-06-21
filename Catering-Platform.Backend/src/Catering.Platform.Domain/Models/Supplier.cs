namespace Catering.Platform.Domain.Models;

public class Supplier : User
{
    public Guid CompanyId { get; set; }
    public string Position { get; set; }
}
