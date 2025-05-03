namespace Catering.Platform.Domain.Shared;

public static class ErrorMessages
{
    public const string TenantNotFound = "Tenant does not exist.";
    public const string TenantAlreadyBlocked = "Tenant already blocked.";
    public const string TenantAlreadyUnBlocked = "Tenant already unblocked.";
    public const string TenantInactive = "Cannot perform operation for an inactive tenant";
}
