using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Users;

public static class UserValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidateRequiredString<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string propertyName,
        int maxLength)
    {
        return ruleBuilder
            .NotEmpty().WithMessage($"{propertyName} is required")
            .MaximumLength(maxLength).WithMessage($"{propertyName} must not exceed {maxLength} characters");
    }

    public static IRuleBuilderOptions<T, string?> ValidateOptionalString<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string propertyName,
        int maxLength)
    {
        return ruleBuilder
            .MaximumLength(maxLength).When(x => x != null)
            .WithMessage($"{propertyName} must not exceed {maxLength} characters");
    }

    public static IRuleBuilderOptions<T, string> ValidateFirstName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidateRequiredString("FirstName", Constants.MAX_TEXT_LENGTH_128);
    }

    public static IRuleBuilderOptions<T, string> ValidateLastName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidateRequiredString("LastName", Constants.MAX_TEXT_LENGTH_128);
    }

    public static IRuleBuilderOptions<T, string?> ValidateMiddleName<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.ValidateOptionalString("MiddleName", Constants.MAX_TEXT_LENGTH_128);
    }

    public static IRuleBuilderOptions<T, string> ValidateEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidateRequiredString("Email", Constants.MAX_TEXT_LENGTH_100).EmailAddress();
    }

    public static IRuleBuilderOptions<T, string> ValidatePhone<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidateRequiredString("Phone", Constants.MAX_TEXT_LENGTH_20);
    }

    public static IRuleBuilderOptions<T, string> ValidatePassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidateRequiredString("Password", Constants.MAX_TEXT_LENGTH_128);
    }
}
