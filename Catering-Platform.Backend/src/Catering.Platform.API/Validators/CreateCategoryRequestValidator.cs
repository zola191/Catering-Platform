using Catering.Platform.Domain.Requests;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators
{
    public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator()
        {
            RuleFor(f => f.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(f => f.Name).MaximumLength(Constants.MAX_LOW_TEXT_LENGTH).WithMessage("Maximum Length exceeded");
        }
    }
}
