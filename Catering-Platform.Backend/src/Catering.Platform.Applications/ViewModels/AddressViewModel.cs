using Catering.Platform.Domain.Models;

namespace Catering.Platform.Applications.ViewModels
{
    public record AddressViewModel
    {
        public Guid Id { get; init; }
        public Guid TenantId { get; init; }
        public string Country { get; init; }
        public string StreetAndBuilding { get; init; }
        public string Zip { get; init; }
        public string City { get; init; }
        public string Region { get; init; }
        public string? Comment { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }

        public static AddressViewModel MapToViewModel(Address address)
        {
            return new AddressViewModel()
            {
                Id = address.Id,
                TenantId = address.TenantId,
                Country = address.Country,
                StreetAndBuilding = address.StreetAndBuilding,
                Zip = address.Zip,
                City = address.City,
                Region = address.Region,
                Comment = address.Comment,
                Description = address.Description,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt,
            };
        }
    }
}
