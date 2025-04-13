using Catering.Platform.Applications.Validators;
using Catering.Platform.Domain.Requests;
using FluentValidation;

namespace Catering.Platform.API.Validators
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(f => f.Name).ApplyNameRules();
            RuleFor(f => f.Description).ApplyDescriptionRules();
        }
    }
}
