using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;

namespace Catering.Platform.Domain.Exceptions;

public class CompanyNotFoundException : NotFoundException
{
    public string? TaxNumber { get; }
    public string? Name { get; }

    private CompanyNotFoundException(Guid id) : base(nameof(Company), id, ErrorMessages.CompanyNotFound)
    {
        TaxNumber = null;
    }

    private CompanyNotFoundException(string taxNumber, string? name)
        : base(nameof(Company), null,
            name != null
                ? $"{ErrorMessages.CompanyNotFound} (Name: {name})"
                : $"{ErrorMessages.CompanyNotFound} (TaxNumber: {taxNumber})")
    {
        TaxNumber = taxNumber;
        Name = name;
    }


    public static CompanyNotFoundException ById(Guid id) => new(id);
    public static CompanyNotFoundException ByTaxNumber(string taxNumber) => new(taxNumber, null);
    public static CompanyNotFoundException ByName(string name) => new(null, name);
}
