using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;

namespace Catering.Platform.Domain.Exceptions;

public class CompanyNotFoundException : NotFoundException
{
    public string? TaxNumber { get; }

    public CompanyNotFoundException(Guid id) : base(nameof(Company), id, ErrorMessages.CompanyNotFound)
    {
        TaxNumber = null;
    }

    public CompanyNotFoundException(string taxNumber)
    : base(nameof(Company), null, $"{ErrorMessages.CompanyNotFound} (TaxNumber: {taxNumber})")
    {
        TaxNumber = taxNumber;
    }
}
