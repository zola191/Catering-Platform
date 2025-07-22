using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Tenants
{
    public static class TenantValidationRules
    {
        public static IRuleBuilderOptions<T, string> ApplyNameRules<T>(
        this IRuleBuilder<T, string> rule)
        {
            return rule
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(Constants.MAX_TEXT_LENGTH_200).WithMessage("Maximum Length exceeded");
        }
    }
}
