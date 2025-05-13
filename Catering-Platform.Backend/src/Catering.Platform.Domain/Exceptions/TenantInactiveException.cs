using Catering.Platform.Domain.Shared;

namespace Catering.Platform.Domain.Exceptions
{
    public class TenantInactiveException : Exception
    {
        public TenantInactiveException() : base(ErrorMessages.TenantInactive) { }
    }
}
