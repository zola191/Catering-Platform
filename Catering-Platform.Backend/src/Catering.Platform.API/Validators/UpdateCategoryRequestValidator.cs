using Catering.Platform.Domain.Requests;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(f => f.Name).ApplyNameRules();
            RuleFor(f => f.Description).ApplyDescriptionRules();
        }
    }
}
