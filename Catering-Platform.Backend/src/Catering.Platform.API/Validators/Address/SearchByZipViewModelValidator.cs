using Catering.Platform.Applications.ViewModels;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Address
{
    public class SearchByZipViewModelValidator : AbstractValidator<SearchByZipViewModel>
    {
        public SearchByZipViewModelValidator()
        {
            RuleFor(f => f.Zip)
            .NotEmpty().WithMessage("Zip code is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_6).WithMessage($"Zip code must not exceed {Constants.MAX_TEXT_LENGTH_6} characters")
            .Matches(@"^\d+$").WithMessage("Zip code must contain only digits");
        }
    }
}
