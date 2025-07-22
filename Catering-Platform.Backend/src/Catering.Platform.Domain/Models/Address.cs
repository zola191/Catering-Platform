namespace Catering.Platform.Domain.Models
{
    public class Address : Entity
    {
        public Guid TenantId { get; set; }
        public string Country { get; set; }
        public string StreetAndBuilding { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string? Region { get; set; }
        public string? Comment { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Tenant Tenant { get; set; }
    }
}
