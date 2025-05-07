using Catering.Platform.Domain.Requests.Adress;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Address;

public abstract class UpdateAddressViewModelValidator : AbstractValidator<UpdateAddressViewModel>
{
    protected UpdateAddressViewModelValidator()
    {
        RuleFor(f => f.Country)
                    .NotEmpty().WithMessage("Country is required")
                    .MaximumLength(Constants.MAX_TEXT_LENGTH_64).WithMessage($"Country must not exceed {Constants.MAX_TEXT_LENGTH_64} characters");

        RuleFor(f => f.StreetAndBuilding)
            .NotEmpty().WithMessage("Street and Building is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_256).WithMessage($"Street and Building must not exceed {Constants.MAX_TEXT_LENGTH_256} characters");

        RuleFor(f => f.Zip)
            .NotEmpty().WithMessage("Zip code is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_6).WithMessage($"Zip code must not exceed {Constants.MAX_TEXT_LENGTH_6} characters")
            .Matches(@"^\d+$").WithMessage("Zip code must contain only digits");

        RuleFor(f => f.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_64).WithMessage($"City must not exceed {Constants.MAX_TEXT_LENGTH_64} characters");

        RuleFor(f => f.Region)
            .NotEmpty().WithMessage("Region is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_64).WithMessage($"Region must not exceed {Constants.MAX_TEXT_LENGTH_64} characters");

        RuleFor(f => f.Comment)
            .MaximumLength(Constants.MAX_TEXT_LENGTH_256).WithMessage($"Comment must not exceed {Constants.MAX_TEXT_LENGTH_256} characters");

        RuleFor(f => f.Description)
            .MaximumLength(Constants.MAX_TEXT_LENGTH_512).WithMessage($"Description must not exceed {Constants.MAX_TEXT_LENGTH_512} characters");
    }
}
