using Catering.Platform.Domain.Shared;

namespace Catering.Platform.Domain.Exceptions;

public class TenantNotFoundException : Exception
{
    public TenantNotFoundException() : base(ErrorMessages.TenantNotFound) { }
}
