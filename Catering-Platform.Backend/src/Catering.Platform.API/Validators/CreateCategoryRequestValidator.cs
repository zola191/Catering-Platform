using Catering.Platform.Domain.Requests;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators
{
    public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(f => f.Name).ApplyNameRules();
            RuleFor(f => f.Description).ApplyDescriptionRules();
        }
    }
}
