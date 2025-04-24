using Catering.Platform.Domain.Requests.Tenant;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Tenants
{
    public class CreateTenantRequestValidatior : AbstractValidator<CreateTenantRequest>
    {
        public CreateTenantRequestValidatior()
        {
            RuleFor(request => request.Name)
                .NotEmpty().WithMessage("Name is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_200).WithMessage("Maximum Length exceeded");
        }
    }
}
