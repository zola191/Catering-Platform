using Catering.Platform.Domain.Requests.Tenant;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Tenants;

public class BlockTenantRequestValidator : AbstractValidator<BlockTenantRequest>
{
    public BlockTenantRequestValidator()
    {
        RuleFor(f => f.Reason).NotEmpty().WithMessage("The reason must be provided");
        RuleFor(f => f.Reason).MaximumLength(Constants.MAX_TEXT_LENGTH_500).WithMessage("Maximum Length exceeded");
    }
}
