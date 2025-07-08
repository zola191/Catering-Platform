using FluentValidation;
using MediatR;

namespace Catering.Platform.API.Validators.Categories;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        IEnumerable<IValidator<TRequest>> validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken ct)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            foreach (var error in failures)
            {
                _logger.LogWarning("Validation failed for property {PropertyName}: {ErrorMessage}",
                    error.PropertyName, error.ErrorMessage);
            }

            throw new ValidationException(failures);
        }

        return await next(ct);
    }
}