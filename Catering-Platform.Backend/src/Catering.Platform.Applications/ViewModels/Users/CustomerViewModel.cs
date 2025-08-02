using Catering.Platform.Domain.Models;

namespace Catering.Platform.Applications.ViewModels.Users;

public class CustomerViewModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdayedAt { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? AddressId { get; set; }
    public int TaxNumber { get; set; }

    public static CustomerViewModel MapToViewModel(Customer customer)
    {
        return new CustomerViewModel()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MiddleName = customer.MiddleName,
            Email = customer.Email,
            Phone = customer.Phone,
            IsBlocked = customer.IsBlocked,
            BlockReason = customer.BlockReason,
            CreatedAt = customer.CreatedAt,
            CompanyId = customer.CompanyId,
            AddressId = customer.AddressId,
            TaxNumber = customer.TaxNumber,
        };
    }
}
