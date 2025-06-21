using Catering.Platform.Domain.Models;

namespace Catering.Platform.Applications.ViewModels.Users;

public class BrokerViewModel
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
    public RoleVideModel RoleViewModel { get; set; }

    public static BrokerViewModel MapToViewModel(Broker broker)
    {
        return new BrokerViewModel()
        {
            Id = broker.Id,
            FirstName = broker.FirstName,
            LastName = broker.LastName,
            MiddleName = broker.MiddleName,
            Email = broker.Email,
            Phone = broker.Phone,
            IsBlocked = broker.IsBlocked,
            BlockReason = broker.BlockReason,
            CreatedAt = broker.CreatedAt,
            UpdayedAt = broker.UpdatedAt,
            RoleViewModel = (RoleVideModel)broker.Role
        };
    }
}
