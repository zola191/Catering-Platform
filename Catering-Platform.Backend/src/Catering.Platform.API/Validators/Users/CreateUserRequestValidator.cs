using Catering.Platform.Domain.Requests.User;
using FluentValidation;

namespace Catering.Platform.API.Validators.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(f => f.FirstName).ValidateFirstName();
        RuleFor(f => f.LastName).ValidateLastName();
        RuleFor(f => f.MiddleName).ValidateMiddleName();
        RuleFor(f => f.Email).ValidateEmail();
        RuleFor(f => f.Phone).ValidatePhone();
        RuleFor(f => f.Password).ValidatePassword();
    }
}
