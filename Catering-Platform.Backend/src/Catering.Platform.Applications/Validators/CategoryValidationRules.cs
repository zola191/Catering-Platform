using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.Applications.Validators;

public static class CategoryValidationRules
{
    public static IRuleBuilderOptions<T, string> ApplyNameRules<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(Constants.MAX_LOW_TEXT_LENGTH).WithMessage("Maximum Length exceeded");
    }

    public static IRuleBuilderOptions<T, string> ApplyDescriptionRules<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .MaximumLength(Constants.MAX_HIGH_TEXT_LENGTH).WithMessage("Maximum Length exceeded");
    }
}
