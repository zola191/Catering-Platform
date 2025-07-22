namespace Catering.Platform.Domain.Requests.Adress;

public record CreateAddressViewModel
{
    public string Country { get; init; }
    public string StreetAndBuilding { get; init; }
    public string Zip { get; init; }
    public string City { get; init; }
    public string? Region { get; init; }
    public string? Comment { get; init; }
    public string? Description { get; init; }

    public static Models.Address MapToDomain(CreateAddressViewModel request, Guid tenantId)
    {
        return new Models.Address()
        {
            TenantId = tenantId,
            Country = request.Country,
            StreetAndBuilding = request.StreetAndBuilding,
            Zip = request.Zip,
            City = request.City,
            Region = request.Region,
            Comment = request.Comment,
            Description = request.Description,
        };
    }
}
