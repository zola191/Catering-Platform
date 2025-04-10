using Catering.Platform.Domain.Requests;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators
{
    public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
    {
        public UpdateCategoryRequestValidator()
        {
            RuleFor(f => f.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(f => f.Name).MaximumLength(Constants.MAX_LOW_TEXT_LENGTH).WithMessage("Maximum Length exceeded");
            RuleFor(f => f.Description).MaximumLength(Constants.MAX_HIGH_TEXT_LENGTH).WithMessage("Maximum Length exceeded");
        }
    }
}
