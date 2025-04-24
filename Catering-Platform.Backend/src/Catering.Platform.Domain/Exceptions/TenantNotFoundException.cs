namespace Catering.Platform.Domain.Exceptions;

public class TenantNotFoundException : Exception
{
    public TenantNotFoundException() : base("Tenant does not exist.") { }
}
