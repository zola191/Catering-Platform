using Catering.Platform.Domain.Models;

namespace Catering.Platform.Applications.ViewModels.Users;

public class SupplierViewModel
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
    public Guid CompanyId { get; set; }
    public string Position { get; set; }

    public static SupplierViewModel MapToViewModel(Supplier supplier)
    {
        return new SupplierViewModel()
        {
            Id = supplier.Id,
            FirstName = supplier.FirstName,
            LastName = supplier.LastName,
            MiddleName = supplier.MiddleName,
            Email = supplier.Email,
            Phone = supplier.Phone,
            IsBlocked = supplier.IsBlocked,
            BlockReason = supplier.BlockReason,
            CreatedAt = supplier.CreatedAt,
            CompanyId = supplier.CompanyId,
            Position = supplier.Position,
        };
    }
}
