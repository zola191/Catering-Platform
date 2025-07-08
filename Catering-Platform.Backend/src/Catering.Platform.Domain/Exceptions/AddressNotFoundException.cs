using Catering.Platform.Domain.Models;
using Catering.Platform.Domain.Shared;

namespace Catering.Platform.Domain.Exceptions;

public class AddressNotFoundException : NotFoundException
{
    public AddressNotFoundException(Guid id) : base(nameof(Address), id, ErrorMessages.AddressNotFound) { }
}
