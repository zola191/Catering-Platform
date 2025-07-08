using Catering.Platform.Domain.Requests.Dish;
using Catering.Platform.Domain.Shared;
using FluentValidation;

namespace Catering.Platform.API.Validators.Dishes;

public class UpdateDishRequestValidator : AbstractValidator<CreateDishRequest>
{
    public UpdateDishRequestValidator()
    {
        RuleFor(f => f.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(Constants.MAX_TEXT_LENGTH_256)
            .WithMessage($"Name must not exceed {Constants.MAX_TEXT_LENGTH_256} characters");

        RuleFor(f => f.Description)
            .MaximumLength(Constants.MAX_TEXT_LENGTH_2000)
            .WithMessage($"Description must not exceed {Constants.MAX_TEXT_LENGTH_2000} characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(f => f.Price)
            .NotEmpty().WithMessage("Price is required");

        RuleFor(f => f.IsAvailable)
            .NotEmpty().WithMessage("IsAvailable is required");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required")
            .NotEqual(Guid.Empty).WithMessage("Invalid category");

        RuleFor(x => x.Ingredients)
            .NotNull().WithMessage("Ingredients list cannot be null")
            .ForEach(ingredient =>
                ingredient.NotEmpty().WithMessage("Ingredient cannot be empty")
            );

        RuleFor(x => x.Allergens)
            .NotNull().WithMessage("Allergens list cannot be null");
    }
}
