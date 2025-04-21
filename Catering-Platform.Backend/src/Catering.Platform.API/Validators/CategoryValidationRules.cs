using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators;

public static class CategoryValidationRules
{
    public static IRuleBuilderOptions<T, string> ApplyNameRules<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_200_256).WithMessage("Maximum Length exceeded");
    }

    public static IRuleBuilderOptions<T, string> ApplyDescriptionRules<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .MaximumLength(Constants.MAX_TEXT_LENGTH_200_2000).WithMessage("Maximum Length exceeded");
    }
}
