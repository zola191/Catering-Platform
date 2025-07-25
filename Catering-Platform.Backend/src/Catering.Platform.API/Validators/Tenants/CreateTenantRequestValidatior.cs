﻿using Catering.Platform.Domain.Requests.Tenant;
using FluentValidation;

namespace Catering.Platform.API.Validators.Tenants
{
    public class CreateTenantRequestValidatior : AbstractValidator<CreateTenantRequest>
    {
        public CreateTenantRequestValidatior()
        {
            RuleFor(f => f.Name).ApplyNameRules();
        }
    }
}
