using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;

namespace Catering.Platform.Domain.Exceptions;

public class CompanyNotFoundException : NotFoundException
{
    public CompanyNotFoundException(Guid id) : base(nameof(Company), id, ErrorMessages.CompanyNotFound) { }
}
