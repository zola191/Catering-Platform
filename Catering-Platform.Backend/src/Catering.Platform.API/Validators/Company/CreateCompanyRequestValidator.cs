using Catering.Platform.Domain.Requests.Company;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Company;

public class CreateCompanyRequestValidator : AbstractValidator<CreateCompanyRequest>
{
    public CreateCompanyRequestValidator()
    {
        RuleFor(f => f.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required");

        RuleFor(f => f.Name)
            .NotEmpty()
            .WithMessage($"Name is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_200)
            .WithMessage($"Name must not exceed {Constants.MAX_TEXT_LENGTH_200} characters");

        RuleFor(f => f.TaxNumber)
            .NotEmpty().WithMessage($"TaxNumber is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_20)
            .WithMessage($"Name must not exceed {Constants.MAX_TEXT_LENGTH_20} characters");

        RuleFor(f => f.AddressId)
            .NotEmpty()
            .WithMessage("AddressId is required");

        RuleFor(f => f.Phone)
            .MaximumLength(Constants.MAX_TEXT_LENGTH_20)
            .WithMessage($"Phone must not exceed {Constants.MAX_TEXT_LENGTH_20} characters");

        RuleFor(f => f.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_100)
            .WithMessage($"Email must not exceed {Constants.MAX_TEXT_LENGTH_100} characters");
    }

}
