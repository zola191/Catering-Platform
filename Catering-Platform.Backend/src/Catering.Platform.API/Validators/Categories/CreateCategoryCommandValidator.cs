using Catering.Platform.Applications.Features.Categories.Create;
using FluentValidation;

namespace Catering.Platform.API.Validators.Categories;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(f => f.Name).ApplyNameRules();
        RuleFor(f => f.Description).ApplyDescriptionRules();
    }
}
