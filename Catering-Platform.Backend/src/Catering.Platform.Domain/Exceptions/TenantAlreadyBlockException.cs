using Catering.Platform.Domain.Shared;

namespace Catering.Platform.Domain.Exceptions;

public class TenantAlreadyBlockException : Exception
{
    public TenantAlreadyBlockException() : base(ErrorMessages.TenantAlreadyBlocked) { }
}
