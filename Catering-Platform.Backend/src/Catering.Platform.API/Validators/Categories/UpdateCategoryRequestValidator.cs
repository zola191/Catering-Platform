using Catering.Platform.Domain.Requests;
using FluentValidation;

namespace Catering.Platform.API.Validators.Categories;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(f => f.Name).ApplyNameRules();
        RuleFor(f => f.Description).ApplyDescriptionRules();
    }
}