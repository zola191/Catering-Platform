namespace Catering.Platform.Domain.Models;

public class Customer : User
{
    public Guid? CompanyId { get; set; }
    public Guid? AddressId { get; set; }
    public Address Address { get; set; }
    public int TaxNumber { get; set; }
}
