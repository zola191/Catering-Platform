using Catering.Platform.Domain.Requests.User;
using FluentValidation;

namespace Catering.Platform.API.Validators.Users;

public class СreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
	public СreateCustomerRequestValidator()
	{
        RuleFor(f => f.FirstName).ValidateFirstName();
        RuleFor(f => f.LastName).ValidateLastName();
        RuleFor(f => f.MiddleName).ValidateMiddleName();
        RuleFor(f => f.Email).ValidateEmail();
        RuleFor(f => f.Phone).ValidatePhone();
        RuleFor(f => f.Password).ValidatePassword();

        //TODO
        //CompanyId(Guid, внешний ключ к Company, уникальное, необязательное.Обязательно только для корпоративных заказчиков - компаний)
        //AddressId(Guid, внешний ключ к Address, уникальное, необязательное.Обязательно только для индивидуальных заказчиков)
        //TaxNumber(int ИНН.Для индивидуальных заказчиков ИНН заказчика, для корпоративных ИНН компании)

        RuleFor(f => f.TaxNumber)
            .GreaterThan(0).WithMessage("Tax number must be a positive number.")
            .WithMessage("Invalid TaxNumber. For individual customers, it must be 12 digits; for corporate customers, it must be 10 digits.");
    }
}
