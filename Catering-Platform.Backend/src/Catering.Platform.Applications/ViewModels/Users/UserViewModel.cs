using Catering.Platform.Domain.Models;

namespace Catering.Platform.Applications.ViewModels.Users;

public record UserViewModel
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

    public static UserViewModel MapToViewModel(User user)
    {
        return new UserViewModel()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            Email = user.Email,
            Phone = user.Phone,
            IsBlocked = user.IsBlocked,
            BlockReason = user.BlockReason,
            CreatedAt = user.CreatedAt,
            UpdayedAt = user.UpdatedAt,
        };
    }
}
