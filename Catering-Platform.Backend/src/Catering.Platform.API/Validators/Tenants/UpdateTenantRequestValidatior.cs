using Catering.Platform.Domain.Requests.Tenant;
using FluentValidation;

namespace Catering.Platform.API.Validators.Tenants
{
    public class UpdateTenantRequestValidatior : AbstractValidator<UpdateTenantRequest>
    {
        public UpdateTenantRequestValidatior()
        {
            RuleFor(f => f.Name).ApplyNameRules();
        }
    }
}
