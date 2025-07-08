using Catering.Platform.Domain.Requests.Company;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Company;

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator()
    {
        RuleFor(f => f.CompanyId)
            .NotEmpty()
            .WithMessage("TenantId is required");

        RuleFor(f => f.Name).ApplyNameRules();
        RuleFor(f => f.TaxNumber).ApplyTaxNumberRules();
        RuleFor(f => f.AddressId).ApplyAddressIdRules();
        RuleFor(f => f.Phone).ApplyPhoneRules();
        RuleFor(f => f.Email).ApplyEmailRules();
    }
}
