using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Company;

public static class CompanyValidationRules
{
    public static IRuleBuilderOptions<T, string> ApplyNameRules<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_200)
            .WithMessage($"Name must not exceed {Constants.MAX_TEXT_LENGTH_200} characters");
    }

    public static IRuleBuilderOptions<T, string> ApplyTaxNumberRules<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage($"TaxNumber is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_20)
            .WithMessage($"Name must not exceed {Constants.MAX_TEXT_LENGTH_20} characters");
    }

    public static IRuleBuilderOptions<T, Guid> ApplyAddressIdRules<T>(this IRuleBuilder<T, Guid> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("AddressId is required");
    }

    public static IRuleBuilderOptions<T, string> ApplyPhoneRules<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .MaximumLength(Constants.MAX_TEXT_LENGTH_20)
            .WithMessage($"Phone must not exceed {Constants.MAX_TEXT_LENGTH_20} characters");
    }

    public static IRuleBuilderOptions<T, string> ApplyEmailRules<T>(this IRuleBuilder<T, string> rule)
    {
        return rule
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_100)
            .WithMessage($"Email must not exceed {Constants.MAX_TEXT_LENGTH_100} characters");
    }
}
