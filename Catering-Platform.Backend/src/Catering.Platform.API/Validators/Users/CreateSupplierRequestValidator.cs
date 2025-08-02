using Catering.Platform.Domain.Requests.User;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Users;

public class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
{
    public CreateSupplierRequestValidator()
    {
        RuleFor(f => f.FirstName).ValidateFirstName();
        RuleFor(f => f.LastName).ValidateLastName();
        RuleFor(f => f.MiddleName).ValidateMiddleName();
        RuleFor(f => f.Email).ValidateEmail();
        RuleFor(f => f.Phone).ValidatePhone();
        RuleFor(f => f.Password).ValidatePassword();

        RuleFor(f => f.CompanyId).NotEmpty().WithMessage("CompanyId is required");
        RuleFor(f => f.Position)
            .MaximumLength(Constants.MAX_TEXT_LENGTH_256)
            .WithMessage($"Street and Building must not exceed {Constants.MAX_TEXT_LENGTH_256} characters");
    }
}
