namespace Catering.Platform.Domain.Shared;

public static class ErrorMessages
{
    public const string TenantNotFound = "Tenant does not exist.";
    public const string TenantAlreadyBlocked = "Tenant already blocked.";
    public const string TenantAlreadyUnBlocked = "Tenant already unblocked.";
    public const string TenantInactive = "Cannot perform operation for an inactive tenant";
    public const string SearchQueryEmpty = "Search query cannot be empty.";
    public const string SearchQueryInvalidAfterSanitization = "Failed to process search query after sanitization.";
}
