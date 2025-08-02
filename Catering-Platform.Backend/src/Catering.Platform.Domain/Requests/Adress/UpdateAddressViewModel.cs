namespace Catering.Platform.Domain.Requests.Adress;

public record UpdateAddressViewModel
{
    public string Country { get; init; }
    public string StreetAndBuilding { get; init; }
    public string Zip { get; init; }
    public string City { get; init; }
    public string? Region { get; init; }
    public string? Comment { get; init; }
    public string? Description { get; init; }

    public static void MapToDomain(Models.Address entity, UpdateAddressViewModel request)
    {
        entity.Country = request.Country;
        entity.StreetAndBuilding = request.StreetAndBuilding;
        entity.Zip = request.Zip;
        entity.City = request.City;
        entity.Region = request.Region;
        entity.Comment = request.Comment;
        entity.Description = request.Description;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
