using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Categories;

public static class CategoryValidationRules
{
    public static IRuleBuilderOptions<T, string> ApplyNameRules<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_256).WithMessage("Maximum Length exceeded");
    }

    public static IRuleBuilderOptions<T, string> ApplyDescriptionRules<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .MaximumLength(Constants.MAX_TEXT_LENGTH_2000).WithMessage("Maximum Length exceeded");
    }
}
