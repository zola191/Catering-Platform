using Catering.Platform.Domain.Shared;

namespace Catering.Platform.Domain.Exceptions;

public class TenantHasActiveDataException : Exception
{
    public TenantHasActiveDataException() : base(ErrorMessages.TenantAlreadyUnBlocked) { }
}
