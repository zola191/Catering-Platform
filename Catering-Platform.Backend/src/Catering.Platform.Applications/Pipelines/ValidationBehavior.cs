using FluentValidation;
using MediatR;

namespace Catering.Platform.Applications.Pipelines;

// Generic с открытыми типами изучить

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(_validators.Select(f => f.ValidateAsync(context, cancellationToken)));

            var failures = results.SelectMany(f => f.Errors).Where(f => f != null);
            if (failures.Any())
            {
                throw new ValidationException(failures);
            }
        }
        return await next();
    }
}
